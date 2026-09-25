import { formatBirthDateAsTyped, parseBirthDate } from "@/features/students/birthDateFormatting";

describe("When birth date is typed", () => {
  it("Then it is parsed to iso date", () => {
    const typedBirthDate = formatBirthDateAsTyped("14032018");

    expect(typedBirthDate).toBe("14/03/2018");
    expect(parseBirthDate(typedBirthDate)).toBe("2018-03-14");
  });
});
