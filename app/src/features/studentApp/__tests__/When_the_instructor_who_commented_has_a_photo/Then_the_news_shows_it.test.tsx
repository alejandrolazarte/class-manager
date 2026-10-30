import { screen } from "@testing-library/react-native";
import { NewsRow } from "@/features/studentApp/components/NewsRow";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildStudentAppNewsItem } from "@/testing/studentAppFactory";

const photoUrl = "https://files.example.com/private/photo.webp?sig=abc";

describe("When the instructor who commented has a photo", () => {
  it("Then the news shows it", async () => {
    await renderWithProviders(
      <NewsRow
        item={buildStudentAppNewsItem({
          kind: "InstructorFeedback",
          title: null,
          body: "Muy buen ritmo",
          studentFullNames: ["Tomás Pérez"],
          className: "Natación inicial",
          instructorFullName: "Laura Gómez",
          classDate: "2026-10-28",
          instructorPhotoUrl: photoUrl,
        })}
      />,
    );

    expect(screen.getByTestId("avatar-photo", { includeHiddenElements: true })).toHaveProp(
      "source",
      { uri: photoUrl },
    );
  });
});
