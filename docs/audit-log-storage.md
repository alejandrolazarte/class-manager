# Audit log storage runbook

Date: 2026-10-19

How the storage account of the [audit log](backend/20261012-audit-log/plan.md) is set up and checked. Everything runs in **Azure Cloud Shell** (Bash), signed in as the owner. The account itself was created in the portal on 2026-10-19; the rest is one script.

What the API ends up with: one custom role, `Audit Log Writer`, on this account only. It can create tables, insert rows, create blobs and append to them. It can't read, update or delete anything, and there is no account key it could use instead.

## 1. The account

`classmanageraudit`, in `class-manager-rg`, France Central (the API's region, so writes don't cross regions):

| Setting | Value |
|---|---|
| Kind, performance, redundancy | StorageV2, Standard, LRS, Hot |
| Storage account key access | Disabled |
| Anonymous access on containers | Disabled |
| Default to Microsoft Entra authorization in the portal | Enabled |
| Secure transfer, minimum TLS | Enabled, 1.2 |
| Public network access | Enabled from all networks, no private endpoint |
| Blob and container soft delete | 7 days (portal default) |
| Versioning, change feed, version-level immutability | Disabled. Security events use a retention policy on their container ([step 5 of the plan](backend/20261012-audit-log/plan.md#immutable-security-events)) |

The same account as a command, only if it has to be created again:

```bash
az storage account create --name classmanageraudit --resource-group class-manager-rg --location francecentral --sku Standard_LRS --kind StorageV2 --access-tier Hot --https-only true --min-tls-version TLS1_2 --allow-blob-public-access false --allow-shared-key-access false --default-to-oauth-authentication true --allow-cross-tenant-replication false --public-network-access Enabled
```

Because there is no account key, the subscription Owner can manage the account but can't read its data without a data role (the script gives the owner the reader roles).

## 2. The `Audit Log Writer` role

| Permission | Kind | Operation it allows |
|---|---|---|
| `Microsoft.Storage/storageAccounts/tableServices/tables/write` | Action | Create Table (the table of the month) |
| `Microsoft.Storage/storageAccounts/tableServices/tables/entities/add/action` | Data action | Insert Entity, alone or in a batch. Not Insert Or Replace, Insert Or Merge, Update, Merge, Query or Delete |
| `Microsoft.Storage/storageAccounts/blobServices/containers/blobs/add/action` | Data action | Put Blob for a blob that doesn't exist yet (the day's append blob) and Append Block. Not overwriting, reading or deleting a blob |

Checked against [Authorize with Microsoft Entra ID](https://learn.microsoft.com/rest/api/storageservices/authorize-with-azure-active-directory#permissions-for-calling-data-operations) on 2026-10-19. `tables/read` (list tables) and `blobs/write` (overwrite a blob) are left out because the writer doesn't need them. The API can't create containers, so the script creates `security-events` with the owner's account.

## 3. Run the setup script

[`scripts/setup-audit-log-storage.sh`](../scripts/setup-audit-log-storage.sh) does, in order:

1. Checks that the account has key access and anonymous access disabled and TLS 1.2, and stops otherwise.
2. Creates the `security-events` container.
3. Creates the `Audit Log Writer` role, or checks that the existing one has exactly the permissions above.
4. **Proves the role** with a temporary service principal that has only this role, signed in to a separate Azure CLI profile. The owner's account can't prove it: Owner already allows creating and deleting tables. It waits up to 10 minutes for the role to apply, then expects:

   | Allowed | Refused (403) |
   |---|---|
   | Create a table, insert a row, create an append blob, append to it | Read, merge, replace, upsert and delete a row; list and delete tables; overwrite, read and delete a blob |

   Then it deletes the temporary service principal, its role assignment, the test table and the test container. If any check failed it stops here, without touching the API.
5. Gives `Audit Log Writer` to the API's system-assigned managed identity (the same one it uses for SQL, see [pilot deployment](pilot-deployment.md)).
6. Gives the signed-in owner `Storage Table Data Reader` and `Storage Blob Data Reader`.
7. Prints the role assignments on the account.

The repository is private, so copy the script into Cloud Shell: **Manage files → Upload** (or paste it with `cat > setup-audit-log-storage.sh`), then:

```bash
bash setup-audit-log-storage.sh
```

It can be run again at any time: it skips what already exists and first removes any temporary service principal left by a run that was cut off (Cloud Shell closes after 20 minutes without input). Everything it prints is also appended to `~/setup-audit-log-storage.log`, but an ephemeral Cloud Shell session (no storage account attached) deletes it when the session closes: keep the tab active until the script prints `Done`.

The API reaches the account through its service URIs, which are not secrets: `https://classmanageraudit.table.core.windows.net` and `https://classmanageraudit.blob.core.windows.net`. The setting names are added with the writer's code ([step 3 of the plan](backend/20261012-audit-log/plan.md#steps)). Development and tests use Azurite, which has no Entra and keeps its connection string.

## 4. Reading the logs

The owner reads with their own account, never with the API's identity: open the account in Azure Storage Explorer signed in with Entra. Commands are filtered by `PartitionKey` (the business) and `RowKey` range; security events are read by downloading a day's blob.

## 5. Audit commands

Run them after any change to the account or its roles, and before locking the retention policy ([step 6 of the plan](backend/20261012-audit-log/plan.md#steps)).

```bash
az storage account show -n classmanageraudit -g class-manager-rg --query "{accountKeyAccess:allowSharedKeyAccess, anonymousAccess:allowBlobPublicAccess, minimumTls:minimumTlsVersion}"
az role definition list --name "Audit Log Writer" --custom-role-only true --query "[0].permissions[0].{actions:actions, dataActions:dataActions}"
az role assignment list --scope $(az storage account show -n classmanageraudit -g class-manager-rg --query id -o tsv) --include-inherited --query "[].{principal:principalName, type:principalType, role:roleDefinitionName}" -o table
```

Expected:

- `accountKeyAccess` and `anonymousAccess` false, `TLS1_2`.
- The role has exactly the three permissions of [section 2](#2-the-audit-log-writer-role).
- On the account: `Audit Log Writer` for the API's identity, and the two reader roles for the owner. Inherited from above: the owner's Owner role, and `Contributor` for `class-manager-github-deploy` on the resource group, which manages resources but has no data actions. No other identity has a data role, nobody else has `Audit Log Writer`, and no `class-manager-audit-role-check` is left.

## 6. Deleting old command tables

Each month has its own table, kept for 24 months ([retention](backend/20261012-audit-log/plan.md#retention)). The API can't delete them; the owner does, with their own account, once a month from the end of 2028:

```bash
az storage table delete --name Commands202610 --account-name classmanageraudit --auth-mode login --fail-not-exist
```
