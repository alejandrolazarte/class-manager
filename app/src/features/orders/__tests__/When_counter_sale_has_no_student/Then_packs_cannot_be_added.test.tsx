import { fireEvent, screen, within } from "@testing-library/react-native";
import { listClassPacks } from "@/features/classPacks/classPacksApi";
import { CounterSaleScreen } from "@/features/orders/screens/CounterSaleScreen";
import { listProducts } from "@/features/products/productsApi";
import { translate } from "@/i18n/translate";
import { buildClassPack } from "@/testing/classPackFactory";
import { buildProduct } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/orders/ordersApi");
jest.mock("@/features/products/productsApi");
jest.mock("@/features/classPacks/classPacksApi");

describe("When counter sale has no student", () => {
  beforeEach(() => {
    jest.mocked(listProducts).mockResolvedValue([buildProduct()]);
    jest.mocked(listClassPacks).mockResolvedValue([buildClassPack()]);
  });

  it("Then packs cannot be added", async () => {
    const classPack = buildClassPack();
    await renderWithProviders(<CounterSaleScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("orders.counterSale.addItems") }),
    );

    const catalog = within(screen.getByTestId("counter-sale-catalog"));
    expect(
      await catalog.findByRole("button", {
        name: translate("orders.counterSale.add", { name: classPack.name }),
      }),
    ).toBeDisabled();
    expect(catalog.getByText(translate("orders.counterSale.packsNeedStudent"))).toBeOnTheScreen();
  });
});
