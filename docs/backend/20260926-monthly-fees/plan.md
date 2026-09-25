# Backend plan — Monthly fees (M5)

Last milestone of the [MVP plan](../../mvp-plan.md). The business sets a monthly fee, records what each client pays, and sees who owes a month.

## Decisions

- **The fee is per client**, not per student: a family pays one fee (open decision in the MVP plan, settled here). A sibling discount is a lower fee on that client.
- **Default fee on the business**, optional override per client. Effective fee = client's fee, else the business's, else none ("no fee defined").
- **Who is charged a month**: clients with at least one student enrolled in any class during that month. A client who left in August isn't charged in September.
- Payments are recorded against a month (`2026-09`), not against sessions: an instructor charges "September", not "8 classes".

## Scope

| Operation | Endpoint | Use case |
|---|---|---|
| Set the business's default fee | `PUT /api/business/monthly-fee` | `SetDefaultMonthlyFeeUseCase` |
| Override a client's fee | `PUT /api/clients/{clientId}/monthly-fee` | `SetClientMonthlyFeeUseCase` |
| Record a payment | `POST /api/clients/{clientId}/payments` | `RecordPaymentUseCase` |
| A client's payments | `GET /api/clients/{clientId}/payments` | `ListClientPaymentsUseCase` |
| Delete a payment (mistakes) | `DELETE /api/payments/{paymentId}` | `DeletePaymentUseCase` |
| Fees of a month | `GET /api/fees?month=2026-09` | `ListMonthlyFeesUseCase` |

`GET /api/business` and `GET /api/clients/{id}` gain `defaultMonthlyFee` / `monthlyFee`. All writes are owner-only.

## Contract

```json
PUT /api/business/monthly-fee          { "amount": 12000 }          (null removes it)
PUT /api/clients/{clientId}/monthly-fee { "amount": 10000 }          (null follows the business's)

POST /api/clients/{clientId}/payments
{ "amount": 12000, "month": "2026-09", "paidOn": "2026-09-05", "method": "Transfer", "notes": null }
```

`month` defaults to the current month and `paidOn` to today (business time zone). `method` is `Cash`, `Transfer`, `Card` or `Other`.

```json
GET /api/fees?month=2026-09
{
  "month": "2026-09",
  "totalDue": 36000,
  "totalPaid": 24000,
  "clients": [
    {
      "clientId": "0192f0c4-...",
      "clientFullName": "Ana Pérez",
      "clientPhoneNumber": "+541122334455",
      "studentNames": ["Lucía Pérez", "Tomás Pérez"],
      "fee": 12000,
      "paid": 5000,
      "balance": 7000,
      "status": "Partial"
    }
  ]
}
```

`status` is `Paid` (paid ≥ fee), `Partial`, `Unpaid` or `NoFee`. Ordered: `Unpaid`, `Partial`, `NoFee`, `Paid`, then by name.

## Business rules

| # | Rule | Result |
|---|---|---|
| F1 | Amounts are greater than 0, at most 10,000,000, with at most 2 decimals | Validation |
| F2 | `paidOn` is not after today | Validation |
| F3 | `month` has the `YYYY-MM` format, from 2000-01 to 12 months after the current month | Validation |
| F4 | `method` is one of the four values | Validation |
| F5 | `notes`, if present, ≤ 200 characters | Validation |
| F6 | Client and payment belong to the business | `404` |

Overpaying is allowed (paid 13,000 for a 12,000 fee shows as `Paid`); the balance never goes below zero in the list.

## Design

```text
src/Core/Domain/Fees/
  MonthlyFee.cs          → Validate(decimal?) shared by business and client (F1)
  BillingMonth.cs        → value object: first day of the month, Parse("2026-09"), ToString, FirstDay, LastDay
  Payment.cs             → Create(clientId, amount, month, paidOn, method, notes, today, createdAt)
  PaymentMethod.cs, FeeStatus.cs, FeeErrorCodes.cs
src/Core/Domain/Businesses/Business.cs   → DefaultMonthlyFee + SetDefaultMonthlyFee
src/Core/Domain/Clients/Client.cs        → MonthlyFee + SetMonthlyFee
src/Core/Abstractions/Persistence/
  IPaymentRepository.cs                  → Add, Remove, GetForUpdateAsync, ListByClientAsync, SumByClientForMonthAsync
  IEnrollmentRepository.cs               → + ListEnrolledInPeriodAsync(from, to) → EnrolledStudentInPeriod rows
  IClientRepository.cs                   → + GetForUpdateAsync
src/Core/UseCases/Fees/ ...
src/Infrastructure/                      → Payment configuration (decimal(12,2), method as string), migration AddMonthlyFees
src/Api/Endpoints/FeeEndpoints.cs
```

## Tests (write first)

### Unit

```text
Domain/Fees/
  When_MonthlyFee_has_three_decimals/Then_validation_fails.cs
  When_BillingMonth_is_parsed/Then_first_and_last_day_are_known.cs
  When_BillingMonth_is_malformed/Then_validation_fails.cs
  When_Payment_is_paid_in_the_future/Then_validation_fails.cs
UseCases/Fees/
  When_ListMonthlyFees_client_has_own_fee/Then_it_overrides_the_default.cs
  When_ListMonthlyFees_client_paid_part/Then_status_is_partial_with_balance.cs
  When_ListMonthlyFees_without_any_fee/Then_status_is_no_fee.cs
  When_ListMonthlyFees/Then_debtors_come_first.cs
  When_RecordPayment_without_month/Then_current_month_is_used.cs
```

### Integration

```text
Endpoints/Fees/
  When_payment_is_recorded/Then_month_shows_client_as_paid.cs
  When_setting_default_fee/Then_enrolled_clients_owe_it.cs
  When_recording_payment_for_client_of_another_business/Then_returns_404.cs
  When_deleting_payment/Then_client_owes_again.cs
Persistence/
  When_Payment_belongs_to_another_business/Then_it_is_not_returned.cs
```

## Out of scope

- Class packs, per-class prices and discounts as rules.
- Receipts, invoices and online payments.
- Reminders to clients who owe (WhatsApp milestone).
