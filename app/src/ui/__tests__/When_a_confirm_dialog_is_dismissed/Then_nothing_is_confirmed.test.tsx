import { fireEvent, screen } from "@testing-library/react-native";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { ConfirmDialog } from "@/ui/ConfirmDialog";

describe("When a confirm dialog is dismissed", () => {
  it("Then nothing is confirmed", async () => {
    const onConfirm = jest.fn();
    await renderWithProviders(
      <ConfirmDialog
        title="¿Borrar esta clase?"
        confirmLabel="Borrar"
        onCancel={jest.fn()}
        onConfirm={onConfirm}
      />,
    );

    await fireEvent.press(screen.getAllByRole("button", { name: translate("common.back") })[1]!);

    expect(onConfirm).not.toHaveBeenCalled();
  });
});
