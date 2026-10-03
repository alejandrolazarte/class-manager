import { render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { StatusPill } from "@/ui/StatusPill";
import { TitleWithPills } from "@/ui/TitleWithPills";

const title = "Sebastian Alejandro Lazarte";
const pillLabel = "Dueño de marca";

describe("When title has pills", () => {
  it("Then pills wrap with the title", async () => {
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <TitleWithPills title={title} testID="title">
          <StatusPill label={pillLabel} tone="neutral" isSmall />
        </TitleWithPills>
      </ThemeProvider>,
    );

    const titleRow = screen.getByTestId("title").parent;
    expect(titleRow?.props.className).toContain("flex-wrap");
    expect(titleRow).toContainElement(screen.getByText(pillLabel));
  });
});
