import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { notificationPreferenceQueryKeys } from "@/features/notifications/notificationPreferenceQueryKeys";
import {
  listMyNotificationPreferences,
  updateMyNotificationPreference,
} from "@/features/notifications/notificationPreferencesApi";
import {
  NotificationPreference,
  UpdateNotificationPreferenceRequest,
} from "@/features/notifications/notificationPreferenceTypes";

function withChannel(
  preferences: NotificationPreference[],
  { category, channel, isEnabled }: UpdateNotificationPreferenceRequest,
): NotificationPreference[] {
  return preferences.map((preference) =>
    preference.category === category
      ? {
          ...preference,
          channels: preference.channels.map((candidate) =>
            candidate.channel === channel ? { ...candidate, isEnabled } : candidate,
          ),
        }
      : preference,
  );
}

export function useNotificationPreferences() {
  return useQuery({
    queryKey: notificationPreferenceQueryKeys.mine,
    queryFn: listMyNotificationPreferences,
  });
}

export function useUpdateNotificationPreference(onFailure: () => void) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (request: UpdateNotificationPreferenceRequest) =>
      updateMyNotificationPreference(request),
    onMutate: async (request: UpdateNotificationPreferenceRequest) => {
      await queryClient.cancelQueries({ queryKey: notificationPreferenceQueryKeys.mine });
      const previous = queryClient.getQueryData<NotificationPreference[]>(
        notificationPreferenceQueryKeys.mine,
      );
      if (previous) {
        queryClient.setQueryData(
          notificationPreferenceQueryKeys.mine,
          withChannel(previous, request),
        );
      }
      return { previous };
    },
    onError: (_error, _request, context) => {
      if (context?.previous) {
        queryClient.setQueryData(notificationPreferenceQueryKeys.mine, context.previous);
      }
      onFailure();
    },
    onSuccess: (saved) => {
      queryClient.setQueryData<NotificationPreference[]>(
        notificationPreferenceQueryKeys.mine,
        (current) =>
          current?.map((preference) =>
            preference.category === saved.category ? saved : preference,
          ),
      );
    },
  });
}
