import { screen, within } from "@testing-library/react-native";
import {
  getStudentAppHome,
  getStudentAppNews,
  getStudentAppShop,
} from "@/features/studentApp/studentAppApi";
import { StudentAppHomeScreen } from "@/features/studentApp/screens/StudentAppHomeScreen";
import { translate } from "@/i18n/translate";
import {
  buildStudentAppHome,
  buildStudentAppNews,
  buildStudentAppNewsItem,
  buildStudentAppShop,
} from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

describe("When the school pins an announcement", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppShop).mockResolvedValue(buildStudentAppShop());
    jest.mocked(getStudentAppHome).mockResolvedValue(buildStudentAppHome());
    jest.mocked(getStudentAppNews).mockResolvedValue(
      buildStudentAppNews({
        items: [
          buildStudentAppNewsItem({
            id: "pinned",
            title: "Horarios de verano",
            category: "Important",
            isPinned: true,
          }),
        ],
      }),
    );
  });

  it("Then it leads the news of home", async () => {
    await renderStudentAppScreen(<StudentAppHomeScreen />);

    const pinned = within(await screen.findByTestId("pinned-news"));
    expect(pinned.getByText("Horarios de verano")).toBeOnTheScreen();
    expect(pinned.getByText(translate("student.news.pinned"))).toBeOnTheScreen();
    expect(screen.getByText(translate("student.news.homeTitle"))).toBeOnTheScreen();
  });
});
