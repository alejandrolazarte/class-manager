import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { createProduct } from "@/features/products/productsApi";
import { ProductFormScreen } from "@/features/products/screens/ProductFormScreen";
import { translate } from "@/i18n/translate";
import { buildProduct } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/products/productsApi");

describe("When product is created with sizes", () => {
  beforeEach(() => {
    jest.mocked(createProduct).mockResolvedValue(buildProduct());
  });

  it("Then each size is sent", async () => {
    await renderWithProviders(<ProductFormScreen />);

    await fireEvent.changeText(screen.getByLabelText(translate("products.form.name")), "Malla");
    await fireEvent.changeText(screen.getByLabelText(translate("products.form.price")), "35,5");
    await fireEvent.changeText(
      screen.getByLabelText(translate("products.form.variants")),
      "S, M, L",
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() =>
      expect(createProduct).toHaveBeenCalledWith({
        name: "Malla",
        description: null,
        price: 35.5,
        stockMode: "Tracked",
        isVisibleInApp: true,
        variants: [
          { id: null, name: "S" },
          { id: null, name: "M" },
          { id: null, name: "L" },
        ],
      }),
    );
  });
});
