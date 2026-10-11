import { fireEvent, render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { AppText } from "@/ui/AppText";
import { HairlineRow } from "@/ui/HairlineRow";

describe("When a hairline row is pressed", () => {
  it("Then its action runs", async () => {
    const open = jest.fn();
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <HairlineRow onPress={open} accessibilityLabel="Ana Torres">
          <AppText>Ana Torres</AppText>
        </HairlineRow>
      </ThemeProvider>,
    );

    await fireEvent.press(screen.getByRole("button", { name: "Ana Torres" }));

    expect(open).toHaveBeenCalledTimes(1);
  });
});
