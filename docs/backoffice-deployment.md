# Backoffice deployment

Date: 2026-10-20 (deployed the same day, see [How it went](#how-it-went))

How the backoffice reaches production: a second container app for `Backoffice.Api` (`admin-api.laztty.com`) and a second static web app for its web build (`admin.laztty.com`), next to the app's. Run once, in this order. Who may enter afterwards is in [backoffice access](backoffice-access.md); the design in the [backoffice plan](backend/20261017-backoffice/plan.md#deployment-and-cost).

`$resourceGroup`, `$containerEnvironment`, `$sqlServer`, `$database`, `$repository` and `$githubPackagesToken` are the ones from the [pilot deployment](pilot-deployment.md#1-names-and-sign-in). Both pieces cost nothing within the free allowances, like the app's ([hosting plan](hosting-plan.md)).

## What the deploy does

`deploy.yml`, after the API's job (which applies every migration, the `backoffice` schema included):

| Job | Runs when | Does |
|---|---|---|
| `image` | always | Also builds and pushes `ghcr.io/alejandrolazarte/class-manager-backoffice-api:<commit>` from `src/Backoffice.Api/Containerfile` |
| `backoffice-api` | `AZURE_BACKOFFICE_CONTAINER_APP` is set | Points the container app at the new image and waits until the new revision is ready and answers `/health` (`scripts/wait-for-container-app-revision.sh`) |
| `backoffice-web` | `AZURE_BACKOFFICE_STATIC_WEB_APP` is set | `pnpm export:backoffice` with `EXPO_PUBLIC_API_BASE_URL` = `BACKOFFICE_API_BASE_URL`, uploads `app/dist-backoffice` |

Until the variables exist the jobs are skipped, so the app's deploy is unaffected. The backoffice image is built and pushed only once `AZURE_BACKOFFICE_CONTAINER_APP` is set.

## 1. Container app

A new signing key, never the API's: a leaked key of one host must not mint tokens for the other. The Google client secret is the **Laztty Backoffice** client's ([backoffice access](backoffice-access.md#google-client)). The organization id comes from:

```sql
SELECT Id, Name, CreatedAt FROM Organizations WHERE Name LIKE N'%Laztty%';
```

```powershell
$backofficeContainerApp = "class-manager-backoffice-api"
$keyBytes = New-Object byte[] 48
[Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($keyBytes)
$backofficeSigningKey = [Convert]::ToBase64String($keyBytes)
$googleClientSecret = Read-Host "Laztty Backoffice client secret"
$platformOrganizationId = Read-Host "Laztty organization id"
$databaseConnectionString = "Server=tcp:$sqlServer.database.windows.net,1433;Database=$database;Authentication=Active Directory Managed Identity;Encrypt=True;Connect Timeout=60;ConnectRetryCount=6;ConnectRetryInterval=20"

az containerapp create --name $backofficeContainerApp --resource-group $resourceGroup --environment $containerEnvironment `
  --image mcr.microsoft.com/dotnet/samples:aspnetapp --target-port 8080 --ingress external `
  --min-replicas 0 --max-replicas 1 --cpu 0.25 --memory 0.5Gi --system-assigned `
  --registry-server ghcr.io --registry-username alejandrolazarte --registry-password $githubPackagesToken `
  --secrets "jwt-signing-key=$backofficeSigningKey" "google-client-secret=$googleClientSecret" `
  --env-vars "Authentication__Jwt__SigningKey=secretref:jwt-signing-key" `
    "Backoffice__Google__ClientSecret=secretref:google-client-secret" `
    "ConnectionStrings__BusinessDatabase=$databaseConnectionString" `
    "Backoffice__PlatformOrganizationId=$platformOrganizationId" `
    "Backoffice__WebAppUrl=https://admin.laztty.com" `
    "Cors__AllowedOrigins__0=https://admin.laztty.com"
```

`Backoffice__PlatformOrganizationId` is the number in `Organizations.Id` (`SELECT Id, Name FROM Organizations`). A database recreated from scratch gives Laztty a new id, so update the variable then. A value the app can't read as a number (an id from before ids were numbers, for example) stops every new revision from starting.

It starts with Microsoft's sample image, like the API did; the first deploy replaces it. Rotating the Google secret later is one line:

```powershell
az containerapp secret set --name $backofficeContainerApp --resource-group $resourceGroup --secrets "google-client-secret=$googleClientSecret"
```

## 2. Database user

In the Query editor, with your Entra account. The same rights as the API: it reads across businesses, and writes refresh tokens, access requests and, later, corrections. Never the schema.

```sql
CREATE USER [class-manager-backoffice-api] FROM EXTERNAL PROVIDER;
ALTER ROLE db_datareader ADD MEMBER [class-manager-backoffice-api];
ALTER ROLE db_datawriter ADD MEMBER [class-manager-backoffice-api];
```

## 3. Static web app

Source **Other**, never connected to GitHub from the portal ([why](pilot-deployment.md#9-web-app-azure-static-web-apps)):

```powershell
$backofficeStaticWebApp = "class-manager-backoffice-web"
az staticwebapp create --name $backofficeStaticWebApp --resource-group $resourceGroup --location eastus2 --sku Free
```

The region only holds the resource's metadata: the files are served from Azure's global network wherever it is. `westeurope` answered `RequestDisallowedByAzure` ("not accepting new customers") on 2026-10-20, so the backoffice's static web app is in `eastus2`.

## 4. Domains

Records in Cloudflare, every one **DNS only** ([why](domain-and-dns.md#why-dns-only)):

| Type | Name | Value |
|---|---|---|
| `CNAME` | `admin` | The backoffice static web app's default host (**Overview**) |
| `CNAME` | `admin-api` | The backoffice container app's **Application Url** host |
| `TXT` | `asuid.admin-api` | The backoffice container app's **Custom domain verification ID** |

```powershell
az staticwebapp hostname set --name $backofficeStaticWebApp --resource-group $resourceGroup --hostname admin.laztty.com
az containerapp hostname add --name $backofficeContainerApp --resource-group $resourceGroup --hostname admin-api.laztty.com
az containerapp hostname bind --name $backofficeContainerApp --resource-group $resourceGroup --hostname admin-api.laztty.com --validation-method CNAME
```

If the bind fails as it did for `api` ([domain and DNS](domain-and-dns.md#setup)), create the managed certificate in the environment and add the binding from the portal. `admin` and `admin-api` are the same site (`laztty.com`), which the `SameSite=Strict` session cookie needs.

## 5. GitHub variables

Repository variables; none grants access by itself:

```powershell
gh variable set BACKOFFICE_API_BASE_URL --repo $repository --body "https://admin-api.laztty.com"
gh variable set AZURE_BACKOFFICE_STATIC_WEB_APP --repo $repository --body $backofficeStaticWebApp
gh variable set AZURE_BACKOFFICE_CONTAINER_APP --repo $repository --body $backofficeContainerApp
```

`AZURE_BACKOFFICE_CONTAINER_APP` last: it turns the backoffice jobs on.

## 6. First deploy and first operator

Re-run the latest CI run on `main` (or merge any pull request). Then:

1. `https://admin-api.laztty.com/health` answers `Healthy`.
2. `https://admin.laztty.com` shows **Laztty Admin** with **Entrar con Google**.
3. Sign in: Google asks for the account, and the page comes back with "Acceso no permitido" and a code.
4. Enable yourself with `scripts/enable-first-operator.sql` and that code ([the first operator](backoffice-access.md#the-first-operator)), then sign in again: the page shows "Entraste como …".

The first push creates the `class-manager-backoffice-api` package on GitHub as private. Leave it private.

## Not yet

- The logs account roles and URIs for the Registros tab: [audit log storage, section 9](audit-log-storage.md#9-the-backoffice).
- `X-Robots-Tag: noindex` on the backoffice's static web app. The backoffice API already sends the app's security headers (`SecurityHeadersMiddleware`, now in `Infrastructure`).

## How it went

Deployed on 2026-10-20 from a phone, with Azure Cloud Shell (PowerShell) in the browser. What to know for next time:

- **Paste one line.** Pasting several lines from a phone joins them into one; every command was run as a single line with `;` between statements, and the console couldn't be kept between steps, so each line read again what it needed (for example the API's connection string and registry token):

  ```powershell
  $rg="class-manager-rg"; $api="class-manager-api"; $envId=az containerapp show -n $api -g $rg --query properties.environmentId -o tsv; $conn=az containerapp show -n $api -g $rg --query "properties.template.containers[0].env[?name=='ConnectionStrings__BusinessDatabase'].value | [0]" -o tsv; $regPass=az containerapp secret list -n $api -g $rg --show-values --query "[?contains(name,'ghcr')].value | [0]" -o tsv
  ```

- Cloud Shell opens in **PowerShell**; Bash syntax (`name=value`) fails there with "is not recognized as a name of a cmdlet".
- `asuid.admin-api` has the same value as `asuid.api`: the verification ID belongs to the container apps environment, which both share.
- The managed certificate for `admin-api` was created and bound by `az containerapp hostname bind` on the first try.
- Google answered `redirect_uri_mismatch` until the redirect URI was right in the client ([backoffice access](backoffice-access.md#google-client)).
- The owner is a team member of two Laztty branches, so `scripts/enable-first-operator.sql` takes `@BranchName` to choose where the platform role goes.
