import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { pickCatalogImage } from "@/features/catalogImages/pickCatalogImage";
import { createClassPack, uploadClassPackImage } from "@/features/classPacks/classPacksApi";
import { ClassPackFormScreen } from "@/features/classPacks/screens/ClassPackFormScreen";
import { translate } from "@/i18n/translate";
import { buildPickedImage } from "@/testing/catalogImageFactory";
import { buildClassPack } from "@/testing/classPackFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classPacks/classPacksApi");
jest.mock("@/features/classGroups/classGroupsApi");
jest.mock("@/features/catalogImages/pickCatalogImage", () => ({
  ...jest.requireActual("@/features/catalogImages/pickCatalogImage"),
  pickCatalogImage: jest.fn(),
}));

const pickedImage = buildPickedImage();
const createdClassPack = buildClassPack({ id: "new-pack" });

describe("When class pack is created with a photo", () => {
  beforeEach(() => {
    jest.mocked(pickCatalogImage).mockResolvedValue(pickedImage);
    jest.mocked(createClassPack).mockResolvedValue(createdClassPack);
    jest.mocked(uploadClassPackImage).mockResolvedValue(createdClassPack);
  });

  it("Then the photo is uploaded to the new pack", async () => {
    await renderWithProviders(<ClassPackFormScreen />);

    await fireEvent.changeText(
      screen.getByLabelText(translate("classPacks.form.name")),
      "4 clases",
    );
    await fireEvent.changeText(screen.getByLabelText(translate("classPacks.form.classCount")), "4");
    await fireEvent.changeText(screen.getByLabelText(translate("classPacks.form.price")), "80");
    await fireEvent.press(screen.getByRole("button", { name: translate("catalogImages.upload") }));
    await screen.findByTestId("catalog-image");
    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() => expect(uploadClassPackImage).toHaveBeenCalledWith("new-pack", pickedImage));
  });
});
