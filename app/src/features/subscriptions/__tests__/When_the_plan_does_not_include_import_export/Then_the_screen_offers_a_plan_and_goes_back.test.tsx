import { screen } from "@testing-library/react-native";
import { ImportExportScreen } from "@/features/importExport/screens/ImportExportScreen";
import { translate } from "@/i18n/translate";
import { routerMock } from "@/testing/expoRouterMock";
import { buildCurrentMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSubscription } from "@/testing/subscriptionFactory";

jest.mock("@/features/importExport/importExportApi");

describe("When the plan does not include import and export", () => {
  it("Then the screen offers a plan and goes back", async () => {
    await renderWithProviders(<ImportExportScreen />, {
      member: buildCurrentMember({ subscription: buildSubscription() }),
    });

    expect(await screen.findByText(translate("subscriptions.locked.importExport"))).toBeTruthy();
    expect(screen.queryByRole("button", { name: translate("importExport.chooseFile") })).toBeNull();
    expect(routerMock.back).toHaveBeenCalled();
  });
});
