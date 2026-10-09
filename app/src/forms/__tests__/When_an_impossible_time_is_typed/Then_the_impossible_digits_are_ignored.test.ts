import { formatStartTimeAsTyped } from "@/features/classGroups/startTimeFormatting";

describe("When an impossible time is typed", () => {
  it("Then the impossible digits are ignored", () => {
    expect(formatStartTimeAsTyped("2473")).toBe("23");
  });
});
