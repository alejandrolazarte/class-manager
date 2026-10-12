import { render, screen } from "@testing-library/react-native";
import { translate } from "@/i18n/translate";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { FilterBar } from "@/ui/FilterBar";

describe("When no filter is active", () => {
  it("Then clear filters is hidden", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <FilterBar
          items={[{ key: "size", label: "Talle", isActive: false, onPress: () => undefined }]}
          onClear={() => undefined}
        />
      </ThemeProvider>,
    );

    expect(screen.getByRole("button", { name: "Talle" })).toBeOnTheScreen();
    expect(screen.queryByRole("button", { name: translate("filters.clear") })).toBeNull();
  });
});
