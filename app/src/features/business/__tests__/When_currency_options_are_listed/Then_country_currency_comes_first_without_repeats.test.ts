import { currencyOptions } from "@/features/business/currencyOptions";

describe("When currency options are listed", () => {
  it("Then country currency comes first without repeats", () => {
    expect(currencyOptions("UYU", "UYU")).toEqual(["UYU", "USD", "EUR"]);
    expect(currencyOptions("USD", "EUR")).toEqual(["USD", "EUR"]);
  });
});
