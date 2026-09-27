import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { createClassPack } from "@/features/classPacks/classPacksApi";
import { ClassPackFormScreen } from "@/features/classPacks/screens/ClassPackFormScreen";
import { translate } from "@/i18n/translate";
import { buildClassPack } from "@/testing/classPackFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classPacks/classPacksApi");

const materialUrl = "https://example.com/adg-de-autor.pdf";

describe("When pack has class duration and material", () => {
  beforeEach(() => {
    jest.mocked(createClassPack).mockResolvedValue(buildClassPack());
  });

  it("Then they are sent", async () => {
    await renderWithProviders(<ClassPackFormScreen />);
    await fireEvent.changeText(
      screen.getByLabelText(translate("classPacks.form.name")),
      "ADG De Autor",
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("classPacks.form.classCount")),
      "10",
    );
    await fireEvent.changeText(screen.getByLabelText(translate("classPacks.form.price")), "450");
    await fireEvent.press(
      screen.getByRole("button", {
        name: translate("classGroups.form.durationOption", { minutes: 45 }),
      }),
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("classPacks.form.materialUrl")),
      materialUrl,
    );

    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() =>
      expect(createClassPack).toHaveBeenCalledWith(
        expect.objectContaining({ classDurationMinutes: 45, materialUrl }),
      ),
    );
  });
});
