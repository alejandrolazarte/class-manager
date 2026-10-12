import { fireEvent, render, screen } from "@testing-library/react-native";
import { translate } from "@/i18n/translate";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { FilterBar } from "@/ui/FilterBar";

describe("When a filter is active", () => {
  it("Then clear filters is offered", async () => {
    const onClear = jest.fn();
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <FilterBar
          items={[{ key: "size", label: "Talle: S", isActive: true, onPress: () => undefined }]}
          onClear={onClear}
        />
      </ThemeProvider>,
    );

    await fireEvent.press(screen.getByRole("button", { name: translate("filters.clear") }));

    expect(onClear).toHaveBeenCalled();
  });
});
