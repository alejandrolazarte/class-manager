import { screen } from "@testing-library/react-native";
import { listMonthlyFees } from "@/features/fees/feesApi";
import { CollectionsScreen } from "@/features/fees/screens/CollectionsScreen";
import { formatMoney } from "@/formatting/money";
import { getOrderSummary, listOrders } from "@/features/orders/ordersApi";
import { translate } from "@/i18n/translate";
import { buildBusiness } from "@/testing/businessFactory";
import { buildMonthlyFees, feeMonth } from "@/testing/feeFactory";
import { buildOrderSummary } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");
jest.mock("@/features/orders/ordersApi");

describe("When class packs were sold this month", () => {
  beforeEach(() => {
    jest
      .mocked(listMonthlyFees)
      .mockResolvedValue({ ...buildMonthlyFees([]), classPackSales: 200 });
    jest.mocked(getOrderSummary).mockResolvedValue(buildOrderSummary());
    jest.mocked(listOrders).mockResolvedValue([]);
  });

  it("Then the classes view shows them", async () => {
    await renderWithProviders(<CollectionsScreen initialMonth={feeMonth} />);

    expect(
      await screen.findByText(
        translate("fees.month.classPackSales", {
          amount: formatMoney(200, buildBusiness().currencyCode),
        }),
      ),
    ).toBeOnTheScreen();
  });
});
