import { fireEvent, screen } from "@testing-library/react-native";
import { ClassPackFormScreen } from "@/features/classPacks/screens/ClassPackFormScreen";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classPacks/classPacksApi");
jest.mock("@/features/classGroups/classGroupsApi");

describe("When a monthly plan is being created", () => {
  it("Then validity is not asked", async () => {
    await renderWithProviders(<ClassPackFormScreen />);

    await fireEvent.press(screen.getByText(translate("classPacks.form.offerMonthlyPlan")));

    expect(screen.queryByLabelText(translate("classPacks.form.validityMonths"))).toBeNull();
  });
});
