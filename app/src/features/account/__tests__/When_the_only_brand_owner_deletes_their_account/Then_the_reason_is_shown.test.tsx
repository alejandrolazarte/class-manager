import { fireEvent, screen } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import { deleteMyAccount } from "@/features/account/accountApi";
import { accountErrorCodes } from "@/features/account/accountErrorCodes";
import { DeleteAccountScreen } from "@/features/account/screens/DeleteAccountScreen";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/account/accountApi");
const mockSignOut = jest.fn();
jest.mock("@/features/authentication/useSession", () => ({
  useSession: () => ({ signOut: mockSignOut }),
}));

const conflictStatus = 409;

describe("When the only brand owner deletes their account", () => {
  beforeEach(() => {
    jest
      .mocked(deleteMyAccount)
      .mockRejectedValue(new ApiError(conflictStatus, { code: accountErrorCodes.lastBrandOwner }));
  });

  it("Then the reason is shown", async () => {
    await renderWithProviders(<DeleteAccountScreen />);
    await fireEvent.changeText(
      screen.getByLabelText(translate("profile.delete.currentPassword")),
      "a long passphrase",
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("profile.delete.submit") }));

    await fireEvent.press(
      screen.getByRole("button", { name: translate("profile.delete.confirm") }),
    );

    expect(await screen.findByText(translate("profile.delete.lastBrandOwner"))).toBeOnTheScreen();
    expect(mockSignOut).not.toHaveBeenCalled();
  });
});
