import { fireEvent, screen } from "@testing-library/react-native";
import { getFamilyNews, markFamilyNewsSeen } from "@/features/family/familyApi";
import { FamilyNewsScreen } from "@/features/family/screens/FamilyNewsScreen";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { routerMock } from "@/testing/expoRouterMock";
import { buildFamilyNews, buildFamilyNewsItem } from "@/testing/familyFactory";
import { renderFamilyScreen } from "@/testing/renderFamilyScreen";

jest.mock("@/features/family/familyApi");

describe("When an order is ready in the news", () => {
  beforeEach(() => {
    jest.mocked(markFamilyNewsSeen).mockResolvedValue();
    jest.mocked(getFamilyNews).mockResolvedValue(
      buildFamilyNews({
        items: [
          buildFamilyNewsItem({
            id: "order",
            kind: "OrderReady",
            title: null,
            body: null,
            orderId: "order-1",
          }),
        ],
      }),
    );
  });

  it("Then it opens my orders", async () => {
    await renderFamilyScreen(<FamilyNewsScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("family.news.orderReady.cta") }),
    );

    expect(routerMock.navigate).toHaveBeenCalledWith(routes.familyOrders);
  });
});
