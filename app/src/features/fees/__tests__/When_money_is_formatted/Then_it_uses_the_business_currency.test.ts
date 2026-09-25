import { formatMoney } from "@/features/fees/money";

describe("When money is formatted", () => {
  it("Then it uses the business currency", () => {
    expect(formatMoney(12000, "ARS")).toMatch(/^\$\s?12\.000$/);
  });
});
