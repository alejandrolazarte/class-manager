import { screen } from "@testing-library/react-native";
import { listClassPacks } from "@/features/classPacks/classPacksApi";
import { CounterSaleScreen } from "@/features/orders/screens/CounterSaleScreen";
import { listProducts } from "@/features/products/productsApi";
import { translate } from "@/i18n/translate";
import { buildProduct } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/orders/ordersApi");
jest.mock("@/features/products/productsApi");
jest.mock("@/features/classPacks/classPacksApi");

describe("When counter sale has nothing yet", () => {
  beforeEach(() => {
    jest.mocked(listProducts).mockResolvedValue([buildProduct()]);
    jest.mocked(listClassPacks).mockResolvedValue([]);
  });

  it("Then continue is disabled", async () => {
    await renderWithProviders(<CounterSaleScreen />);

    expect(
      await screen.findByRole("button", { name: translate("common.continue") }),
    ).toBeDisabled();
  });
});
