import { screen } from "@testing-library/react-native";
import { listActiveClassGroups } from "@/features/classGroups/classGroupsApi";
import { listClassPacks } from "@/features/classPacks/classPacksApi";
import { ClassPackFormScreen } from "@/features/classPacks/screens/ClassPackFormScreen";
import { translate } from "@/i18n/translate";
import { buildClassGroup } from "@/testing/classGroupFactory";
import { buildClassPack } from "@/testing/classPackFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classPacks/classPacksApi");
jest.mock("@/features/classGroups/classGroupsApi");

const classPack = buildClassPack({ coversAnyGroupClass: true });

describe("When editing a pack for any group class", () => {
  beforeEach(() => {
    jest.mocked(listClassPacks).mockResolvedValue([classPack]);
    jest
      .mocked(listActiveClassGroups)
      .mockResolvedValue([buildClassGroup({ id: "class-group-aquagym", name: "Aquagym" })]);
  });

  it("Then the class list is hidden", async () => {
    await renderWithProviders(<ClassPackFormScreen classPackId={classPack.id} />);

    expect(
      await screen.findByRole("switch", {
        name: translate("classPacks.form.anyGroupClass"),
        checked: true,
      }),
    ).toBeOnTheScreen();
    expect(
      screen.queryByRole("button", { name: translate("classPacks.form.chooseClassGroup") }),
    ).toBeNull();
  });
});
