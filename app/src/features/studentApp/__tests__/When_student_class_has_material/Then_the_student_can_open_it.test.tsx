import { fireEvent, screen } from "@testing-library/react-native";
import { Linking } from "react-native";
import { getStudentAppHome } from "@/features/studentApp/studentAppApi";
import { StudentAppClassesScreen } from "@/features/studentApp/screens/StudentAppClassesScreen";
import { translate } from "@/i18n/translate";
import { buildAccountStudent, buildStudentAppHome } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

const materialUrl = "https://example.com/natacion-adultos.pdf";
const className = "Natación adultos";

describe("When student class has material", () => {
  beforeEach(() => {
    jest.spyOn(Linking, "openURL").mockResolvedValue(true);
    jest.mocked(getStudentAppHome).mockResolvedValue(
      buildStudentAppHome({
        students: [
          buildAccountStudent({
            materials: [
              { classGroupId: "0192f0c5-0000-7000-8000-000000000010", className, url: materialUrl },
            ],
          }),
        ],
      }),
    );
  });

  it("Then the student can open it", async () => {
    await renderStudentAppScreen(<StudentAppClassesScreen />);

    await fireEvent.press(
      await screen.findByRole("link", {
        name: translate("student.materials.open", { className }),
      }),
    );

    expect(Linking.openURL).toHaveBeenCalledWith(materialUrl);
  });
});
