import { formatMoney } from "@/formatting/money";

describe("When money is formatted", () => {
  it("Then pounds show the pound sign", () => {
    expect(formatMoney(40, "GBP")).toMatch(/^£\s?40$/);
  });
});
