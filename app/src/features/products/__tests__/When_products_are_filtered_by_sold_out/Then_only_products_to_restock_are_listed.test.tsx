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

describe("When products are filtered by sold out", () => {
  beforeEach(() => {
    jest.mocked(listProducts).mockResolvedValue([capWithASoldOutSize, bottle]);
  });

  it("Then only products to restock are listed", async () => {
    await renderWithProviders(<ProductListScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("filters.availability") }),
    );
    await fireEvent.press(
      screen.getByRole("radio", { name: translate("filters.availability.soldOut") }),
    );

    expect(screen.getByText(capWithASoldOutSize.name)).toBeOnTheScreen();
    expect(screen.queryByText(bottle.name)).toBeNull();
  });
});
