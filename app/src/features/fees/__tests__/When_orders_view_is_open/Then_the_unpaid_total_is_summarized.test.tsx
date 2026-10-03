import { fireEvent, screen } from "@testing-library/react-native";
import { listMonthlyFees } from "@/features/fees/feesApi";
import { CollectionsScreen } from "@/features/fees/screens/CollectionsScreen";
import { formatMoney } from "@/features/fees/money";
import { getOrderSummary, listOrders } from "@/features/orders/ordersApi";
import { translate, translateCount } from "@/i18n/translate";
import { buildBusiness } from "@/testing/businessFactory";
import { buildMonthlyFees, feeMonth } from "@/testing/feeFactory";
import { buildOrder, buildOrderSummary } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");
jest.mock("@/features/orders/ordersApi");

describe("When orders view is open", () => {
  beforeEach(() => {
    jest.mocked(listMonthlyFees).mockResolvedValue(buildMonthlyFees([]));
    jest.mocked(getOrderSummary).mockResolvedValue(buildOrderSummary({ unpaid: 12200 }));
    jest
      .mocked(listOrders)
      .mockImplementation(async (_clientId, filter) =>
        filter === "requested"
          ? [
              buildOrder({ id: "first", status: "Requested", awaitsPickup: false, total: 3400 }),
              buildOrder({ id: "second", status: "Requested", awaitsPickup: false, total: 8800 }),
            ]
          : [],
      );
  });

  it("Then the unpaid total is summarized", async () => {
    await renderWithProviders(<CollectionsScreen initialMonth={feeMonth} />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("collections.orders") }),
    );

    expect(
      await screen.findByText(
        translateCount("collections.unpaidOrders", 2, {
          amount: formatMoney(12200, buildBusiness().currencyCode),
        }),
      ),
    ).toBeOnTheScreen();
  });
});
