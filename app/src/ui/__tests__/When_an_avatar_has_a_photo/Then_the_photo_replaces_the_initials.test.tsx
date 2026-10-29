import { render, screen } from "@testing-library/react-native";
import { Avatar } from "@/ui/Avatar";

const photoUrl = "https://files.example.com/private/laura.webp?sig=abc";

describe("When an avatar has a photo", () => {
  it("Then the photo replaces the initials", async () => {
    await render(<Avatar name="Laura Gómez" photoUrl={photoUrl} />);

    expect(screen.getByTestId("avatar-photo", { includeHiddenElements: true })).toHaveProp(
      "source",
      { uri: photoUrl },
    );
    expect(screen.queryByText("LG", { includeHiddenElements: true })).toBeNull();
  });
});
