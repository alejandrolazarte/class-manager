import { screen } from "@testing-library/react-native";
import { listBranches } from "@/features/branches/branchesApi";
import { SettingsScreen } from "@/features/settings/screens/SettingsScreen";
import { listPlans } from "@/features/subscriptions/subscriptionsApi";
import { translate } from "@/i18n/translate";
import { buildCurrentMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildPlans, buildSubscription } from "@/testing/subscriptionFactory";

jest.mock("@/features/branches/branchesApi");
jest.mock("@/features/subscriptions/subscriptionsApi");
jest.mock("@/features/authentication/useSession", () => ({
  useSession: () => ({ signOut: jest.fn() }),
}));

describe("When the plan does not include a feature", () => {
  beforeEach(() => {
    jest.mocked(listBranches).mockResolvedValue([]);
    jest.mocked(listPlans).mockResolvedValue(buildPlans());
  });

  it("Then settings shows the plan that includes it", async () => {
    await renderWithProviders(<SettingsScreen />, {
      member: buildCurrentMember({ subscription: buildSubscription() }),
    });

    const importExportRow = await screen.findByRole("button", {
      name: translate("settings.importExport"),
    });

    expect(importExportRow.props.accessibilityHint).toBe(translate("subscriptions.plan.lite"));
  });
});
