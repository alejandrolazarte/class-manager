import { fireEvent, screen } from "@testing-library/react-native";
import { ProductDetailPage } from "@/features/studentApp/components/ProductDetailPage";
import { StudentAppShopProduct } from "@/features/studentApp/types";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

const product: StudentAppShopProduct = {
  id: "product-1",
  name: "Remera",
  description: null,
  price: 12,
  images: [],
  variants: [
    { id: "s-black", size: "S", color: "black", availability: "Available", maximumUnits: null },
    { id: "l-black", size: "L", color: "black", availability: "Available", maximumUnits: null },
    { id: "s-white", size: "S", color: "white", availability: "Available", maximumUnits: null },
    { id: "l-white", size: "L", color: "white", availability: "SoldOut", maximumUnits: null },
  ],
};

describe("When student picks a color of a product", () => {
  it("Then sizes sold out in that color are disabled", async () => {
    await renderWithProviders(
      <ProductDetailPage
        product={product}
        currencyCode="EUR"
        onClose={() => undefined}
        onAdd={() => undefined}
      />,
    );
    expect(screen.getByRole("button", { name: "L" })).toBeEnabled();

    await fireEvent.press(screen.getByRole("button", { name: translate("products.color.white") }));

    expect(screen.getByRole("button", { name: "L" })).toBeDisabled();
  });
});
