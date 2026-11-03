import { screen } from "@testing-library/react-native";
import { listMonthlyFees } from "@/features/fees/feesApi";
import { CollectionsScreen } from "@/features/fees/screens/CollectionsScreen";
import { getOrderSummary, listOrders } from "@/features/orders/ordersApi";
import { translate } from "@/i18n/translate";
import { buildMonthlyFees, feeMonth } from "@/testing/feeFactory";
import { buildOrder, buildOrderSummary } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");
jest.mock("@/features/orders/ordersApi");

describe("When orders await pickup", () => {
  beforeEach(() => {
    jest.mocked(listMonthlyFees).mockResolvedValue(buildMonthlyFees([]));
    jest.mocked(getOrderSummary).mockResolvedValue(buildOrderSummary());
    jest
      .mocked(listOrders)
      .mockImplementation(async (_clientId, filter) =>
        filter === "awaitingPickup"
          ? [buildOrder({ id: "ready-1" }), buildOrder({ id: "ready-2" })]
          : [],
      );
  });

  it("Then the summary shows how many to hand over", async () => {
    await renderWithProviders(<CollectionsScreen initialMonth={feeMonth} />);

    expect(
      await screen.findByText(translate("collections.toHandOverCount", { count: 2 })),
    ).toBeOnTheScreen();
  });
});
