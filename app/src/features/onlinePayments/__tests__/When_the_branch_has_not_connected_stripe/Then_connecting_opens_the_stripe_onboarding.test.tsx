import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { Platform } from "react-native";
import {
  getOnlinePayments,
  startOnlinePaymentOnboarding,
} from "@/features/onlinePayments/onlinePaymentsApi";
import { OnlinePaymentsScreen } from "@/features/onlinePayments/screens/OnlinePaymentsScreen";
import { goToStripePage } from "@/features/onlinePayments/stripeRedirect";
import { translate } from "@/i18n/translate";
import { buildOnlinePayments } from "@/testing/onlinePaymentFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/onlinePayments/onlinePaymentsApi");
jest.mock("@/features/onlinePayments/stripeRedirect", () => ({
  ...jest.requireActual("@/features/onlinePayments/stripeRedirect"),
  goToStripePage: jest.fn(),
}));

describe("When the branch has not connected stripe", () => {
  beforeEach(() => {
    Platform.OS = "web";
  });

  it("Then connecting opens the stripe onboarding", async () => {
    const onboardingUrl = "https://connect.stripe.test/onboarding";
    jest.mocked(getOnlinePayments).mockResolvedValue(buildOnlinePayments());
    jest.mocked(startOnlinePaymentOnboarding).mockResolvedValue({ url: onboardingUrl });
    await renderWithProviders(<OnlinePaymentsScreen />);

    fireEvent.press(await screen.findByText(translate("onlinePayments.connect")));

    await waitFor(() => expect(goToStripePage).toHaveBeenCalledWith(onboardingUrl));
  });
});
