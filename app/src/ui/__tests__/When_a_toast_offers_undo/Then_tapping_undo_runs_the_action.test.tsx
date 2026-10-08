import { fireEvent, screen } from "@testing-library/react-native";
import { useEffect } from "react";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { useToast } from "@/ui/ToastProvider";

const undo = jest.fn();

function DeletedPayment() {
  const { showToast } = useToast();
  useEffect(() => {
    showToast("Pago borrado", { label: translate("common.undo"), onPress: undo });
  }, [showToast]);
  return null;
}

describe("When a toast offers undo", () => {
  it("Then tapping undo runs the action", async () => {
    await renderWithProviders(<DeletedPayment />);

    await fireEvent.press(await screen.findByRole("button", { name: translate("common.undo") }));

    expect(undo).toHaveBeenCalledTimes(1);
  });
});
