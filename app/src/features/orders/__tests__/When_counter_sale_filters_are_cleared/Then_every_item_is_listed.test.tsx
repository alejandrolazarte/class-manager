import { fireEvent, screen } from "@testing-library/react-native";
import { listClassPacks } from "@/features/classPacks/classPacksApi";
import { CounterSaleScreen } from "@/features/orders/screens/CounterSaleScreen";
import { listProducts } from "@/features/products/productsApi";
import { translate, translateCount } from "@/i18n/translate";
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

describe("When counter sale filters are cleared", () => {
  beforeEach(() => {
    jest.mocked(listProducts).mockResolvedValue([capWithSizes]);
    jest.mocked(listClassPacks).mockResolvedValue([]);
  });

  it("Then every item is listed", async () => {
    await renderWithProviders(<CounterSaleScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("orders.counterSale.addItems") }),
    );
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("filters.availability") }),
    );
    await fireEvent.press(
      screen.getByRole("radio", { name: translate("filters.availability.available") }),
    );
    await fireEvent.press(
      screen.getByRole("button", { name: translateCount("filters.showResults", 2) }),
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("filters.clear") }));

    expect(screen.getByRole("button", { name: addButtonName("M") })).toBeOnTheScreen();
  });
});
