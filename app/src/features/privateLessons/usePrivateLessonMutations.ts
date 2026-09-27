import { useMutation, useQueryClient } from "@tanstack/react-query";
import { classPackQueryKeys } from "@/features/classPacks/classPackQueryKeys";
import { privateLessonQueryKeys } from "@/features/privateLessons/privateLessonQueryKeys";
import {
  cancelPrivateLesson,
  deletePrivateLesson,
  recordPrivateLessonAttendance,
  reschedulePrivateLesson,
  restorePrivateLesson,
  schedulePrivateLesson,
} from "@/features/privateLessons/privateLessonsApi";
import {
  PrivateLesson,
  PrivateLessonDetails,
  SchedulePrivateLessonRequest,
} from "@/features/privateLessons/types";
import { sessionQueryKeys } from "@/features/sessions/sessionQueryKeys";
import { AttendanceStatus } from "@/features/sessions/types";

function useInvalidateAgenda() {
  const queryClient = useQueryClient();
  return () =>
    Promise.all([
      queryClient.invalidateQueries({ queryKey: sessionQueryKeys.all }),
      queryClient.invalidateQueries({ queryKey: privateLessonQueryKeys.all }),
      queryClient.invalidateQueries({ queryKey: classPackQueryKeys.all }),
    ]);
}

export function useSchedulePrivateLesson() {
  const invalidateAgenda = useInvalidateAgenda();
  return useMutation({
    mutationFn: (request: SchedulePrivateLessonRequest) => schedulePrivateLesson(request),
    onSuccess: invalidateAgenda,
  });
}

export function useReschedulePrivateLesson(privateLessonId: string) {
  const invalidateAgenda = useInvalidateAgenda();
  return useMutation({
    mutationFn: (details: PrivateLessonDetails) =>
      reschedulePrivateLesson(privateLessonId, details),
    onSuccess: invalidateAgenda,
  });
}

export function useCancelPrivateLesson(privateLessonId: string) {
  const invalidateAgenda = useInvalidateAgenda();
  return useMutation({
    mutationFn: (reason: string | null) => cancelPrivateLesson(privateLessonId, reason),
    onSuccess: invalidateAgenda,
  });
}

export function useRestorePrivateLesson(privateLessonId: string) {
  const invalidateAgenda = useInvalidateAgenda();
  return useMutation({
    mutationFn: () => restorePrivateLesson(privateLessonId),
    onSuccess: invalidateAgenda,
  });
}

export function useDeletePrivateLesson(privateLessonId: string) {
  const invalidateAgenda = useInvalidateAgenda();
  return useMutation({
    mutationFn: () => deletePrivateLesson(privateLessonId),
    onSuccess: invalidateAgenda,
  });
}

interface RecordAttendanceVariables {
  studentId: string;
  status: AttendanceStatus | null;
}

export function useRecordPrivateLessonAttendance(privateLessonId: string) {
  const queryClient = useQueryClient();
  const invalidateAgenda = useInvalidateAgenda();
  return useMutation({
    mutationFn: ({ studentId, status }: RecordAttendanceVariables) =>
      recordPrivateLessonAttendance(privateLessonId, studentId, status),
    onSuccess: async (_, { studentId, status }) => {
      queryClient.setQueryData<PrivateLesson>(
        privateLessonQueryKeys.lesson(privateLessonId),
        (lesson) =>
          lesson && {
            ...lesson,
            students: lesson.students.map((student) =>
              student.studentId === studentId ? { ...student, status } : student,
            ),
          },
      );
      await invalidateAgenda();
    },
  });
}
