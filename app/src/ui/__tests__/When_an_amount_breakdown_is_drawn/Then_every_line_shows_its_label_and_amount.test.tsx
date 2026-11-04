import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { AmountBreakdown } from "@/ui/AmountBreakdown";

describe("When an amount breakdown is drawn", () => {
  it("Then every line shows its label and amount", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <AmountBreakdown
          items={[
            { key: "plans", label: "Planes", amount: "$1.000.000" },
            { key: "classPacks", label: "Packs", amount: "$250.000" },
          ]}
        />
      </ThemeProvider>,
    );

    expect(screen.getByText("Planes")).toBeOnTheScreen();
    expect(screen.getByText("$1.000.000")).toBeOnTheScreen();
    expect(screen.getByText("Packs")).toBeOnTheScreen();
    expect(screen.getByText("$250.000")).toBeOnTheScreen();
  });
});
