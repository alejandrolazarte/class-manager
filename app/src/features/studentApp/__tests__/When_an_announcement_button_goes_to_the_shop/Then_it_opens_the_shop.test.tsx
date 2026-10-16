import { fireEvent, screen } from "@testing-library/react-native";
import { getStudentAppNews, markStudentAppNewsSeen } from "@/features/studentApp/studentAppApi";
import { StudentAppNewsScreen } from "@/features/studentApp/screens/StudentAppNewsScreen";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { routerMock } from "@/testing/expoRouterMock";
import { buildStudentAppNews, buildStudentAppNewsItem } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

describe("When an announcement button goes to the shop", () => {
  beforeEach(() => {
    jest.mocked(markStudentAppNewsSeen).mockResolvedValue();
    jest.mocked(getStudentAppNews).mockResolvedValue(
      buildStudentAppNews({
        items: [
          buildStudentAppNewsItem({
            title: "Llegaron remeras",
            announcementId: "announcement-1",
            category: "News",
            action: "Shop",
          }),
        ],
      }),
    );
  });

  it("Then it opens the shop", async () => {
    await renderStudentAppScreen(<StudentAppNewsScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("student.news.action.shop") }),
    );

    expect(routerMock.navigate).toHaveBeenCalledWith(routes.studentAppShop);
  });
});
