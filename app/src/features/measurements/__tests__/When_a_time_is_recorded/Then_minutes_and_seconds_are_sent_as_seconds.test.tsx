import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { getClient } from "@/features/clients/clientsApi";
import {
  getStudentMeasurements,
  listMeasurements,
  recordMeasurement,
} from "@/features/measurements/measurementsApi";
import { RecordMeasurementScreen } from "@/features/measurements/screens/RecordMeasurementScreen";
import { todayIsoDate } from "@/features/sessions/dates";
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
    jest.mocked(recordMeasurement).mockResolvedValue(buildMeasurementRecord(todayIsoDate(), 65.3));
  });

  it("Then minutes and seconds are sent as seconds", async () => {
    await renderWithProviders(
      <RecordMeasurementScreen clientId="client-1" studentId={student.id} />,
    );

    await fireEvent.changeText(
      await screen.findByLabelText(translate("measurements.recordForm.minutes")),
      "1",
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("measurements.recordForm.seconds")),
      "5,3",
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() =>
      expect(recordMeasurement).toHaveBeenCalledWith(student.id, measurement.id, {
        measuredOn: todayIsoDate(),
        value: 65.3,
        note: null,
      }),
    );
  });
});
