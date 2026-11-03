import { screen } from "@testing-library/react-native";
import { listMonthlyFees } from "@/features/fees/feesApi";
import { CollectionsScreen } from "@/features/fees/screens/CollectionsScreen";
import { getOrderSummary, listOrders } from "@/features/orders/ordersApi";
import { translate } from "@/i18n/translate";
import { buildClientFee, buildMonthlyFees, feeMonth } from "@/testing/feeFactory";
import { buildOrderSummary } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");
jest.mock("@/features/orders/ordersApi");

const clientWithoutFee = buildClientFee({ fee: null, paid: 0, balance: 0, status: "NoFee" });

describe("When month has nothing to collect", () => {
  beforeEach(() => {
    jest.mocked(listMonthlyFees).mockResolvedValue(buildMonthlyFees([clientWithoutFee]));
    jest.mocked(getOrderSummary).mockResolvedValue(buildOrderSummary());
    jest.mocked(listOrders).mockResolvedValue([]);
  });

  it("Then it does not say everything was collected", async () => {
    await renderWithProviders(<CollectionsScreen initialMonth={feeMonth} />);

    expect(await screen.findByText(translate("collections.empty"))).toBeOnTheScreen();
    expect(screen.queryByText(translate("collections.everythingCollected"))).toBeNull();
  });
});
