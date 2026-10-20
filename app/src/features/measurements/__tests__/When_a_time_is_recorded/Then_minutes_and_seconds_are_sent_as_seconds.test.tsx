import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { getClient } from "@/features/clients/clientsApi";
import {
  getStudentMeasurements,
  listMeasurements,
  recordMeasurement,
} from "@/features/measurements/measurementsApi";
import { RecordMeasurementScreen } from "@/features/measurements/screens/RecordMeasurementScreen";
import { todayIsoDate } from "@/dates/dates";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import { buildMeasurementRecord, buildTimeMeasurement } from "@/testing/measurementFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildStudent } from "@/testing/studentFactory";

jest.mock("@/features/clients/clientsApi");
jest.mock("@/features/measurements/measurementsApi");

const student = buildStudent();
const measurement = buildTimeMeasurement();

describe("When a time is recorded", () => {
  beforeEach(() => {
    jest.mocked(getClient).mockResolvedValue(buildClient({ students: [student] }));
    jest.mocked(listMeasurements).mockResolvedValue([measurement]);
    jest.mocked(getStudentMeasurements).mockResolvedValue([]);
    jest.mocked(recordMeasurement).mockResolvedValue(buildMeasurementRecord(todayIsoDate(), 65));
  });

  it("Then minutes and seconds are sent as seconds", async () => {
    await renderWithProviders(
      <RecordMeasurementScreen clientId="client-1" studentId={student.id} />,
    );

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("measurements.recordForm.minutes") }),
    );
    await fireEvent.press(
      screen.getByRole("button", { name: `${translate("durationField.minutes")} 01` }),
    );
    await fireEvent.press(
      screen.getByRole("button", { name: `${translate("durationField.seconds")} 05` }),
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("dateField.done") }));
    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() =>
      expect(recordMeasurement).toHaveBeenCalledWith(student.id, measurement.id, {
        measuredOn: todayIsoDate(),
        value: 65,
        note: null,
      }),
    );
  });
});
