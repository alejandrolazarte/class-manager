import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { AmountBreakdown } from "@/ui/AmountBreakdown";

describe("When an amount breakdown is drawn", () => {
  it("Then every line shows its label and amount", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <AmountBreakdown
          items={[
            { key: "classes", label: "Clases", amount: "$1.250.000" },
            { key: "plans", label: "Planes", amount: "$1.000.000", isNested: true },
          ]}
        />
      </ThemeProvider>,
    );

    expect(screen.getByText("Clases")).toBeOnTheScreen();
    expect(screen.getByText("$1.250.000")).toBeOnTheScreen();
    expect(screen.getByText("Planes")).toBeOnTheScreen();
    expect(screen.getByText("$1.000.000")).toBeOnTheScreen();
  });
});
