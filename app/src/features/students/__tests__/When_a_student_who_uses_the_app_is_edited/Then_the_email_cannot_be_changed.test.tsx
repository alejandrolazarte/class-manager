import { screen } from "@testing-library/react-native";
import { getClient } from "@/features/clients/clientsApi";
import { EditStudentScreen } from "@/features/students/screens/EditStudentScreen";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import { buildStudent } from "@/testing/studentFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");
jest.mock("@/features/students/studentsApi");

const child = buildStudent({
  email: "tomas@example.com",
  appAccess: { status: "Active", invitedEmail: null, signInEmail: "tomas@example.com" },
});
const client = buildClient({ students: [child] });

describe("When a student who uses the app is edited", () => {
  beforeEach(() => {
    jest.mocked(getClient).mockResolvedValue(client);
  });

  it("Then the email cannot be changed", async () => {
    await renderWithProviders(<EditStudentScreen clientId={client.id} studentId={child.id} />);

    expect(await screen.findByLabelText(translate("students.fields.email"))).toHaveProp(
      "editable",
      false,
    );
    expect(screen.getByText(translate("clients.edit.emailHintActive"))).toBeOnTheScreen();
  });
});
