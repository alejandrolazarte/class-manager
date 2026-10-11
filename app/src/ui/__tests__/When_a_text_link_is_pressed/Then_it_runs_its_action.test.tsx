import { fireEvent, render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { TextLink } from "@/ui/TextLink";

describe("When a text link is pressed", () => {
  it("Then it runs its action", async () => {
    const seeAll = jest.fn();
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <TextLink label="Ver todas" trailingIcon="next" onPress={seeAll} />
      </ThemeProvider>,
    );

    await fireEvent.press(screen.getByRole("button", { name: "Ver todas" }));

    expect(seeAll).toHaveBeenCalledTimes(1);
  });
});
