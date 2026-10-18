import { screen } from "@testing-library/react-native";
import {
  listClassMeasurementRecords,
  listMeasurements,
} from "@/features/measurements/measurementsApi";
import { SessionScreen } from "@/features/sessions/screens/SessionScreen";
import { getSession } from "@/features/sessions/sessionsApi";
import { translate, translateCount } from "@/i18n/translate";
import { buildDistanceMeasurement, buildMeasurementRecord } from "@/testing/measurementFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSessionDetails, buildSessionStudent, sessionDate } from "@/testing/sessionFactory";

jest.mock("@/features/measurements/measurementsApi");
jest.mock("@/features/sessions/sessionsApi");

const measurement = buildDistanceMeasurement();
const session = buildSessionDetails({
  students: [
    buildSessionStudent({ studentId: "student-1", studentFullName: "Tomás Pérez" }),
    buildSessionStudent({ studentId: "student-2", studentFullName: "Mía Fernández" }),
  ],
});

describe("When class measurements were recorded", () => {
  beforeEach(() => {
    jest.mocked(getSession).mockResolvedValue(session);
    jest.mocked(listMeasurements).mockResolvedValue([measurement]);
    jest.mocked(listClassMeasurementRecords).mockResolvedValue([
      buildMeasurementRecord(sessionDate, 400, {
        id: "record-1",
        studentId: "student-1",
        measurementId: measurement.id,
      }),
      buildMeasurementRecord(sessionDate, 350, {
        id: "record-2",
        studentId: "student-2",
        measurementId: measurement.id,
      }),
    ]);
  });

  it("Then the class shows them", async () => {
    await renderWithProviders(
      <SessionScreen classGroupId={session.classGroupId} sessionDate={sessionDate} />,
    );

    expect(
      await screen.findByText(translateCount("measurements.class.recordedStudents", 2)),
    ).toBeOnTheScreen();
    expect(screen.getByText(measurement.name)).toBeOnTheScreen();
    expect(
      screen.getByRole("button", { name: translate("measurements.class.change") }),
    ).toBeOnTheScreen();
  });
});
