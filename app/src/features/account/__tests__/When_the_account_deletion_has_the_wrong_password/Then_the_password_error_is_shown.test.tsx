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

const badRequestStatus = 400;

describe("When the account deletion has the wrong password", () => {
  beforeEach(() => {
    jest
      .mocked(deleteMyAccount)
      .mockRejectedValue(
        new ApiError(badRequestStatus, { code: accountErrorCodes.invalidCurrentPassword }),
      );
  });

  it("Then the password error is shown", async () => {
    await renderWithProviders(<DeleteAccountScreen />);
    await fireEvent.changeText(
      screen.getByLabelText(translate("profile.delete.currentPassword")),
      "not the right one",
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("profile.delete.submit") }));

    await fireEvent.press(
      screen.getByRole("button", { name: translate("profile.delete.confirm") }),
    );

    expect(await screen.findByText(translate("profile.delete.wrongPassword"))).toBeOnTheScreen();
    expect(mockSignOut).not.toHaveBeenCalled();
  });
});
