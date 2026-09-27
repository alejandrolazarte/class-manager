import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { themes } from "@/theme/themes";
import { Button } from "@/ui/Button";

describe("When button is loading", () => {
  it("Then spinner uses theme foreground color", async () => {
    await render(
      <ThemeProvider initialThemeName="ocean" initialColorSchemePreference="dark">
        <Button label="Guardar" onPress={() => undefined} isLoading />
      </ThemeProvider>,
    );

    expect(screen.getByTestId("button-spinner")).toHaveProp(
      "color",
      themes.ocean.dark["primary-foreground"],
    );
  });
});
