import { fireEvent, render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { ChipGroup } from "@/ui/ChipGroup";

jest.mock("@/ui/motion/useReducedMotion", () => ({ useReducedMotion: () => true }));

describe("When reduced motion is on", () => {
  it("Then a sliding selection changes the option", async () => {
    const onChange = jest.fn();
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <ChipGroup
          options={[
            { value: "debtors", label: "Deben" },
            { value: "all", label: "Todos" },
          ]}
          selectedValue="debtors"
          onChange={onChange}
        />
      </ThemeProvider>,
    );

    await fireEvent.press(screen.getByRole("button", { name: "Todos" }));

    expect(onChange).toHaveBeenCalledWith("all");
    expect(screen.getByRole("button", { name: "Deben" })).toBeSelected();
  });
});
