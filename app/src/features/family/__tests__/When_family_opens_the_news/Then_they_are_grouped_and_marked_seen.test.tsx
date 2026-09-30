import { screen, waitFor } from "@testing-library/react-native";
import { getFamilyNews, markFamilyNewsSeen } from "@/features/family/familyApi";
import { FamilyNewsScreen } from "@/features/family/screens/FamilyNewsScreen";
import { translate } from "@/i18n/translate";
import { buildFamilyNews, buildFamilyNewsItem } from "@/testing/familyFactory";
import { renderFamilyScreen } from "@/testing/renderFamilyScreen";

jest.mock("@/features/family/familyApi");

describe("When family opens the news", () => {
  beforeEach(() => {
    jest.mocked(markFamilyNewsSeen).mockResolvedValue();
    jest.mocked(getFamilyNews).mockResolvedValue(
      buildFamilyNews({
        items: [
          buildFamilyNewsItem({
            id: "today",
            title: "Lunes 12 cerrado",
            occurredAt: new Date().toISOString(),
          }),
          buildFamilyNewsItem({
            id: "old",
            title: "Muestra de fin de año",
            occurredAt: "2026-01-10T12:00:00Z",
            isUnread: false,
          }),
        ],
        unreadCount: 1,
      }),
    );
  });

  it("Then they are grouped and marked seen", async () => {
    await renderFamilyScreen(<FamilyNewsScreen />);

    expect(await screen.findByText("Lunes 12 cerrado")).toBeOnTheScreen();
    expect(screen.getByText(translate("family.news.group.today"))).toBeOnTheScreen();
    expect(screen.getByText(translate("family.news.group.earlier"))).toBeOnTheScreen();
    await waitFor(() => expect(markFamilyNewsSeen).toHaveBeenCalled());
  });
});
