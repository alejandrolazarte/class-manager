import { screen } from "@testing-library/react-native";
import { Platform } from "react-native";
import { getOnlinePayments } from "@/features/onlinePayments/onlinePaymentsApi";
import { OnlinePaymentsScreen } from "@/features/onlinePayments/screens/OnlinePaymentsScreen";
import { translate } from "@/i18n/translate";
import { buildCurrentMember } from "@/testing/memberFactory";
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

describe("When a branch owner opens online payments", () => {
  beforeEach(() => {
    Platform.OS = "web";
  });

  it("Then other branch accounts are not offered", async () => {
    const branchOwner = buildCurrentMember({
      isBrandOwner: false,
      permissions: buildCurrentMember().permissions.filter(
        (permission) => permission !== "onlinePayments.share",
      ),
    });
    jest.mocked(getOnlinePayments).mockResolvedValue(
      buildOnlinePayments({
        brandAccounts: [buildOnlinePaymentAccount({ branchNames: [secondBranchName] })],
      }),
    );
    await renderWithProviders(<OnlinePaymentsScreen />, { member: branchOwner });

    await screen.findByText(translate("onlinePayments.connect"));

    expect(screen.queryByText(translate("onlinePayments.brandAccounts"))).toBeNull();
  });
});
