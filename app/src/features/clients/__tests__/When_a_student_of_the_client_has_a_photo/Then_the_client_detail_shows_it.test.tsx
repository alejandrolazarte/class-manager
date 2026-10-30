import { screen } from "@testing-library/react-native";
import { getClient } from "@/features/clients/clientsApi";
import { ClientDetailScreen } from "@/features/clients/screens/ClientDetailScreen";
import { listStudentEnrollments } from "@/features/enrollments/enrollmentsApi";
import { buildClient } from "@/testing/clientFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildStudent } from "@/testing/studentFactory";

jest.mock("@/features/clients/clientsApi");
jest.mock("@/features/enrollments/enrollmentsApi");
jest.mock("@/features/fees/feesApi");
jest.mock("@/features/classPacks/classPacksApi");

const photoUrl = "https://files.example.com/private/photo.webp?sig=abc";
const client = buildClient({
  students: [
    buildStudent({ id: "student-1", fullName: "Tomás Pérez", photoUrl }),
    buildStudent({ id: "student-2", fullName: "Lucía Pérez" }),
  ],
});

describe("When a student of the client has a photo", () => {
  beforeEach(() => {
    jest.mocked(getClient).mockResolvedValue(client);
    jest.mocked(listStudentEnrollments).mockResolvedValue([]);
  });

  it("Then the client detail shows it", async () => {
    await renderWithProviders(<ClientDetailScreen clientId={client.id} />);

    expect(await screen.findByText("Tomás Pérez")).toBeOnTheScreen();
    expect(screen.getByTestId("avatar-photo", { includeHiddenElements: true })).toHaveProp(
      "source",
      { uri: photoUrl },
    );
  });
});
