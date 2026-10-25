import { Passkey } from "react-native-passkey";
import { isPasskeySupported } from "@/features/passkeys/webAuthn.android";

jest.mock("react-native-passkey", () => ({ Passkey: { isSupported: jest.fn() } }));

describe("When android passkeys are not enabled", () => {
  beforeEach(() => {
    delete process.env.EXPO_PUBLIC_ANDROID_PASSKEYS;
    jest.mocked(Passkey.isSupported).mockReturnValue(true);
  });

  it("Then passkeys are hidden on android", async () => {
    expect(await isPasskeySupported()).toBe(false);
  });
});
