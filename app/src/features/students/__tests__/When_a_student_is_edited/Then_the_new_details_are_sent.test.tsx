import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { getClient } from "@/features/clients/clientsApi";
import { EditStudentScreen } from "@/features/students/screens/EditStudentScreen";
import { updateStudent } from "@/features/students/studentsApi";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import { buildStudent } from "@/testing/studentFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");
jest.mock("@/features/students/studentsApi");

const child = buildStudent({ fullName: "Tomas Perez", notes: null });
const client = buildClient({ students: [child] });

describe("When a student is edited", () => {
  beforeEach(() => {
    jest.mocked(getClient).mockResolvedValue(client);
    jest.mocked(updateStudent).mockResolvedValue(child);
  });

  it("Then the new details are sent", async () => {
    await renderWithProviders(<EditStudentScreen clientId={client.id} studentId={child.id} />);
    await fireEvent.changeText(
      await screen.findByLabelText(translate("students.fields.fullName")),
      "Tomás Pérez",
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("students.fields.notes")),
      "Grupo avanzado",
    );

    await fireEvent.press(screen.getByRole("button", { name: translate("students.edit.submit") }));

    await waitFor(() =>
      expect(updateStudent).toHaveBeenCalledWith(child.id, {
        fullName: "Tomás Pérez",
        birthDate: "2018-03-14",
        notes: "Grupo avanzado",
        email: null,
      }),
    );
  });
});
