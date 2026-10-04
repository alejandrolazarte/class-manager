import { screen } from "@testing-library/react-native";
import {
  getStudentAppHome,
  getStudentAppNews,
  getStudentAppShop,
} from "@/features/studentApp/studentAppApi";
import { StudentAppHomeScreen } from "@/features/studentApp/screens/StudentAppHomeScreen";
import { translateCount } from "@/i18n/translate";
import {
  buildStudentAppHome,
  buildStudentAppNews,
  buildStudentAppNewsItem,
  buildStudentAppShop,
} from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

describe("When student has unread news", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppHome).mockResolvedValue(buildStudentAppHome());
    jest.mocked(getStudentAppShop).mockResolvedValue(buildStudentAppShop());
    jest.mocked(getStudentAppNews).mockResolvedValue(
      buildStudentAppNews({
        items: [buildStudentAppNewsItem({ id: "a" }), buildStudentAppNewsItem({ id: "b" })],
        unreadCount: 2,
      }),
    );
  });

  it("Then the bell shows the count", async () => {
    await renderStudentAppScreen(<StudentAppHomeScreen />);

    expect(
      await screen.findByRole("button", { name: translateCount("student.news.bell", 2) }),
    ).toBeOnTheScreen();
  });
});
