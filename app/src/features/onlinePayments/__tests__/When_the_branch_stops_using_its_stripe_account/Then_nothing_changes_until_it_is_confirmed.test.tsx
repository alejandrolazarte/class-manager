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

  it("Then nothing changes until it is confirmed", async () => {
    Platform.OS = "ios";
    jest
      .mocked(getOnlinePayments)
      .mockResolvedValue(
        buildOnlinePayments({ status: "Active", account: buildOnlinePaymentAccount() }),
      );
    await renderWithProviders(<OnlinePaymentsScreen />);

    fireEvent.press(await screen.findByText(translate("onlinePayments.stopUsing")));

    expect(await screen.findByText(translate("onlinePayments.stopQuestion"))).toBeTruthy();
    expect(stopUsingOnlinePaymentAccount).not.toHaveBeenCalled();
  });
});
