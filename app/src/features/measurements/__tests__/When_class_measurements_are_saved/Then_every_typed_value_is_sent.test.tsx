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

describe("When class measurements are saved", () => {
  beforeEach(() => {
    jest
      .mocked(getSession)
      .mockResolvedValue(buildSessionDetails({ students: [firstStudent, secondStudent] }));
    jest.mocked(listMeasurements).mockResolvedValue([measurement]);
    jest.mocked(listClassMeasurementRecords).mockResolvedValue([]);
    jest.mocked(getStudentMeasurements).mockResolvedValue([]);
    jest.mocked(recordClassMeasurements).mockResolvedValue([]);
  });

  it("Then every typed value is sent", async () => {
    await renderWithProviders(
      <ClassMeasurementsScreen classGroupId="class-1" sessionDate={sessionDate} />,
    );

    await fireEvent.changeText(
      await screen.findByLabelText(
        translate("measurements.class.valueOf", {
          measurement: measurement.name,
          name: firstStudent.studentFullName,
        }),
      ),
      "29,7",
    );
    await fireEvent.changeText(
      screen.getByLabelText(
        translate("measurements.class.valueOf", {
          measurement: measurement.name,
          name: secondStudent.studentFullName,
        }),
      ),
      "12",
    );
    await fireEvent.press(
      screen.getByRole("button", { name: translateCount("measurements.class.save", 2) }),
    );

    await waitFor(() =>
      expect(recordClassMeasurements).toHaveBeenCalledWith(measurement.id, sessionDate, [
        { studentId: firstStudent.studentId, value: 29.7 },
        { studentId: secondStudent.studentId, value: 12 },
      ]),
    );
  });
});
