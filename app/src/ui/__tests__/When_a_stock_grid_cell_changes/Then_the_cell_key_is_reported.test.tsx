import { fireEvent, render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { StockGrid } from "@/ui/StockGrid";

describe("When a stock grid cell changes", () => {
  it("Then the cell key is reported", async () => {
    const onChangeCell = jest.fn();
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <StockGrid
          columnLabels={["S", "M"]}
          rows={[
            {
              key: "black",
              label: "Negro",
              cells: [
                { key: "S|black", value: 0, accessibilityLabel: "S · Negro" },
                { key: "M|black", value: 2, accessibilityLabel: "M · Negro" },
              ],
            },
          ]}
          onChangeCell={onChangeCell}
        />
      </ThemeProvider>,
    );

    await fireEvent.changeText(screen.getByLabelText("M · Negro"), "5");

    expect(onChangeCell).toHaveBeenCalledWith("M|black", "5");
  });
});
