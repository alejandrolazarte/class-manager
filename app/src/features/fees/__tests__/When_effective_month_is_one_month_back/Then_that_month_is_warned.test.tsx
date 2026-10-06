import { screen } from "@testing-library/react-native";
import { EffectiveMonthPicker } from "@/features/fees/components/EffectiveMonthPicker";
import { addMonths, formatMonth, monthOf } from "@/features/fees/months";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

describe("When effective month is one month back", () => {
  it("Then that month is warned", async () => {
    const previousMonth = addMonths(monthOf(), -1);

    await renderWithProviders(<EffectiveMonthPicker month={previousMonth} onChange={jest.fn()} />);

    expect(
      screen.getByText(
        translate("fees.effectiveMonth.pastWarning.one", { month: formatMonth(previousMonth) }),
      ),
    ).toBeTruthy();
  });
});
