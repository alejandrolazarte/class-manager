import { fireEvent, screen } from "@testing-library/react-native";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { OverflowMenu } from "@/ui/OverflowMenu";

describe("When an overflow menu item is pressed", () => {
  it("Then its action runs and the menu closes", async () => {
    const onExport = jest.fn();
    await renderWithProviders(
      <OverflowMenu
        accessibilityLabel="Más opciones"
        items={[
          { key: "import", label: "Importar", onPress: jest.fn() },
          { key: "export", label: "Exportar", onPress: onExport },
        ]}
      />,
    );

    await fireEvent.press(screen.getByRole("button", { name: "Más opciones" }));
    await fireEvent.press(screen.getByRole("menuitem", { name: "Exportar" }));

    expect(onExport).toHaveBeenCalledTimes(1);
    expect(screen.queryByRole("menuitem", { name: "Importar" })).toBeNull();
  });
});
