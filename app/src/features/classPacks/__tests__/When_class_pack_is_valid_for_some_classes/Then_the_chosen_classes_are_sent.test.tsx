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
const adults = buildClassGroup({ id: "class-group-adults", name: "Natación adultos" });

describe("When class pack is valid for some classes", () => {
  beforeEach(() => {
    jest.mocked(createClassPack).mockResolvedValue(buildClassPack());
    jest.mocked(listActiveClassGroups).mockResolvedValue([aquagym, adults]);
  });

  it("Then the chosen classes are sent", async () => {
    await renderWithProviders(<ClassPackFormScreen />);

    await fireEvent.changeText(
      screen.getByLabelText(translate("classPacks.form.name")),
      "8 clases",
    );
    await fireEvent.changeText(screen.getByLabelText(translate("classPacks.form.classCount")), "8");
    await fireEvent.changeText(screen.getByLabelText(translate("classPacks.form.price")), "160");
    await fireEvent.press(await screen.findByRole("button", { name: aquagym.name }));
    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() =>
      expect(createClassPack).toHaveBeenCalledWith(
        expect.objectContaining({ classGroupIds: [aquagym.id] }),
      ),
    );
  });
});
