import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { deleteMyAccount } from "@/features/account/accountApi";
import { DeleteAccountScreen } from "@/features/account/screens/DeleteAccountScreen";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/account/accountApi");
const mockSignOut = jest.fn();
jest.mock("@/features/authentication/useSession", () => ({
  useSession: () => ({ signOut: mockSignOut }),
}));

const currentPassword = "a long passphrase";

describe("When the account deletion is confirmed", () => {
  beforeEach(() => {
    jest.mocked(deleteMyAccount).mockResolvedValue(undefined);
  });

  it("Then the session ends", async () => {
    await renderWithProviders(<DeleteAccountScreen />);
    await fireEvent.changeText(
      screen.getByLabelText(translate("profile.delete.currentPassword")),
      currentPassword,
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("profile.delete.submit") }));

    await fireEvent.press(
      screen.getByRole("button", { name: translate("profile.delete.confirm") }),
    );

    await waitFor(() => expect(mockSignOut).toHaveBeenCalled());
    expect(deleteMyAccount).toHaveBeenCalledWith({ currentPassword });
  });
});
