import { fireEvent, screen } from "@testing-library/react-native";
import { ImportExportMenu } from "@/features/importExport/components/ImportExportMenu";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { routerMock } from "@/testing/expoRouterMock";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/importExport/importExportApi");

describe("When import is chosen from the menu", () => {
  it("Then the import screen opens", async () => {
    await renderWithProviders(
      <ImportExportMenu module="students" importRoute={routes.importStudents} />,
    );

    await fireEvent.press(
      screen.getByRole("button", { name: translate("importExport.moreOptions") }),
    );
    await fireEvent.press(
      screen.getByRole("menuitem", { name: translate("importExport.import.students") }),
    );

    expect(routerMock.push).toHaveBeenCalledWith(routes.importStudents);
  });
});
