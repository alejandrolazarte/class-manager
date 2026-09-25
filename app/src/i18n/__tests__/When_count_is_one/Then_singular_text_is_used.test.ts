import { translate, translateCount } from "@/i18n/translate";

describe("When count is one", () => {
  it("Then singular text is used", () => {
    expect(translateCount("sessions.attendance.absentCount", 1)).toBe(
      translate("sessions.attendance.absentCount.one", { count: 1 }),
    );
    expect(translateCount("sessions.attendance.absentCount", 2)).toBe(
      translate("sessions.attendance.absentCount.other", { count: 2 }),
    );
  });
});
