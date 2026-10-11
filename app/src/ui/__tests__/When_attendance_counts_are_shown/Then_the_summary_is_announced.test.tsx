import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { CountsRow } from "@/ui/CountsRow";

describe("When attendance counts are shown", () => {
  it("Then the summary is announced", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <CountsRow
          accessibilityLabel="5 vinieron · 1 faltó · 2 sin marcar"
          items={[
            { key: "present", count: 5, label: "vinieron", tone: "success" },
            { key: "absent", count: 1, label: "faltó", tone: "danger" },
            { key: "unmarked", count: 2, label: "sin marcar", tone: "default" },
          ]}
        />
      </ThemeProvider>,
    );

    expect(screen.getByLabelText("5 vinieron · 1 faltó · 2 sin marcar")).toHaveTextContent(
      "5vinieron1faltó2sin marcar",
    );
  });
});
