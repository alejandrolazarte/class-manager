import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { createClassPack } from "@/features/classPacks/classPacksApi";
import { ClassPackFormScreen } from "@/features/classPacks/screens/ClassPackFormScreen";
import { translate } from "@/i18n/translate";
import { buildClassPack } from "@/testing/classPackFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classPacks/classPacksApi");
jest.mock("@/features/classGroups/classGroupsApi");

describe("When class pack is created", () => {
  beforeEach(() => {
    jest.mocked(createClassPack).mockResolvedValue(buildClassPack());
  });

  it("Then classes price and validity are sent", async () => {
    await renderWithProviders(<ClassPackFormScreen />);

    await fireEvent.changeText(
      screen.getByLabelText(translate("classPacks.form.name")),
      "8 clases",
    );
    await fireEvent.changeText(screen.getByLabelText(translate("classPacks.form.classCount")), "8");
    await fireEvent.changeText(screen.getByLabelText(translate("classPacks.form.price")), "160");
    await fireEvent.changeText(
      screen.getByLabelText(translate("classPacks.form.validityMonths")),
      "2",
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() =>
      expect(createClassPack).toHaveBeenCalledWith({
        name: "8 clases",
        description: null,
        classCount: 8,
        price: 160,
        validityMonths: 2,
        classDurationMinutes: null,
        classGroupIds: [],
      }),
    );
  });
});
