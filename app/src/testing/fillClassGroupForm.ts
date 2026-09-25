import { fireEvent, screen } from "@testing-library/react-native";
import { weekdayLongLabel } from "@/features/classGroups/weekdays";
import { Weekday } from "@/features/classGroups/types";
import { translate } from "@/i18n/translate";

export async function fillClassGroupForm({
  name = "Natación inicial",
  weekdays = ["Tuesday", "Thursday"],
  startTime = "1800",
  durationMinutes = 45,
  capacity = "8",
}: {
  name?: string;
  weekdays?: Weekday[];
  startTime?: string;
  durationMinutes?: number;
  capacity?: string;
} = {}): Promise<void> {
  await fireEvent.changeText(screen.getByLabelText(translate("classGroups.form.name")), name);
  for (const weekday of weekdays) {
    await fireEvent.press(screen.getByRole("button", { name: weekdayLongLabel(weekday) }));
  }
  await fireEvent.changeText(
    screen.getByLabelText(translate("classGroups.form.startTime")),
    startTime,
  );
  await fireEvent.press(
    screen.getByRole("button", {
      name: translate("classGroups.form.durationOption", { minutes: durationMinutes }),
    }),
  );
  await fireEvent.changeText(
    screen.getByLabelText(translate("classGroups.form.capacity")),
    capacity,
  );
}

export async function submitClassGroupForm(): Promise<void> {
  await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));
}
