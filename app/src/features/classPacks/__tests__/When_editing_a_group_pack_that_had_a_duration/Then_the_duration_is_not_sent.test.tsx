import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { listActiveClassGroups } from "@/features/classGroups/classGroupsApi";
import { listClassPacks, updateClassPack } from "@/features/classPacks/classPacksApi";
import { ClassPackFormScreen } from "@/features/classPacks/screens/ClassPackFormScreen";
import { translate } from "@/i18n/translate";
import { buildClassGroup } from "@/testing/classGroupFactory";
import { buildClassPack } from "@/testing/classPackFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classPacks/classPacksApi");
jest.mock("@/features/classGroups/classGroupsApi");

const aquagym = buildClassGroup({ id: "class-group-aquagym", name: "Aquagym" });
const classPack = buildClassPack({ classDurationMinutes: 45, classGroupIds: [aquagym.id] });

describe("When editing a group pack that had a duration", () => {
  beforeEach(() => {
    jest.mocked(listClassPacks).mockResolvedValue([classPack]);
    jest.mocked(listActiveClassGroups).mockResolvedValue([aquagym]);
    jest.mocked(updateClassPack).mockResolvedValue(classPack);
  });

  it("Then the duration is not sent", async () => {
    await renderWithProviders(<ClassPackFormScreen classPackId={classPack.id} />);
    await screen.findByText(aquagym.name);

    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() =>
      expect(updateClassPack).toHaveBeenCalledWith(
        classPack.id,
        expect.objectContaining({ classDurationMinutes: null, classGroupIds: [aquagym.id] }),
      ),
    );
  });
});
