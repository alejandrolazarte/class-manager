# Storage library and catalog images

Date: 2026-10-03

Product and class pack photos are files in **Azure Blob Storage**. The database keeps only each photo's public URL (`Products.ImageUrl`, `ClassPacks.ImageUrl`). The browser and the phone download the photo straight from Blob Storage, so photos never go through the API or the database.

`src/Storage` and `src/Storage.AzureBlob` are a reusable library like [tenancy](tenancy.md), [notifications](notifications.md) and [import and export](import-export.md). They save, serve and delete files and recognize image formats. They know nothing about products, packs or businesses. ARCH008 keeps them free of `ClassManager.Core`, `Infrastructure`, `Api`, `Security`, `Tenancy`, `ImportExport`, `Notifications` and `Subscriptions` (see [analyzers](analyzers.md)).

## Why Blob Storage, and is anything cheaper

| Option | Storage price (approx., hot tier) | Downloads | Notes |
|---|---|---|---|
| **Azure Blob Storage (LRS, hot)** | about USD 0.02 per GB per month | First 100 GB per month of internet egress free per subscription, then billed per GB | Same Azure subscription and bill as the API and the database. Managed identity can replace the connection string later |
| Cloudflare R2 | about USD 0.015 per GB per month, first 10 GB free | No egress fees | Cheaper, especially if photos are downloaded a lot. It uses an S3-compatible API, so it needs an S3 client and a second set of credentials |
| Database (`varbinary`) | Counts against the 32 GB of the free Azure SQL offer | Every download costs API and database time | Used for the brand logo (one small file per business, up to 512 KB). Wrong for many photos of up to 5 MB |

Prices change; check the Azure Blob Storage and Cloudflare R2 pricing pages before relying on them.

At pilot scale both are almost free. For example, 500 photos of 300 KB are about 150 MB, which costs well under one cent a month on either service. **Decision: Azure Blob Storage**, because the hosting is already on Azure and the setup is simpler. If downloads ever make egress the main cost, write a `Storage.S3` adapter for R2 that implements `IFileStorage` (see [Changing provider](#changing-provider)). Nothing else would change.

## Contents

### `src/Storage` (no package dependencies)

| Type | Purpose |
|---|---|
| `Files/IFileStorage` | `SaveAsync(FileToStore)` returns the file's public URL. `DeleteAsync(Uri)` deletes it and returns `false` when the URL is not in this storage or the file is already gone |
| `Files/FileToStore` | Path, content and content type of a file to save |
| `Files/FilePath` | `Combine(segments)` joins path segments with `/` and rejects empty, `.`, `..` and segments containing a slash |
| `Images/ImageFormat` | Detects PNG, JPEG and WebP from the file's first bytes (never trusts the name or the declared type). Also used by the brand logo |

### `src/Storage.AzureBlob` (Azure.Storage.Blobs)

| Type | Purpose |
|---|---|
| `AzureBlobFileStorage` | Saves blobs with their content type and `Cache-Control: public, max-age=31536000, immutable`. The first save creates the container if it is missing, with anonymous read access to blobs (not to the container listing) |
| `BlobStorageOptions` | `FileStorage:ContainerName` (default `public-files`) and `FileStorage:CacheControl` |
| `FileStorageServiceCollectionExtensions` | `AddAzureBlobFileStorage()` reads the `FileStorage` connection string. It fails when the storage is first used without one, not at startup, so the API and the e2e tests run without storage until a photo is uploaded |

## What stays in the app

| In `Core` | In `Infrastructure` |
|---|---|
| `CatalogImage` (format check, 5 MB limit, error codes `image.unsupported` and `image.too_large`), `ImageUrl` on `Product` and `ClassPack`, the set and remove use cases, `ICatalogImageService` | `CatalogImageService`: builds the path, saves through `IFileStorage`, and logs (does not throw) when a delete fails |

Paths are `<tenantId>/<products|class-packs>/<ownerId>/<new id>.<extension>`. Each upload gets a new file name, so a browser never shows a cached old photo, and the long cache header is safe.

Flow of `PUT /api/products/{id}/image` (same for `/api/class-packs/{id}/image`):

1. Load the product. Tenant query filters return 404 for another business's product.
2. Check that the file is PNG, JPG or WebP and at most 5 MB (400 otherwise).
3. Upload the new file, save its URL, then delete the previous file.
4. If saving to the database fails, delete the file just uploaded, so it is not left orphaned.

`DELETE .../image` clears the URL and deletes the file. Both endpoints need the same permission as editing the product or pack.

**Photos are public.** Anyone with a URL can open it. URLs contain random ids and can't be guessed or listed, and shop photos are meant to be seen by families. Don't use this storage for private documents (medical certificates, IDs); those need a private container and short-lived SAS links.

## Configuration

| Setting | Value |
|---|---|
| `ConnectionStrings:FileStorage` | Storage account connection string (secret) |
| `FileStorage:ContainerName` | Optional, default `public-files` |
| `FILES_BASE_URL` (web build) | Optional. The blob endpoint, for example `https://<account>.blob.core.windows.net`. `app/scripts/writeWebHeaders.js` adds its origin to `img-src` in the [Content Security Policy](frontend/content-security-policy.md) |

### Local development

Run Azurite, the Blob Storage emulator, next to the development database:

```powershell
podman run -d --name class-manager-azurite -p 10000:10000 mcr.microsoft.com/azure-storage/azurite azurite-blob --blobHost 0.0.0.0 --skipApiVersionCheck
dotnet user-secrets set "ConnectionStrings:FileStorage" "UseDevelopmentStorage=true" --project src/Api
```

Afterwards, `podman start class-manager-azurite` is enough. `--skipApiVersionCheck` is needed because the Azure SDK is often newer than the latest Azurite release.

### Azure

```powershell
az storage account create --name <account> --resource-group <group> --location <region> --sku Standard_LRS --kind StorageV2 --access-tier Hot --min-tls-version TLS1_2 --allow-blob-public-access true
az storage account show-connection-string --name <account> --resource-group <group> --query connectionString -o tsv
az containerapp secret set --name <api app> --resource-group <group> --secrets file-storage="<connection string>"
az containerapp update --name <api app> --resource-group <group> --set-env-vars ConnectionStrings__FileStorage=secretref:file-storage
```

New storage accounts block anonymous access by default. `--allow-blob-public-access true` turns it on for the account. The container itself is created by the API with blob-level (read-only) access. Then set `FILES_BASE_URL` in Cloudflare Pages to `https://<account>.blob.core.windows.net`.

Costs to watch: storage used and egress, both in the Azure cost analysis of the storage account. Orphaned files only appear when a delete fails (see the `could not be deleted and stays orphaned` warning in the logs). They cost almost nothing and can be removed by hand.

## Tests

| Project | What it covers |
|---|---|
| `tests/ClassManager.Storage.U.Tests` | Image format detection and path rules |
| `tests/ClassManager.Storage.I.Tests` | `AzureBlobFileStorage` against Azurite in Testcontainers: anonymous download, content type, cache header, paths with spaces, delete, URLs of another container, missing connection string |
| `tests/ClassManager.Core.U.Tests` | `CatalogImage` validation, replacing and removing images, deleting the new file when saving fails |
| `tests/ClassManager.Api.I.Tests` | Endpoints against SQL Server and Azurite: upload, replace, remove, invalid and oversized files, permissions, business B can't change business A's photos, and photos in the family shop |

`ApiFixture` starts Azurite next to SQL Server and passes its connection string to the API. Both containers are started with Podman (see [CLAUDE.md](../CLAUDE.md)).

## Changing provider

1. Add `src/Storage.<Provider>` with a class that implements `IFileStorage`, and a `Add<Provider>FileStorage()` extension.
2. Copy the tests in `tests/ClassManager.Storage.I.Tests` against that provider's emulator (MinIO for S3 and R2).
3. Replace `AddAzureBlobFileStorage()` in `InfrastructureServiceCollectionExtensions`.
4. Copy existing files and update the URLs in `Products.ImageUrl` and `ClassPacks.ImageUrl` with a one-off script.
