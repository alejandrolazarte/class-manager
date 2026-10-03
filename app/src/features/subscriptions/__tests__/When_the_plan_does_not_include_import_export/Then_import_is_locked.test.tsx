import { screen } from "@testing-library/react-native";
import { ImportExportScreen } from "@/features/importExport/screens/ImportExportScreen";
import { translate } from "@/i18n/translate";
import { buildCurrentMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSubscription } from "@/testing/subscriptionFactory";

jest.mock("@/features/importExport/importExportApi");

describe("When the plan does not include import and export", () => {
  it("Then import is locked", async () => {
    await renderWithProviders(<ImportExportScreen />, {
      member: buildCurrentMember({ subscription: buildSubscription() }),
    });

    expect(screen.getByText(translate("subscriptions.locked.importExport"))).toBeTruthy();
    expect(screen.queryByRole("button", { name: translate("importExport.chooseFile") })).toBeNull();
  });
});
