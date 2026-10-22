import { screen } from "@testing-library/react-native";
import { listMonthlyFees } from "@/features/fees/feesApi";
import { MonthlyFeesScreen } from "@/features/fees/screens/MonthlyFeesScreen";
import { translate } from "@/i18n/translate";
import { buildBusiness } from "@/testing/businessFactory";
import { buildMonthlyFees, feeMonth } from "@/testing/feeFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");

describe("When billing mode is not chosen", () => {
  beforeEach(() => {
    jest.mocked(listMonthlyFees).mockResolvedValue(buildMonthlyFees([]));
  });

  it("Then choosing it is offered", async () => {
    await renderWithProviders(<MonthlyFeesScreen initialMonth={feeMonth} />, {
      business: buildBusiness({
        defaultMonthlyFee: null,
        defaultMonthlyFeeChanges: [],
        billingMode: null,
      }),
    });

    expect(
      await screen.findByRole("button", { name: translate("fees.month.chooseBillingMode") }),
    ).toBeOnTheScreen();
    expect(screen.queryByText(translate("fees.month.noDefaultFee"))).toBeNull();
  });
});
