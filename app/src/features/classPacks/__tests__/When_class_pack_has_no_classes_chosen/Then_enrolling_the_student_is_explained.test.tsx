import { fireEvent, screen } from "@testing-library/react-native";
import { listActiveClassGroups } from "@/features/classGroups/classGroupsApi";
import { ClassPackFormScreen } from "@/features/classPacks/screens/ClassPackFormScreen";
import { translate } from "@/i18n/translate";
import { buildClassGroup } from "@/testing/classGroupFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classPacks/classPacksApi");
jest.mock("@/features/classGroups/classGroupsApi");

const aquagym = buildClassGroup({ id: "class-group-aquagym", name: "Aquagym" });

describe("When class pack has no classes chosen", () => {
  beforeEach(() => {
    jest.mocked(listActiveClassGroups).mockResolvedValue([aquagym]);
  });

  it("Then enrolling the student is explained", async () => {
    await renderWithProviders(<ClassPackFormScreen />);

    expect(
      await screen.findByText(translate("classPacks.form.classGroupsNoneHint")),
    ).toBeOnTheScreen();
    await fireEvent.press(
      screen.getByRole("button", { name: translate("classPacks.form.chooseClassGroup") }),
    );
    await fireEvent.press(await screen.findByRole("button", { name: aquagym.name }));
    expect(screen.getByText(translate("classPacks.form.classGroupsHint"))).toBeOnTheScreen();
  });
});
