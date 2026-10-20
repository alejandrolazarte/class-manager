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

const aquagym = buildClassGroup({ id: "class-group-aquagym", name: "Aquagym" });

describe("When pack is for private lessons", () => {
  beforeEach(() => {
    jest.mocked(createClassPack).mockResolvedValue(buildClassPack());
    jest.mocked(listActiveClassGroups).mockResolvedValue([aquagym]);
  });

  it("Then the duration is sent without classes", async () => {
    await renderWithProviders(<ClassPackFormScreen />);
    await fireEvent.changeText(
      screen.getByLabelText(translate("classPacks.form.name")),
      "10 clases",
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("classPacks.form.classCount")),
      "10",
    );
    await fireEvent.changeText(screen.getByLabelText(translate("classPacks.form.price")), "450");
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("classPacks.form.chooseClassGroup") }),
    );
    await fireEvent.press(await screen.findByRole("button", { name: aquagym.name }));
    await fireEvent.press(
      screen.getByRole("button", { name: translate("classPacks.form.kindPrivateLessons") }),
    );
    await fireEvent.press(
      screen.getByRole("button", {
        name: translate("classGroups.form.durationOption", { minutes: 45 }),
      }),
    );

    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() =>
      expect(createClassPack).toHaveBeenCalledWith(
        expect.objectContaining({ classDurationMinutes: 45, classGroupIds: [] }),
      ),
    );
  });
});
