import { fireEvent, screen } from "@testing-library/react-native";
import { getOrder } from "@/features/orders/ordersApi";
import { OrderDetailScreen } from "@/features/orders/screens/OrderDetailScreen";
import { translate } from "@/i18n/translate";
import { buildOrder } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/orders/ordersApi");

const orderWithoutClient = buildOrder({ clientFullName: null });

describe("When branch presses cancel on an order without client", () => {
  beforeEach(() => {
    jest.mocked(getOrder).mockResolvedValue(orderWithoutClient);
  });

  it("Then the question has no client name", async () => {
    await renderWithProviders(<OrderDetailScreen orderId={orderWithoutClient.id} />);

    await fireEvent.press(await screen.findByRole("button", { name: translate("orders.cancel") }));

    expect(
      screen.getByText(translate("orders.cancelQuestion", { number: orderWithoutClient.number })),
    ).toBeOnTheScreen();
  });
});
