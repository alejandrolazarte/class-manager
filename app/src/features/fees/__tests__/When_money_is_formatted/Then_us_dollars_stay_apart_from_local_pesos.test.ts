import { formatMoney } from "@/formatting/money";

describe("When money is formatted", () => {
  it("Then us dollars stay apart from local pesos", () => {
    expect(formatMoney(40, "USD")).toMatch(/^US\$\s?40$/);
  });
});
