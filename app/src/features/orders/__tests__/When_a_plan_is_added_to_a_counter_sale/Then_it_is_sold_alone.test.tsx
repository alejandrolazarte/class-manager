import { fireEvent, screen, within } from "@testing-library/react-native";
import { listClassPacks } from "@/features/classPacks/classPacksApi";
import { listDeliveryClasses } from "@/features/orders/ordersApi";
import { CounterSaleScreen } from "@/features/orders/screens/CounterSaleScreen";
import { listProducts } from "@/features/products/productsApi";
import { searchStudents } from "@/features/students/studentsApi";
import { translate } from "@/i18n/translate";
import { buildMonthlyPlan } from "@/testing/monthlyPlanFactory";
import { buildProduct } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildStudentSummary } from "@/testing/studentFactory";

jest.mock("@/features/orders/ordersApi");
jest.mock("@/features/products/productsApi");
jest.mock("@/features/classPacks/classPacksApi");
jest.mock("@/features/students/studentsApi");

const student = buildStudentSummary();
const plan = buildMonthlyPlan();
const product = buildProduct();

describe("When a plan is added to a counter sale", () => {
  beforeEach(() => {
    jest.mocked(listProducts).mockResolvedValue([product]);
    jest.mocked(listClassPacks).mockResolvedValue([plan]);
    jest.mocked(listDeliveryClasses).mockResolvedValue([]);
    jest.mocked(searchStudents).mockResolvedValue([student]);
  });

  it("Then it is sold alone", async () => {
    await renderWithProviders(<CounterSaleScreen />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("orders.counterSale.chooseStudent") }),
    );
    await fireEvent.press(
      await within(screen.getByTestId("counter-sale-student-picker")).findByRole("button", {
        name: student.fullName,
      }),
    );
    await fireEvent.press(
      screen.getByRole("button", { name: translate("orders.counterSale.addItems") }),
    );
    const catalog = within(screen.getByTestId("counter-sale-catalog"));
    await fireEvent.press(
      await catalog.findByRole("button", {
        name: translate("orders.counterSale.add", { name: product.name }),
      }),
    );
    await fireEvent.press(
      catalog.getByRole("button", {
        name: translate("orders.counterSale.add", { name: plan.name }),
      }),
    );
    await fireEvent.press(catalog.getByRole("button", { name: translate("common.done") }));

    expect(screen.getByText(plan.name)).toBeOnTheScreen();
    expect(screen.queryByText(product.name)).toBeNull();
    expect(screen.getByText(translate("orders.counterSale.planSoldAlone"))).toBeOnTheScreen();
  });
});
