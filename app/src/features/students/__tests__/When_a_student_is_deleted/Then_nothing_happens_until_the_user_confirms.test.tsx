import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { getClient } from "@/features/clients/clientsApi";
import { ClientDetailScreen } from "@/features/clients/screens/ClientDetailScreen";
import { listStudentEnrollments } from "@/features/enrollments/enrollmentsApi";
import { deleteStudent } from "@/features/students/studentsApi";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import { buildStudent } from "@/testing/studentFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");
jest.mock("@/features/enrollments/enrollmentsApi");
jest.mock("@/features/students/studentsApi");

const child = buildStudent({ fullName: "Tomás Pérez" });
const client = buildClient({
  students: [
    buildStudent({ id: "0192f0c5-0000-7000-8000-000000000002", fullName: "Ana Pérez" }),
    child,
  ],
});

describe("When a student is deleted", () => {
  beforeEach(() => {
    jest.mocked(getClient).mockResolvedValue(client);
    jest.mocked(listStudentEnrollments).mockResolvedValue([]);
    jest.mocked(deleteStudent).mockResolvedValue(undefined);
  });

  it("Then nothing happens until the user confirms", async () => {
    await renderWithProviders(<ClientDetailScreen clientId={client.id} />);

    await fireEvent.press(
      await screen.findByLabelText(
        translate("students.delete.accessibility", { name: child.fullName }),
      ),
    );
    expect(
      await screen.findByText(translate("students.delete.question", { name: child.fullName })),
    ).toBeOnTheScreen();
    expect(deleteStudent).not.toHaveBeenCalled();

    await fireEvent.press(
      screen.getByRole("button", { name: translate("students.delete.confirm") }),
    );

    await waitFor(() => expect(deleteStudent).toHaveBeenCalledWith(child.id));
  });
});
