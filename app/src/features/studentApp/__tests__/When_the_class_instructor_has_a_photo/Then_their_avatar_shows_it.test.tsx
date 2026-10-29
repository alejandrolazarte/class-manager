import { screen } from "@testing-library/react-native";
import { getStudentAppHome } from "@/features/studentApp/studentAppApi";
import { StudentAppClassScreen } from "@/features/studentApp/screens/StudentAppClassScreen";
import { translate } from "@/i18n/translate";
import { searchParametersMock } from "@/testing/expoRouterMock";
import {
  buildAccountStudent,
  buildStudentAppHome,
  buildStudentAppNextClass,
} from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

const nextClass = buildStudentAppNextClass({
  instructorPhotoUrl: "https://files.example.com/private/laura.webp?sig=abc",
});

describe("When the class instructor has a photo", () => {
  beforeEach(() => {
    searchParametersMock.current = {
      classGroupId: nextClass.classGroupId!,
      date: nextClass.date,
      startTime: nextClass.startTime,
    };
    jest
      .mocked(getStudentAppHome)
      .mockResolvedValue(
        buildStudentAppHome({ students: [buildAccountStudent({ nextClasses: [nextClass] })] }),
      );
  });

  it("Then their avatar shows it", async () => {
    await renderStudentAppScreen(<StudentAppClassScreen />);

    expect(await screen.findByText(translate("student.class.instructor"))).toBeOnTheScreen();
    expect(screen.getByTestId("avatar-photo", { includeHiddenElements: true })).toHaveProp(
      "source",
      {
        uri: nextClass.instructorPhotoUrl,
      },
    );
  });
});
