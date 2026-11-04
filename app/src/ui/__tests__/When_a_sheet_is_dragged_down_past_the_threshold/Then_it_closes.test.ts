import { shouldCloseDraggedSheet } from "@/ui/sheetDrag";

describe("When a sheet is dragged down past the threshold", () => {
  it("Then it closes", () => {
    expect(shouldCloseDraggedSheet(140, 0.1)).toBe(true);
  });
});
