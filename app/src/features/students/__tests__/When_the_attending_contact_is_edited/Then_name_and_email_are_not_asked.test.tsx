import { screen } from "@testing-library/react-native";
import { getClient } from "@/features/clients/clientsApi";
import { EditStudentScreen } from "@/features/students/screens/EditStudentScreen";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import { buildStudent } from "@/testing/studentFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");
jest.mock("@/features/students/studentsApi");

const contact = buildStudent({ fullName: "Ana Pérez", birthDate: "1985-01-20" });
const client = buildClient({ fullName: "Ana Pérez", students: [contact] });

describe("When the attending contact is edited", () => {
  beforeEach(() => {
    jest.mocked(getClient).mockResolvedValue(client);
  });

  it("Then name and email are not asked", async () => {
    await renderWithProviders(<EditStudentScreen clientId={client.id} studentId={contact.id} />);

    expect(await screen.findByLabelText(translate("students.fields.birthDate"))).toBeOnTheScreen();
    expect(screen.queryByLabelText(translate("students.fields.fullName"))).not.toBeOnTheScreen();
    expect(screen.queryByLabelText(translate("students.fields.email"))).not.toBeOnTheScreen();
  });
});
