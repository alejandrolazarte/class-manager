import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import {
  getStudentMeasurements,
  listMeasurements,
  recordClassMeasurements,
} from "@/features/measurements/measurementsApi";
import { ClassTestDayScreen } from "@/features/measurements/screens/ClassTestDayScreen";
import { getSession } from "@/features/sessions/sessionsApi";
import { translate, translateCount } from "@/i18n/translate";
import { buildDistanceMeasurement } from "@/testing/measurementFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSessionDetails, buildSessionStudent, sessionDate } from "@/testing/sessionFactory";

jest.mock("@/features/measurements/measurementsApi");
jest.mock("@/features/sessions/sessionsApi");

const measurement = buildDistanceMeasurement();
const firstStudent = buildSessionStudent({
  studentId: "student-1",
  studentFullName: "Tomás Pérez",
});
const secondStudent = buildSessionStudent({
  studentId: "student-2",
  studentFullName: "Mía Fernández",
});

describe("When a test day is saved", () => {
  beforeEach(() => {
    jest
      .mocked(getSession)
      .mockResolvedValue(buildSessionDetails({ students: [firstStudent, secondStudent] }));
    jest.mocked(listMeasurements).mockResolvedValue([measurement]);
    jest.mocked(getStudentMeasurements).mockResolvedValue([]);
    jest.mocked(recordClassMeasurements).mockResolvedValue([]);
  });

  it("Then every typed value is sent", async () => {
    await renderWithProviders(
      <ClassTestDayScreen classGroupId="class-1" sessionDate={sessionDate} />,
    );

    await fireEvent.changeText(
      await screen.findByLabelText(
        translate("measurements.testDay.valueOf", {
          measurement: measurement.name,
          name: firstStudent.studentFullName,
        }),
      ),
      "29,7",
    );
    await fireEvent.changeText(
      screen.getByLabelText(
        translate("measurements.testDay.valueOf", {
          measurement: measurement.name,
          name: secondStudent.studentFullName,
        }),
      ),
      "12",
    );
    await fireEvent.press(
      screen.getByRole("button", { name: translateCount("measurements.testDay.save", 2) }),
    );

    await waitFor(() =>
      expect(recordClassMeasurements).toHaveBeenCalledWith(measurement.id, sessionDate, [
        { studentId: firstStudent.studentId, value: 29.7 },
        { studentId: secondStudent.studentId, value: 12 },
      ]),
    );
  });
});
