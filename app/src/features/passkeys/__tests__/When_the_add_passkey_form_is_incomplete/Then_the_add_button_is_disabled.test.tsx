import { screen } from "@testing-library/react-native";
import { AddPasskeyScreen } from "@/features/passkeys/screens/AddPasskeyScreen";
import { suggestedPasskeyName } from "@/features/passkeys/webAuthn";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/passkeys/passkeysApi");
jest.mock("@/features/passkeys/webAuthn");

describe("When the add passkey form is incomplete", () => {
  beforeEach(() => {
    jest.mocked(suggestedPasskeyName).mockReturnValue("iPhone");
  });

  it("Then the add button is disabled", async () => {
    await renderWithProviders(<AddPasskeyScreen />);

    expect(screen.getByRole("button", { name: translate("profile.passkeys.add") })).toBeDisabled();
  });
});
