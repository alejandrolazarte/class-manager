import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import {
  beginPasskeyRegistration,
  finishPasskeyRegistration,
} from "@/features/passkeys/passkeysApi";
import { AddPasskeyScreen } from "@/features/passkeys/screens/AddPasskeyScreen";
import { createPasskeyCredential, suggestedPasskeyName } from "@/features/passkeys/webAuthn";
import { translate } from "@/i18n/translate";
import { routerMock } from "@/testing/expoRouterMock";
import {
  buildPasskey,
  buildPasskeyRegistrationCeremony,
  buildPasskeyRegistrationCredential,
} from "@/testing/passkeyFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/passkeys/passkeysApi");
jest.mock("@/features/passkeys/webAuthn");

const currentPassword = "a long passphrase";
const ceremony = buildPasskeyRegistrationCeremony();
const credential = buildPasskeyRegistrationCredential();

describe("When a passkey is added", () => {
  beforeEach(() => {
    jest.mocked(suggestedPasskeyName).mockReturnValue("iPhone");
    jest.mocked(beginPasskeyRegistration).mockResolvedValue(ceremony);
    jest.mocked(createPasskeyCredential).mockResolvedValue(credential);
    jest.mocked(finishPasskeyRegistration).mockResolvedValue(buildPasskey());
  });

  it("Then it is saved with its name", async () => {
    await renderWithProviders(<AddPasskeyScreen />);
    await fireEvent.changeText(
      screen.getByLabelText(translate("profile.email.currentPassword")),
      currentPassword,
    );

    await fireEvent.press(screen.getByRole("button", { name: translate("profile.passkeys.add") }));

    await waitFor(() => expect(routerMock.back).toHaveBeenCalled());
    expect(beginPasskeyRegistration).toHaveBeenCalledWith(currentPassword);
    expect(createPasskeyCredential).toHaveBeenCalledWith(ceremony.options);
    expect(finishPasskeyRegistration).toHaveBeenCalledWith({
      ceremonyId: ceremony.ceremonyId,
      credential,
      name: "iPhone",
    });
  });
});
