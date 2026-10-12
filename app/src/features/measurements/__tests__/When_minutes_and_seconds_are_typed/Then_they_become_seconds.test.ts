import { secondsFromTyped } from "@/features/measurements/measurementFormatting";

describe("When minutes and seconds are typed", () => {
  it("Then they become seconds", () => {
    expect(secondsFromTyped("1", "5,3")).toBe(65.3);
  });
});
