import Ionicons from "@expo/vector-icons/Ionicons";
import { render, screen, within } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { ChipGroup } from "@/ui/ChipGroup";

describe("When a chip group option has an icon", () => {
  it("Then the chip draws it", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <ChipGroup
          options={[
            { value: "Event", label: "Evento", icon: "celebration" },
            { value: "Reminder", label: "Recordatorio", icon: "notifications" },
          ]}
          selectedValue="Event"
          onChange={jest.fn()}
        />
      </ThemeProvider>,
    );

    const glyph = String.fromCodePoint(Ionicons.glyphMap["notifications-outline"] as number);
    expect(
      within(screen.getByRole("button", { name: "Recordatorio" })).getByText(glyph, {
        includeHiddenElements: true,
      }),
    ).toBeOnTheScreen();
  });
});
