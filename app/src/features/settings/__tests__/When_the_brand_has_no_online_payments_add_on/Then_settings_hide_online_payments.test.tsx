import { screen } from "@testing-library/react-native";
import { listBranches } from "@/features/branches/branchesApi";
import { getOnlinePayments } from "@/features/onlinePayments/onlinePaymentsApi";
import { SettingsScreen } from "@/features/settings/screens/SettingsScreen";
import { translate } from "@/i18n/translate";
import { buildCurrentMember } from "@/testing/memberFactory";
import { buildOnlinePayments } from "@/testing/onlinePaymentFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSubscription } from "@/testing/subscriptionFactory";

jest.mock("@/features/branches/branchesApi");
jest.mock("@/features/onlinePayments/onlinePaymentsApi");
jest.mock("@/features/authentication/useSession", () => ({
  useSession: () => ({ signOut: jest.fn() }),
}));

describe("When the brand has no online payments add on", () => {
  beforeEach(() => {
    jest.mocked(listBranches).mockResolvedValue([]);
    jest.mocked(getOnlinePayments).mockResolvedValue(buildOnlinePayments());
  });

  it("Then settings hide online payments", async () => {
    await renderWithProviders(<SettingsScreen />, {
      member: buildCurrentMember({ subscription: buildSubscription() }),
    });

    expect(screen.queryByRole("button", { name: translate("settings.onlinePayments") })).toBeNull();
  });
});
