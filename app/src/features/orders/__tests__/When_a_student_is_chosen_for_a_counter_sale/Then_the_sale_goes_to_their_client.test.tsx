import { fireEvent, screen, waitFor, within } from "@testing-library/react-native";
import { listClassPacks } from "@/features/classPacks/classPacksApi";
import { createCounterSale, listDeliveryClasses } from "@/features/orders/ordersApi";
import { CounterSaleScreen } from "@/features/orders/screens/CounterSaleScreen";
import { listProducts } from "@/features/products/productsApi";
import { searchStudents } from "@/features/students/studentsApi";
import { translate } from "@/i18n/translate";
import { buildOrder, buildProduct } from "@/testing/productFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildStudentSummary } from "@/testing/studentFactory";

jest.mock("@/features/orders/ordersApi");
jest.mock("@/features/products/productsApi");
jest.mock("@/features/classPacks/classPacksApi");
jest.mock("@/features/students/studentsApi");

const student = buildStudentSummary();

describe("When a student is chosen for a counter sale", () => {
  beforeEach(() => {
    jest.mocked(listProducts).mockResolvedValue([buildProduct()]);
    jest.mocked(listClassPacks).mockResolvedValue([]);
    jest.mocked(listDeliveryClasses).mockResolvedValue([]);
    jest.mocked(searchStudents).mockResolvedValue([student]);
    jest.mocked(createCounterSale).mockResolvedValue(buildOrder());
  });

  it("Then the sale goes to their client", async () => {
    const product = buildProduct();
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
    await fireEvent.press(catalog.getByRole("button", { name: translate("common.done") }));
    await fireEvent.press(screen.getByRole("button", { name: translate("common.continue") }));

    await fireEvent.press(
      screen.getByRole("button", { name: translate("orders.counterSale.charge") }),
    );

    await waitFor(() =>
      expect(createCounterSale).toHaveBeenCalledWith(
        expect.objectContaining({ clientId: student.clientId }),
      ),
    );
  });
});
