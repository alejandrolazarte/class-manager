import { screen } from "@testing-library/react-native";
import { listOrders } from "@/features/orders/ordersApi";
import { NotifiedOrdersScreen } from "@/features/orders/screens/NotifiedOrdersScreen";
import { buildCurrentMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/orders/ordersApi");

describe("When order notification is opened by collector", () => {
  beforeEach(() => {
    jest.mocked(listOrders).mockResolvedValue([]);
  });

  it("Then a skeleton is shown instead of the app logo", async () => {
    await renderWithProviders(<NotifiedOrdersScreen />, { member: buildCurrentMember() });

    expect(screen.getByRole("progressbar", { name: "Cargando…" })).toBeOnTheScreen();
    expect(screen.queryByTestId("brand-mark")).not.toBeOnTheScreen();
  });
});
