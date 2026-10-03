import { screen } from "@testing-library/react-native";
import { listMonthlyFees } from "@/features/fees/feesApi";
import { MonthlyFeesScreen } from "@/features/fees/screens/MonthlyFeesScreen";
import { formatMoney } from "@/features/fees/money";
import { translate } from "@/i18n/translate";
import { buildBusiness } from "@/testing/businessFactory";
import { buildMonthlyFees, feeMonth } from "@/testing/feeFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");

describe("When month sold class packs", () => {
  beforeEach(() => {
    jest
      .mocked(listMonthlyFees)
      .mockResolvedValue({ ...buildMonthlyFees([]), classPackSales: 200 });
  });

  it("Then the sales are left to orders", async () => {
    await renderWithProviders(<MonthlyFeesScreen initialMonth={feeMonth} />);

    expect(await screen.findByText(translate("fees.month.nobodyOwes"))).toBeOnTheScreen();
    expect(
      screen.queryByText(formatMoney(200, buildBusiness().currencyCode), { exact: false }),
    ).not.toBeOnTheScreen();
  });
});
