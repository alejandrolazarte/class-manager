import { Passkey } from "react-native-passkey";
import { isPasskeySupported } from "@/features/passkeys/webAuthn.android";

jest.mock("react-native-passkey", () => ({ Passkey: { isSupported: jest.fn() } }));

describe("When android passkeys are enabled", () => {
  beforeEach(() => {
    process.env.EXPO_PUBLIC_ANDROID_PASSKEYS = "true";
    jest.mocked(Passkey.isSupported).mockReturnValue(true);
  });

  afterEach(() => {
    delete process.env.EXPO_PUBLIC_ANDROID_PASSKEYS;
  });

  it("Then passkeys show on supported phones", async () => {
    expect(await isPasskeySupported()).toBe(true);
  });
});
