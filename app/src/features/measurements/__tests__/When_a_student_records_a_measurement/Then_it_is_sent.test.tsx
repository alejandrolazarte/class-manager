import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import {
  getStudentAppMeasurements,
  recordStudentAppMeasurement,
} from "@/features/measurements/measurementsApi";
import { StudentAppRecordMeasurementScreen } from "@/features/measurements/screens/StudentAppRecordMeasurementScreen";
import { todayIsoDate } from "@/dates/dates";
import { translate } from "@/i18n/translate";
import {
  buildDistanceMeasurement,
  buildMeasurementRecord,
  buildStudentMeasurement,
  buildTimeMeasurement,
} from "@/testing/measurementFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/measurements/measurementsApi");

const recordable = buildDistanceMeasurement();

describe("When a student records a measurement", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppMeasurements).mockResolvedValue({
      students: [
        {
          studentId: "student-1",
          fullName: "Tomás Pérez",
          measurements: [
            buildStudentMeasurement(buildTimeMeasurement(), []),
            buildStudentMeasurement(recordable, []),
          ],
        },
      ],
    });
    jest
      .mocked(recordStudentAppMeasurement)
      .mockResolvedValue(buildMeasurementRecord(todayIsoDate(), 400, { recordedByStudent: true }));
  });

  it("Then it is sent", async () => {
    await renderStudentAppScreen(<StudentAppRecordMeasurementScreen studentId="student-1" />);

    expect(
      screen.queryByRole("button", { name: buildTimeMeasurement().name }),
    ).not.toBeOnTheScreen();
    await fireEvent.changeText(
      await screen.findByLabelText(
        translate("measurements.recordForm.value", { unit: recordable.unit }),
      ),
      "400",
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() =>
      expect(recordStudentAppMeasurement).toHaveBeenCalledWith("student-1", recordable.id, {
        measuredOn: todayIsoDate(),
        value: 400,
        note: null,
      }),
    );
  });
});
