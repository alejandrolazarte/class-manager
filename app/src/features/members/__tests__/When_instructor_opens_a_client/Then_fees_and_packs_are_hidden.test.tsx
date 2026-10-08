import { screen } from "@testing-library/react-native";
import { getClassBalance } from "@/features/classPacks/classPacksApi";
import { getClient } from "@/features/clients/clientsApi";
import { ClientDetailScreen } from "@/features/clients/screens/ClientDetailScreen";
import { listStudentEnrollments } from "@/features/enrollments/enrollmentsApi";
import { listClientPayments } from "@/features/fees/feesApi";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import { buildInstructorMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");
jest.mock("@/features/enrollments/enrollmentsApi");
jest.mock("@/features/fees/feesApi");
jest.mock("@/features/classPacks/classPacksApi");

const client = buildClient({ billingPlan: { kind: "ClassPacks", customFee: null } });

describe("When instructor opens a client", () => {
  beforeEach(() => {
    jest.mocked(getClient).mockResolvedValue(client);
    jest.mocked(listStudentEnrollments).mockResolvedValue([]);
  });

  it("Then fees and packs are hidden", async () => {
    await renderWithProviders(<ClientDetailScreen clientId={client.id} />, {
      member: buildInstructorMember(),
    });

    expect(await screen.findByText(client.fullName)).toBeOnTheScreen();
    expect(screen.getByText(translate("clients.detail.addStudent"))).toBeOnTheScreen();
    expect(screen.queryByRole("tab", { name: translate("fees.client.title") })).toBeNull();
    expect(listClientPayments).not.toHaveBeenCalled();
    expect(getClassBalance).not.toHaveBeenCalled();
  });
});
