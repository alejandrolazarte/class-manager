import { fireEvent, screen } from "@testing-library/react-native";
import { getStudentAppNews, markStudentAppNewsSeen } from "@/features/studentApp/studentAppApi";
import { StudentAppNewsScreen } from "@/features/studentApp/screens/StudentAppNewsScreen";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { routerMock } from "@/testing/expoRouterMock";
import { buildStudentAppNews, buildStudentAppNewsItem } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

describe("When an order is ready in the news", () => {
  beforeEach(() => {
    jest.mocked(markStudentAppNewsSeen).mockResolvedValue();
    jest.mocked(getStudentAppNews).mockResolvedValue(
      buildStudentAppNews({
        items: [
          buildStudentAppNewsItem({
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
    await renderStudentAppScreen(<StudentAppNewsScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("student.news.orderReady.cta") }),
    );

    expect(routerMock.navigate).toHaveBeenCalledWith(routes.studentAppOrders);
  });
});
