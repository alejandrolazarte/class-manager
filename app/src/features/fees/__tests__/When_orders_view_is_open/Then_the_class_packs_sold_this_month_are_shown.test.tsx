import { fireEvent, screen } from "@testing-library/react-native";
import { listMonthlyFees } from "@/features/fees/feesApi";
import { CollectionsScreen } from "@/features/fees/screens/CollectionsScreen";
import { formatMoney } from "@/features/fees/money";
import { getOrderSummary, listOrders } from "@/features/orders/ordersApi";
import { translate } from "@/i18n/translate";
import { buildBusiness } from "@/testing/businessFactory";
import { buildMonthlyFees, feeMonth } from "@/testing/feeFactory";
import { buildOrderSummary } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");
jest.mock("@/features/orders/ordersApi");

describe("When orders view is open", () => {
  beforeEach(() => {
    jest.mocked(listMonthlyFees).mockResolvedValue(buildMonthlyFees([]));
    jest
      .mocked(getOrderSummary)
      .mockResolvedValue(buildOrderSummary({ collected: 200, classPackSales: 200 }));
    jest.mocked(listOrders).mockResolvedValue([]);
  });

  it("Then the class packs sold this month are shown", async () => {
    await renderWithProviders(<CollectionsScreen initialMonth={feeMonth} />);

    await fireEvent.press(
      await screen.findByRole("tab", { name: translate("collections.orders") }),
    );

    expect(
      await screen.findByText(
        translate("collections.classPackSales", {
          amount: formatMoney(200, buildBusiness().currencyCode),
        }),
      ),
    ).toBeOnTheScreen();
  });
});
