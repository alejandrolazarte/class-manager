import { screen } from "@testing-library/react-native";
import { getStudentAppShop } from "@/features/studentApp/studentAppApi";
import { StudentAppShopScreen } from "@/features/studentApp/screens/StudentAppShopScreen";
import { translate } from "@/i18n/translate";
import { buildCatalogImages } from "@/testing/catalogImageFactory";
import { buildStudentAppShop } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

const photoCount = 3;

describe("When shop product has several photos", () => {
  beforeEach(() => {
    const shop = buildStudentAppShop();
    jest.mocked(getStudentAppShop).mockResolvedValue({
      ...shop,
      products: shop.products.map((product) => ({
        ...product,
        images: buildCatalogImages(photoCount),
      })),
    });
  });

  it("Then the photo count is shown", async () => {
    await renderStudentAppScreen(<StudentAppShopScreen />);

    expect(
      await screen.findByLabelText(translate("student.shop.photoCount", { count: photoCount })),
    ).toBeOnTheScreen();
  });
});
