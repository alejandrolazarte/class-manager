import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { AnswerButton } from "@/ui/AnswerButton";

describe("When an answer is chosen", () => {
  it("Then it takes its answer color", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <AnswerButton label="No voy" answer="no" state="chosen" onPress={jest.fn()} />
      </ThemeProvider>,
    );

    expect(screen.getByText("No voy").props.className).toContain("text-danger-foreground");
  });
});
