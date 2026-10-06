import { fireEvent, screen } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import { getMyAccount, requestEmailChange } from "@/features/account/accountApi";
import { accountErrorCodes } from "@/features/account/accountErrorCodes";
import { MyProfileScreen } from "@/features/account/screens/MyProfileScreen";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/account/accountApi");

const badRequestStatus = 400;

describe("When the current password is wrong", () => {
  beforeEach(() => {
    jest.mocked(getMyAccount).mockResolvedValue({
      email: "laura@example.com",
      fullName: "Laura Gómez",
    });
    jest
      .mocked(requestEmailChange)
      .mockRejectedValue(
        new ApiError(badRequestStatus, { code: accountErrorCodes.invalidCurrentPassword }),
      );
  });

  it("Then the password error is shown", async () => {
    await renderWithProviders(<MyProfileScreen />);
    await fireEvent.changeText(
      screen.getByLabelText(translate("profile.email.newEmail")),
      "laura.nueva@example.com",
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("profile.email.currentPassword")),
      "not the right one",
    );

    await fireEvent.press(screen.getByRole("button", { name: translate("profile.email.submit") }));

    expect(await screen.findByText(translate("profile.email.wrongPassword"))).toBeOnTheScreen();
  });
});
