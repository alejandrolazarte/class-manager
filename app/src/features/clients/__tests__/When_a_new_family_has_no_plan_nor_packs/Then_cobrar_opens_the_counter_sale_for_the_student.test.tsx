import { fireEvent, screen } from "@testing-library/react-native";
import { getClassBalance } from "@/features/classPacks/classPacksApi";
import { getClient } from "@/features/clients/clientsApi";
import { ClientDetailScreen } from "@/features/clients/screens/ClientDetailScreen";
import { listStudentEnrollments } from "@/features/enrollments/enrollmentsApi";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { buildClassBalance } from "@/testing/classPackFactory";
import { buildClient } from "@/testing/clientFactory";
import { routerMock } from "@/testing/expoRouterMock";
import { buildStudentPlan } from "@/testing/feeFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildStudent } from "@/testing/studentFactory";

jest.mock("@/features/clients/clientsApi");
jest.mock("@/features/enrollments/enrollmentsApi");
jest.mock("@/features/classPacks/classPacksApi");

const student = buildStudent({ fullName: "Ana Pérez" });
const client = buildClient({
  students: [student],
  plans: {
    monthlyDue: null,
    students: [
      buildStudentPlan({
        studentId: student.id,
        studentFullName: student.fullName,
        classPackId: null,
        planName: null,
        price: null,
        since: null,
      }),
    ],
  },
});

describe("When a new family has no plan nor packs", () => {
  beforeEach(() => {
    jest.mocked(getClient).mockResolvedValue(client);
    jest.mocked(listStudentEnrollments).mockResolvedValue([]);
    jest
      .mocked(getClassBalance)
      .mockResolvedValue(buildClassBalance({ availableClasses: 0, purchases: [] }));
  });

  it("Then cobrar opens the counter sale for the student", async () => {
    await renderWithProviders(<ClientDetailScreen clientId={client.id} />);

    expect(await screen.findByText(translate("clients.detail.noPlanNorPacks"))).toBeOnTheScreen();
    await fireEvent.press(
      screen.getByRole("button", { name: translate("orders.counterSale.charge") }),
    );

    expect(routerMock.push).toHaveBeenCalledWith(
      routes.clientCounterSale("students", client.id, student.id),
    );
  });
});
