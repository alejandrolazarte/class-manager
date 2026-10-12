import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { FilterOptionRow } from "@/ui/FilterOptionRow";

describe("When a filter option allows several choices", () => {
  it("Then it is a checkbox", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <FilterOptionRow
          label="Negro"
          count={3}
          choice="multiple"
          isSelected
          onPress={() => undefined}
        />
      </ThemeProvider>,
    );

    expect(screen.getByRole("checkbox", { name: "Negro" })).toBeChecked();
  });
});
