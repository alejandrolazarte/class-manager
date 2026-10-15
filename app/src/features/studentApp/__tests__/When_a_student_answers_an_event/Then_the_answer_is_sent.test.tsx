import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import {
  getStudentAppNews,
  markStudentAppNewsSeen,
  replyToAnnouncement,
} from "@/features/studentApp/studentAppApi";
import { StudentAppNewsScreen } from "@/features/studentApp/screens/StudentAppNewsScreen";
import { translate } from "@/i18n/translate";
import { buildStudentAppNews, buildStudentAppNewsItem } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

describe("When a student answers an event", () => {
  beforeEach(() => {
    jest.mocked(markStudentAppNewsSeen).mockResolvedValue();
    jest.mocked(replyToAnnouncement).mockResolvedValue();
    jest.mocked(getStudentAppNews).mockResolvedValue(
      buildStudentAppNews({
        items: [
          buildStudentAppNewsItem({
            announcementId: "announcement-1",
            title: "Muestra de fin de año",
            category: "Event",
          }),
        ],
      }),
    );
  });

  it("Then the answer is sent", async () => {
    await renderStudentAppScreen(<StudentAppNewsScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("student.news.reply.going") }),
    );

    await waitFor(() => expect(replyToAnnouncement).toHaveBeenCalledWith("announcement-1", true));
    expect(await screen.findByText(translate("student.news.reply.saved"))).toBeOnTheScreen();
  });
});
