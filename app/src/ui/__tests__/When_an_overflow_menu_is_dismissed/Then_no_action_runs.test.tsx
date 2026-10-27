import { fireEvent, screen } from "@testing-library/react-native";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { OverflowMenu } from "@/ui/OverflowMenu";

describe("When an overflow menu is dismissed", () => {
  it("Then no action runs", async () => {
    const onImport = jest.fn();
    await renderWithProviders(
      <OverflowMenu
        accessibilityLabel="Más opciones"
        items={[{ key: "import", label: "Importar", onPress: onImport }]}
      />,
    );

    await fireEvent.press(screen.getByRole("button", { name: "Más opciones" }));
    await fireEvent.press(screen.getByRole("button", { name: translate("common.close") }));

    expect(onImport).not.toHaveBeenCalled();
    expect(screen.queryByRole("menuitem", { name: "Importar" })).toBeNull();
  });
});
