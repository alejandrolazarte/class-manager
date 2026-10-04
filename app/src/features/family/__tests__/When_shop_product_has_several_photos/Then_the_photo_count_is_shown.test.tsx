import { screen } from "@testing-library/react-native";
import { getFamilyShop } from "@/features/family/familyApi";
import { FamilyShopScreen } from "@/features/family/screens/FamilyShopScreen";
import { translate } from "@/i18n/translate";
import { buildCatalogImages } from "@/testing/catalogImageFactory";
import { buildFamilyShop } from "@/testing/familyFactory";
import { renderFamilyScreen } from "@/testing/renderFamilyScreen";

jest.mock("@/features/family/familyApi");

const photoCount = 3;

describe("When shop product has several photos", () => {
  beforeEach(() => {
    const shop = buildFamilyShop();
    jest.mocked(getFamilyShop).mockResolvedValue({
      ...shop,
      products: shop.products.map((product) => ({
        ...product,
        images: buildCatalogImages(photoCount),
      })),
    });
  });

  it("Then the photo count is shown", async () => {
    await renderFamilyScreen(<FamilyShopScreen />);

    expect(
      await screen.findByLabelText(translate("family.shop.photoCount", { count: photoCount })),
    ).toBeOnTheScreen();
  });
});
