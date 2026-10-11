# Currencies and time zones

Each business (`Business`) has one time zone (`TimeZoneId`, IANA) and one currency (`CurrencyCode`, ISO 4217). The owner can change both in the business settings. This page explains what changes when they do.

## Currency

### Money that was charged keeps its currency

Every record of money that changed hands stores the currency it was charged in, copied from the business when the record is created:

| Record | Column |
|---|---|
| `Payment` (monthly fee payments) | `Payments.CurrencyCode` |
| `ClassPackPurchase` (pack sales) | `ClassPackPurchases.CurrencyCode` |
| `Order` (shop and counter sales, all its lines) | `Orders.CurrencyCode` |

`IBusinessCurrencyService` gives use cases the current currency of the business. A class pack credited when an order is paid takes the currency of the order, not the business's currency at payment time, so a student app order requested before a currency change is still credited in the currency it was priced in.

The API returns `currencyCode` with each payment, pack sale, pack usage and order, and the app formats each amount with its own currency. Changing the business currency from EUR to USD therefore leaves earlier payments shown as euros and only new ones in dollars.

The migration `StoreCurrencyOfPaymentsAndOrders` filled the new columns of existing rows with the currency their business had at that moment.

### Totals never add two currencies

Amounts in different currencies are never summed. Totals that compare money to prices use only records in the business's current currency:

- The monthly fee balance (`IPaymentRepository.SumByClientForMonthAsync`, used by the fees screen and the student app) counts only payments in the current currency.
- The sales summary (`GetOrderSummaryUseCase`) counts only orders in the current currency.

Payments and orders in an earlier currency are still listed, with their own currency, but don't count toward these totals.

### Prices are not converted

Prices are a single amount in the business's currency: the default monthly fee, a client's own fee, class pack prices and product prices. They don't store a currency and are not converted. After a currency change the owner reviews them and enters them in the new currency. The settings screen says so when the currency is changed.

There are no exchange rates in the app. This follows the usual model for money that has already been charged (Dynamics 365 calls it the *transaction currency*): each record keeps its own currency, and changing a setting never rewrites it.

### Stripe (not built yet)

When online payments arrive (see the [online payments plan](backend/20260929-online-shop-and-payments/plan.md)), each charge is created in the business's current currency. That is Stripe's *presentment currency*. Stripe pays out in the currency of the branch's bank account (the *settlement currency*) and converts if they differ. A Spanish branch that charges in USD with a bank account in EUR still receives euros. The app records what the family paid (`Amount` + `CurrencyCode`); what reached the bank is in Stripe's balance transactions.

## Time zone

Class times are wall-clock times of the business, with no time zone stored:

| Record | Stored as |
|---|---|
| `ClassGroup` schedule | weekdays + `StartTime` (`TimeOnly`) |
| `ClassSession` | `Date` (`DateOnly`) + `RescheduledStartTime` (`TimeOnly?`) |
| `PrivateLesson` | `Date` + `StartTime` |

Classes are in person, so a class at 19:00 means 19:00 where the business is. If the business changes its time zone (for example from `Europe/Madrid` to `America/Argentina/Buenos_Aires`), every class stays at 19:00 local time. The real instant moves (19:00 in Madrid is not 19:00 in Buenos Aires), and new classes are created in the new local time in the same way.

"Today" and "now" for the business (`Business.TodayAt`, `Business.LocalTimeAt`, `IBusinessCalendarService`) use the time zone, so changing it also moves rules that compare against the start of a class: absence notices, makeups, and whether a class has started.

Audit timestamps (`CreatedAt`, `CancelledAt`, `DeletedOn` and others) are stored as `DateTimeOffset` in UTC and are not affected.
