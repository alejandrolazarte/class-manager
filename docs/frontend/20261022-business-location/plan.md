# Business location

Date: 2026-10-22

Status: built.

Families asked where the classes are, and other class apps show it. Each branch now keeps its location: a name or address students read, and optionally a Google Maps link that opens the exact place. It was drawn on a canvas before building (private to the owner): [Ubicación de la sede](https://claude.ai/artifact/Xyw4oNz2m317WBwEghXHpe).

## What changes

| Where | What |
|---|---|
| Team, Ajustes → Negocio and Ajustes → Sedes → Nueva sede | A **Ubicación** section at the end: the Google Maps link (no visible label, the explanation below it with **Google Maps** in bold and "¿Cómo lo copio?"), and **Nombre o dirección** |
| Team, a valid link | The field turns green with "Es un link de Google Maps." and **Probar en Google Maps** opens it, so the owner checks it shows the branch before saving |
| Team, "¿Cómo lo copio?" | A sheet with three steps: find the branch in Google Maps, tap Compartir, tap Copiar enlace and paste it |
| Student app, Ajustes | A **Ubicación** group above "Tu teléfono" with one row: the name or address and, when there is a link, "Ver en Maps", which opens it. Without a link the row is plain text. Without a location the group is hidden |

The location is never shown on each class: a branch rarely moves, and a button on every class would be noise. When a class is taught somewhere else, that belongs to the places plan ([pilot plan](../../pilot-df-swimming.md#places)).

## Decisions

- **Both fields are optional, but a link needs a name.** A name or address alone is fine; a link alone would leave students an empty row, so "Nombre o dirección" becomes required (asterisk, main button grey) as soon as there is a link. The API refuses it too (`400`, field `locationName`).
- **The owner pastes a Google Maps link instead of typing an address we geocode.** A typed address can be wrong and nobody notices; a link copied from Google Maps points to the pin the owner chose, and "Probar en Google Maps" lets them check it. Only `https` Google Maps links are accepted (`GoogleMapsLink` in `Core`, `isGoogleMapsLink` in the app, same rules), so a website or free text can't be saved as the link (`400`, field `locationMapsUrl`).
- **The name is not read from the link.** Following the short link would give the place name, often not the address, through an undocumented URL format; the owner writes it once instead. Google's Places API would read it properly but needs a billing account, which is not worth it for a field written once.
- **No API key and no cost.** Opening a Maps link needs no key ([Maps URLs](https://developers.google.com/maps/documentation/urls/guide)).

## API

| Endpoint | Change |
|---|---|
| `GET` / `PUT /api/business` | `locationName` and `locationMapsUrl` |
| `POST /api/organization/branches` | `locationName` and `locationMapsUrl` |
| `GET /api/student-app/home` | `location`: `{ name, mapsUrl }` or `null` |

Columns `Businesses.LocationName` (`nvarchar(200)`) and `Businesses.LocationMapsUrl` (`nvarchar(500)`), migration `AddBusinessLocation`.
