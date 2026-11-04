import { shouldCloseDraggedSheet } from "@/ui/sheetDrag";

describe("When a sheet is flicked down", () => {
  it("Then it closes", () => {
    expect(shouldCloseDraggedSheet(40, 1.2)).toBe(true);
  });
});
