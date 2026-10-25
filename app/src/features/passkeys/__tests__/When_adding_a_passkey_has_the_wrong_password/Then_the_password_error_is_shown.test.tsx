import { fireEvent, screen } from "@testing-library/react-native";
import { ApiError } from "@/api/httpClient";
import { passkeyErrorCodes } from "@/features/passkeys/passkeyErrorCodes";
import { beginPasskeyRegistration } from "@/features/passkeys/passkeysApi";
import { AddPasskeyScreen } from "@/features/passkeys/screens/AddPasskeyScreen";
import { createPasskeyCredential, suggestedPasskeyName } from "@/features/passkeys/webAuthn";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/passkeys/passkeysApi");
jest.mock("@/features/passkeys/webAuthn");

const badRequestStatus = 400;

describe("When adding a passkey has the wrong password", () => {
  beforeEach(() => {
    jest.mocked(suggestedPasskeyName).mockReturnValue("iPhone");
    jest
      .mocked(beginPasskeyRegistration)
      .mockRejectedValue(
        new ApiError(badRequestStatus, { code: passkeyErrorCodes.invalidCurrentPassword }),
      );
  });

  it("Then the password error is shown", async () => {
    await renderWithProviders(<AddPasskeyScreen />);
    await fireEvent.changeText(
      screen.getByLabelText(translate("profile.email.currentPassword")),
      "not the right one",
    );

    await fireEvent.press(screen.getByRole("button", { name: translate("profile.passkeys.add") }));

    expect(await screen.findByText(translate("profile.email.wrongPassword"))).toBeOnTheScreen();
    expect(createPasskeyCredential).not.toHaveBeenCalled();
  });
});
