import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { Platform } from "react-native";
import {
  chooseOnlinePaymentAccount,
  getOnlinePayments,
} from "@/features/onlinePayments/onlinePaymentsApi";
import { OnlinePaymentsScreen } from "@/features/onlinePayments/screens/OnlinePaymentsScreen";
import { translate } from "@/i18n/translate";
import {
  buildOnlinePaymentAccount,
  buildOnlinePayments,
  secondBranchName,
} from "@/testing/onlinePaymentFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/onlinePayments/onlinePaymentsApi");
jest.mock("@/features/onlinePayments/stripeRedirect", () => ({
  ...jest.requireActual("@/features/onlinePayments/stripeRedirect"),
  goToStripePage: jest.fn(),
}));

describe("When the brand owner opens online payments", () => {
  beforeEach(() => {
    Platform.OS = "web";
  });

  it("Then another branch account is used once confirmed", async () => {
    Platform.OS = "ios";
    const account = buildOnlinePaymentAccount({ id: "account-2", branchNames: [secondBranchName] });
    jest
      .mocked(getOnlinePayments)
      .mockResolvedValue(buildOnlinePayments({ brandAccounts: [account] }));
    jest
      .mocked(chooseOnlinePaymentAccount)
      .mockResolvedValue(buildOnlinePayments({ status: "Active", account }));
    await renderWithProviders(<OnlinePaymentsScreen />);
    fireEvent.press(
      await screen.findByLabelText(
        translate("onlinePayments.useAccountNamed", { branches: secondBranchName }),
      ),
    );
    expect(chooseOnlinePaymentAccount).not.toHaveBeenCalled();

    fireEvent.press(await screen.findByText(translate("onlinePayments.chooseConfirm")));

    await waitFor(() =>
      expect(jest.mocked(chooseOnlinePaymentAccount).mock.calls[0]?.[0]).toBe("account-2"),
    );
  });
});
