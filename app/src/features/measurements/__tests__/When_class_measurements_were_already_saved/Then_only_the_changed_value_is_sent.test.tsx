import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import {
  getStudentMeasurements,
  listClassMeasurementRecords,
  listMeasurements,
  recordClassMeasurements,
} from "@/features/measurements/measurementsApi";
import { ClassMeasurementsScreen } from "@/features/measurements/screens/ClassMeasurementsScreen";
import { getSession } from "@/features/sessions/sessionsApi";
import { translate, translateCount } from "@/i18n/translate";
import { buildDistanceMeasurement, buildMeasurementRecord } from "@/testing/measurementFactory";
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

function valueLabelOf(studentFullName: string): string {
  return translate("measurements.class.valueOf", {
    measurement: measurement.name,
    name: studentFullName,
  });
}

describe("When class measurements were already saved", () => {
  beforeEach(() => {
    jest
      .mocked(getSession)
      .mockResolvedValue(buildSessionDetails({ students: [firstStudent, secondStudent] }));
    jest.mocked(listMeasurements).mockResolvedValue([measurement]);
    jest.mocked(listClassMeasurementRecords).mockResolvedValue([
      buildMeasurementRecord(sessionDate, 400, {
        id: "record-1",
        studentId: firstStudent.studentId,
        measurementId: measurement.id,
      }),
      buildMeasurementRecord(sessionDate, 350.5, {
        id: "record-2",
        studentId: secondStudent.studentId,
        measurementId: measurement.id,
      }),
    ]);
    jest.mocked(getStudentMeasurements).mockResolvedValue([]);
    jest.mocked(recordClassMeasurements).mockResolvedValue([]);
  });

  it("Then only the changed value is sent", async () => {
    await renderWithProviders(
      <ClassMeasurementsScreen classGroupId="class-1" sessionDate={sessionDate} />,
    );

    expect(
      (await screen.findByLabelText(valueLabelOf(secondStudent.studentFullName))).props.value,
    ).toBe("350,5");
    await fireEvent.changeText(
      screen.getByLabelText(valueLabelOf(firstStudent.studentFullName)),
      "425",
    );
    await fireEvent.press(
      screen.getByRole("button", { name: translateCount("measurements.class.save", 1) }),
    );

    await waitFor(() =>
      expect(recordClassMeasurements).toHaveBeenCalledWith(measurement.id, sessionDate, [
        { studentId: firstStudent.studentId, value: 425 },
      ]),
    );
  });
});
