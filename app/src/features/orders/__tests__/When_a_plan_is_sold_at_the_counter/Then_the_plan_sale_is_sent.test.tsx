import { fireEvent, screen, waitFor, within } from "@testing-library/react-native";
import { listClassPacks } from "@/features/classPacks/classPacksApi";
import { sellStudentPlan } from "@/features/fees/feesApi";
import { createCounterSale, listDeliveryClasses } from "@/features/orders/ordersApi";
import { CounterSaleScreen } from "@/features/orders/screens/CounterSaleScreen";
import { listProducts } from "@/features/products/productsApi";
import { searchStudents } from "@/features/students/studentsApi";
import { translate } from "@/i18n/translate";
import { buildMonthlyPlan } from "@/testing/monthlyPlanFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildStudentSummary } from "@/testing/studentFactory";

jest.mock("@/features/orders/ordersApi");
jest.mock("@/features/products/productsApi");
jest.mock("@/features/classPacks/classPacksApi");
jest.mock("@/features/students/studentsApi");
jest.mock("@/features/fees/feesApi");

const student = buildStudentSummary();
const plan = buildMonthlyPlan();

describe("When a plan is sold at the counter", () => {
  beforeEach(() => {
    jest.mocked(listProducts).mockResolvedValue([]);
    jest.mocked(listClassPacks).mockResolvedValue([plan]);
    jest.mocked(listDeliveryClasses).mockResolvedValue([]);
    jest.mocked(searchStudents).mockResolvedValue([student]);
    jest.mocked(sellStudentPlan).mockResolvedValue({ monthlyDue: null, students: [] });
  });

  it("Then the plan sale is sent", async () => {
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
        name: translate("orders.counterSale.add", { name: plan.name }),
      }),
    );
    await fireEvent.press(catalog.getByRole("button", { name: translate("common.done") }));
    await fireEvent.press(screen.getByRole("button", { name: translate("common.continue") }));

    await fireEvent.press(
      screen.getByRole("button", { name: translate("orders.counterSale.charge") }),
    );

    await waitFor(() =>
      expect(sellStudentPlan).toHaveBeenCalledWith(student.id, {
        classPackId: plan.id,
        paidAmount: plan.price,
        method: "Cash",
        notes: null,
      }),
    );
    expect(createCounterSale).not.toHaveBeenCalled();
  });
});
