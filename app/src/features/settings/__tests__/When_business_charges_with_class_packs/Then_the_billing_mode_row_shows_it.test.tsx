import { screen } from "@testing-library/react-native";
import { listBranches } from "@/features/branches/branchesApi";
import { SettingsScreen } from "@/features/settings/screens/SettingsScreen";
import { translate } from "@/i18n/translate";
import { buildBusiness } from "@/testing/businessFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/branches/branchesApi");
jest.mock("@/features/authentication/useSession", () => ({
  useSession: () => ({ signOut: jest.fn() }),
}));

describe("When business charges with class packs", () => {
  beforeEach(() => {
    jest.mocked(listBranches).mockResolvedValue([]);
  });

  it("Then the billing mode row shows it", async () => {
    await renderWithProviders(<SettingsScreen />, {
      business: buildBusiness({ billingMode: "ClassPacks" }),
    });

    expect(await screen.findByLabelText(translate("fees.billingPlan.title"))).toHaveProp(
      "accessibilityHint",
      translate("fees.billingMode.ClassPacks"),
    );
  });
});
