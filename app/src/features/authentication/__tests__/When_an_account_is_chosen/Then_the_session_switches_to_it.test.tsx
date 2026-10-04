import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { listAccounts } from "@/features/authentication/authenticationApi";
import { ChooseAccountScreen } from "@/features/authentication/screens/ChooseAccountScreen";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { buildAccount } from "@/testing/accountFactory";
import { routerMock } from "@/testing/expoRouterMock";
import { renderWithProviders } from "@/testing/renderWithProviders";

const mockSwitchAccount = jest.fn();
const mockConfirmAccount = jest.fn();

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/useSession", () => ({
  useSession: () => ({
    session: { status: "signedIn", kind: "team", startedBy: "signIn", mustChooseAccount: true },
    switchAccount: mockSwitchAccount,
    confirmAccount: mockConfirmAccount,
  }),
}));

const studentAccount = buildAccount({
  businessId: "business-school",
  businessName: "Escuela Brazada",
  kind: "student",
  isCurrent: false,
});

describe("When an account is chosen", () => {
  beforeEach(() => {
    mockSwitchAccount.mockReset().mockResolvedValue(undefined);
    jest.mocked(listAccounts).mockResolvedValue([buildAccount(), studentAccount]);
  });

  it("Then the session switches to it", async () => {
    await renderWithProviders(<ChooseAccountScreen />);

    await fireEvent.press(
      await screen.findByRole("button", {
        name: translate("accounts.asStudent", { name: studentAccount.businessName }),
      }),
    );

    await waitFor(() => expect(routerMock.replace).toHaveBeenCalledWith(routes.studentApp));
    expect(mockSwitchAccount).toHaveBeenCalledWith("business-school", "student");
  });
});
