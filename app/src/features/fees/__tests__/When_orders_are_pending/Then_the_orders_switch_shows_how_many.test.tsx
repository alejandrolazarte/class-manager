import { screen } from "@testing-library/react-native";
import { listMonthlyFees } from "@/features/fees/feesApi";
import { CollectionsScreen } from "@/features/fees/screens/CollectionsScreen";
import { listOrders } from "@/features/orders/ordersApi";
import { translate } from "@/i18n/translate";
import { buildMonthlyFees, feeMonth } from "@/testing/feeFactory";
import { buildOrder } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");
jest.mock("@/features/orders/ordersApi");

describe("When orders are pending", () => {
  beforeEach(() => {
    jest.mocked(listMonthlyFees).mockResolvedValue(buildMonthlyFees([]));
    jest
      .mocked(listOrders)
      .mockImplementation(async (_clientId, filter) =>
        filter === "requested"
          ? [buildOrder({ id: "requested-1", status: "Requested", awaitsPickup: false })]
          : filter === "awaitingPickup"
            ? [buildOrder({ id: "ready-1" }), buildOrder({ id: "ready-2" })]
            : [],
      );
  });

  it("Then the orders switch shows how many", async () => {
    await renderWithProviders(<CollectionsScreen initialMonth={feeMonth} />);

    expect(
      await screen.findByText(translate("collections.ordersWithCount", { count: 3 })),
    ).toBeOnTheScreen();
  });
});
