import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { AppText } from "@/ui/AppText";
import { OutlinedBlock } from "@/ui/OutlinedBlock";

describe("When an outlined block is drawn", () => {
  it("Then its content is shown", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <OutlinedBlock>
          <AppText>Racha</AppText>
        </OutlinedBlock>
      </ThemeProvider>,
    );

    expect(screen.getByText("Racha")).toBeOnTheScreen();
  });
});
