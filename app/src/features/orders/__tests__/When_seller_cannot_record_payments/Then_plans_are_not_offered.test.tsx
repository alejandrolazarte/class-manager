import { fireEvent, screen, within } from "@testing-library/react-native";
import { listClassPacks } from "@/features/classPacks/classPacksApi";
import { permissions } from "@/features/members/permissions";
import { CounterSaleScreen } from "@/features/orders/screens/CounterSaleScreen";
import { listProducts } from "@/features/products/productsApi";
import { translate } from "@/i18n/translate";
import { buildCurrentMember } from "@/testing/memberFactory";
import { buildMonthlyPlan } from "@/testing/monthlyPlanFactory";
import { buildProduct } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/orders/ordersApi");
jest.mock("@/features/products/productsApi");
jest.mock("@/features/classPacks/classPacksApi");

const plan = buildMonthlyPlan();
const product = buildProduct();
const seller = buildCurrentMember({
  permissions: [permissions.ordersViewAll, permissions.ordersManage, permissions.classPacksView],
});

describe("When seller cannot record payments", () => {
  beforeEach(() => {
    jest.mocked(listProducts).mockResolvedValue([product]);
    jest.mocked(listClassPacks).mockResolvedValue([plan]);
  });

  it("Then plans are not offered", async () => {
    await renderWithProviders(<CounterSaleScreen />, { member: seller });

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("orders.counterSale.addItems") }),
    );

    const catalog = within(screen.getByTestId("counter-sale-catalog"));
    expect(
      await catalog.findByRole("button", {
        name: translate("orders.counterSale.add", { name: product.name }),
      }),
    ).toBeOnTheScreen();
    expect(
      catalog.queryByRole("button", {
        name: translate("orders.counterSale.add", { name: plan.name }),
      }),
    ).toBeNull();
  });
});
