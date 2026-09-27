import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { setDefaultMonthlyFee } from "@/features/fees/feesApi";
import { addMonths, monthOf } from "@/features/fees/months";
import { DefaultMonthlyFeeScreen } from "@/features/fees/screens/DefaultMonthlyFeeScreen";
import { translate } from "@/i18n/translate";
import { buildBusiness } from "@/testing/businessFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");

describe("When default fee is changed from next month", () => {
  beforeEach(() => {
    jest
      .mocked(setDefaultMonthlyFee)
      .mockResolvedValue(buildBusiness({ defaultMonthlyFee: 15000 }));
  });

  it("Then effective month is sent", async () => {
    await renderWithProviders(<DefaultMonthlyFeeScreen />);

    await fireEvent.changeText(screen.getByLabelText(translate("fees.defaultFee.amount")), "15000");
    await fireEvent.press(screen.getByLabelText(translate("fees.effectiveMonth.next")));
    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() =>
      expect(setDefaultMonthlyFee).toHaveBeenCalledWith(15000, addMonths(monthOf(), 1)),
    );
  });
});
