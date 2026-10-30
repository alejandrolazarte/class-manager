import { screen } from "@testing-library/react-native";
import { FeedbackNote } from "@/features/studentApp/components/FeedbackNote";
import { renderWithProviders } from "@/testing/renderWithProviders";

const photoUrl = "https://files.example.com/private/photo.webp?sig=abc";

describe("When the instructor who commented has a photo", () => {
  it("Then it is shown next to the comment", async () => {
    await renderWithProviders(
      <FeedbackNote
        feedback={{
          date: "2026-10-28",
          className: "Natación inicial",
          instructorFullName: "Laura Gómez",
          text: "Muy buen ritmo",
          instructorPhotoUrl: photoUrl,
        }}
      />,
    );

    expect(screen.getByTestId("avatar-photo", { includeHiddenElements: true })).toHaveProp(
      "source",
      { uri: photoUrl },
    );
  });
});
