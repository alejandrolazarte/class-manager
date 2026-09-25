import { summarizeWeekdays } from "@/features/enrollments/weekdaySummary";

describe("When weekdays are summarized", () => {
  it("Then they use short names in week order", () => {
    expect(summarizeWeekdays(["Friday", "Monday", "Wednesday"])).toBe("Lun, Mié y Vie");
  });
});
