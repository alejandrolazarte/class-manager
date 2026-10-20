import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { setDefaultMonthlyFee } from "@/features/fees/feesApi";
import { monthOf } from "@/features/fees/months";
import { DefaultMonthlyFeeScreen } from "@/features/fees/screens/DefaultMonthlyFeeScreen";
import { translate } from "@/i18n/translate";
import { buildBusiness } from "@/testing/businessFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");

describe("When class packs is chosen as billing mode", () => {
  beforeEach(() => {
    jest
      .mocked(setDefaultMonthlyFee)
      .mockResolvedValue(buildBusiness({ billingMode: "ClassPacks" }));
  });

  it("Then the business charges with class packs", async () => {
    await renderWithProviders(<DefaultMonthlyFeeScreen />);

    await fireEvent.press(
      screen.getByRole("radio", { name: translate("fees.billingMode.ClassPacks") }),
    );

    expect(screen.queryByLabelText(translate("fees.payment.amount"))).toBeNull();
    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));
    await waitFor(() =>
      expect(setDefaultMonthlyFee).toHaveBeenCalledWith({
        amount: 12000,
        effectiveFrom: monthOf(),
        billingMode: "ClassPacks",
      }),
    );
  });
});
