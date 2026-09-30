# Pilot deployment runbook

Date: 2026-09-26

Step-by-step commands to put class-manager online for the pilot, following the [hosting plan](hosting-plan.md). Run them once, in order, from PowerShell on the owner's PC. After this setup, every merge to `main` deploys by itself (see [Automatic deploys](#automatic-deploys)).

## What the repository already has

| Piece | Where |
|---|---|
| API image | `src/Api/Containerfile` (non-root, port 8080). CI builds it on every pull request |
| Migrations | `scripts/migrate-database.mjs` applies `AppDbContext` and `SecurityDbContext` to the database in `MIGRATIONS_CONNECTION_STRING` |
| Deploy pipeline | `.github/workflows/deploy.yml`, runs after CI passes on `main`; skipped until the `AZURE_CLIENT_ID` variable exists |
| Installable web app (PWA) | `app/public/`: `index.html` template, `manifest.webmanifest`, icons and `service-worker.js`, copied into `dist/` by `expo export` |
| Android pilot build | `app/eas.json`, profile `pilot`: an APK installed from a link (optional, see step 11) |

## Before starting

- An Azure account with a subscription (the free account is enough).
- [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli), .NET 10 SDK and Node.js 22 on the PC.
- A Cloudflare account and an Expo account.

## 1. Names and sign-in

```powershell
az login
$location = "francecentral"
$resourceGroup = "class-manager-rg"
$sqlServer = "class-manager-sql-$(Get-Random -Maximum 99999)"
$database = "ClassManager"
$containerEnvironment = "class-manager-env"
$containerApp = "class-manager-api"
$deployApplication = "class-manager-github-deploy"
$repository = "alejandrolazarte/class-manager"
$subscriptionId = az account show --query id --output tsv
$tenantId = az account show --query tenantId --output tsv
```

The SQL server name must be unique across Azure; write down the generated `$sqlServer`, it goes into GitHub later.

The region comes from the [hosting plan](hosting-plan.md#region). Check that Container Apps and serverless SQL are offered there before creating anything:

```powershell
az provider register --namespace Microsoft.App --wait
az provider register --namespace Microsoft.Sql --wait
az provider show --namespace Microsoft.App --query "resourceTypes[?resourceType=='managedEnvironments'].locations" --output tsv
az sql db list-editions --location $location --edition GeneralPurpose --output table
```

The first list must include "France Central" and the second must show `GP_S_Gen5` objectives. These checks don't prove the free offer is available: only creating the database does (Spain Central passed both checks and still refused it). If step 3 fails, delete the empty server and try the next EU region from the hosting plan.

## 2. Resource group and budget alert

```powershell
az group create --name $resourceGroup --location $location
```

In the portal: **Cost Management → Budgets → Add**, scope `class-manager-rg`, USD 1 per month, email alert at 100%. It's the safety net if something stops being free.

## 3. SQL server and free database

The server only accepts Microsoft Entra sign-ins (no SQL passwords). The owner is its Entra admin; a Gmail or Outlook account works as a guest user of the Azure directory.

```powershell
$owner = az ad signed-in-user show --query "{id:id, name:userPrincipalName}" --output json | ConvertFrom-Json
az sql server create --name $sqlServer --resource-group $resourceGroup --location $location `
  --enable-ad-only-auth --external-admin-principal-type User `
  --external-admin-name $owner.name --external-admin-sid $owner.id

az sql db create --name $database --server $sqlServer --resource-group $resourceGroup `
  --edition GeneralPurpose --family Gen5 --capacity 2 --compute-model Serverless `
  --use-free-limit --free-limit-exhaustion-behavior AutoPause --backup-storage-redundancy Local

az sql server firewall-rule create --name AllowAzureServices --server $sqlServer --resource-group $resourceGroup `
  --start-ip-address 0.0.0.0 --end-ip-address 0.0.0.0
```

`AllowAzureServices` lets the API reach the database. The PC and GitHub get temporary rules only while they run migrations.

## 4. Deploy identity for GitHub Actions

An Entra application that GitHub signs in as through OpenID Connect: no client secret exists, so there is nothing to leak or rotate.

```powershell
$deployClientId = az ad app create --display-name $deployApplication --query appId --output tsv
az ad sp create --id $deployClientId

$repositoryInfo = gh api "repos/$repository" | ConvertFrom-Json
$deploySubject = "repo:$($repositoryInfo.owner.login)@$($repositoryInfo.owner.id)/$($repositoryInfo.name)@$($repositoryInfo.id):ref:refs/heads/main"
$deploySubject

@{
  name = "main-branch"
  issuer = "https://token.actions.githubusercontent.com"
  subject = $deploySubject
  audiences = @("api://AzureADTokenExchange")
} | ConvertTo-Json | Set-Content federated-credential.json
az ad app federated-credential create --id $deployClientId --parameters federated-credential.json
Remove-Item federated-credential.json

$deployObjectId = az ad sp show --id $deployClientId --query id --output tsv
az role assignment create --assignee-object-id $deployObjectId --assignee-principal-type ServicePrincipal --role Contributor `
  --scope "/subscriptions/$subscriptionId/resourceGroups/$resourceGroup"
az role assignment list --assignee-object-id $deployObjectId --all --query "[].[roleDefinitionName, scope]" --output tsv
```

GitHub sends the subject with the owner's and the repository's numeric ids (`repo:alejandrolazarte@55626992/class-manager@1388192765:ref:refs/heads/main`), not just their names, so a repository recreated with the same name can't reuse this trust. `$deploySubject` must print that shape. Entra can take a minute or two to apply a new or changed credential: a deploy started right after fails with `AADSTS700213` and works when re-run.

Assigning by object id avoids a race with the service principal that was just created. The last command must print `Contributor` and the resource group. `Contributor` on the resource group only: the pipeline updates the container app and opens a temporary firewall rule; it can't touch anything outside `class-manager-rg`.

## 5. Container Apps

### Registry token

The image lives in a private package on `ghcr.io`. Container Apps needs a token to pull it. GitHub Packages only accepts **classic** personal access tokens: create one at GitHub → Settings → Developer settings → Personal access tokens (classic), with only the `read:packages` scope and a one-year expiry.

### JWT signing key

```powershell
$keyBytes = New-Object byte[] 48
[Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($keyBytes)
$jwtSigningKey = [Convert]::ToBase64String($keyBytes)
```

Used only by this environment; never the local user-secrets key. Don't save it anywhere else: Container Apps keeps it as a secret.

### Environment and app

`use_dynamic_install` lets the CLI install the `containerapp` extension without an interactive prompt. The environment has no Log Analytics workspace (`--logs-destination none`), so logs cost nothing; live logs still work with `az containerapp logs show`.

The app starts with Microsoft's sample ASP.NET image, which also listens on 8080. The first deploy from GitHub replaces it.

```powershell
az config set extension.use_dynamic_install=yes_without_prompt --only-show-errors
az containerapp env create --name $containerEnvironment --resource-group $resourceGroup --location $location `
  --logs-destination none

$githubPackagesToken = Read-Host "Classic token with read:packages"
$databaseConnectionString = "Server=tcp:$sqlServer.database.windows.net,1433;Database=$database;Authentication=Active Directory Managed Identity;Encrypt=True;Connect Timeout=60;ConnectRetryCount=6;ConnectRetryInterval=20"

az containerapp create --name $containerApp --resource-group $resourceGroup --environment $containerEnvironment `
  --image mcr.microsoft.com/dotnet/samples:aspnetapp --target-port 8080 --ingress external `
  --min-replicas 0 --max-replicas 1 --cpu 0.25 --memory 0.5Gi --system-assigned `
  --registry-server ghcr.io --registry-username alejandrolazarte --registry-password $githubPackagesToken `
  --secrets "jwt-signing-key=$jwtSigningKey" `
  --env-vars "Authentication__Jwt__SigningKey=secretref:jwt-signing-key" "ConnectionStrings__BusinessDatabase=$databaseConnectionString"

$apiHost = az containerapp show --name $containerApp --resource-group $resourceGroup --query properties.configuration.ingress.fqdn --output tsv
"https://$apiHost"
```

The connection string has no password: the app signs in to SQL with its managed identity. `ConnectRetry*` covers the few seconds the free database takes to resume after a pause.

## 6. Database users

Open the database in the portal: **SQL databases → ClassManager → Query editor**, sign in with your Entra account, and run:

```sql
CREATE USER [class-manager-api] FROM EXTERNAL PROVIDER;
ALTER ROLE db_datareader ADD MEMBER [class-manager-api];
ALTER ROLE db_datawriter ADD MEMBER [class-manager-api];

CREATE USER [class-manager-github-deploy] FROM EXTERNAL PROVIDER;
ALTER ROLE db_ddladmin ADD MEMBER [class-manager-github-deploy];
ALTER ROLE db_datareader ADD MEMBER [class-manager-github-deploy];
ALTER ROLE db_datawriter ADD MEMBER [class-manager-github-deploy];
```

| User | Is | Can |
|---|---|---|
| `class-manager-api` | The container app's managed identity | Read and write data; never change the schema |
| `class-manager-github-deploy` | The GitHub Actions identity | Also change the schema (`db_ddladmin`), to run migrations |

## 7. GitHub variables

Repository variables, not secrets: none of these values grants access by itself.

```powershell
gh variable set AZURE_CLIENT_ID --body $deployClientId --repo $repository
gh variable set AZURE_TENANT_ID --body $tenantId --repo $repository
gh variable set AZURE_SUBSCRIPTION_ID --body $subscriptionId --repo $repository
gh variable set AZURE_RESOURCE_GROUP --body $resourceGroup --repo $repository
gh variable set AZURE_CONTAINER_APP --body $containerApp --repo $repository
gh variable set AZURE_SQL_SERVER --body $sqlServer --repo $repository
gh variable set AZURE_SQL_DATABASE --body $database --repo $repository
```

`AZURE_CLIENT_ID` goes last on purpose if you set them by hand: the deploy workflow is skipped while it is empty.

## 8. First deploy

Re-run the latest CI run on `main` from the Actions tab (or merge any pull request). When it passes, the **Deploy** workflow:

1. Builds the image and pushes it to `ghcr.io/alejandrolazarte/class-manager-api:<commit>`.
2. Signs in to Azure with OpenID Connect and opens the SQL firewall to the runner's IP.
3. Applies the migrations, then closes the firewall rule. The free database pauses when idle and answers `40613 Database 'ClassManager' is not currently available` while it resumes (about a minute), so `scripts/migrate-database.mjs` retries each context on that error only, up to 4 attempts 30 seconds apart. Any other migration error fails the deploy on the first attempt.
4. Points the container app at the new image and waits for `/health`.

Check it: `https://<api host>/health` answers `Healthy`.

The first image push creates the `class-manager-api` package on GitHub as private and linked to the repository. Leave it private.

## 9. Web app (Cloudflare Pages)

In Cloudflare: **Compute → Workers & Pages → Create**. The create screen defaults to Workers; use **Continue to Pages** (the "legacy Pages workflow" link at the bottom), then **Connect to Git** with access to `alejandrolazarte/class-manager` only, project name `class-manager`. The pilot got `class-manager-3cm.pages.dev` because `class-manager` was taken.

| Setting | Value |
|---|---|
| Production branch | `main` |
| Root directory | `app` |
| Build command | `corepack enable && pnpm install --frozen-lockfile && pnpm export:web` |
| Build output directory | `dist` |
| Environment variable `NODE_VERSION` | `22` |
| Environment variable `EXPO_PUBLIC_API_BASE_URL` | `https://<api host>` |

Cloudflare builds on its own servers on every push to `main`; no GitHub Action is involved. `pnpm export:web` runs `expo export --platform web` and then `scripts/relocateNodeModulesAssets.js`: Expo writes package assets (the Ionicons font, navigation icons) under `dist/assets/node_modules/`, and Pages does not publish folders named `node_modules`, so the script moves them to `dist/assets/vendor/` and rewrites the references. Without it the icons render as empty boxes. Pages serves `index.html` for any path, which is what the single-page web build needs.

Then allow the Pages domain in the API's CORS list:

```powershell
az containerapp update --name $containerApp --resource-group $resourceGroup `
  --set-env-vars "Cors__AllowedOrigins__0=https://class-manager.pages.dev" "WebApp__Url=https://class-manager.pages.dev"
```

`WebApp__Url` is where the links in emails point (see [Email](#email)).

Use the domain Cloudflare actually assigned if `class-manager` was taken.

## 10. Installing the app on phones (PWA)

The web app from step 9 is also an installable app (a Progressive Web App). Instructors install it from the browser: no APK, no "allow installs from unknown sources", no store. It updates itself: every push to `main` rebuilds Pages, and the next time the app opens it loads the new version.

**Android (Chrome):** open `https://<pages domain>`, then **⋮ → Install app** (or the "Install" banner Chrome shows). The app appears in the launcher with its icon and opens full screen.

**iPhone:** open the same link in Safari, tap **Share → Add to Home Screen → Add**. Since iOS 16.4 the Share menu in other browsers (Chrome, Edge) offers it too.

What makes it installable, all under `app/public/` (Expo copies the folder into `dist/`):

| File | Why |
|---|---|
| `index.html` | Replaces Expo's default HTML template: Spanish `lang`, `theme-color`, the manifest link, the Apple home-screen tags, and the service worker registration |
| `manifest.webmanifest` | Name, icons, colors and `display: standalone` (full screen, no browser bar) |
| `icons/` | 192 and 512 px icons for Android (the 512 one also as `maskable`) and the 180 px `apple-touch-icon` for iPhone, resized from `assets/images/icon.png` |
| `service-worker.js` | Only handles page navigations: always asks the network first, so a deploy is never hidden behind a stale copy. API calls, scripts and images are not intercepted. It caches the page only as an offline fallback |

The icons are copies of the app icon: when the icon changes, regenerate them at the same sizes.

Check it after a deploy: Chrome DevTools → **Application → Manifest** shows no errors and **Service workers** shows `service-worker.js` activated. Lighthouse's PWA checks were removed from recent Chrome versions; the Application tab is the reference.

## 11. Android APK (optional)

Not needed for the pilot: the PWA covers it. Kept for testing native-only behaviour, and as the base for Google Play and the App Store later. Installing an APK from a link makes Android ask to allow installs from unknown sources, which is why it isn't the default.

```powershell
cd app
pnpm dlx eas-cli login
pnpm dlx eas-cli init
pnpm dlx eas-cli env:create --environment preview --name EXPO_PUBLIC_API_BASE_URL --value "https://<api host>" --visibility plaintext
pnpm dlx eas-cli build --platform android --profile pilot
```

`eas init` asks which Expo account owns the project (the pilot uses the personal account `alejandro-lazarte`), links the app to the Expo project and adds `extra.eas.projectId` and `owner` to `app.json`; commit that change in a pull request. `eas build` asks to generate an Android keystore the first time: accept, Expo stores it. The build ends with a link and a QR code: pilot instructors open it on their phone and install the APK. The Android package id is `com.alejandrolazarte.classmanager`; it can't change once the app is on Google Play.

The API URL is baked into the APK at build time, so changing the API host means building a new APK.

## Email

The API sends email (today, only "¿Olvidaste tu contraseña?" links) through SMTP. The pilot uses a Gmail account; any SMTP provider works by changing the same settings, and another kind of provider only needs a new `IEmailSender` in `src/Infrastructure/Email`.

1. Create a Gmail account for the app (for example `classmanager.app@gmail.com`).
2. In that account: **Security → 2-Step Verification** on, then **App passwords** (<https://myaccount.google.com/apppasswords>) → create one named `class-manager`. Copy the 16 letters.
3. Give them to the API:

```powershell
$gmailAddress = "classmanager.app@gmail.com"
az containerapp secret set --name $containerApp --resource-group $resourceGroup --secrets "smtp-password=<16-letter app password>"
az containerapp update --name $containerApp --resource-group $resourceGroup `
  --set-env-vars "Email__Smtp__Host=smtp.gmail.com" "Email__Smtp__Port=587" `
  "Email__Smtp__UserName=$gmailAddress" "Email__Smtp__Password=secretref:smtp-password" `
  "Email__Smtp__FromName=Class Manager"
```

Gmail sends from the account's own address and allows about 500 emails per day, plenty for the pilot. Test it with **¿Olvidaste tu contraseña?** on the web app: the email arrives from the Gmail address with a link to `WebApp__Url/reset-password?token=…`, which works once and expires after 24 hours.

Without `Email__Smtp__Host` the API doesn't send anything: it logs a warning, and the email body at `Debug` level, which is how local development gets the link (`appsettings.Development.example.json` turns that on).

## Push notifications

Families can turn on notifications in **Ajustes → Notificaciones** of the web app. They then get a notification on that device when the school publishes an announcement, cancels one of their classes, a coach leaves a comment, or an order is ready. The notification opens the matching screen of the app.

The API signs these with a VAPID key pair (Web Push, RFC 8292) and encrypts them itself (RFC 8291), with no third-party service. Generate the keys once and keep them: changing them makes every existing subscription stop working until each family turns notifications on again.

```powershell
npx web-push generate-vapid-keys
az containerapp secret set --name $containerApp --resource-group $resourceGroup --secrets "vapid-private-key=<Private Key>"
az containerapp update --name $containerApp --resource-group $resourceGroup `
  --set-env-vars "WebPush__Vapid__Subject=mailto:$gmailAddress" "WebPush__Vapid__PublicKey=<Public Key>" `
  "WebPush__Vapid__PrivateKey=secretref:vapid-private-key"
```

Without these settings the API sends nothing and the app hides the option. On iPhone, notifications only work when the family opens the app from the home screen (Compartir → Agregar a inicio, iOS 16.4 or later); the app explains this when the browser can't show them. Subscriptions that the push service reports as gone are deleted automatically.

## Automatic deploys

| On | What happens |
|---|---|
| Pull request | CI builds and tests; also builds the API image (without pushing) |
| Merge to `main`, CI green | `deploy.yml`: image to `ghcr.io`, migrations, container app updated |
| Push to `main` | Cloudflare Pages rebuilds the web app |
| Push to `main` | Installed PWAs pick up the new web app the next time they open |
| New APK (optional) | Manual: `eas build --profile pilot`, only when native behaviour needs testing |

Migrations run before the new image starts, while the previous version is still serving. If a migration fails, the new image is never deployed but Cloudflare Pages still publishes the new web app, so the web and the API can be out of step until the next successful deploy: check the **Deploy** run after every merge. Keep them additive (add columns and tables); dropping or renaming something the running version uses takes two deploys.

## Audit commands

```powershell
az containerapp show --name $containerApp --resource-group $resourceGroup --query "{image:properties.template.containers[0].image, minReplicas:properties.template.scale.minReplicas, identity:identity.type}"
az containerapp secret list --name $containerApp --resource-group $resourceGroup --query "[].name"
az sql server firewall-rule list --server $sqlServer --resource-group $resourceGroup --output table
az sql db show --name $database --server $sqlServer --resource-group $resourceGroup --query "{free:useFreeLimit, whenExhausted:freeLimitExhaustionBehavior}"
az role assignment list --assignee-object-id $deployObjectId --all --output table
gh variable list --repo $repository
```

Expected: the image is `ghcr.io/...:<commit>`, `minReplicas` is 0, the only secrets are `jwt-signing-key`, `smtp-password` and the registry password, only `AllowAzureServices` remains in the firewall, and the database uses the free limit with `AutoPause`.

## Rotating secrets

- **JWT key:** `az containerapp secret set ... --secrets jwt-signing-key=<new key>` and restart the revision. Everyone is signed out once; refresh tokens keep working.
- **Gmail app password:** create a new one in the Gmail account, `az containerapp secret set ... --secrets smtp-password=<new>`, restart the revision, then delete the old app password.
- **Registry token:** create a new classic token, then `az containerapp registry set --name $containerApp --resource-group $resourceGroup --server ghcr.io --username alejandrolazarte --password <token>`. Do it before the old one expires or new deploys fail to pull the image.
