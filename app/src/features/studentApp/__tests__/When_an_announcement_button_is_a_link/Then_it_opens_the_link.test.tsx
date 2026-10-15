import { fireEvent, screen } from "@testing-library/react-native";
import { Linking } from "react-native";
import { getStudentAppNews, markStudentAppNewsSeen } from "@/features/studentApp/studentAppApi";
import { StudentAppNewsScreen } from "@/features/studentApp/screens/StudentAppNewsScreen";
import { translate } from "@/i18n/translate";
import { buildStudentAppNews, buildStudentAppNewsItem } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

describe("When an announcement button is a link", () => {
  beforeEach(() => {
    jest.spyOn(Linking, "openURL").mockResolvedValue(true);
    jest.mocked(markStudentAppNewsSeen).mockResolvedValue();
    jest.mocked(getStudentAppNews).mockResolvedValue(
      buildStudentAppNews({
        items: [
          buildStudentAppNewsItem({
            title: "Inscripciones abiertas",
            announcementId: "announcement-1",
            category: "News",
            action: "Link",
            actionUrl: "https://example.com/inscripcion",
          }),
        ],
      }),
    );
  });

  it("Then it opens the link", async () => {
    await renderStudentAppScreen(<StudentAppNewsScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("student.news.action.link") }),
    );

    expect(Linking.openURL).toHaveBeenCalledWith("https://example.com/inscripcion");
  });
});
