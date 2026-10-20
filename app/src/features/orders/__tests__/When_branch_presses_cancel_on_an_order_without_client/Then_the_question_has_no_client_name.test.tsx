import { fireEvent, screen } from "@testing-library/react-native";
import { PaymentPanel } from "@/features/orders/components/PaymentPanel";
import { translate } from "@/i18n/translate";
import { buildOrder } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/orders/ordersApi");

const orderWithoutClient = buildOrder({ status: "Requested", clientFullName: null });

describe("When branch presses cancel on an order without client", () => {
  it("Then the question has no client name", async () => {
    await renderWithProviders(<PaymentPanel order={orderWithoutClient} />);

    await fireEvent.press(screen.getByRole("button", { name: translate("orders.cancel") }));

    expect(
      screen.getByText(translate("orders.cancelQuestion", { number: orderWithoutClient.number })),
    ).toBeOnTheScreen();
  });
});
