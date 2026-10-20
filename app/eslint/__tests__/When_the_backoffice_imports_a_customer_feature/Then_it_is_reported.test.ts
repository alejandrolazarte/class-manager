import { restrictedImportMessages } from "../lintWithProjectConfig";

const importOfAFeature =
  'import { SignInScreen } from "@/features/authentication/screens/SignInScreen";\nexport const screen = SignInScreen;\n';

describe("When the backoffice imports a customer feature", () => {
  it("Then it is reported", async () => {
    const messages = await restrictedImportMessages(importOfAFeature, "backoffice/example.tsx");

    expect(messages).toHaveLength(1);
  });
});
