import { fireEvent, screen } from "@testing-library/react-native";
import { listActiveClassGroups } from "@/features/classGroups/classGroupsApi";
import { ClassPackFormScreen } from "@/features/classPacks/screens/ClassPackFormScreen";
import { translate } from "@/i18n/translate";
import { buildClassGroup } from "@/testing/classGroupFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classPacks/classPacksApi");
jest.mock("@/features/classGroups/classGroupsApi");

const aquagym = buildClassGroup({ id: "class-group-aquagym", name: "Aquagym" });

describe("When group pack has no class chosen", () => {
  beforeEach(() => {
    jest.mocked(listActiveClassGroups).mockResolvedValue([aquagym]);
  });

  it("Then it cannot be saved", async () => {
    await renderWithProviders(<ClassPackFormScreen />);
    await fireEvent.changeText(
      screen.getByLabelText(translate("classPacks.form.name")),
      "8 clases",
    );
    await fireEvent.changeText(screen.getByLabelText(translate("classPacks.form.classCount")), "8");
    await fireEvent.changeText(screen.getByLabelText(translate("classPacks.form.price")), "160");
    await screen.findByRole("button", { name: translate("classPacks.form.chooseClassGroup") });

    expect(screen.getByRole("button", { name: translate("common.save") })).toBeDisabled();
  });
});
