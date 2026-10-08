# Forms

Every form tells the user what it needs before they press the button, and the main button stays grey until it has it.

## Why

- **One rule everywhere.** Before, some forms marked optional fields ("Email (opcional)"), most marked nothing, and the button was always blue: the user learned what was missing only after pressing it. Now every form reads the same way.
- **Material Design.** Required fields carry an asterisk, explained once at the top of the form, and a filled button that cannot act yet is shown disabled (grey) instead of failing on press.
- **The button explains itself.** A grey "Continuar" or "Guardar" says the form is not complete; the asterisks say what completes it.

## The rule

- Mark every required field with `isRequired` (`TextField`, `PasswordField`, `NumberStepper`, `AmountField`). The label gets " *"; the accessible name of the input stays the plain label, so tests and screen readers keep finding it by its name.
- Labels of required choices that are not text fields (chips, option cards, pickers) use `withRequiredMark(label)`.
- Put `<RequiredFieldsLegend />` ("* Obligatorio") at the top of every screen-level form that has a required field. Small inline panels (refund, stock, reschedule) only mark the fields.
- Never write "(opcional)" in a label: a field without an asterisk is optional.
- Disable the main button until every required field has a value:
  - forms on `react-hook-form`: `useRequiredFieldsFilled(form.control, [...names])`;
  - forms on plain state: `isFilled(value)` or `areFilled([...values])`.
  "Filled" means non-blank text, a non-empty list or a set value. Format rules (a valid email, a minimum length, a valid date) still run on blur and on press, and show the error under the field.
- Disabled buttons are grey (`bg-border` and the `disabled` text tone), not a faded primary. A loading button keeps its color and shows the spinner.
- Sign-in, forgot password and reset password don't mark fields or show the legend: everyone knows those screens from other apps, and with one or two fields the marks are noise. Their button still stays grey until the fields are filled. Sign-up does mark them: it asks for more and in two steps.
- Fields that are only sometimes required stay unmarked and keep their validation on press. Example: accepting an invitation needs a name and a password only when the email has no account yet, which the form cannot know.
- Errors and satisfied hints carry an icon (error or check) next to the text, so they don't depend on color alone.

```tsx
const areRequiredFieldsFilled = useRequiredFieldsFilled(form.control, ["fullName", "phoneNumber"]);

<RequiredFieldsLegend />
<TextField label={translate("clients.register.fullName")} isRequired … />
<Button label={translate("common.save")} onPress={save} disabled={!areRequiredFieldsFilled} />
```

## Dates and times

- Every date is a `DateField` (`BirthDateField` for birth dates) and every time is a `TimeField`, both from `app/src/forms`. Never a bare `TextField` with a date or time mask.
- The user can always type: the field keeps the `dd/mm/aaaa` or `hh:mm` mask and the number keyboard, which is the fastest way for a known date (a birth date) or an odd time (18:47).
- The button at the right of the field opens a bottom sheet to pick instead: a month calendar (tap the month title to jump to a year) or a grid of hours and minutes (every 5 minutes by default, `minuteStep`). **Cancelar** leaves the typed value untouched; **Listo** writes the picked value back as text, so validation and the form values don't change.
- The sheet opens on what is typed when it is valid; a typed minute that is off the grid still shows selected.
- Limit the calendar with `earliestIsoDate` and `latestIsoDate` when the range has a meaning: `BirthDateField` stops at today.

## Steps and totals

- Forms in steps (sign-up, counter sale) use the same header: back arrow, a progress bar with one segment per step that fills as the step is completed, "Paso N de M" and the step title (`StepScreen`, or `AuthenticationScreenLayout` with `StepProgress`).
- Screens that end in a payment or an order (cart, counter sale) close with the same `TotalBar`: "Total", the amount, and the button below it.

## Enforced by

- `app/src/forms/__tests__/When_required_values_are_checked/` defines what counts as filled.
- `app/src/ui/__tests__/When_a_field_is_required/` checks the asterisk and the accessible name.
- `app/src/forms/__tests__/When_a_date_is_picked_from_the_calendar/`, `When_a_date_is_typed/`, `When_the_date_picker_is_cancelled/`, `When_a_birth_date_calendar_is_opened/`, `When_a_time_is_picked/` and `When_a_time_is_typed/` check that typing and picking agree.
- Screen tests check the grey button where the form starts empty: sign-up (`When_account_step_is_incomplete`), new student (`When_register_form_is_empty`), class (`When_class_form_has_no_days`), edit student (`When_edited_client_has_no_name`), billing plan (`When_client_switches_to_an_own_fee_without_amount`) and counter sale (`When_counter_sale_has_nothing_yet`).
