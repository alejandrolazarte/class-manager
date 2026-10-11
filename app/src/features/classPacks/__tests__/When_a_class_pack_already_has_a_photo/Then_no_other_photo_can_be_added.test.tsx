import { screen } from "@testing-library/react-native";
import { listClassPacks } from "@/features/classPacks/classPacksApi";
import { ClassPackFormScreen } from "@/features/classPacks/screens/ClassPackFormScreen";
import { translate } from "@/i18n/translate";
import { buildCatalogImages } from "@/testing/catalogImageFactory";
import { buildClassPack } from "@/testing/classPackFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classPacks/classPacksApi");
jest.mock("@/features/classGroups/classGroupsApi");

const classPack = buildClassPack({ images: buildCatalogImages(1) });

describe("When a class pack already has a photo", () => {
  beforeEach(() => {
    jest.mocked(listClassPacks).mockResolvedValue([classPack]);
  });

  it("Then no other photo can be added", async () => {
    await renderWithProviders(<ClassPackFormScreen classPackId={classPack.id} />);

    await screen.findByRole("button", { name: translate("catalogImages.remove", { position: 1 }) });
    expect(screen.queryByRole("button", { name: translate("catalogImages.add") })).toBeNull();
  });
});
