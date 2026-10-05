import { screen } from "@testing-library/react-native";
import { EffectiveMonthPicker } from "@/features/fees/components/EffectiveMonthPicker";
import { addMonths, monthAndYear, monthOf } from "@/features/fees/months";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

describe("When effective month is in the past", () => {
  it("Then changed months are warned", async () => {
    const currentMonth = monthOf();
    const effectiveFrom = addMonths(currentMonth, -3);

    await renderWithProviders(<EffectiveMonthPicker month={effectiveFrom} onChange={jest.fn()} />);

    expect(screen.getByText(translate("fees.effectiveMonth.pastWarning.title"))).toBeTruthy();
    expect(
      screen.getByText(
        translate("fees.effectiveMonth.pastWarning.other", {
          from: monthAndYear(effectiveFrom),
          to: monthAndYear(addMonths(currentMonth, -1)),
        }),
      ),
    ).toBeTruthy();
  });
});
