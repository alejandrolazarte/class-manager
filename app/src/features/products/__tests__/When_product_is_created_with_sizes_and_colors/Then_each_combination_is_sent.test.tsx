import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { createProduct } from "@/features/products/productsApi";
import { ProductFormScreen } from "@/features/products/screens/ProductFormScreen";
import { translate, translateCount } from "@/i18n/translate";
import { buildProduct } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/products/productsApi");

describe("When product is created with sizes and colors", () => {
  beforeEach(() => {
    jest.mocked(createProduct).mockResolvedValue(buildProduct());
  });

  it("Then each combination is sent", async () => {
    await renderWithProviders(<ProductFormScreen />);

    await fireEvent.changeText(screen.getByLabelText(translate("products.form.name")), "Remera");
    await fireEvent.changeText(screen.getByLabelText(translate("products.form.price")), "12");
    await fireEvent.changeText(screen.getByLabelText(translate("products.form.sizes")), "S, M,");
    await fireEvent.press(
      screen.getByRole("button", { name: translate("products.form.chooseColors") }),
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("products.color.black") }));
    await fireEvent.press(screen.getByRole("button", { name: translate("products.color.white") }));
    await fireEvent.press(
      screen.getByRole("button", { name: translateCount("products.colors.done", 2) }),
    );
    await fireEvent.changeText(screen.getByLabelText("S · Negro"), "4");
    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() =>
      expect(jest.mocked(createProduct).mock.calls[0][0].variants).toEqual([
        { id: null, size: "S", color: "black", initialStock: 4 },
        { id: null, size: "S", color: "white", initialStock: 0 },
        { id: null, size: "M", color: "black", initialStock: 0 },
        { id: null, size: "M", color: "white", initialStock: 0 },
      ]),
    );
  });
});
