import { formatMeasurementValue } from "@/features/measurements/measurementFormatting";

describe("When a time under a minute is shown", () => {
  it("Then it keeps two decimals", () => {
    expect(formatMeasurementValue({ kind: "Time" as const, unit: "s" }, 29.7)).toBe("29,70 s");
  });
});
