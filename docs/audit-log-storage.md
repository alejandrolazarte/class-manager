# Audit log storage runbook

Date: 2026-10-19

Commands to set up the storage account of the [audit log](backend/20261012-audit-log/plan.md) and to check it later. Run them from PowerShell on the owner's PC, signed in with the owner's account. The account itself was created in the portal on 2026-10-19; [section 1](#1-the-account) has the same settings as a command, to check them or to create the account again.

What the API ends up with: one custom role, `Audit Log Writer`, on this account only. It can create tables, insert rows, create blobs and append to them. It can't read, update or delete anything, and there is no account key it could use instead.

## 0. Names and sign-in

```powershell
az login
$location = "francecentral"
$resourceGroup = "class-manager-rg"
$containerApp = "class-manager-api"
$auditAccount = "classmanageraudit"
$securityEventsContainer = "security-events"
$writerRole = "Audit Log Writer"
$subscriptionId = az account show --query id --output tsv
$auditAccountId = az storage account show --name $auditAccount --resource-group $resourceGroup --query id --output tsv
```

## 1. The account

The account is in the same region as the API (France Central), so writes don't cross regions. To check its settings:

```powershell
az storage account show --name $auditAccount --resource-group $resourceGroup `
  --query "{location:location, sku:sku.name, kind:kind, httpsOnly:enableHttpsTrafficOnly, minimumTls:minimumTlsVersion, anonymousAccess:allowBlobPublicAccess, accountKeyAccess:allowSharedKeyAccess, entraInPortal:defaultToOAuthAuthentication, crossTenantReplication:allowCrossTenantReplication, publicNetworkAccess:publicNetworkAccess}"
```

Expected: `francecentral`, `Standard_LRS`, `StorageV2`, `httpsOnly` true, `TLS1_2`, `anonymousAccess` false, `accountKeyAccess` false, `entraInPortal` true, `crossTenantReplication` false, `publicNetworkAccess` `Enabled`.

The same account as a command, only if it has to be created again:

```powershell
az storage account create --name $auditAccount --resource-group $resourceGroup --location $location `
  --sku Standard_LRS --kind StorageV2 --access-tier Hot --https-only true --min-tls-version TLS1_2 `
  --allow-blob-public-access false --allow-shared-key-access false --default-to-oauth-authentication true `
  --allow-cross-tenant-replication false --public-network-access Enabled
```

Blob and container soft delete keep the portal's default of 7 days. Version-level immutability stays off: security events use a retention policy on their container ([step 5 of the plan](backend/20261012-audit-log/plan.md#immutable-security-events)).

Because there is no account key, the subscription Owner can manage the account but can't read its data without a data role ([section 6](#6-reading-the-logs)).

## 2. The security events container

The API can't create containers, so the owner creates the one for security events once, through Azure Resource Manager:

```powershell
az storage container-rm create --storage-account $auditAccount --resource-group $resourceGroup `
  --name $securityEventsContainer --public-access off
```

The command tables (`Commands{yyyyMM}`) are not created here: the API creates the table of each month on its first write.

## 3. The `Audit Log Writer` role

| Permission | Kind | Operation it allows |
|---|---|---|
| `Microsoft.Storage/storageAccounts/tableServices/tables/write` | Action | Create Table (the table of the month) |
| `Microsoft.Storage/storageAccounts/tableServices/tables/entities/add/action` | Data action | Insert Entity, alone or in a batch. Not Insert Or Replace, Insert Or Merge, Update, Merge, Query or Delete |
| `Microsoft.Storage/storageAccounts/blobServices/containers/blobs/add/action` | Data action | Put Blob for a blob that doesn't exist yet (the day's append blob) and Append Block. Not overwriting, reading or deleting a blob |

Checked against [Authorize with Microsoft Entra ID](https://learn.microsoft.com/rest/api/storageservices/authorize-with-azure-active-directory#permissions-for-calling-data-operations) on 2026-10-19. `tables/read` (list tables) and `blobs/write` (overwrite a blob) are left out because the writer doesn't need them.

```powershell
$roleDefinitionFile = New-TemporaryFile
@"
{
  "Name": "$writerRole",
  "Description": "Adds audit log entries: creates tables, inserts rows, creates blobs and appends to them. Can't read, update or delete.",
  "Actions": [
    "Microsoft.Storage/storageAccounts/tableServices/tables/write"
  ],
  "NotActions": [],
  "DataActions": [
    "Microsoft.Storage/storageAccounts/tableServices/tables/entities/add/action",
    "Microsoft.Storage/storageAccounts/blobServices/containers/blobs/add/action"
  ],
  "NotDataActions": [],
  "AssignableScopes": [
    "/subscriptions/$subscriptionId/resourceGroups/$resourceGroup"
  ]
}
"@ | Set-Content -Path $roleDefinitionFile
az role definition create --role-definition $roleDefinitionFile
Remove-Item $roleDefinitionFile
```

A new role can take a few minutes to become assignable. If the next command says the role doesn't exist, wait and run it again.

## 4. Prove the role

The owner's own account can't prove it: Owner already allows creating and deleting tables. A temporary service principal with only `Audit Log Writer` stands in for the API.

```powershell
$checkPrincipal = az ad sp create-for-rbac --name "class-manager-audit-role-check" --only-show-errors | ConvertFrom-Json
$checkObjectId = az ad sp show --id $checkPrincipal.appId --query id --output tsv
az role assignment create --assignee-object-id $checkObjectId --assignee-principal-type ServicePrincipal `
  --role $writerRole --scope $auditAccountId
az storage container-rm create --storage-account $auditAccount --resource-group $resourceGroup --name role-check --public-access off
```

Sign in as it in a separate Azure CLI profile, so the owner's session stays untouched. A new role assignment can take up to 10 minutes to apply to data: if the first two commands answer 403, wait and repeat them.

```powershell
$env:AZURE_CONFIG_DIR = Join-Path $env:TEMP "audit-role-check"
az login --service-principal --username $checkPrincipal.appId --password $checkPrincipal.password `
  --tenant $checkPrincipal.tenant --allow-no-subscriptions

$tableArguments = @("--account-name", $auditAccount, "--auth-mode", "login")
$blobUrl = "https://$auditAccount.blob.core.windows.net/role-check/day.jsonl"
$blobArguments = @("--resource", "https://storage.azure.com", "--headers", "x-ms-version=2023-11-03")
```

Each command must give the result on its right. A refused one prints `AuthorizationPermissionMismatch` (403).

| Command | Expected |
|---|---|
| `az storage table create --name RoleCheck @tableArguments` | `"created": true` |
| `az storage entity insert --table-name RoleCheck --entity PartitionKey=check RowKey=1 Value=first @tableArguments` | Works |
| `az storage entity query --table-name RoleCheck @tableArguments` | 403 |
| `az storage entity merge --table-name RoleCheck --entity PartitionKey=check RowKey=1 Value=changed @tableArguments` | 403 |
| `az storage entity replace --table-name RoleCheck --entity PartitionKey=check RowKey=1 Value=changed @tableArguments` | 403 |
| `az storage entity insert --table-name RoleCheck --entity PartitionKey=check RowKey=1 Value=changed --if-exists replace @tableArguments` | 403 |
| `az storage entity delete --table-name RoleCheck --partition-key check --row-key 1 @tableArguments` | 403 |
| `az storage table list @tableArguments` | 403 |
| `az storage table delete --name RoleCheck --fail-not-exist @tableArguments` | 403 |
| `az rest --method put --url $blobUrl @blobArguments x-ms-blob-type=AppendBlob If-None-Match=*` | Works (creates the append blob) |
| `az rest --method put --url "$($blobUrl)?comp=appendblock" --body "first line" @blobArguments` | Works (appends) |
| `az rest --method put --url $blobUrl --body "replaced" @blobArguments x-ms-blob-type=BlockBlob` | 403 (overwrite) |
| `az rest --method get --url $blobUrl @blobArguments` | 403 |
| `az rest --method delete --url $blobUrl @blobArguments` | 403 |

If any line differs, don't assign the role to the API: fix the role definition (`az role definition update`) and repeat this section.

Back to the owner's profile, then remove the temporary principal and what it wrote:

```powershell
Remove-Item Env:AZURE_CONFIG_DIR
az role assignment delete --assignee $checkObjectId --role $writerRole --scope $auditAccountId
az ad app delete --id $checkPrincipal.appId
az storage container-rm delete --storage-account $auditAccount --resource-group $resourceGroup --name role-check --yes
az rest --method delete --url "https://management.azure.com$auditAccountId/tableServices/default/tables/RoleCheck?api-version=2023-05-01"
Remove-Item -Recurse (Join-Path $env:TEMP "audit-role-check")
```

## 5. Give the role to the API

The API signs in with its system-assigned managed identity, the same one it uses for SQL ([pilot deployment](pilot-deployment.md)).

```powershell
$apiPrincipalId = az containerapp show --name $containerApp --resource-group $resourceGroup --query identity.principalId --output tsv
az role assignment create --assignee-object-id $apiPrincipalId --assignee-principal-type ServicePrincipal `
  --role $writerRole --scope $auditAccountId
```

The API reaches the account through its service URIs, which are not secrets:

| Service | URI |
|---|---|
| Tables | `https://classmanageraudit.table.core.windows.net` |
| Blobs | `https://classmanageraudit.blob.core.windows.net` |

The setting names are added with the writer's code ([step 3 of the plan](backend/20261012-audit-log/plan.md#steps)). Development and tests use Azurite, which has no Entra and keeps its connection string.

## 6. Reading the logs

The owner reads with their own account, never with the API's identity:

```powershell
$ownerObjectId = az ad signed-in-user show --query id --output tsv
az role assignment create --assignee-object-id $ownerObjectId --assignee-principal-type User `
  --role "Storage Table Data Reader" --scope $auditAccountId
az role assignment create --assignee-object-id $ownerObjectId --assignee-principal-type User `
  --role "Storage Blob Data Reader" --scope $auditAccountId
```

Then open the account in Azure Storage Explorer signed in with Entra: commands filtered by `PartitionKey` (the business) and `RowKey` range, security events by downloading a day's blob.

## 7. Audit commands

Run them after any change to the account or its roles, and before locking the retention policy ([step 6 of the plan](backend/20261012-audit-log/plan.md#steps)).

```powershell
az storage account show --name $auditAccount --resource-group $resourceGroup `
  --query "{anonymousAccess:allowBlobPublicAccess, accountKeyAccess:allowSharedKeyAccess, minimumTls:minimumTlsVersion}"
az role definition list --name $writerRole --query "[0].permissions[0].{actions:actions, dataActions:dataActions}"
az role assignment list --scope $auditAccountId --include-inherited `
  --query "[].{principal:principalName, type:principalType, role:roleDefinitionName, scope:scope}" --output table
```

Expected:

- `anonymousAccess` and `accountKeyAccess` false, `TLS1_2`.
- The role has exactly the three permissions of [section 3](#3-the-audit-log-writer-role).
- On the account: `Audit Log Writer` for the API's identity, and the two reader roles for the owner. Inherited from above: the owner's Owner role, and `Contributor` for `class-manager-github-deploy` on the resource group, which manages resources but has no data actions. No other identity has a data role, and nobody has `Audit Log Writer` besides the API.

## 8. Deleting old command tables

Each month has its own table, kept for 24 months ([retention](backend/20261012-audit-log/plan.md#retention)). The API can't delete them; the owner does, with their own account, once a month from the end of 2028:

```powershell
az storage table delete --name Commands202610 --account-name $auditAccount --auth-mode login --fail-not-exist
```
