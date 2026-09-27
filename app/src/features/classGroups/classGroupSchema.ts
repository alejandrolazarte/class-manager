import { z } from "zod";
import {
  minutesPerDay,
  startTimePattern,
  toMinutesOfDay,
} from "@/features/classGroups/startTimeFormatting";
import { ClassGroup, SaveClassGroupRequest, Weekday } from "@/features/classGroups/types";
import { sortWeekdays, weekOrder } from "@/features/classGroups/weekdays";
import { translate } from "@/i18n/translate";

export const classGroupLimits = {
  nameMinimumLength: 2,
  nameMaximumLength: 80,
  minimumDurationMinutes: 15,
  maximumDurationMinutes: 240,
  durationStepMinutes: 5,
  minimumCapacity: 1,
  maximumCapacity: 100,
  locationMaximumLength: 80,
} as const;

export const commonDurationsInMinutes = [30, 45, 60, 90] as const;

const wholeNumberPattern = /^\d+$/;

export function isWholeNumberBetween(value: string, minimum: number, maximum: number): boolean {
  const trimmedValue = value.trim();
  if (!wholeNumberPattern.test(trimmedValue)) {
    return false;
  }
  const number = Number(trimmedValue);
  return number >= minimum && number <= maximum;
}

export const classGroupSchema = z
  .object({
    name: z
      .string()
      .trim()
      .min(1, translate("classGroups.validation.nameRequired"))
      .min(classGroupLimits.nameMinimumLength, translate("classGroups.validation.nameTooShort"))
      .max(classGroupLimits.nameMaximumLength, translate("classGroups.validation.nameTooLong")),
    weekdays: z
      .array(z.enum(weekOrder as [Weekday, ...Weekday[]]))
      .min(1, translate("classGroups.validation.weekdaysRequired")),
    startTime: z
      .string()
      .trim()
      .regex(startTimePattern, translate("classGroups.validation.startTimeInvalid")),
    durationMinutes: z
      .string()
      .refine(
        (durationMinutes) =>
          isWholeNumberBetween(
            durationMinutes,
            classGroupLimits.minimumDurationMinutes,
            classGroupLimits.maximumDurationMinutes,
          ) && Number(durationMinutes) % classGroupLimits.durationStepMinutes === 0,
        translate("classGroups.validation.durationInvalid"),
      ),
    capacity: z
      .string()
      .refine(
        (capacity) =>
          isWholeNumberBetween(
            capacity,
            classGroupLimits.minimumCapacity,
            classGroupLimits.maximumCapacity,
          ),
        translate("classGroups.validation.capacityInvalid"),
      ),
    instructorId: z.string().min(1, translate("classGroups.validation.instructorRequired")),
    location: z
      .string()
      .max(
        classGroupLimits.locationMaximumLength,
        translate("classGroups.validation.locationTooLong"),
      ),
  })
  .refine(
    (formValues) =>
      !startTimePattern.test(formValues.startTime.trim()) ||
      toMinutesOfDay(formValues.startTime.trim()) + Number(formValues.durationMinutes) <=
        minutesPerDay,
    {
      path: ["durationMinutes"],
      message: translate("classGroups.validation.endsAfterMidnight"),
    },
  );

export type ClassGroupFormValues = z.input<typeof classGroupSchema>;

export const classGroupFieldNames = [
  "name",
  "weekdays",
  "startTime",
  "durationMinutes",
  "capacity",
  "instructorId",
  "location",
] as const satisfies readonly (keyof ClassGroupFormValues)[];

export function toClassGroupFormValues({
  classGroup,
  initialWeekday,
  defaultInstructorId,
}: {
  classGroup?: ClassGroup;
  initialWeekday?: Weekday;
  defaultInstructorId?: string;
}): ClassGroupFormValues {
  if (classGroup !== undefined) {
    return {
      name: classGroup.name,
      weekdays: classGroup.weekdays,
      startTime: classGroup.startTime,
      durationMinutes: String(classGroup.durationMinutes),
      capacity: String(classGroup.capacity),
      instructorId: classGroup.instructorId,
      location: classGroup.location ?? "",
    };
  }
  return {
    name: "",
    weekdays: initialWeekday ? [initialWeekday] : [],
    startTime: "",
    durationMinutes: "",
    capacity: "",
    instructorId: defaultInstructorId ?? "",
    location: "",
  };
}

export function toSaveClassGroupRequest(formValues: ClassGroupFormValues): SaveClassGroupRequest {
  const location = formValues.location.trim();
  return {
    name: formValues.name.trim(),
    instructorId: formValues.instructorId,
    weekdays: sortWeekdays(formValues.weekdays),
    startTime: formValues.startTime.trim(),
    durationMinutes: Number(formValues.durationMinutes),
    capacity: Number(formValues.capacity.trim()),
    location: location.length > 0 ? location : null,
  };
}
