import { screen } from "@testing-library/react-native";
import { EffectiveMonthPicker } from "@/features/fees/components/EffectiveMonthPicker";
import { monthOf } from "@/features/fees/months";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

describe("When effective month is the current month", () => {
  it("Then previous month is disabled", async () => {
    await renderWithProviders(<EffectiveMonthPicker month={monthOf()} onChange={jest.fn()} />);

    expect(screen.getByLabelText(translate("fees.effectiveMonth.previous"))).toBeDisabled();
  });
});
