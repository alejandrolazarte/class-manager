import { fireEvent, screen } from "@testing-library/react-native";
import { listProducts } from "@/features/products/productsApi";
import { ProductListScreen } from "@/features/products/screens/ProductListScreen";
import { translate } from "@/i18n/translate";
import { buildProduct } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/products/productsApi");

const capWithASoldOutSize = buildProduct({
  variants: [
    { id: "0192f0c4-0000-7000-8000-00000000b0a1", size: "S", color: null, stock: 0 },
    { id: "0192f0c4-0000-7000-8000-00000000b0a2", size: "M", color: null, stock: 3 },
  ],
});

const bottle = buildProduct({
  id: "0192f0c4-0000-7000-8000-00000000b002",
  name: "Botella",
  variants: [{ id: "0192f0c4-0000-7000-8000-00000000b0b1", size: "", color: null, stock: 5 }],
});

describe("When products are searched", () => {
  beforeEach(() => {
    jest.mocked(listProducts).mockResolvedValue([capWithASoldOutSize, bottle]);
  });

  it("Then only matching products are listed", async () => {
    await renderWithProviders(<ProductListScreen />);

    await fireEvent.changeText(
      await screen.findByLabelText(translate("products.list.search")),
      "bote",
    );

    expect(screen.getByText(bottle.name)).toBeOnTheScreen();
    expect(screen.queryByText(capWithASoldOutSize.name)).toBeNull();
  });
});
