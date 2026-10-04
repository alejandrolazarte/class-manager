import { fireEvent, screen } from "@testing-library/react-native";
import { listMonthlyFees } from "@/features/fees/feesApi";
import { CollectionsScreen } from "@/features/fees/screens/CollectionsScreen";
import { listOrders } from "@/features/orders/ordersApi";
import { translate } from "@/i18n/translate";
import { buildMonthlyFees, feeMonth } from "@/testing/feeFactory";
import { buildOrder } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");
jest.mock("@/features/orders/ordersApi");

describe("When owner opens cobros", () => {
  beforeEach(() => {
    jest.mocked(listMonthlyFees).mockResolvedValue(buildMonthlyFees([]));
    jest.mocked(listOrders).mockResolvedValue([buildOrder({ number: 1042 })]);
  });

  it("Then orders can be shown", async () => {
    await renderWithProviders(<CollectionsScreen initialMonth={feeMonth} />);

    await fireEvent.press(
      await screen.findByRole("tab", { name: translate("collections.orders") }),
    );

    expect(await screen.findByText(/n\.º 1042/)).toBeOnTheScreen();
  });
});
