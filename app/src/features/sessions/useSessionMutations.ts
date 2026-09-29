import { useMutation, useQueryClient } from "@tanstack/react-query";
import { sessionQueryKeys } from "@/features/sessions/sessionQueryKeys";
import {
  assignSubstitute,
  cancelSession,
  recordAttendance,
  removeSubstitute,
  rescheduleSession,
  restoreSession,
  restoreSessionSchedule,
} from "@/features/sessions/sessionsApi";
import { AttendanceStatus, SessionDetails } from "@/features/sessions/types";

interface RecordAttendanceVariables {
  studentId: string;
  status: AttendanceStatus | null;
}

export function useRecordAttendance(classGroupId: string, sessionDate: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ studentId, status }: RecordAttendanceVariables) =>
      recordAttendance(classGroupId, sessionDate, studentId, status),
    onSuccess: async (_, { studentId, status }) => {
      queryClient.setQueryData<SessionDetails>(
        sessionQueryKeys.session(classGroupId, sessionDate),
        (session) =>
          session && {
            ...session,
            students: session.students.map((student) =>
              student.studentId === studentId ? { ...student, status } : student,
            ),
          },
      );
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: sessionQueryKeys.day(sessionDate) }),
        queryClient.invalidateQueries({ queryKey: sessionQueryKeys.calendars() }),
      ]);
    },
  });
}

function useInvalidateSession(classGroupId: string, sessionDate: string) {
  const queryClient = useQueryClient();
  return () =>
    Promise.all([
      queryClient.invalidateQueries({
        queryKey: sessionQueryKeys.session(classGroupId, sessionDate),
      }),
      queryClient.invalidateQueries({ queryKey: sessionQueryKeys.day(sessionDate) }),
      queryClient.invalidateQueries({ queryKey: sessionQueryKeys.calendars() }),
    ]);
}

export function useCancelSession(classGroupId: string, sessionDate: string) {
  const invalidateSession = useInvalidateSession(classGroupId, sessionDate);
  return useMutation({
    mutationFn: (reason: string | null) => cancelSession(classGroupId, sessionDate, reason),
    onSuccess: invalidateSession,
  });
}

export function useRestoreSession(classGroupId: string, sessionDate: string) {
  const invalidateSession = useInvalidateSession(classGroupId, sessionDate);
  return useMutation({
    mutationFn: () => restoreSession(classGroupId, sessionDate),
    onSuccess: invalidateSession,
  });
}

export function useRescheduleSession(classGroupId: string, sessionDate: string) {
  const invalidateSession = useInvalidateSession(classGroupId, sessionDate);
  return useMutation({
    mutationFn: (startTime: string) => rescheduleSession(classGroupId, sessionDate, startTime),
    onSuccess: invalidateSession,
  });
}

export function useRestoreSessionSchedule(classGroupId: string, sessionDate: string) {
  const invalidateSession = useInvalidateSession(classGroupId, sessionDate);
  return useMutation({
    mutationFn: () => restoreSessionSchedule(classGroupId, sessionDate),
    onSuccess: invalidateSession,
  });
}

export function useAssignSubstitute(classGroupId: string, sessionDate: string) {
  const invalidateSession = useInvalidateSession(classGroupId, sessionDate);
  return useMutation({
    mutationFn: (instructorId: string) => assignSubstitute(classGroupId, sessionDate, instructorId),
    onSuccess: invalidateSession,
  });
}

export function useRemoveSubstitute(classGroupId: string, sessionDate: string) {
  const invalidateSession = useInvalidateSession(classGroupId, sessionDate);
  return useMutation({
    mutationFn: () => removeSubstitute(classGroupId, sessionDate),
    onSuccess: invalidateSession,
  });
}
