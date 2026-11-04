import { fireEvent, screen, within } from "@testing-library/react-native";
import { listClassPacks } from "@/features/classPacks/classPacksApi";
import { CounterSaleScreen } from "@/features/orders/screens/CounterSaleScreen";
import { listProducts } from "@/features/products/productsApi";
import { translate } from "@/i18n/translate";
import { buildMonthlyPlan } from "@/testing/monthlyPlanFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/orders/ordersApi");
jest.mock("@/features/products/productsApi");
jest.mock("@/features/classPacks/classPacksApi");

const plan = buildMonthlyPlan();

describe("When counter sale has no student for a plan", () => {
  beforeEach(() => {
    jest.mocked(listProducts).mockResolvedValue([]);
    jest.mocked(listClassPacks).mockResolvedValue([plan]);
  });

  it("Then plans cannot be added", async () => {
    await renderWithProviders(<CounterSaleScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("orders.counterSale.addItems") }),
    );

    const catalog = within(screen.getByTestId("counter-sale-catalog"));
    expect(
      await catalog.findByRole("button", {
        name: translate("orders.counterSale.add", { name: plan.name }),
      }),
    ).toBeDisabled();
    expect(catalog.getByText(translate("orders.counterSale.plansNeedStudent"))).toBeOnTheScreen();
  });
});
