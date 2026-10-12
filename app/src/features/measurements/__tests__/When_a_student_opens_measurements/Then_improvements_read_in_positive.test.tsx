import { fireEvent, screen } from "@testing-library/react-native";
import { getStudentAppMeasurements } from "@/features/measurements/measurementsApi";
import { getStudentAppHome } from "@/features/studentApp/studentAppApi";
import { StudentAppProgressScreen } from "@/features/studentApp/screens/StudentAppProgressScreen";
import { translate } from "@/i18n/translate";
import {
  buildMeasurementRecord,
  buildStudentMeasurement,
  buildTimeMeasurement,
} from "@/testing/measurementFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";
import { buildAccountStudent, buildStudentAppHome } from "@/testing/studentAppFactory";

jest.mock("@/features/measurements/measurementsApi");
jest.mock("@/features/studentApp/studentAppApi");

const student = buildAccountStudent();

describe("When a student opens measurements", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppHome).mockResolvedValue(buildStudentAppHome({ students: [student] }));
    jest.mocked(getStudentAppMeasurements).mockResolvedValue({
      students: [
        {
          studentId: student.id,
          fullName: student.fullName,
          measurements: [
            buildStudentMeasurement(buildTimeMeasurement(), [
              buildMeasurementRecord("2026-04-07", 38.4),
              buildMeasurementRecord("2026-10-06", 29.7),
            ]),
          ],
        },
      ],
    });
  });

  it("Then improvements read in positive", async () => {
    await renderStudentAppScreen(<StudentAppProgressScreen />);

    await fireEvent.press(
      await screen.findByRole("tab", { name: translate("student.progress.tabs.measurements") }),
    );

    expect(
      await screen.findByText(
        translate("student.measurements.less", { amount: "8,70 s", month: "abril" }),
      ),
    ).toBeOnTheScreen();
  });
});
