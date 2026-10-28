import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { listActiveClassGroups } from "@/features/classGroups/classGroupsApi";
import { createClassPack } from "@/features/classPacks/classPacksApi";
import { ClassPackFormScreen } from "@/features/classPacks/screens/ClassPackFormScreen";
import { translate } from "@/i18n/translate";
import { buildClassGroup } from "@/testing/classGroupFactory";
import { buildClassPack } from "@/testing/classPackFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classPacks/classPacksApi");
jest.mock("@/features/classGroups/classGroupsApi");

describe("When class pack is for any group class", () => {
  beforeEach(() => {
    jest.mocked(createClassPack).mockResolvedValue(buildClassPack());
    jest
      .mocked(listActiveClassGroups)
      .mockResolvedValue([buildClassGroup({ id: "class-group-aquagym", name: "Aquagym" })]);
  });

  it("Then it is saved without choosing classes", async () => {
    await renderWithProviders(<ClassPackFormScreen />);

    await fireEvent.changeText(
      screen.getByLabelText(translate("classPacks.form.name")),
      "8 clases",
    );
    await fireEvent.changeText(screen.getByLabelText(translate("classPacks.form.classCount")), "8");
    await fireEvent.changeText(screen.getByLabelText(translate("classPacks.form.price")), "160");
    await fireEvent.press(
      screen.getByRole("switch", { name: translate("classPacks.form.anyGroupClass") }),
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() =>
      expect(createClassPack).toHaveBeenCalledWith(
        expect.objectContaining({ coversAnyGroupClass: true, classGroupIds: [] }),
      ),
    );
  });
});
