import { screen } from "@testing-library/react-native";
import { getFamilyHome, getFamilyNews, getFamilyShop } from "@/features/family/familyApi";
import { FamilyHomeScreen } from "@/features/family/screens/FamilyHomeScreen";
import { translateCount } from "@/i18n/translate";
import {
  buildFamilyHome,
  buildFamilyNews,
  buildFamilyNewsItem,
  buildFamilyShop,
} from "@/testing/familyFactory";
import { renderFamilyScreen } from "@/testing/renderFamilyScreen";

jest.mock("@/features/family/familyApi");

describe("When family has unread news", () => {
  beforeEach(() => {
    jest.mocked(getFamilyHome).mockResolvedValue(buildFamilyHome());
    jest.mocked(getFamilyShop).mockResolvedValue(buildFamilyShop());
    jest.mocked(getFamilyNews).mockResolvedValue(
      buildFamilyNews({
        items: [buildFamilyNewsItem({ id: "a" }), buildFamilyNewsItem({ id: "b" })],
        unreadCount: 2,
      }),
    );
  });

  it("Then the bell shows the count", async () => {
    await renderFamilyScreen(<FamilyHomeScreen />);

    expect(
      await screen.findByRole("button", { name: translateCount("family.news.bell", 2) }),
    ).toBeOnTheScreen();
  });
});
