const sheetCloseDragDistance = 110;
const sheetCloseFlickVelocity = 0.8;
const sheetMinimumFlickDistance = 20;
export const sheetDragActivationDistance = 4;
const sheetUpwardDragResistance = 4;
const sheetMaximumUpwardDrag = 20;

export function shouldCloseDraggedSheet(dragDistance: number, dragVelocity: number): boolean {
  if (dragDistance > sheetCloseDragDistance) {
    return true;
  }
  return dragVelocity > sheetCloseFlickVelocity && dragDistance > sheetMinimumFlickDistance;
}

export function resistedSheetDragDistance(dragDistance: number): number {
  if (dragDistance >= 0) {
    return dragDistance;
  }
  return Math.max(-sheetMaximumUpwardDrag, dragDistance / sheetUpwardDragResistance);
}
