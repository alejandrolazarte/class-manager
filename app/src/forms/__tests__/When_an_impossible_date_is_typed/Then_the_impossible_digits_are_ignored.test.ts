import { formatBirthDateAsTyped } from "@/features/students/birthDateFormatting";

describe("When an impossible date is typed", () => {
  it("Then the impossible digits are ignored", () => {
    expect(formatBirthDateAsTyped("3911219851")).toBe("31/12/1985");
  });
});
