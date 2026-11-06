import { fireEvent, screen } from "@testing-library/react-native";
import { Platform } from "react-native";
import {
  getOnlinePayments,
  stopUsingOnlinePaymentAccount,
} from "@/features/onlinePayments/onlinePaymentsApi";
import { OnlinePaymentsScreen } from "@/features/onlinePayments/screens/OnlinePaymentsScreen";
import { translate } from "@/i18n/translate";
import { buildOnlinePaymentAccount, buildOnlinePayments } from "@/testing/onlinePaymentFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/onlinePayments/onlinePaymentsApi");
jest.mock("@/features/onlinePayments/stripeRedirect", () => ({
  ...jest.requireActual("@/features/onlinePayments/stripeRedirect"),
  goToStripePage: jest.fn(),
}));

describe("When the branch stops using its stripe account", () => {
  beforeEach(() => {
    Platform.OS = "web";
  });

  it("Then it stops once confirmed", async () => {
    Platform.OS = "ios";
    jest
      .mocked(getOnlinePayments)
      .mockResolvedValue(
        buildOnlinePayments({ status: "Active", account: buildOnlinePaymentAccount() }),
      );
    jest.mocked(stopUsingOnlinePaymentAccount).mockResolvedValue(buildOnlinePayments());
    await renderWithProviders(<OnlinePaymentsScreen />);
    fireEvent.press(await screen.findByText(translate("onlinePayments.stopUsing")));

    fireEvent.press(await screen.findByText(translate("onlinePayments.stopConfirm")));

    expect(await screen.findByText(translate("onlinePayments.status.notConnected"))).toBeTruthy();
  });
});
