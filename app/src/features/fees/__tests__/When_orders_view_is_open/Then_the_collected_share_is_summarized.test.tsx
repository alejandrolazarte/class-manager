import { fireEvent, screen } from "@testing-library/react-native";
import { listMonthlyFees } from "@/features/fees/feesApi";
import { CollectionsScreen } from "@/features/fees/screens/CollectionsScreen";
import { getOrderSummary, listOrders } from "@/features/orders/ordersApi";
import { formatMoney } from "@/features/fees/money";
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
      .mockResolvedValue(buildOrderSummary({ collected: 300, unpaid: 100 }));
    jest.mocked(listOrders).mockResolvedValue([]);
  });

  it("Then the collected share is summarized", async () => {
    await renderWithProviders(<CollectionsScreen initialMonth={feeMonth} />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("collections.orders") }),
    );

    const money = (amount: number) => formatMoney(amount, buildBusiness().currencyCode);
    expect(
      await screen.findByLabelText(
        translate("collections.summary", { collected: money(300), total: money(400) }),
      ),
    ).toBeOnTheScreen();
  });
});
