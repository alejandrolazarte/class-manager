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

const client = buildClient({
  students: [
    buildStudent({ fullName: "Ana Pérez" }),
    buildStudent({ id: "0192f0c5-0000-7000-8000-000000000002", fullName: "Tomás Pérez" }),
  ],
});

describe("When client has another student", () => {
  beforeEach(() => {
    jest.mocked(getClient).mockResolvedValue(client);
    jest.mocked(listStudentEnrollments).mockResolvedValue([]);
  });

  it("Then the family label is shown", async () => {
    await renderWithProviders(<ClientDetailScreen clientId={client.id} />);

    expect(await screen.findByText(translate("clients.detail.title"))).toBeOnTheScreen();
    expect(screen.getByText("Tomás Pérez")).toBeOnTheScreen();
    expect(screen.getAllByText(client.fullName)).toHaveLength(2);
  });
});
