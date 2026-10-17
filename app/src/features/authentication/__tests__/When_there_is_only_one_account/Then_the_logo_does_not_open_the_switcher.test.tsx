import { screen, waitFor } from "@testing-library/react-native";
import { listAccounts } from "@/features/authentication/authenticationApi";
import { AccountSwitcher } from "@/features/authentication/components/AccountSwitcher";
import { translate } from "@/i18n/translate";
import { buildAccount } from "@/testing/accountFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/useSession", () => ({
  useSession: () => ({
    session: { status: "signedIn", kind: "team", startedBy: "signIn" },
    switchAccount: jest.fn(),
    confirmAccount: jest.fn(),
  }),
}));

describe("When there is only one account", () => {
  beforeEach(() => {
    jest.mocked(listAccounts).mockResolvedValue([buildAccount()]);
  });

  it("Then the logo does not open the switcher", async () => {
    await renderWithProviders(<AccountSwitcher displayName="Sede Centro" />);

    await waitFor(() => expect(listAccounts).toHaveBeenCalled());

    expect(screen.queryByRole("button", { name: translate("accounts.switch") })).toBeNull();
  });
});
