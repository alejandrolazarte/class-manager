import { formatStartTimeAsTyped } from "@/features/classGroups/startTimeFormatting";

describe("When start time is typed", () => {
  it("Then it is formatted as hours and minutes", () => {
    expect(formatStartTimeAsTyped("1830")).toBe("18:30");
  });
});
