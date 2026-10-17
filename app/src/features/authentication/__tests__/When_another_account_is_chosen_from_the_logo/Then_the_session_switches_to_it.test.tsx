import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { listAccounts } from "@/features/authentication/authenticationApi";
import { AccountSwitcher } from "@/features/authentication/components/AccountSwitcher";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { buildAccount } from "@/testing/accountFactory";
import { routerMock } from "@/testing/expoRouterMock";
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

const otherBranch = buildAccount({
  businessId: "business-north",
  businessName: "Sede Norte",
  isCurrent: false,
});

describe("When another account is chosen from the logo", () => {
  beforeEach(() => {
    mockSwitchAccount.mockReset().mockResolvedValue(undefined);
    jest.mocked(listAccounts).mockResolvedValue([buildAccount(), otherBranch]);
  });

  it("Then the session switches to it", async () => {
    await renderWithProviders(<AccountSwitcher displayName="Sede Centro" />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("accounts.switch") }),
    );
    await fireEvent.press(
      await screen.findByRole("button", {
        name: translate("accounts.asTeam", { name: otherBranch.businessName }),
      }),
    );

    await waitFor(() => expect(routerMock.replace).toHaveBeenCalledWith(routes.today));
    expect(mockSwitchAccount).toHaveBeenCalledWith("business-north", "team");
  });
});
