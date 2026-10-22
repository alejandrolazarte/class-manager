import { fireEvent, screen } from "@testing-library/react-native";
import { ImportExportScreen } from "@/features/importExport/screens/ImportExportScreen";
import { pickImportFile } from "@/features/importExport/pickImportFile";
import { translate } from "@/i18n/translate";
import { buildCurrentMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSubscription } from "@/testing/subscriptionFactory";

jest.mock("@/features/importExport/importExportApi");
jest.mock("@/features/importExport/pickImportFile");

describe("When the plan does not include import and export", () => {
  it("Then choosing a file offers a plan", async () => {
    await renderWithProviders(<ImportExportScreen />, {
      member: buildCurrentMember({ subscription: buildSubscription() }),
    });

    await fireEvent.press(
      screen.getByRole("button", { name: translate("importExport.chooseFile") }),
    );

    expect(screen.getByText(translate("subscriptions.locked.importExport"))).toBeTruthy();
    expect(pickImportFile).not.toHaveBeenCalled();
  });
});
