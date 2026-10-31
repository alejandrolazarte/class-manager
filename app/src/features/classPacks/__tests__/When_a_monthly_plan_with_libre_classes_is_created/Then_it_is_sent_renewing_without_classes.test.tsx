import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { createClassPack } from "@/features/classPacks/classPacksApi";
import { ClassPackFormScreen } from "@/features/classPacks/screens/ClassPackFormScreen";
import { translate } from "@/i18n/translate";
import { buildClassPack } from "@/testing/classPackFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classPacks/classPacksApi");
jest.mock("@/features/classGroups/classGroupsApi");

describe("When a monthly plan with libre classes is created", () => {
  beforeEach(() => {
    jest.mocked(createClassPack).mockResolvedValue(buildClassPack());
  });

  it("Then it is sent renewing without classes", async () => {
    await renderWithProviders(<ClassPackFormScreen />);

    await fireEvent.press(screen.getByText(translate("classPacks.form.offerMonthlyPlan")));
    await fireEvent.changeText(
      screen.getByLabelText(translate("classPacks.form.name")),
      "Cuota mensual",
    );
    await fireEvent.changeText(screen.getByLabelText(translate("classPacks.form.price")), "25000");
    await fireEvent.press(
      screen.getByRole("switch", { name: translate("classPacks.form.anyGroupClass") }),
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() =>
      expect(createClassPack).toHaveBeenCalledWith(
        expect.objectContaining({ renewsMonthly: true, classCount: null, validityMonths: null }),
      ),
    );
  });
});
