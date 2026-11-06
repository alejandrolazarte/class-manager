import { screen } from "@testing-library/react-native";
import { Platform } from "react-native";
import {
  getOnlinePayments,
  refreshOnlinePaymentAccount,
} from "@/features/onlinePayments/onlinePaymentsApi";
import { OnlinePaymentsScreen } from "@/features/onlinePayments/screens/OnlinePaymentsScreen";
import { translate } from "@/i18n/translate";
import { searchParametersMock } from "@/testing/expoRouterMock";
import { buildOnlinePaymentAccount, buildOnlinePayments } from "@/testing/onlinePaymentFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/onlinePayments/onlinePaymentsApi");
jest.mock("@/features/onlinePayments/stripeRedirect", () => ({
  ...jest.requireActual("@/features/onlinePayments/stripeRedirect"),
  goToStripePage: jest.fn(),
}));

describe("When the branch returns from stripe", () => {
  beforeEach(() => {
    Platform.OS = "web";
  });

  it("Then the account is read again", async () => {
    searchParametersMock.current = { onboarding: "return" };
    jest.mocked(getOnlinePayments).mockResolvedValue(
      buildOnlinePayments({
        status: "Pending",
        account: buildOnlinePaymentAccount({ status: "Pending" }),
      }),
    );
    jest
      .mocked(refreshOnlinePaymentAccount)
      .mockResolvedValue(
        buildOnlinePayments({ status: "Active", account: buildOnlinePaymentAccount() }),
      );

    await renderWithProviders(<OnlinePaymentsScreen />);

    expect(await screen.findByText(translate("onlinePayments.status.active"))).toBeTruthy();
  });
});
