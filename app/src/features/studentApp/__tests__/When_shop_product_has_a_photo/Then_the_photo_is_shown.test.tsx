import { screen } from "@testing-library/react-native";
import { getStudentAppShop } from "@/features/studentApp/studentAppApi";
import { StudentAppShopScreen } from "@/features/studentApp/screens/StudentAppShopScreen";
import { buildCatalogImages } from "@/testing/catalogImageFactory";
import { buildStudentAppShop } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

const productImages = buildCatalogImages(1);

describe("When shop product has a photo", () => {
  beforeEach(() => {
    const shop = buildStudentAppShop();
    jest.mocked(getStudentAppShop).mockResolvedValue({
      ...shop,
      products: shop.products.map((product) => ({ ...product, images: productImages })),
    });
  });

  it("Then the photo is shown", async () => {
    await renderStudentAppScreen(<StudentAppShopScreen />);

    const images = await screen.findAllByTestId("catalog-image");

    expect(images[0].props.source).toEqual({ uri: productImages[0].url });
  });
});
