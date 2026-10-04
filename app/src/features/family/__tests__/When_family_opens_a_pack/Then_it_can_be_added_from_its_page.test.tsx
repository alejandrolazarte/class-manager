import { fireEvent, screen } from "@testing-library/react-native";
import { getFamilyShop } from "@/features/family/familyApi";
import { FamilyShopScreen } from "@/features/family/screens/FamilyShopScreen";
import { formatMoney } from "@/features/fees/money";
import { translate } from "@/i18n/translate";
import { buildFamilyShop } from "@/testing/familyFactory";
import { renderFamilyScreen } from "@/testing/renderFamilyScreen";

jest.mock("@/features/family/familyApi");

describe("When family opens a pack", () => {
  beforeEach(() => {
    jest.mocked(getFamilyShop).mockResolvedValue(buildFamilyShop());
  });

  it("Then it can be added from its page", async () => {
    await renderFamilyScreen(<FamilyShopScreen />);

    await fireEvent.press(await screen.findByRole("button", { name: "8 clases" }));
    await fireEvent.press(
      screen.getByRole("button", {
        name: translate("family.shop.addToCart", { total: formatMoney(120, "EUR") }),
      }),
    );

    expect(screen.getByRole("button", { name: /Ver carrito/ })).toBeOnTheScreen();
  });
});
