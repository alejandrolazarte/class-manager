import { fireEvent, screen } from "@testing-library/react-native";
import { getClient } from "@/features/clients/clientsApi";
import { ClientDetailScreen } from "@/features/clients/screens/ClientDetailScreen";
import { listStudentEnrollments } from "@/features/enrollments/enrollmentsApi";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import { buildStudentEnrollment } from "@/testing/enrollmentFactory";
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

describe("When an enrolled student is deleted", () => {
  beforeEach(() => {
    jest.mocked(getClient).mockResolvedValue(client);
    jest.mocked(listStudentEnrollments).mockImplementation((studentId) =>
      Promise.resolve(
        studentId === child.id
          ? [
              buildStudentEnrollment(),
              buildStudentEnrollment({
                enrollmentId: "0192f0e1-0000-7000-8000-000000000002",
                classGroupName: "Grupo de la tarde",
              }),
            ]
          : [],
      ),
    );
  });

  it("Then the dialog says how many classes they leave", async () => {
    await renderWithProviders(<ClientDetailScreen clientId={client.id} />);
    await screen.findByText("Grupo de la tarde");

    await fireEvent.press(
      screen.getByLabelText(translate("students.delete.accessibility", { name: child.fullName })),
    );

    expect(
      await screen.findByText(translate("students.delete.enrollments.other", { count: 2 })),
    ).toBeOnTheScreen();
  });
});
