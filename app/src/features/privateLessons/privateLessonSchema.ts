import { z } from "zod";
import { classGroupLimits, isWholeNumberBetween } from "@/features/classGroups/classGroupSchema";
import {
  minutesPerDay,
  startTimePattern,
  toMinutesOfDay,
} from "@/features/classGroups/startTimeFormatting";
import {
  PrivateLesson,
  PrivateLessonDetails,
  SchedulePrivateLessonRequest,
} from "@/features/privateLessons/types";
import { formatBirthDateForDisplay, parseBirthDate } from "@/features/students/birthDateFormatting";
import { translate } from "@/i18n/translate";

export const privateLessonLimits = {
  minimumStudents: 1,
  maximumStudents: 4,
  minimumRepeatWeeks: 1,
  maximumRepeatWeeks: 52,
  notesMaximumLength: 500,
} as const;

const defaultDurationMinutes = "45";

const lessonStudentSchema = z.object({ id: z.string(), fullName: z.string() });

export type LessonStudent = z.infer<typeof lessonStudentSchema>;

export const privateLessonSchema = z
  .object({
    students: z
      .array(lessonStudentSchema)
      .min(
        privateLessonLimits.minimumStudents,
        translate("privateLessons.validation.studentsRequired"),
      )
      .max(
        privateLessonLimits.maximumStudents,
        translate("privateLessons.validation.tooManyStudents"),
      ),
    instructorId: z.string().min(1, translate("privateLessons.validation.coachRequired")),
    date: z
      .string()
      .refine(
        (typedDate) => parseBirthDate(typedDate) !== null,
        translate("privateLessons.validation.dateInvalid"),
      ),
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
    location: z
      .string()
      .max(
        classGroupLimits.locationMaximumLength,
        translate("classGroups.validation.locationTooLong"),
      ),
    notes: z
      .string()
      .max(privateLessonLimits.notesMaximumLength, translate("students.validation.notesTooLong")),
    repeatWeeks: z.string(),
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

export type PrivateLessonFormValues = z.input<typeof privateLessonSchema>;

export const privateLessonFieldNames = [
  "instructorId",
  "date",
  "startTime",
  "durationMinutes",
  "location",
  "notes",
  "repeatWeeks",
] as const satisfies readonly (keyof PrivateLessonFormValues)[];

export function toPrivateLessonFormValues({
  lesson,
  initialDate,
  defaultInstructorId,
}: {
  lesson?: PrivateLesson;
  initialDate: string;
  defaultInstructorId?: string;
}): PrivateLessonFormValues {
  if (lesson !== undefined) {
    return {
      students: lesson.students.map((student) => ({
        id: student.studentId,
        fullName: student.studentFullName,
      })),
      instructorId: lesson.instructorId,
      date: formatBirthDateForDisplay(lesson.date),
      startTime: lesson.startTime,
      durationMinutes: String(lesson.durationMinutes),
      location: lesson.location ?? "",
      notes: lesson.notes ?? "",
      repeatWeeks: String(privateLessonLimits.minimumRepeatWeeks),
    };
  }
  return {
    students: [],
    instructorId: defaultInstructorId ?? "",
    date: formatBirthDateForDisplay(initialDate),
    startTime: "",
    durationMinutes: defaultDurationMinutes,
    location: "",
    notes: "",
    repeatWeeks: String(privateLessonLimits.minimumRepeatWeeks),
  };
}

function toOptionalText(text: string): string | null {
  const trimmedText = text.trim();
  return trimmedText.length > 0 ? trimmedText : null;
}

export function toPrivateLessonDetails(formValues: PrivateLessonFormValues): PrivateLessonDetails {
  return {
    instructorId: formValues.instructorId,
    date: parseBirthDate(formValues.date) ?? "",
    startTime: formValues.startTime.trim(),
    durationMinutes: Number(formValues.durationMinutes),
    location: toOptionalText(formValues.location),
    notes: toOptionalText(formValues.notes),
  };
}

export function toSchedulePrivateLessonRequest(
  formValues: PrivateLessonFormValues,
): SchedulePrivateLessonRequest {
  return {
    ...toPrivateLessonDetails(formValues),
    studentIds: formValues.students.map((student) => student.id),
    repeatWeeks: Number(formValues.repeatWeeks) || privateLessonLimits.minimumRepeatWeeks,
  };
}
