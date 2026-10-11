import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { BulletRow } from "@/ui/BulletRow";

describe("When a bullet row is drawn", () => {
  it("Then label and detail are shown", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <BulletRow label="Nivel inicial" detail="Lun y mié · 10:00" />
      </ThemeProvider>,
    );

    expect(screen.getByText("Nivel inicial")).toBeOnTheScreen();
    expect(screen.getByText("Lun y mié · 10:00")).toBeOnTheScreen();
  });
});
