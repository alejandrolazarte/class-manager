import { render, screen } from "@testing-library/react-native";
import { Avatar } from "@/ui/Avatar";

describe("When an avatar has no photo", () => {
  it("Then the initials are shown", async () => {
    await render(<Avatar name="Laura Gómez" photoUrl={null} />);

    expect(screen.getByText("LG", { includeHiddenElements: true })).toBeTruthy();
    expect(screen.queryByTestId("avatar-photo", { includeHiddenElements: true })).toBeNull();
  });
});
