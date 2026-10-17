import { fireEvent, screen } from "@testing-library/react-native";
import { deleteMyAccount } from "@/features/account/accountApi";
import { DeleteAccountScreen } from "@/features/account/screens/DeleteAccountScreen";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/account/accountApi");
const mockSignOut = jest.fn();
jest.mock("@/features/authentication/useSession", () => ({
  useSession: () => ({ signOut: mockSignOut }),
}));

describe("When the account deletion is not confirmed", () => {
  it("Then the account is kept", async () => {
    await renderWithProviders(<DeleteAccountScreen />);
    await fireEvent.changeText(
      screen.getByLabelText(translate("profile.delete.currentPassword")),
      "a long passphrase",
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("profile.delete.submit") }));

    await fireEvent.press(screen.getAllByRole("button", { name: translate("common.back") })[1]!);

    expect(deleteMyAccount).not.toHaveBeenCalled();
    expect(mockSignOut).not.toHaveBeenCalled();
  });
});
