import { fireEvent, screen } from "@testing-library/react-native";
import { getFamilyShop } from "@/features/family/familyApi";
import { FamilyShopScreen } from "@/features/family/screens/FamilyShopScreen";
import { buildFamilyShop } from "@/testing/familyFactory";
import { renderFamilyScreen } from "@/testing/renderFamilyScreen";

jest.mock("@/features/family/familyApi");

const description = "Técnica de los cuatro estilos, virajes y resistencia.";

describe("When family opens a pack with a description", () => {
  beforeEach(() => {
    const shop = buildFamilyShop();
    jest.mocked(getFamilyShop).mockResolvedValue({
      ...shop,
      packs: shop.packs.map((pack) => ({ ...pack, description })),
    });
  });

  it("Then the description is shown", async () => {
    await renderFamilyScreen(<FamilyShopScreen />);

    await fireEvent.press(await screen.findByRole("button", { name: "8 clases" }));

    expect(screen.getByText(description)).toBeOnTheScreen();
  });
});
