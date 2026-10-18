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
import { buildTimeMeasurement } from "@/testing/measurementFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSessionDetails, buildSessionStudent, sessionDate } from "@/testing/sessionFactory";

jest.mock("@/features/measurements/measurementsApi");
jest.mock("@/features/sessions/sessionsApi");

const measurement = buildTimeMeasurement();
const student = buildSessionStudent({ studentId: "student-1", studentFullName: "Tomás Pérez" });

describe("When a class measurement time is picked", () => {
  beforeEach(() => {
    jest.mocked(getSession).mockResolvedValue(buildSessionDetails({ students: [student] }));
    jest.mocked(listMeasurements).mockResolvedValue([measurement]);
    jest.mocked(listClassMeasurementRecords).mockResolvedValue([]);
    jest.mocked(getStudentMeasurements).mockResolvedValue([]);
    jest.mocked(recordClassMeasurements).mockResolvedValue([]);
  });

  it("Then it is sent as seconds", async () => {
    await renderWithProviders(
      <ClassMeasurementsScreen classGroupId="class-1" sessionDate={sessionDate} />,
    );

    await fireEvent.press(
      await screen.findByRole("button", {
        name: translate("measurements.class.valueOf", {
          measurement: measurement.name,
          name: student.studentFullName,
        }),
      }),
    );
    await fireEvent.press(
      screen.getByRole("button", { name: `${translate("durationField.minutes")} 01` }),
    );
    await fireEvent.press(
      screen.getByRole("button", { name: `${translate("durationField.seconds")} 02` }),
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("dateField.done") }));
    await fireEvent.press(
      screen.getByRole("button", { name: translateCount("measurements.class.save", 1) }),
    );

    await waitFor(() =>
      expect(recordClassMeasurements).toHaveBeenCalledWith(measurement.id, sessionDate, [
        { studentId: student.studentId, value: 62 },
      ]),
    );
  });
});
