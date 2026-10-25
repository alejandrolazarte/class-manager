# Domain and DNS

Date: 2026-10-14

Where the `laztty.com` domain is registered, which DNS records it has and why none of them go through the Cloudflare proxy.

## Decision

| Piece | Choice | Why |
|---|---|---|
| Registrar | Cloudflare Registrar | Sells at cost (no markup, renewals at the same price), free WHOIS privacy and DNSSEC |
| DNS | Cloudflare DNS, every record **DNS only** (grey cloud) | See [Why DNS only](#why-dns-only) |
| Web app | `app.laztty.com` → Azure Static Web Apps | See [hosting plan](hosting-plan.md#6-web-app-azure-static-web-apps) |
| API | `api.laztty.com` → Azure Container Apps | Free managed certificate from Container Apps |
| `laztty.com` (apex) | No record yet; planned as a second custom domain of the web app | It must serve `/.well-known/assetlinks.json` for passkeys in the Android app ([Passkeys in the Android app](pilot-deployment.md#passkeys-in-the-android-app)); until a landing page exists it shows the web app. A redirect from Cloudflare would need the proxy, see below |

Hosting and registrar are independent: moving the web app or the API to another host only changes the records below, never the registration.

## Why DNS only

Since February 2025 a court order obtained by LaLiga makes Spanish ISPs (Movistar, Orange, Vodafone, Digi and others) block Cloudflare IP addresses during football matches. Those addresses are shared by many sites, so legitimate sites behind the Cloudflare proxy (orange cloud), Cloudflare Pages or Workers become unreachable from Spain for hours. Customers are expected in Spain, so:

- No record is proxied. A DNS only record answers with the Azure address, and traffic goes straight to Azure without touching Cloudflare's addresses.
- The web app moved from Cloudflare Pages to Azure Static Web Apps for the same reason.

Check the status of the blocking before turning the proxy on for anything.

## Records

| Type | Name | Value | Proxy |
|---|---|---|---|
| `CNAME` | `app` | `<static web app>.azurestaticapps.net` (the default host in the static web app's **Overview**) | DNS only |
| `CNAME` | `api` | `<container app>.<environment>.francecentral.azurecontainerapps.io` (the container app's **Application Url**) | DNS only |
| `TXT` | `asuid.api` | The container app's **Custom domain verification ID** | DNS only |
| `CNAME` | `admin` | The backoffice static web app's default host ([backoffice deployment](backoffice-deployment.md#4-domains)) | DNS only |
| `CNAME` | `admin-api` | The backoffice container app's **Application Url** host | DNS only |
| `TXT` | `asuid.admin-api` | The backoffice container app's **Custom domain verification ID** | DNS only |

## Setup

Run once. `$resourceGroup` and `$containerApp` are the names from the [pilot deployment](pilot-deployment.md#1-names-and-sign-in).

1. Create the `CNAME` and `TXT` records above in Cloudflare (**DNS → Records → Add record**), with the proxy off.
2. Web app: in the static web app, **Custom domains → Add → Custom domain on other DNS**, enter `app.laztty.com`, then **Add**. Validation and the certificate take a few minutes. Or:

   ```powershell
   az staticwebapp hostname set --name class-manager-web --resource-group $resourceGroup --hostname app.laztty.com
   ```

3. API: add the hostname and bind a free managed certificate:

   ```powershell
   az containerapp hostname add --name $containerApp --resource-group $resourceGroup --hostname api.laztty.com
   az containerapp hostname bind --name $containerApp --resource-group $resourceGroup --hostname api.laztty.com --validation-method CNAME
   ```

   In the pilot the portal's **Add custom domain** returned "Internal server error" after validation passed: the hostname was added without a certificate ("No binding"). Creating the managed certificate in the environment (**Certificates → Managed certificate → Add certificate**) and then **Add binding** on the container app finished it.

Check it: `https://api.laztty.com/health` answers `Healthy`, and `https://app.laztty.com` loads the web app.

## Audit commands

```powershell
az staticwebapp hostname list --name class-manager-web --resource-group $resourceGroup --output table
az containerapp hostname list --name $containerApp --resource-group $resourceGroup --output table
Resolve-DnsName app.laztty.com -Type CNAME
Resolve-DnsName api.laztty.com -Type CNAME
```

Expected: both hostnames listed and secured, and both names resolve to a `CNAME` pointing at Azure (a Cloudflare address means the proxy was turned on).
