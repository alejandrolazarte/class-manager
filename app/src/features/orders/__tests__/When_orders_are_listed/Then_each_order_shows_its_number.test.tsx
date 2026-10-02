import { screen } from "@testing-library/react-native";
import { listOrders } from "@/features/orders/ordersApi";
import { OrderListScreen } from "@/features/orders/screens/OrderListScreen";
import { buildOrder } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/orders/ordersApi");

describe("When orders are listed", () => {
  beforeEach(() => {
    jest.mocked(listOrders).mockResolvedValue([buildOrder({ number: 57 })]);
  });

  it("Then each order shows its number", async () => {
    await renderWithProviders(<OrderListScreen />);

    expect(await screen.findByText(/n\.º 57/)).toBeOnTheScreen();
  });
});
