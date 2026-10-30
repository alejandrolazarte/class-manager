import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { AnswerButton } from "@/ui/AnswerButton";

describe("When the other answer is chosen", () => {
  it("Then this answer fades", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <AnswerButton label="Voy" answer="yes" state="notChosen" onPress={jest.fn()} />
      </ThemeProvider>,
    );

    expect(screen.getByText("Voy").props.className).toContain("text-muted-foreground");
  });
});
