import { fireEvent, screen } from "@testing-library/react-native";
import { Linking } from "react-native";
import { getStudentAppHome } from "@/features/studentApp/studentAppApi";
import { StudentAppClassScreen } from "@/features/studentApp/screens/StudentAppClassScreen";
import { todayIsoDate } from "@/dates/dates";
import { translate } from "@/i18n/translate";
import { searchParametersMock } from "@/testing/expoRouterMock";
import {
  buildAccountStudent,
  buildStudentAppHome,
  buildStudentAppNextClass,
} from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

const classGroupId = "0192f0c5-0000-7000-8000-000000000010";
const className = "Grupo inicial";
const linkUrl = "https://example.com/video";
const fileUrl = "https://files.example.com/guia.pdf";
const today = todayIsoDate();

describe("When student class has a link and a pdf", () => {
  beforeEach(() => {
    jest.spyOn(Linking, "openURL").mockResolvedValue(true);
    searchParametersMock.current = { classGroupId, date: today, startTime: "18:00" };
    jest.mocked(getStudentAppHome).mockResolvedValue(
      buildStudentAppHome({
        students: [
          buildAccountStudent({
            nextClasses: [
              buildStudentAppNextClass({
                name: className,
                classGroupId,
                date: today,
                startTime: "18:00",
              }),
            ],
            materials: [
              { classGroupId, className, linkUrl, file: { url: fileUrl, sizeInBytes: 2_400_000 } },
            ],
          }),
        ],
      }),
    );
  });

  it("Then the student can open both", async () => {
    await renderStudentAppScreen(<StudentAppClassScreen />);

    await fireEvent.press(
      await screen.findByRole("button", {
        name: translate("student.materials.openLink", { className }),
      }),
    );
    await fireEvent.press(
      screen.getByRole("button", { name: translate("student.materials.openFile", { className }) }),
    );

    expect(jest.mocked(Linking.openURL).mock.calls).toEqual([[linkUrl], [fileUrl]]);
  });
});
