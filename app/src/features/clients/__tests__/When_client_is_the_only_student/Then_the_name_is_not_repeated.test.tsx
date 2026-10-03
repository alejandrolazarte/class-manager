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

const client = buildClient({ students: [buildStudent({ fullName: "Ana Pérez" })] });

describe("When client is the only student", () => {
  beforeEach(() => {
    jest.mocked(getClient).mockResolvedValue(client);
    jest.mocked(listStudentEnrollments).mockResolvedValue([]);
  });

  it("Then the name is not repeated", async () => {
    await renderWithProviders(<ClientDetailScreen clientId={client.id} />);

    expect(await screen.findAllByText(client.fullName)).toHaveLength(1);
    expect(screen.getByText(translate("clients.detail.classes"))).toBeOnTheScreen();
    expect(await screen.findByText(translate("enrollments.studentClasses.none"))).toBeOnTheScreen();
    expect(screen.getByText(translate("clients.detail.addOtherPerson"))).toBeOnTheScreen();
  });
});
