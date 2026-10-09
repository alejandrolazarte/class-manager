import { formatStartTimeAsTyped } from "@/features/classGroups/startTimeFormatting";

describe("When an hour starts above two", () => {
  it("Then a leading zero is added", () => {
    expect(formatStartTimeAsTyped("930")).toBe("09:30");
  });
});
