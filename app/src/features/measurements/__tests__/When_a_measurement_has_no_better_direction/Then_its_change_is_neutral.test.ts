import { measurementTrend } from "@/features/measurements/measurementFormatting";
import { buildMeasurementRecord } from "@/testing/measurementFactory";

describe("When a measurement has no better direction", () => {
  it("Then its change is neutral", () => {
    const trend = measurementTrend({ improvesWhen: "Neither" }, [
      buildMeasurementRecord("2026-03-03", 132),
      buildMeasurementRecord("2026-09-01", 136),
    ]);

    expect(trend?.tone).toBe("neutral");
  });
});
