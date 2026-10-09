# Database on the Basic tier

Date: 2026-10-09

The difference between the DTU and vCore purchasing models, what the Azure SQL **Basic** tier costs and includes, and the runbook to move the pilot database from the free offer to Basic for a trial month and back. Variables (`$resourceGroup`, `$sqlServer`, `$database`, `$containerApp`) are the ones from step 1 of the [pilot deployment runbook](pilot-deployment.md#1-names-and-sign-in).

## DTU and vCore

Azure SQL Database sells the same SQL Server engine through two purchasing models. They differ in how compute is chosen and billed, not in what the database can store or run.

| | DTU | vCore |
|---|---|---|
| What you buy | A bundle of CPU, memory and IO expressed as one number (Database Transaction Units) | A number of virtual CPU cores; memory follows from the hardware |
| Sizes | Fixed list: Basic (5 DTU), Standard S0–S12 (10–3000 DTU), Premium P1–P15 | Any core count within a tier: General Purpose, Business Critical, Hyperscale |
| Storage | Included up to the tier's size (2 GB on Basic, 250 GB on Standard) | Billed separately per GB |
| Compute modes | Provisioned only: online all the time | Provisioned, or **serverless**: scales with load, auto-pauses when idle, billed per second online |
| Price | Fixed per month | Per core-hour (provisioned) or per vCore-second (serverless) |
| Smallest size | Basic, about 1/20 of a core, about USD 5 a month | 0.5 vCore serverless, about USD 190 a month if online all month |
| Good for | Small, steady workloads that must stay online at a predictable price | Workloads that need more power, pause for long periods, or reuse existing SQL Server licenses (Azure Hybrid Benefit) |

Microsoft's rule of thumb for comparing them: **100 DTU on Standard ≈ 1 vCore on General Purpose** (125 DTU on Premium ≈ 1 vCore on Business Critical).

Where class-manager sits:

- **Today:** the free offer is **vCore serverless**: plenty of power, few online hours per month, auto-pause.
- **Online all the time and cheap:** **DTU Basic**: little power, always online, fixed price.
- **When several businesses use it all day:** Standard S0/S1 or vCore provisioned. Switching between models is a setting change that keeps the data; the only one-way step is leaving the free offer.

## Why Basic

The free offer gives 100,000 vCore-seconds per month. A serverless database uses at least 0.5 vCore while it is online, so the allowance covers about 55 online hours per month; a database online all month needs about 1,296,000. When a business uses the app all day, the free database runs out mid-month (see [hosting plan](hosting-plan.md#when-to-leave-the-free-setup)).

| Option | Online | Cost per month (list price) |
|---|---|---|
| Free offer (serverless, auto-pause) | Only while used; first request after a pause waits for it to resume | 0, until the allowance runs out |
| **Basic (5 DTU)** | **Always** | **About USD 5, fixed** |
| Serverless billed over the free allowance | While used | About USD 0.26 per online hour (0.5 vCore minimum): 16 hours a day is about USD 110–125 |
| Standard S0 (10 DTU) | Always | About USD 15, fixed |

Basic is the cheapest way to keep the database online all the time. It can't be paused or scheduled: Azure SQL Database has no stop/start, and auto-pause only exists in serverless, which costs more per hour than Basic costs per month.

Prices change and vary by region. Check the current price for the pilot region before deciding:

```powershell
$priceFilter = "serviceName eq 'SQL Database' and armRegionName eq 'francecentral' and skuName eq 'B' and priceType eq 'Consumption'"
(Invoke-RestMethod "https://prices.azure.com/api/retail/prices?currencyCode='EUR'&`$filter=$priceFilter").Items |
  Select-Object productName, meterName, unitOfMeasure, retailPrice, currencyCode
```

## What Basic includes

The price is **only the database**. It is billed per hour while the database exists, whatever the number of queries, connections or CPU used.

| Included | Limit |
|---|---|
| Compute | 5 DTU, less than one vCore. When the app asks for more, queries slow down; nothing extra is billed |
| Data | 2 GB, included and maximum. At 2 GB writes fail; it never grows or bills on its own. Photos live in Blob Storage, not in the database |
| Connections | 30 concurrent workers, 30 concurrent logins, 300 sessions |
| Backups | Automatic, point-in-time restore for up to 7 days, included |
| Engine | The same SQL Server engine as today: migrations, filtered indexes and query filters work unchanged. No columnstore and no in-memory OLTP, which the app doesn't use |
| Storage | Hard disk (Standard Page Blobs): fine for a pilot, slower than the SSD of the serverless tier |
| Availability | Online all the time, with an SLA. The free offer has no SLA |

Not included, and only billed if turned on: long-term backup retention, geo-replication, and data leaving Azure beyond the free monthly allowance.

The rest of the hosting is unchanged and still costs nothing or cents (see [hosting plan](hosting-plan.md#architecture)): the API on Container Apps, the web app on Cloudflare Pages, photos on Blob Storage. Moving to Basic adds about USD 5 a month to the bill and nothing else.

## Things that can't be undone

- A free offer database moved to a paid tier **can't go back to the free offer**. Going back means a new free database and moving the data with a `.bacpac` (see [Going back to the free offer](#going-back-to-the-free-offer)).
- Azure's automatic backups stay in Azure: they can't be downloaded, and restoring one always creates a **paid** database. A deleted database can be restored only within its retention (7 days). Deleting the server deletes every backup. The `.bacpac` exported below is the only copy that is fully under the owner's control.

## Before starting

- [SqlPackage](https://learn.microsoft.com/sql/tools/sqlpackage/sqlpackage-download) on the PC: `dotnet tool install --global microsoft.sqlpackage`.
- Signed in with `az login` as the server's Entra admin. The SqlPackage connection strings below use `Active Directory Default`, which reuses that sign-in.
- The database must be under 2 GB: check **SQL databases → ClassManager → Overview → Data space used** in the portal.

## 1. Maintenance window and firewall

A `.bacpac` export is not transactionally consistent while the app writes. Stop traffic first, at a time nobody uses the app:

```powershell
az containerapp ingress disable --name $containerApp --resource-group $resourceGroup
$myIp = (Invoke-RestMethod "https://api.ipify.org")
az sql server firewall-rule create --name OwnerPc --server $sqlServer --resource-group $resourceGroup `
  --start-ip-address $myIp --end-ip-address $myIp
```

With ingress disabled no request reaches the API, so the container scales to zero and its background workers stop too.

## 2. Back up the free database

```powershell
$backupFolder = "$HOME\class-manager-backups"
New-Item -ItemType Directory -Force $backupFolder | Out-Null
$backupFile = "$backupFolder\ClassManager-free-$(Get-Date -Format yyyyMMdd-HHmm).bacpac"
$sqlConnectionString = "Server=tcp:$sqlServer.database.windows.net,1433;Database=$database;Authentication=Active Directory Default;Encrypt=True;Connect Timeout=60"

SqlPackage /Action:Export /SourceConnectionString:$sqlConnectionString /TargetFile:$backupFile
```

Keep this file somewhere safe (a second disk or a private Blob container). It is the way back if anything below goes wrong.

## 3. Rehearse the way back (once, before the trial)

Prove that the `.bacpac` imports into a free database before giving up the current one. A new free database costs nothing.

```powershell
$rehearsalDatabase = "ClassManagerRehearsal"
az sql db create --name $rehearsalDatabase --server $sqlServer --resource-group $resourceGroup `
  --edition GeneralPurpose --family Gen5 --capacity 2 --compute-model Serverless `
  --use-free-limit --free-limit-exhaustion-behavior AutoPause --backup-storage-redundancy Local

$rehearsalConnectionString = "Server=tcp:$sqlServer.database.windows.net,1433;Database=$rehearsalDatabase;Authentication=Active Directory Default;Encrypt=True;Connect Timeout=60"
SqlPackage /Action:Import /SourceFile:$backupFile /TargetConnectionString:$rehearsalConnectionString
```

In the portal's **Query editor** on `ClassManagerRehearsal`, check that the data is there and that the app users came along:

```sql
SELECT COUNT(*) FROM [__EFMigrationsHistory];
SELECT name, type_desc FROM sys.database_principals WHERE name IN ('class-manager-api', 'class-manager-github-deploy');
```

Both users must be listed. If they are not, the way back needs the `CREATE USER ... FROM EXTERNAL PROVIDER` statements from [step 6 of the pilot runbook](pilot-deployment.md#6-database-users). Then delete the rehearsal database:

```powershell
az sql db delete --name $rehearsalDatabase --server $sqlServer --resource-group $resourceGroup --yes
```

## 4. Move to Basic

The database keeps its name, data, users and connection string; only the tier changes. The API and the deploy pipeline need no change.

```powershell
az sql db update --name $database --server $sqlServer --resource-group $resourceGroup `
  --use-free-limit false --edition Basic --service-objective Basic --max-size 2GB
```

In the portal the same change is **SQL databases → ClassManager → Compute + storage**: turn off the free offer, choose **Basic**, **Apply**. If the CLI refuses to change both in one call, use the portal.

Then reopen the app and close the firewall:

```powershell
az containerapp ingress enable --name $containerApp --resource-group $resourceGroup --type external --target-port 8080
az sql server firewall-rule delete --name OwnerPc --server $sqlServer --resource-group $resourceGroup
```

Raise the Cost Management budget from USD 1 to **USD 7** per month, so it still warns when something else starts billing.

## Going back to the free offer

When the trial ends and the database should be free again. Run [step 1](#1-maintenance-window-and-firewall) first.

```powershell
$backupFile = "$backupFolder\ClassManager-basic-$(Get-Date -Format yyyyMMdd-HHmm).bacpac"
SqlPackage /Action:Export /SourceConnectionString:$sqlConnectionString /TargetFile:$backupFile

az sql db delete --name $database --server $sqlServer --resource-group $resourceGroup --yes

az sql db create --name $database --server $sqlServer --resource-group $resourceGroup `
  --edition GeneralPurpose --family Gen5 --capacity 2 --compute-model Serverless `
  --use-free-limit --free-limit-exhaustion-behavior AutoPause --backup-storage-redundancy Local

SqlPackage /Action:Import /SourceFile:$backupFile /TargetConnectionString:$sqlConnectionString
```

The new database keeps the name `ClassManager`, so the API's connection string and the `AZURE_SQL_DATABASE` GitHub variable stay valid. Check the users as in [step 3](#3-rehearse-the-way-back-once-before-the-trial), then reopen the app and close the firewall as at the end of [step 4](#4-move-to-basic), and set the budget back to USD 1.

Billing is per hour, so the Basic database costs only the days it existed. For 7 days after the delete, the Basic database can still be restored from **SQL servers → Deleted databases**, as a paid database.

## Audit commands

```powershell
az sql db show --name $database --server $sqlServer --resource-group $resourceGroup `
  --query "{tier:sku.tier, objective:currentServiceObjectiveName, maxSizeBytes:maxSizeBytes, free:useFreeLimit, status:status}"
az sql server firewall-rule list --server $sqlServer --resource-group $resourceGroup --output table
az containerapp ingress show --name $containerApp --resource-group $resourceGroup --query "{external:external, port:targetPort}"
```

Expected on Basic: `tier` is `Basic`, `objective` is `Basic`, `maxSizeBytes` is `2147483648`, `free` is `false` or empty, `status` is `Online`. Only `AllowAzureServices` remains in the firewall, and ingress is external on port 8080. After going back: `tier` is `GeneralPurpose`, `free` is `true`, as in the [pilot runbook audit](pilot-deployment.md#audit-commands).

## Sources

- [Resource limits for single databases using the DTU purchasing model](https://learn.microsoft.com/azure/azure-sql/database/resource-limits-dtu-single-databases?view=azuresql)
- [DTU-based purchasing model overview](https://learn.microsoft.com/azure/azure-sql/database/service-tiers-dtu?view=azuresql)
- [Purchasing models: DTU and vCore](https://learn.microsoft.com/azure/azure-sql/database/purchasing-models?view=azuresql) and [migrating from DTU to vCore](https://learn.microsoft.com/azure/azure-sql/database/migrate-dtu-to-vcore?view=azuresql)
- [Azure SQL Database free offer](https://learn.microsoft.com/azure/azure-sql/database/free-offer?view=azuresql) and [FAQ](https://learn.microsoft.com/azure/azure-sql/database/free-offer-faq?view=azuresql)
- [SqlPackage Import](https://learn.microsoft.com/sql/tools/sqlpackage/sqlpackage-import?view=sql-server-ver17) and [Export](https://learn.microsoft.com/sql/tools/sqlpackage/sqlpackage-export?view=sql-server-ver17)
- [Azure SQL Database pricing](https://azure.microsoft.com/pricing/details/azure-sql-database/single/)
