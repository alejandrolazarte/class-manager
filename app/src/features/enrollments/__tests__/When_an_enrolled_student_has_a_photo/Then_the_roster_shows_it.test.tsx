import { screen } from "@testing-library/react-native";
import { RosterItem } from "@/features/enrollments/components/RosterItem";
import { buildRosterEntry } from "@/testing/enrollmentFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

const photoUrl = "https://files.example.com/private/photo.webp?sig=abc";

describe("When an enrolled student has a photo", () => {
  it("Then the roster shows it", async () => {
    await renderWithProviders(
      <RosterItem entry={buildRosterEntry({ studentPhotoUrl: photoUrl })} today="2026-10-30" />,
    );

    expect(screen.getByTestId("avatar-photo", { includeHiddenElements: true })).toHaveProp(
      "source",
      { uri: photoUrl },
    );
  });
});
