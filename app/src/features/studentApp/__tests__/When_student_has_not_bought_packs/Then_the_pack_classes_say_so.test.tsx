import { screen } from "@testing-library/react-native";
import { getStudentAppClassPacks, getStudentAppHome } from "@/features/studentApp/studentAppApi";
import { StudentAppClassPacksScreen } from "@/features/studentApp/screens/StudentAppClassPacksScreen";
import { translate } from "@/i18n/translate";
import { buildStudentAppClassPacks, buildStudentAppHome } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

describe("When student has not bought packs", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppHome).mockResolvedValue(buildStudentAppHome());
    jest
      .mocked(getStudentAppClassPacks)
      .mockResolvedValue(buildStudentAppClassPacks({ purchases: [] }));
  });

  it("Then the pack classes say so", async () => {
    await renderStudentAppScreen(<StudentAppClassPacksScreen />);

    expect(await screen.findByText(translate("student.classPacks.empty"))).toBeOnTheScreen();
  });
});
