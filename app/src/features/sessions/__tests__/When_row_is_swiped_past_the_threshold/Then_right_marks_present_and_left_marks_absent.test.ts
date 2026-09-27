import { attendanceStatusForSwipe } from "@/features/sessions/attendanceSwipe";

describe("When row is swiped past the threshold", () => {
  it("Then right marks present and left marks absent", () => {
    expect([
      attendanceStatusForSwipe(120),
      attendanceStatusForSwipe(-120),
      attendanceStatusForSwipe(40),
    ]).toEqual(["Present", "Absent", null]);
  });
});
