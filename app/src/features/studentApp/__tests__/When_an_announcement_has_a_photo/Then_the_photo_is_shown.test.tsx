import { screen } from "@testing-library/react-native";
import { getStudentAppNews, markStudentAppNewsSeen } from "@/features/studentApp/studentAppApi";
import { StudentAppNewsScreen } from "@/features/studentApp/screens/StudentAppNewsScreen";
import { buildStudentAppNews, buildStudentAppNewsItem } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

const imageUrl = "https://files.example.com/announcements/photo.png";

describe("When an announcement has a photo", () => {
  beforeEach(() => {
    jest.mocked(markStudentAppNewsSeen).mockResolvedValue();
    jest
      .mocked(getStudentAppNews)
      .mockResolvedValue(buildStudentAppNews({ items: [buildStudentAppNewsItem({ imageUrl })] }));
  });

  it("Then the photo is shown", async () => {
    await renderStudentAppScreen(<StudentAppNewsScreen />);

    expect((await screen.findByTestId("catalog-image")).props.source).toEqual({ uri: imageUrl });
  });
});
