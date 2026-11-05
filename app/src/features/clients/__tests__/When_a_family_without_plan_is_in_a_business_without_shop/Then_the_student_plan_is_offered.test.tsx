import { fireEvent, screen } from "@testing-library/react-native";
import { getClassBalance } from "@/features/classPacks/classPacksApi";
import { getClient } from "@/features/clients/clientsApi";
import { ClientDetailScreen } from "@/features/clients/screens/ClientDetailScreen";
import { listStudentEnrollments } from "@/features/enrollments/enrollmentsApi";
import { featureCodes } from "@/features/subscriptions/subscriptionCodes";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { buildClassBalance } from "@/testing/classPackFactory";
import { buildClient } from "@/testing/clientFactory";
import { routerMock } from "@/testing/expoRouterMock";
import { buildStudentPlan } from "@/testing/feeFactory";
import { buildCurrentMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildStudent } from "@/testing/studentFactory";
import { buildSubscription } from "@/testing/subscriptionFactory";

jest.mock("@/features/clients/clientsApi");
jest.mock("@/features/enrollments/enrollmentsApi");
jest.mock("@/features/classPacks/classPacksApi");

const member = buildCurrentMember({
  subscription: buildSubscription({ features: [{ code: featureCodes.classPacks, limit: null }] }),
});
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

describe("When a family without plan is in a business without shop", () => {
  beforeEach(() => {
    jest.mocked(getClient).mockResolvedValue(client);
    jest.mocked(listStudentEnrollments).mockResolvedValue([]);
    jest
      .mocked(getClassBalance)
      .mockResolvedValue(buildClassBalance({ availableClasses: 0, purchases: [] }));
  });

  it("Then the student plan is offered", async () => {
    await renderWithProviders(<ClientDetailScreen clientId={client.id} />, { member });

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("clients.detail.choosePlan") }),
    );

    expect(routerMock.push).toHaveBeenCalledWith(
      routes.studentPlan("students", client.id, student.id),
    );
  });
});
