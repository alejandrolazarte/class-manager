import { fireEvent, screen } from "@testing-library/react-native";
import { ImportExportMenu } from "@/features/importExport/components/ImportExportMenu";
import { downloadExport } from "@/features/importExport/importExportApi";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/importExport/importExportApi");
jest.mock("@/features/importExport/saveSpreadsheetFile");

describe("When the export download fails", () => {
  beforeEach(() => {
    jest.mocked(downloadExport).mockRejectedValue(new Error("offline"));
  });

  it("Then a toast explains it", async () => {
    await renderWithProviders(
      <ImportExportMenu module="students" importRoute={routes.importStudents} />,
    );

    await fireEvent.press(
      screen.getByRole("button", { name: translate("importExport.moreOptions") }),
    );
    await fireEvent.press(
      screen.getByRole("menuitem", { name: translate("importExport.export.students") }),
    );

    expect(await screen.findByText(translate("importExport.downloadFailed"))).toBeOnTheScreen();
  });
});
