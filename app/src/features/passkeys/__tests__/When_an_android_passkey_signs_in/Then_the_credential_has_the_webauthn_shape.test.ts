import { Passkey } from "react-native-passkey";
import { getPasskeyCredential } from "@/features/passkeys/webAuthn.android";
import { buildPasskeySignInCeremony } from "@/testing/passkeyFactory";

jest.mock("react-native-passkey", () => ({ Passkey: { get: jest.fn() } }));

describe("When an android passkey signs in", () => {
  beforeEach(() => {
    jest.mocked(Passkey.get).mockResolvedValue({
      id: "credential-android",
      response: {
        clientDataJSON: "client-data",
        authenticatorData: "authenticator-data",
        signature: "signature",
        userHandle: "user-handle",
      },
    });
  });

  it("Then the credential has the webauthn shape", async () => {
    const ceremony = buildPasskeySignInCeremony();

    const credential = await getPasskeyCredential(ceremony.options);

    expect(Passkey.get).toHaveBeenCalledWith(ceremony.options);
    expect(credential).toEqual({
      id: "credential-android",
      rawId: "credential-android",
      type: "public-key",
      clientExtensionResults: {},
      response: {
        clientDataJSON: "client-data",
        authenticatorData: "authenticator-data",
        signature: "signature",
        userHandle: "user-handle",
      },
    });
  });
});
