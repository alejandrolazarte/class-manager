import { fireEvent, render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { Checkbox } from "@/ui/Checkbox";

describe("When a checkbox is pressed", () => {
  it("Then it toggles its value", async () => {
    const changeChecked = jest.fn();
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <Checkbox label="Acepto" isChecked={false} onCheckedChange={changeChecked} />
      </ThemeProvider>,
    );

    await fireEvent.press(screen.getByRole("checkbox", { name: "Acepto" }));

    expect(changeChecked).toHaveBeenCalledWith(true);
  });
});
