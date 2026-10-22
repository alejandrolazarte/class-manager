import { screen } from "@testing-library/react-native";
import { listActiveClassGroups } from "@/features/classGroups/classGroupsApi";
import { ClassPackFormScreen } from "@/features/classPacks/screens/ClassPackFormScreen";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classPacks/classPacksApi");
jest.mock("@/features/classGroups/classGroupsApi");

describe("When there are no classes yet", () => {
  beforeEach(() => {
    jest.mocked(listActiveClassGroups).mockResolvedValue([]);
  });

  it("Then creating a class first is explained", async () => {
    await renderWithProviders(<ClassPackFormScreen />);

    expect(
      await screen.findByText(translate("classPacks.form.noClassGroupsYet")),
    ).toBeOnTheScreen();
  });
});
