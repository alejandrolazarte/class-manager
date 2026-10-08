import { screen } from "@testing-library/react-native";
import { getStudentAppHome, getStudentAppShop } from "@/features/studentApp/studentAppApi";
import { StudentAppHomeScreen } from "@/features/studentApp/screens/StudentAppHomeScreen";
import { translate } from "@/i18n/translate";
import {
  buildStudentAppHome,
  buildStudentAppShop,
  buildAccountStudent,
} from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

const comment = "Muy buen ritmo en la serie larga.";

describe("When instructor left a comment", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppShop).mockResolvedValue(buildStudentAppShop());
    jest.mocked(getStudentAppHome).mockResolvedValue(
      buildStudentAppHome({
        students: [
          buildAccountStudent({
            latestFeedback: {
              date: "2026-09-28",
              className: "Natación inicial",
              instructorFullName: "Martín Díaz",
              text: comment,
            },
          }),
        ],
      }),
    );
  });

  it("Then it is shown on home", async () => {
    await renderStudentAppScreen(<StudentAppHomeScreen />);

    expect(
      await screen.findByText(translate("student.feedback.title", { instructor: "Martín" })),
    ).toBeOnTheScreen();
    expect(screen.getByText(`“${comment}”`)).toBeOnTheScreen();
  });
});
