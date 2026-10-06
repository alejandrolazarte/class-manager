import { fireEvent, screen } from "@testing-library/react-native";
import { EffectiveMonthPicker } from "@/features/fees/components/EffectiveMonthPicker";
import { addMonths, monthOf } from "@/features/fees/months";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

describe("When effective month is next month", () => {
  it("Then it can go back to the current month", async () => {
    const onChange = jest.fn();
    await renderWithProviders(
      <EffectiveMonthPicker month={addMonths(monthOf(), 1)} onChange={onChange} />,
    );

    await fireEvent.press(screen.getByLabelText(translate("fees.effectiveMonth.previous")));

    expect(onChange).toHaveBeenCalledWith(monthOf());
  });
});
