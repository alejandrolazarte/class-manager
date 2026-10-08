import { fireEvent, screen, within } from "@testing-library/react-native";
import { listActiveClassGroups } from "@/features/classGroups/classGroupsApi";
import { ClassPackFormScreen } from "@/features/classPacks/screens/ClassPackFormScreen";
import { translate } from "@/i18n/translate";
import { buildClassGroup } from "@/testing/classGroupFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classPacks/classPacksApi");
jest.mock("@/features/classGroups/classGroupsApi");

const aquagym = buildClassGroup({ id: "class-group-aquagym", name: "Aquagym" });
const adults = buildClassGroup({ id: "class-group-adults", name: "Natación adultos" });

describe("When a class is already chosen for a pack", () => {
  beforeEach(() => {
    jest.mocked(listActiveClassGroups).mockResolvedValue([aquagym, adults]);
  });

  it("Then the picker does not offer it again", async () => {
    await renderWithProviders(<ClassPackFormScreen />);
    await fireEvent.press(
      await screen.findByRole("button", { name: translate("classPacks.form.chooseClassGroup") }),
    );
    await fireEvent.press(await screen.findByRole("button", { name: aquagym.name }));

    await fireEvent.press(
      screen.getByRole("button", { name: translate("classPacks.form.addClassGroup") }),
    );

    const picker = within(screen.getByTestId("class-pack-class-group-picker"));
    expect(picker.getByRole("button", { name: adults.name })).toBeOnTheScreen();
    expect(picker.queryByRole("button", { name: aquagym.name })).toBeNull();
  });
});
