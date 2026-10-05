import { screen } from "@testing-library/react-native";
import { getClassBalance } from "@/features/classPacks/classPacksApi";
import { getClient } from "@/features/clients/clientsApi";
import { ClientDetailScreen } from "@/features/clients/screens/ClientDetailScreen";
import { listStudentEnrollments } from "@/features/enrollments/enrollmentsApi";
import { listClientPayments } from "@/features/fees/feesApi";
import { translate } from "@/i18n/translate";
import { buildClassBalance } from "@/testing/classPackFactory";
import { buildClient } from "@/testing/clientFactory";
import { segmentsMock } from "@/testing/expoRouterMock";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");
jest.mock("@/features/enrollments/enrollmentsApi");
jest.mock("@/features/fees/feesApi");
jest.mock("@/features/classPacks/classPacksApi");

const client = buildClient();

describe("When client is opened from fees", () => {
  beforeEach(() => {
    segmentsMock.current = ["(tabs)", "fees", "clients", "[clientId]"];
    jest.mocked(getClient).mockResolvedValue(client);
    jest.mocked(listStudentEnrollments).mockResolvedValue([]);
    jest.mocked(listClientPayments).mockResolvedValue([]);
    jest.mocked(getClassBalance).mockResolvedValue(buildClassBalance());
  });

  it("Then the billing tab is selected", async () => {
    await renderWithProviders(<ClientDetailScreen clientId={client.id} />);

    expect(await screen.findByRole("tab", { name: translate("fees.client.title") })).toBeSelected();
    expect(await screen.findByText(translate("fees.client.recordPayment"))).toBeOnTheScreen();
  });
});
