import { restrictedImportMessages } from "../lintWithProjectConfig";

const importOfTheBackoffice =
  'import { BackofficeSignInScreen } from "@/backoffice/signIn/BackofficeSignInScreen";\nexport const screen = BackofficeSignInScreen;\n';

describe("When the customer app imports the backoffice", () => {
  it("Then it is reported", async () => {
    const messages = await restrictedImportMessages(importOfTheBackoffice, "app/example.tsx");

    expect(messages).toHaveLength(1);
  });
});
