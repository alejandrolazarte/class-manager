import { measurementTrend } from "@/features/measurements/measurementFormatting";
import { buildMeasurementRecord } from "@/testing/measurementFactory";

describe("When a time goes down", () => {
  it("Then it is a personal best", () => {
    const trend = measurementTrend({ improvesWhen: "Decreases" }, [
      buildMeasurementRecord("2026-04-07", 38.4),
      buildMeasurementRecord("2026-10-06", 29.7),
    ]);

    expect(trend?.tone).toBe("good");
    expect(trend?.isPersonalBest).toBe(true);
  });
});
