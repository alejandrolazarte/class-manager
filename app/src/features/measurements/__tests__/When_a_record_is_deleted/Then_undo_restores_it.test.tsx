import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { getClient } from "@/features/clients/clientsApi";
import {
  deleteMeasurementRecord,
  getStudentMeasurements,
  restoreMeasurementRecord,
} from "@/features/measurements/measurementsApi";
import { formatMeasurementDate } from "@/features/measurements/measurementFormatting";
import { StudentMeasurementScreen } from "@/features/measurements/screens/StudentMeasurementScreen";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import {
  buildMeasurementRecord,
  buildStudentMeasurement,
  buildTimeMeasurement,
} from "@/testing/measurementFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");
jest.mock("@/features/measurements/measurementsApi");

const measurement = buildTimeMeasurement();
const record = buildMeasurementRecord("2026-10-06", 29.7);

describe("When a record is deleted", () => {
  beforeEach(() => {
    jest.mocked(getClient).mockResolvedValue(buildClient());
    jest
      .mocked(getStudentMeasurements)
      .mockResolvedValue([
        buildStudentMeasurement(measurement, [buildMeasurementRecord("2026-04-07", 38.4), record]),
      ]);
    jest.mocked(deleteMeasurementRecord).mockResolvedValue(undefined);
    jest.mocked(restoreMeasurementRecord).mockResolvedValue(undefined);
  });

  it("Then undo restores it", async () => {
    await renderWithProviders(
      <StudentMeasurementScreen
        clientId="client-1"
        studentId="student-1"
        measurementId={measurement.id}
      />,
    );

    await fireEvent.press(
      await screen.findByRole("button", {
        name: translate("measurements.detail.deleteRecord", {
          date: formatMeasurementDate(record.measuredOn),
        }),
      }),
    );
    await fireEvent.press(await screen.findByRole("button", { name: translate("common.undo") }));

    await waitFor(() => expect(restoreMeasurementRecord).toHaveBeenCalledWith(record.id));
  });
});
