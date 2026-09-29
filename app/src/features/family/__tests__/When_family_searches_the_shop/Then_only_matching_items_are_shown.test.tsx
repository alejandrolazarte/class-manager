import { fireEvent, screen } from "@testing-library/react-native";
import { getFamilyShop } from "@/features/family/familyApi";
import { FamilyShopScreen } from "@/features/family/screens/FamilyShopScreen";
import { translate } from "@/i18n/translate";
import { buildFamilyShop } from "@/testing/familyFactory";
import { renderFamilyScreen } from "@/testing/renderFamilyScreen";

jest.mock("@/features/family/familyApi");

describe("When family searches the shop", () => {
  beforeEach(() => {
    jest.mocked(getFamilyShop).mockResolvedValue(buildFamilyShop());
  });

  it("Then only matching items are shown", async () => {
    await renderFamilyScreen(<FamilyShopScreen />);

    await fireEvent.changeText(
      await screen.findByLabelText(translate("family.shop.search")),
      "gorro",
    );

    expect(screen.getByRole("button", { name: "Gorro de natación" })).toBeOnTheScreen();
    expect(screen.queryByText("8 clases")).toBeNull();
  });
});
