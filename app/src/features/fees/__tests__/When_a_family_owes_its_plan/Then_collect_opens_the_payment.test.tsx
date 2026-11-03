import { fireEvent, screen } from "@testing-library/react-native";
import { listMonthlyFees } from "@/features/fees/feesApi";
import { CollectionsScreen } from "@/features/fees/screens/CollectionsScreen";
import { getOrderSummary, listOrders } from "@/features/orders/ordersApi";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { routerMock } from "@/testing/expoRouterMock";
import { buildClientFee, buildMonthlyFees, feeMonth } from "@/testing/feeFactory";
import { buildOrderSummary } from "@/testing/productFactory";
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

describe("When a family owes its plan", () => {
  beforeEach(() => {
    jest.mocked(listMonthlyFees).mockResolvedValue(buildMonthlyFees([debtor]));
    jest.mocked(getOrderSummary).mockResolvedValue(buildOrderSummary());
    jest.mocked(listOrders).mockResolvedValue([]);
  });

  it("Then collect opens the payment", async () => {
    await renderWithProviders(<CollectionsScreen initialMonth={feeMonth} />);

    await fireEvent.press(await screen.findByRole("button", { name: translate("orders.collect") }));

    expect(routerMock.push).toHaveBeenCalledWith(
      routes.recordPayment("fees", debtor.clientId, feeMonth),
    );
  });
});
