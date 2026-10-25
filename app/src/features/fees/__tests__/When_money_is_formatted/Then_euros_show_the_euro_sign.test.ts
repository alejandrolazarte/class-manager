import { formatMoney } from "@/formatting/money";

describe("When money is formatted", () => {
  it("Then euros show the euro sign", () => {
    expect(formatMoney(40, "EUR")).toMatch(/^€\s?40$/);
  });
});
