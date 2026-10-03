import { screen } from "@testing-library/react-native";
import { getClient } from "@/features/clients/clientsApi";
import { ClientDetailScreen } from "@/features/clients/screens/ClientDetailScreen";
import { listStudentEnrollments } from "@/features/enrollments/enrollmentsApi";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import { buildStudent } from "@/testing/studentFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");
jest.mock("@/features/enrollments/enrollmentsApi");

const client = buildClient({ students: [buildStudent({ fullName: "ana pérez " })] });

describe("When client is the only student", () => {
  beforeEach(() => {
    jest.mocked(getClient).mockResolvedValue(client);
    jest.mocked(listStudentEnrollments).mockResolvedValue([]);
  });

  it("Then the family label is hidden", async () => {
    await renderWithProviders(<ClientDetailScreen clientId={client.id} />);

    expect(await screen.findByText(client.fullName)).toBeOnTheScreen();
    expect(screen.queryByText(translate("clients.detail.title"))).toBeNull();
  });
});
