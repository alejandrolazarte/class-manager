import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { createClassPack } from "@/features/classPacks/classPacksApi";
import { ClassPackFormScreen } from "@/features/classPacks/screens/ClassPackFormScreen";
import { translate } from "@/i18n/translate";
import { buildClassPack } from "@/testing/classPackFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classPacks/classPacksApi");
jest.mock("@/features/classGroups/classGroupsApi");

describe("When a monthly plan has a limit of classes", () => {
  beforeEach(() => {
    jest.mocked(createClassPack).mockResolvedValue(buildClassPack());
  });

  it("Then the classes per month are sent", async () => {
    await renderWithProviders(<ClassPackFormScreen />);

    await fireEvent.press(screen.getByText(translate("classPacks.form.offerMonthlyPlan")));
    await fireEvent.press(
      screen.getByRole("switch", { name: translate("classPacks.form.libreClasses") }),
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("classPacks.form.name")),
      "2 veces por semana",
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("classPacks.form.classesPerMonth")),
      "8",
    );
    await fireEvent.changeText(screen.getByLabelText(translate("classPacks.form.price")), "20000");
    await fireEvent.press(
      screen.getByRole("switch", { name: translate("classPacks.form.anyGroupClass") }),
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() =>
      expect(createClassPack).toHaveBeenCalledWith(
        expect.objectContaining({ renewsMonthly: true, classCount: 8 }),
      ),
    );
  });
});
