import { fireEvent, screen } from "@testing-library/react-native";
import { listMonthlyFees } from "@/features/fees/feesApi";
import { CollectionsScreen } from "@/features/fees/screens/CollectionsScreen";
import { getOrderSummary, listOrders } from "@/features/orders/ordersApi";
import { translate } from "@/i18n/translate";
import { buildClientFee, buildMonthlyFees, feeMonth } from "@/testing/feeFactory";
import { buildOrder, buildOrderSummary } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");
jest.mock("@/features/orders/ordersApi");

const debtor = buildClientFee({
  clientId: "debtor",
  clientFullName: "Zoe Ruiz",
  paid: 0,
  balance: 12000,
  status: "Unpaid",
});
const productOrder = buildOrder({ id: "product-order", number: 1041 });

describe("When plans are filtered", () => {
  beforeEach(() => {
    jest.mocked(listMonthlyFees).mockResolvedValue(buildMonthlyFees([debtor]));
    jest.mocked(getOrderSummary).mockResolvedValue(buildOrderSummary());
    jest.mocked(listOrders).mockResolvedValue([productOrder]);
  });

  it("Then orders are hidden", async () => {
    await renderWithProviders(<CollectionsScreen initialMonth={feeMonth} />);
    await screen.findByText(/n\.º 1041/);

    await fireEvent.press(
      screen.getByRole("button", { name: translate("collections.kind.plans") }),
    );

    expect(screen.queryByText(/n\.º 1041/)).not.toBeOnTheScreen();
    expect(screen.getByText(debtor.clientFullName)).toBeOnTheScreen();
  });
});
