export type DayMarker = "none" | "classes" | "pending" | "attended" | "missed";

export const dayMarkerClassNames: Record<DayMarker, string> = {
  none: "bg-transparent",
  classes: "bg-subtle-foreground",
  pending: "bg-warning",
  attended: "bg-success",
  missed: "bg-danger",
};
