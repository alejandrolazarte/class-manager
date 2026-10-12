import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { createProduct } from "@/features/products/productsApi";
import { ProductFormScreen } from "@/features/products/screens/ProductFormScreen";
import { translate } from "@/i18n/translate";
import { buildProduct } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/products/productsApi");

describe("When product is created with initial stock", () => {
  beforeEach(() => {
    jest.mocked(createProduct).mockResolvedValue(buildProduct());
  });

  it("Then the units of each size are sent", async () => {
    await renderWithProviders(<ProductFormScreen />);

    await fireEvent.changeText(screen.getByLabelText(translate("products.form.name")), "Remera");
    await fireEvent.changeText(screen.getByLabelText(translate("products.form.price")), "12");
    await fireEvent.press(
      screen.getByRole("button", { name: translate("products.form.variantsLetters") }),
    );
    const increaseSmall = screen.getByRole("button", {
      name: translate("products.stock.increase", { name: "S" }),
    });
    await fireEvent.press(increaseSmall);
    await fireEvent.press(increaseSmall);
    await fireEvent.changeText(screen.getByLabelText("L"), "4");
    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() =>
      expect(jest.mocked(createProduct).mock.calls[0][0].variants).toEqual([
        { id: null, size: "XS", color: null, initialStock: 0 },
        { id: null, size: "S", color: null, initialStock: 2 },
        { id: null, size: "M", color: null, initialStock: 0 },
        { id: null, size: "L", color: null, initialStock: 4 },
        { id: null, size: "XL", color: null, initialStock: 0 },
        { id: null, size: "XXL", color: null, initialStock: 0 },
      ]),
    );
  });
});
