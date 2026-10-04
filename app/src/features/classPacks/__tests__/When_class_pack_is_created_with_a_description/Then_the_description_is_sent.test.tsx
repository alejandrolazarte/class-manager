import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { createClassPack } from "@/features/classPacks/classPacksApi";
import { ClassPackFormScreen } from "@/features/classPacks/screens/ClassPackFormScreen";
import { translate } from "@/i18n/translate";
import { buildClassPack } from "@/testing/classPackFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classPacks/classPacksApi");
jest.mock("@/features/classGroups/classGroupsApi");

const description = "Clases grupales para quienes empiezan: flotación, respiración y patada.";

describe("When class pack is created with a description", () => {
  beforeEach(() => {
    jest.mocked(createClassPack).mockResolvedValue(buildClassPack());
  });

  it("Then the description is sent", async () => {
    await renderWithProviders(<ClassPackFormScreen />);

    await fireEvent.changeText(
      screen.getByLabelText(translate("classPacks.form.name")),
      "Natación inicial",
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("classPacks.form.description")),
      `  ${description}  `,
    );
    await fireEvent.changeText(screen.getByLabelText(translate("classPacks.form.classCount")), "4");
    await fireEvent.changeText(screen.getByLabelText(translate("classPacks.form.price")), "100");
    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() =>
      expect(createClassPack).toHaveBeenCalledWith(expect.objectContaining({ description })),
    );
  });
});
