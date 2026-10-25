import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { listMyPasskeys, removeMyPasskey } from "@/features/passkeys/passkeysApi";
import { PasskeysScreen } from "@/features/passkeys/screens/PasskeysScreen";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { buildPasskey } from "@/testing/passkeyFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/passkeys/passkeysApi");

const passkey = buildPasskey();

describe("When a passkey is removed", () => {
  beforeEach(() => {
    jest.mocked(listMyPasskeys).mockResolvedValue([passkey]);
    jest.mocked(removeMyPasskey).mockResolvedValue(undefined);
  });

  it("Then it is removed after confirming", async () => {
    await renderWithProviders(<PasskeysScreen addPasskeyRoute={routes.addMyPasskey} />);
    await fireEvent.press(
      await screen.findByRole("button", {
        name: translate("profile.passkeys.removeAccessibility", { name: passkey.name }),
      }),
    );
    expect(removeMyPasskey).not.toHaveBeenCalled();

    await fireEvent.press(screen.getByRole("button", { name: translate("common.remove") }));

    await waitFor(() => expect(removeMyPasskey).toHaveBeenCalledWith(passkey.id));
    expect(await screen.findByText(translate("profile.passkeys.removed"))).toBeOnTheScreen();
  });
});
