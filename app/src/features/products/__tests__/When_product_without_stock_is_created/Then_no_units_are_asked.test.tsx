import { fireEvent, screen } from "@testing-library/react-native";
import { ProductFormScreen } from "@/features/products/screens/ProductFormScreen";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/products/productsApi");

describe("When product without stock is created", () => {
  it("Then no units are asked", async () => {
    await renderWithProviders(<ProductFormScreen />);
    expect(screen.getByText(translate("products.stock.initialHint"))).toBeOnTheScreen();

    await fireEvent.press(
      screen.getByRole("button", { name: translate("products.stockMode.Unlimited") }),
    );

    expect(screen.queryByText(translate("products.stock.initialHint"))).not.toBeOnTheScreen();
  });
});
