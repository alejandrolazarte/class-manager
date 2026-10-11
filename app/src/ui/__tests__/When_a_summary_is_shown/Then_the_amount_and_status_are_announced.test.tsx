import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { AppText } from "@/ui/AppText";
import { SummaryBlock } from "@/ui/SummaryBlock";

jest.mock("@/ui/motion/useReducedMotion", () => ({ useReducedMotion: () => true }));

describe("When a summary is shown", () => {
  it("Then the amount and status are announced", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <SummaryBlock
          label="Cobrado"
          amount="$412.000"
          total="de $560.000"
          ratio={0.5}
          accessibilityLabel="Cobrado $412.000 de $560.000"
        >
          <AppText>6 alumnos deben $148.000</AppText>
        </SummaryBlock>
      </ThemeProvider>,
    );

    expect(screen.getByLabelText("Cobrado $412.000 de $560.000")).toHaveTextContent(
      "Cobrado$412.000de $560.0006 alumnos deben $148.000",
    );
  });
});
