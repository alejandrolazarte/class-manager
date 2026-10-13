import { fireEvent, render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { FooterLinks } from "@/ui/FooterLinks";

describe("When a footer link is pressed", () => {
  it("Then it runs its action", async () => {
    const openPrivacy = jest.fn();
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <FooterLinks
          links={[
            { label: "Términos", onPress: jest.fn() },
            { label: "Privacidad", onPress: openPrivacy },
          ]}
        />
      </ThemeProvider>,
    );

    await fireEvent.press(screen.getByRole("link", { name: "Privacidad" }));

    expect(openPrivacy).toHaveBeenCalledTimes(1);
  });
});
