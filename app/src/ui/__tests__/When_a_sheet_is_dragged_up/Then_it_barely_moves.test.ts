import { resistedSheetDragDistance } from "@/ui/sheetDrag";

describe("When a sheet is dragged up", () => {
  it("Then it barely moves", () => {
    expect([resistedSheetDragDistance(-40), resistedSheetDragDistance(-400)]).toEqual([-10, -20]);
  });
});
