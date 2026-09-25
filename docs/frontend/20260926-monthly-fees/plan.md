# Frontend plan — Monthly fees (M5)

A "Cuotas" tab answers the instructor's question at the start of each month: who owes. Recording a payment is two taps from there.

Backend contract: [backend/20260926-monthly-fees](../../backend/20260926-monthly-fees/plan.md).

## Navigation

```text
app/(tabs)/
  fees/
    _layout.tsx
    index.tsx                 → MonthlyFeesScreen (?month=YYYY-MM, current month by default)
    [clientId]/pay.tsx        → RecordPaymentScreen (?month=)
  settings/monthly-fee.tsx    → DefaultMonthlyFeeScreen
  students/clients/[clientId]/index.tsx → gains fee and payments
```

Tab order: Hoy, Clases, Alumnos, Cuotas, Ajustes.

## Screens

### Month — `fees/index.tsx`

- Header "‹ Septiembre 2026 ›".
- Summary: "Cobrado $24.000 de $36.000".
- Filter chips: Deben (default), Todos.
- Each client: name, students ("Lucía y Tomás"), and "Debe $7.000", "Pagó $5.000 de $12.000", "Pagó" or "Sin cuota definida". Tap → record payment.
- No default fee yet: banner "Definí la cuota mensual" linking to settings.

### Record payment — `fees/[clientId]/pay.tsx`

Amount prefilled with the balance, method chips (Efectivo, Transferencia, Tarjeta, Otro; Efectivo by default), paid on (today, `dd/mm/aaaa`), notes. Saved → toast "Pago registrado", back to the month.

### Client detail

"Cuota: $12.000 (general)" or "(propia)", with "Cambiar cuota" (amount or "usar la general"), and the last payments with "Borrar".

### Settings → Cuota mensual

One amount field; empty removes it.

## Money

`formatMoney(amount, currencyCode)` with `Intl.NumberFormat("es-AR", { style: "currency", currency, maximumFractionDigits: 0 when whole })`. Amount inputs accept digits with an optional `,` or `.` decimal part.

## Tests (write first)

```text
src/features/fees/__tests__/
  When_month_has_debtors/Then_only_debtors_are_shown_by_default.test.tsx
  When_all_filter_is_selected/Then_paid_clients_are_shown_too.test.tsx
  When_payment_is_recorded/Then_request_has_amount_month_and_method.test.tsx
  When_payment_screen_opens/Then_amount_is_prefilled_with_balance.test.tsx
  When_money_is_formatted/Then_it_uses_the_business_currency.test.ts
  When_amount_is_typed_with_comma/Then_it_is_parsed_as_decimal.test.ts
  When_default_fee_is_missing/Then_settings_link_is_shown.test.tsx
```

## Implementation notes

- Status: done. Tests in `src/features/fees/__tests__`.
- The "Deben" filter shows `Unpaid` and `Partial`; `NoFee` clients only appear under "Todos".
- The payment date reuses the `dd/mm/aaaa` helpers from students and rejects future dates on the device; the API checks against the business's today.
- Setting the default fee invalidates the business query, so every screen reading `useCurrentBusiness` gets the new fee.
- Pending: manual check on Android (Expo Go).

## Out of scope

- Payment history screen beyond the client's last payments.
- Exporting to a spreadsheet.
