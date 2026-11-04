import { shouldCloseDraggedSheet } from "@/ui/sheetDrag";

describe("When a sheet is dragged down a little", () => {
  it("Then it stays open", () => {
    expect([shouldCloseDraggedSheet(40, 0.1), shouldCloseDraggedSheet(-60, -1.5)]).toEqual([
      false,
      false,
    ]);
  });
});
