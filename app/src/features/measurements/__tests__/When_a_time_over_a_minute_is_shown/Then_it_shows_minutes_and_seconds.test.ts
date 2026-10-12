import { formatMeasurementValue } from "@/features/measurements/measurementFormatting";

describe("When a time over a minute is shown", () => {
  it("Then it shows minutes and seconds", () => {
    expect(formatMeasurementValue({ kind: "Time" as const, unit: "s" }, 105)).toBe("1:45");
  });
});
