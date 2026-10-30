import { screen } from "@testing-library/react-native";
import { StudentListItem } from "@/features/students/components/StudentListItem";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildStudentSummary } from "@/testing/studentFactory";

const photoUrl = "https://files.example.com/private/photo.webp?sig=abc";

describe("When a student has a photo", () => {
  it("Then the list shows it", async () => {
    await renderWithProviders(
      <StudentListItem student={buildStudentSummary({ photoUrl })} onPress={jest.fn()} />,
    );

    expect(screen.getByTestId("avatar-photo", { includeHiddenElements: true })).toHaveProp(
      "source",
      { uri: photoUrl },
    );
  });
});
