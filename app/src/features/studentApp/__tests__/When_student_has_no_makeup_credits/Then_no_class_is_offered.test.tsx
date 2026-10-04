import { screen } from "@testing-library/react-native";
import { getStudentAppHome, getStudentAppMakeups } from "@/features/studentApp/studentAppApi";
import { StudentAppClassesScreen } from "@/features/studentApp/screens/StudentAppClassesScreen";
import { todayIsoDate } from "@/features/sessions/dates";
import { translate } from "@/i18n/translate";
import {
  buildStudentAppHome,
  buildStudentAppMakeups,
  buildStudentAppMakeupSlot,
  buildAccountStudent,
} from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

const today = todayIsoDate();

describe("When student has no makeup credits", () => {
  beforeEach(() => {
    jest
      .mocked(getStudentAppHome)
      .mockResolvedValue(
        buildStudentAppHome({ students: [buildAccountStudent({ nextClasses: [] })] }),
      );
    jest.mocked(getStudentAppMakeups).mockResolvedValue(
      buildStudentAppMakeups({
        credits: [],
        slots: [buildStudentAppMakeupSlot({ date: today })],
      }),
    );
  });

  it("Then no class is offered", async () => {
    await renderStudentAppScreen(<StudentAppClassesScreen />);

    expect(await screen.findByText(translate("student.makeup.none"))).toBeOnTheScreen();
    expect(screen.queryByRole("button", { name: translate("student.makeup.book") })).toBeNull();
  });
});
