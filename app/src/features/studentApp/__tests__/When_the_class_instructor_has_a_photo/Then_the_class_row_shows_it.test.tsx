import { screen } from "@testing-library/react-native";
import { ScheduledClassRow } from "@/features/studentApp/components/ScheduledClassRow";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildStudentAppNextClass } from "@/testing/studentAppFactory";

const photoUrl = "https://files.example.com/private/photo.webp?sig=abc";

describe("When the class instructor has a photo", () => {
  it("Then the class row shows it", async () => {
    await renderWithProviders(
      <ScheduledClassRow
        studentId="student-1"
        scheduledClass={buildStudentAppNextClass({ instructorPhotoUrl: photoUrl })}
        isToday={false}
        onOpen={jest.fn()}
      />,
    );

    expect(screen.getByTestId("avatar-photo", { includeHiddenElements: true })).toHaveProp(
      "source",
      { uri: photoUrl },
    );
  });
});
