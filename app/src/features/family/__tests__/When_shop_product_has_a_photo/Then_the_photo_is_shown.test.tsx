import { screen } from "@testing-library/react-native";
import { getFamilyShop } from "@/features/family/familyApi";
import { FamilyShopScreen } from "@/features/family/screens/FamilyShopScreen";
import { catalogImageUrl } from "@/testing/catalogImageFactory";
import { buildFamilyShop } from "@/testing/familyFactory";
import { renderFamilyScreen } from "@/testing/renderFamilyScreen";

jest.mock("@/features/family/familyApi");

describe("When shop product has a photo", () => {
  beforeEach(() => {
    const shop = buildFamilyShop();
    jest.mocked(getFamilyShop).mockResolvedValue({
      ...shop,
      products: shop.products.map((product) => ({ ...product, imageUrl: catalogImageUrl })),
    });
  });

  it("Then the photo is shown", async () => {
    await renderFamilyScreen(<FamilyShopScreen />);

    const images = await screen.findAllByTestId("catalog-image");

    expect(images[0].props.source).toEqual({ uri: catalogImageUrl });
  });
});
