import { fireEvent, screen } from "@testing-library/react-native";
import { listClassPacks } from "@/features/classPacks/classPacksApi";
import { CounterSaleScreen } from "@/features/orders/screens/CounterSaleScreen";
import { listProducts } from "@/features/products/productsApi";
import { translate } from "@/i18n/translate";
import { buildProduct } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/orders/ordersApi");
jest.mock("@/features/products/productsApi");
jest.mock("@/features/classPacks/classPacksApi");

const capWithSizes = buildProduct({
  variants: [
    { id: "0192f0c4-0000-7000-8000-00000000b0a1", size: "S", color: null, stock: 2 },
    { id: "0192f0c4-0000-7000-8000-00000000b0a2", size: "M", color: null, stock: 0 },
    { id: "0192f0c4-0000-7000-8000-00000000b0a3", size: "L", color: null, stock: 4 },
  ],
});

function addButtonName(size: string): string {
  return translate("orders.counterSale.add", { name: `${capWithSizes.name} · ${size}` });
}

describe("When counter sale is filtered by size", () => {
  beforeEach(() => {
    jest.mocked(listProducts).mockResolvedValue([capWithSizes]);
    jest.mocked(listClassPacks).mockResolvedValue([]);
  });

  it("Then only that size is listed", async () => {
    await renderWithProviders(<CounterSaleScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("orders.counterSale.addItems") }),
    );
    await fireEvent.press(await screen.findByRole("button", { name: translate("filters.size") }));
    await fireEvent.press(screen.getByRole("button", { name: "L" }));

    expect(screen.getByRole("button", { name: addButtonName("L") })).toBeOnTheScreen();
    expect(screen.queryByRole("button", { name: addButtonName("S") })).toBeNull();
  });
});
