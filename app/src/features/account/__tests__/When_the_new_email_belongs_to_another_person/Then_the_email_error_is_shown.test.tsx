import { fireEvent, screen } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import { getMyAccount, requestEmailChange } from "@/features/account/accountApi";
import { ChangeEmailScreen } from "@/features/account/screens/ChangeEmailScreen";
import { userErrorCodes } from "@/features/users/userErrorCodes";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/account/accountApi");

const badRequestStatus = 400;

describe("When the new email belongs to another person", () => {
  beforeEach(() => {
    jest.mocked(getMyAccount).mockResolvedValue({
      email: "laura@example.com",
      fullName: "Laura Gómez",
      hasAcceptedCurrentLegalTerms: true,
      preferredLanguage: "es",
    });
    jest
      .mocked(requestEmailChange)
      .mockRejectedValue(new ApiError(badRequestStatus, { code: userErrorCodes.emailTaken }));
  });

  it("Then the email error is shown", async () => {
    await renderWithProviders(<ChangeEmailScreen />);
    await fireEvent.changeText(
      screen.getByLabelText(translate("profile.email.newEmail")),
      "ana@example.com",
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("profile.email.currentPassword")),
      "a long passphrase",
    );

    await fireEvent.press(screen.getByRole("button", { name: translate("profile.email.submit") }));

    expect(
      await screen.findByText(translate("profile.email.usedByAnotherPerson")),
    ).toBeOnTheScreen();
  });
});
