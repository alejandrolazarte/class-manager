import { currencySymbol } from "@/formatting/money";

describe("When an amount field shows its currency", () => {
  it("Then it shows the currency sign", () => {
    expect(currencySymbol("EUR")).toBe("€");
  });
});
