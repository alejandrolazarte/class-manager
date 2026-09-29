import { screen } from "@testing-library/react-native";
import { AppearanceSettings } from "@/features/settings/components/ThemePicker";
import { renderWithProviders } from "@/testing/renderWithProviders";

const builtInThemeNames = ["Agua", "Violeta", "Océano", "Rosa", "Ciruela", "Índigo", "Grafito"];

describe("When color picker is shown", () => {
  it("Then every built in theme is offered by name", async () => {
    await renderWithProviders(<AppearanceSettings />);

    const offeredThemeNames = builtInThemeNames.filter((themeName) =>
      screen.queryByRole("button", { name: themeName }),
    );

    expect(offeredThemeNames).toEqual(builtInThemeNames);
  });
});
