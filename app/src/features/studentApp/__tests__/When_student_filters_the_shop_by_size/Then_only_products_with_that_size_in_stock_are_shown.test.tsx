import { fireEvent, screen } from "@testing-library/react-native";
import { getStudentAppShop } from "@/features/studentApp/studentAppApi";
import { StudentAppShopScreen } from "@/features/studentApp/screens/StudentAppShopScreen";
import { translate, translateCount } from "@/i18n/translate";
import { buildStudentAppShop } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

describe("When student filters the shop by size", () => {
  beforeEach(() => {
    const shop = buildStudentAppShop();
    jest.mocked(getStudentAppShop).mockResolvedValue({
      ...shop,
      products: [
        ...shop.products,
        {
          id: "product-swimsuit",
          name: "Malla",
          description: null,
          price: 30,
          variants: [
            {
              id: "variant-swimsuit-m",
              size: "M",
              color: null,
              availability: "Available",
              maximumUnits: null,
            },
          ],
          images: [],
        },
      ],
    });
  });

  it("Then only products with that size in stock are shown", async () => {
    await renderStudentAppScreen(<StudentAppShopScreen />);

    await fireEvent.press(await screen.findByRole("button", { name: translate("filters.size") }));
    await fireEvent.press(screen.getByRole("button", { name: "M" }));
    await fireEvent.press(
      screen.getByRole("button", { name: translateCount("filters.showResults", 2) }),
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("filters.availability") }));
    await fireEvent.press(
      screen.getByRole("radio", { name: translate("filters.availability.available") }),
    );

    expect(screen.getByRole("button", { name: "Malla" })).toBeOnTheScreen();
    expect(screen.queryByRole("button", { name: "Gorro de natación" })).toBeNull();
  });
});
