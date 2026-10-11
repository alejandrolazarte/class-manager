import { render, screen } from "@testing-library/react-native";
import { StyleSheet } from "react-native";
import { JsonElement, JsonNode } from "test-renderer";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { ProgressBar } from "@/ui/ProgressBar";

jest.mock("@/ui/motion/useReducedMotion", () => ({ useReducedMotion: () => true }));

function findByRole(node: JsonNode | JsonNode[] | null, role: string): JsonElement | undefined {
  if (node === null || typeof node === "string") {
    return undefined;
  }
  if (Array.isArray(node)) {
    return node.map((child) => findByRole(child, role)).find(Boolean);
  }
  if (node.props.accessibilityRole === role) {
    return node;
  }
  return (node.children ?? []).map((child) => findByRole(child, role)).find(Boolean);
}

describe("When reduced motion is on", () => {
  it("Then a progress bar shows its value", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <ProgressBar ratio={0.75} />
      </ThemeProvider>,
    );

    const track = findByRole(screen.toJSON(), "progressbar");
    const fill = track?.children?.[0] as JsonElement | undefined;
    expect(StyleSheet.flatten(fill?.props.style)?.width).toBe("75%");
  });
});
