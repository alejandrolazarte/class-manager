import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { listAccounts } from "@/features/authentication/authenticationApi";
import { AccountSwitcher } from "@/features/authentication/components/AccountSwitcher";
import { translate } from "@/i18n/translate";
import { buildAccount } from "@/testing/accountFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

const mockSwitchAccount = jest.fn();

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/useSession", () => ({
  useSession: () => ({
    session: { status: "signedIn", kind: "team", startedBy: "signIn" },
    switchAccount: mockSwitchAccount,
    confirmAccount: jest.fn(),
  }),
}));

const currentBranch = buildAccount({ businessName: "Sede Centro" });

describe("When the current account is chosen from the logo", () => {
  beforeEach(() => {
    mockSwitchAccount.mockReset();
    jest
      .mocked(listAccounts)
      .mockResolvedValue([
        currentBranch,
        buildAccount({
          businessId: "business-north",
          businessName: "Sede Norte",
          isCurrent: false,
        }),
      ]);
  });

  it("Then the session stays the same", async () => {
    await renderWithProviders(<AccountSwitcher displayName="Sede Centro" />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("accounts.switch") }),
    );
    await fireEvent.press(
      await screen.findByRole("button", {
        name: translate("accounts.asTeam", { name: currentBranch.businessName }),
      }),
    );

    await waitFor(() =>
      expect(
        screen.queryByRole("button", {
          name: translate("accounts.asTeam", { name: currentBranch.businessName }),
        }),
      ).toBeNull(),
    );
    expect(mockSwitchAccount).not.toHaveBeenCalled();
  });
});
