# Countries

Every business (`Business`) belongs to a country. The countries the app supports, and what each one implies, live in one table instead of being repeated in the app and the API.

## The `Countries` table

| Column | Meaning | Example (Spain) |
|---|---|---|
| `Code` | ISO 3166-1 alpha-2 code, primary key | `ES` |
| `DisplayOrder` | Order in the country picker | `14` |
| `CallingCode` | International calling code | `34` |
| `CurrencyCode` | Usual currency (ISO 4217), suggested when the country is chosen | `EUR` |
| `OwnAccountMinimumAge` | Age from which a person can have an account of their own without the authorization of the client who pays (see [Ages and minors](ages-and-minors.md)) | `14` |

`CountryTimeZones` lists the time zones of each country (`CountryCode`, `TimeZoneId`, `DisplayOrder`). Spain has `Europe/Madrid` and `Atlantic/Canary`.

The rows are seeded by the migrations (`CountryCatalogSeed` in `src/Infrastructure/Persistence`). Adding a country or changing its data means editing the seed and adding a migration, and adding its name to `countries.<code>` in `app/src/i18n/es.json`. The names stay in the app because they are UI copy.

`GET /api/countries` returns the countries in display order with their calling code, currency and time zones. It is anonymous because the sign-up form needs it.

## What the business stores

| Column | Rule |
|---|---|
| `CountryCode` | Required, references `Countries` |
| `TimeZoneId` | Must be one of the country's time zones |
| `CurrencyCode` | Any ISO 4217 currency. The country's currency is only a suggestion: a branch in Spain may charge in USD |
| `DefaultCountryCallingCode` | Always the calling code of the country, set when the country is chosen |

The country is chosen when the owner signs up, when a branch is created and in the business settings. The app suggests the country of the device's time zone at sign-up.

## What the country is used for

- **Time zone choices:** only the country's time zones are accepted.
- **Default currency:** suggested when the country is chosen.
- **Phone numbers typed without a country code:** `DefaultCountryCallingCode` is added to them. A number typed with `+` or `00` keeps its own code, so a client of a branch in Spain can have an Argentine number (`+54 9 11 …`).
- **Minors:** `OwnAccountMinimumAge` of the branch's country decides who needs the authorization of the client who pays. Before the table existed this was guessed from the calling code.

## Migration of existing businesses

The migration `AddCountries` set `CountryCode` of existing businesses from their time zone. If no country has that time zone it used the calling code, and Argentina otherwise. It then set `DefaultCountryCallingCode` to the calling code of that country.
