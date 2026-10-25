import { fireEvent, screen } from "@testing-library/react-native";
import { listMyPasskeys, removeMyPasskey } from "@/features/passkeys/passkeysApi";
import { PasskeysScreen } from "@/features/passkeys/screens/PasskeysScreen";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { buildPasskey } from "@/testing/passkeyFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/passkeys/passkeysApi");

const passkey = buildPasskey();

describe("When removing a passkey is not confirmed", () => {
  beforeEach(() => {
    jest.mocked(listMyPasskeys).mockResolvedValue([passkey]);
  });

  it("Then nothing is removed", async () => {
    await renderWithProviders(<PasskeysScreen addPasskeyRoute={routes.addMyPasskey} />);
    await fireEvent.press(
      await screen.findByRole("button", {
        name: translate("profile.passkeys.removeAccessibility", { name: passkey.name }),
      }),
    );

    await fireEvent.press(
      screen.getAllByRole("button", { name: translate("common.back") }).at(-1)!,
    );

    expect(removeMyPasskey).not.toHaveBeenCalled();
    expect(screen.getByText(passkey.name)).toBeOnTheScreen();
  });
});
