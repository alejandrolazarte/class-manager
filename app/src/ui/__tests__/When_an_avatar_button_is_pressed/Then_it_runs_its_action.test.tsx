import { fireEvent, render, screen } from "@testing-library/react-native";
import { AvatarButton } from "@/ui/AvatarButton";

describe("When an avatar button is pressed", () => {
  it("Then it runs its action", async () => {
    const openProfile = jest.fn();
    await render(
      <AvatarButton name="Laura Gómez" accessibilityLabel="Mi perfil" onPress={openProfile} />,
    );

    await fireEvent.press(screen.getByRole("button", { name: "Mi perfil" }));

    expect(openProfile).toHaveBeenCalledTimes(1);
  });
});
