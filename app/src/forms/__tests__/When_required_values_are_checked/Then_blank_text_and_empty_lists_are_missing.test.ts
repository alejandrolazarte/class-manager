import { areFilled, isFilled } from "@/forms/requiredFields";

describe("When required values are checked", () => {
  it("Then blank text and empty lists are missing", () => {
    expect([isFilled("  "), isFilled([]), isFilled(null), isFilled(undefined)]).toEqual([
      false,
      false,
      false,
      false,
    ]);
    expect(areFilled(["Ana", ["Monday"], 0, false])).toBe(true);
  });
});
