import { act, fireEvent, screen } from "@testing-library/react-native";
import { listMonthlyFees } from "@/features/fees/feesApi";
import { CollectionsScreen } from "@/features/fees/screens/CollectionsScreen";
import { MonthlyFees } from "@/features/fees/types";
import { getOrderSummary, listOrders } from "@/features/orders/ordersApi";
import { translate } from "@/i18n/translate";
import { buildMonthlyFees, feeMonth } from "@/testing/feeFactory";
import { buildOrderSummary } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");
jest.mock("@/features/orders/ordersApi");

describe("When cobros finishes loading", () => {
  let resolveMonthlyFees: (monthlyFees: MonthlyFees) => void = () => undefined;

  beforeEach(() => {
    jest
      .mocked(listMonthlyFees)
      .mockImplementation(() => new Promise((resolve) => (resolveMonthlyFees = resolve)));
    jest.mocked(getOrderSummary).mockResolvedValue(buildOrderSummary());
    jest.mocked(listOrders).mockResolvedValue([]);
  });

  it("Then a tab shown while loading still switches", async () => {
    await renderWithProviders(<CollectionsScreen initialMonth={feeMonth} />);
    const ordersTab = screen.getByRole("tab", { name: translate("collections.orders") });
    await act(async () => resolveMonthlyFees(buildMonthlyFees([])));
    await screen.findByText(translate("fees.month.nobodyOwes"));

    await fireEvent.press(ordersTab);

    expect(
      await screen.findByRole("button", { name: translate("orders.counterSale.open") }),
    ).toBeOnTheScreen();
  });
});
