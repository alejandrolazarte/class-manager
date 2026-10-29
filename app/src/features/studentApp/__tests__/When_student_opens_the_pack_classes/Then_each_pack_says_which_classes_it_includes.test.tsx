import { screen } from "@testing-library/react-native";
import { getStudentAppClassPacks, getStudentAppHome } from "@/features/studentApp/studentAppApi";
import { StudentAppClassPacksScreen } from "@/features/studentApp/screens/StudentAppClassPacksScreen";
import { translate } from "@/i18n/translate";
import {
  buildStudentAppClassPacks,
  buildStudentAppHome,
  buildStudentAppPackPurchase,
} from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

describe("When student opens the pack classes", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppHome).mockResolvedValue(buildStudentAppHome());
    jest.mocked(getStudentAppClassPacks).mockResolvedValue(
      buildStudentAppClassPacks({
        purchases: [buildStudentAppPackPurchase({ classGroupNames: ["Aquagym", "Nivel 2"] })],
      }),
    );
  });

  it("Then each pack says which classes it includes", async () => {
    await renderStudentAppScreen(<StudentAppClassPacksScreen />);

    expect(
      await screen.findByText(
        translate("student.classPacks.includes", { classes: "Aquagym, Nivel 2" }),
      ),
    ).toBeOnTheScreen();
  });
});
