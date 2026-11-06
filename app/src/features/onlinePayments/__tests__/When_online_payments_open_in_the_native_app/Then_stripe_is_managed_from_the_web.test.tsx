import { screen } from "@testing-library/react-native";
import { Platform } from "react-native";
import { getOnlinePayments } from "@/features/onlinePayments/onlinePaymentsApi";
import { OnlinePaymentsScreen } from "@/features/onlinePayments/screens/OnlinePaymentsScreen";
import { translate } from "@/i18n/translate";
import { buildOnlinePayments } from "@/testing/onlinePaymentFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/onlinePayments/onlinePaymentsApi");
jest.mock("@/features/onlinePayments/stripeRedirect", () => ({
  ...jest.requireActual("@/features/onlinePayments/stripeRedirect"),
  goToStripePage: jest.fn(),
}));

describe("When online payments open in the native app", () => {
  beforeEach(() => {
    Platform.OS = "web";
  });

  it("Then stripe is managed from the web", async () => {
    Platform.OS = "ios";
    jest.mocked(getOnlinePayments).mockResolvedValue(buildOnlinePayments());

    await renderWithProviders(<OnlinePaymentsScreen />);

    expect(await screen.findByText(translate("onlinePayments.manageOnWeb"))).toBeTruthy();
    expect(screen.queryByText(translate("onlinePayments.connect"))).toBeNull();
  });
});
