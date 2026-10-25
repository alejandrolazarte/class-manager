import { screen } from "@testing-library/react-native";
import { ClassPackFormScreen } from "@/features/classPacks/screens/ClassPackFormScreen";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classPacks/classPacksApi");
jest.mock("@/features/classGroups/classGroupsApi");

describe("When class pack form is opened", () => {
  it("Then what the validity means is explained", async () => {
    await renderWithProviders(<ClassPackFormScreen />);

    expect(await screen.findByText(translate("classPacks.form.validityHint"))).toBeOnTheScreen();
  });
});
