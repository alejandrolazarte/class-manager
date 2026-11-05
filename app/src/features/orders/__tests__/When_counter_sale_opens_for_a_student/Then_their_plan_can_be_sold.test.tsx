import { fireEvent, screen, waitFor, within } from "@testing-library/react-native";
import { listClassPacks } from "@/features/classPacks/classPacksApi";
import { getClient } from "@/features/clients/clientsApi";
import { sellStudentPlan } from "@/features/fees/feesApi";
import { listDeliveryClasses } from "@/features/orders/ordersApi";
import { CounterSaleScreen } from "@/features/orders/screens/CounterSaleScreen";
import { listProducts } from "@/features/products/productsApi";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import { buildMonthlyPlan } from "@/testing/monthlyPlanFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildStudent } from "@/testing/studentFactory";

jest.mock("@/features/orders/ordersApi");
jest.mock("@/features/products/productsApi");
jest.mock("@/features/classPacks/classPacksApi");
jest.mock("@/features/clients/clientsApi");
jest.mock("@/features/fees/feesApi");

const student = buildStudent({ fullName: "Ana Pérez" });
const client = buildClient({ students: [student] });
const plan = buildMonthlyPlan();

describe("When counter sale opens for a student", () => {
  beforeEach(() => {
    jest.mocked(listProducts).mockResolvedValue([]);
    jest.mocked(listClassPacks).mockResolvedValue([plan]);
    jest.mocked(listDeliveryClasses).mockResolvedValue([]);
    jest.mocked(getClient).mockResolvedValue(client);
    jest.mocked(sellStudentPlan).mockResolvedValue({ monthlyDue: null, students: [] });
  });

  it("Then their plan can be sold", async () => {
    await renderWithProviders(<CounterSaleScreen clientId={client.id} studentId={student.id} />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("orders.counterSale.addItems") }),
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
      expect(sellStudentPlan).toHaveBeenCalledWith(
        student.id,
        expect.objectContaining({ classPackId: plan.id }),
      ),
    );
  });
});
