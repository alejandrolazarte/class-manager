import { fireEvent, screen } from "@testing-library/react-native";
import { getImportSchema, previewImport } from "@/features/importExport/importExportApi";
import { pickImportFile } from "@/features/importExport/pickImportFile";
import { ImportExportScreen } from "@/features/importExport/screens/ImportExportScreen";
import { translate, translateCount } from "@/i18n/translate";
import {
  buildStudentsReport,
  buildStudentsSchema,
  pickedStudentsFile,
} from "@/testing/importExportFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/importExport/importExportApi");
jest.mock("@/features/importExport/pickImportFile");
jest.mock("@/features/importExport/saveSpreadsheetFile");

const roomLeft = 1;

describe("When an import goes past the student limit", () => {
  beforeEach(() => {
    const report = buildStudentsReport();
    jest.mocked(getImportSchema).mockResolvedValue(buildStudentsSchema());
    jest.mocked(pickImportFile).mockResolvedValue(pickedStudentsFile);
    jest.mocked(previewImport).mockResolvedValue({
      ...report,
      planLimit: { featureCode: "students", remaining: roomLeft },
    });
  });

  it("Then the preview explains the room left", async () => {
    await renderWithProviders(<ImportExportScreen />);

    await fireEvent.press(
      screen.getByRole("button", { name: translate("importExport.chooseFile") }),
    );

    const report = buildStudentsReport();
    expect(
      await screen.findByText(
        translateCount("importExport.planLimit", roomLeft, { rows: report.summary.valid }),
      ),
    ).toBeTruthy();
    expect(
      screen.queryByRole("button", {
        name: translateCount("importExport.preview.import", report.summary.valid),
      }),
    ).toBeNull();
  });
});
