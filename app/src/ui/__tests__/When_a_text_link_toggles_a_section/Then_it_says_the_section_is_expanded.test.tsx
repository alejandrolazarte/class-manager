import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { TextLink } from "@/ui/TextLink";

describe("When a text link toggles a section", () => {
  it("Then it says the section is expanded", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <TextLink
          label="2 cambios próximos"
          trailingIcon="collapse"
          isExpanded
          onPress={jest.fn()}
        />
      </ThemeProvider>,
    );

    expect(
      screen.getByRole("button", { name: "2 cambios próximos", expanded: true }),
    ).toBeOnTheScreen();
  });
});
