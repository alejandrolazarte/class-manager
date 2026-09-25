import { parseAmount } from "@/features/fees/money";

describe("When amount is typed with comma", () => {
  it("Then it is parsed as decimal", () => {
    expect(parseAmount("12.500,50")).toBe(12500.5);
    expect(parseAmount("7000,5")).toBe(7000.5);
    expect(parseAmount("12,345")).toBeNull();
  });
});
