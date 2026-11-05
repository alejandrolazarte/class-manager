import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { notificationPreferenceQueryKeys } from "@/features/notifications/notificationPreferenceQueryKeys";
import {
  getMyNotificationPreferences,
  updateMyNotificationPreference,
} from "@/features/notifications/notificationPreferencesApi";
import {
  NotificationPreference,
  NotificationPreferences,
  UpdateNotificationPreferenceRequest,
} from "@/features/notifications/notificationPreferenceTypes";

function withPreference(
  current: NotificationPreferences,
  change: (preference: NotificationPreference) => NotificationPreference,
): NotificationPreferences {
  return { ...current, preferences: current.preferences.map(change) };
}

function withChannel(
  current: NotificationPreferences,
  { category, channel, isEnabled }: UpdateNotificationPreferenceRequest,
): NotificationPreferences {
  return withPreference(current, (preference) =>
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
    queryFn: getMyNotificationPreferences,
  });
}

export function useUpdateNotificationPreference(onFailure: () => void) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (request: UpdateNotificationPreferenceRequest) =>
      updateMyNotificationPreference(request),
    onMutate: async (request: UpdateNotificationPreferenceRequest) => {
      await queryClient.cancelQueries({ queryKey: notificationPreferenceQueryKeys.mine });
      const previous = queryClient.getQueryData<NotificationPreferences>(
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
      queryClient.setQueryData<NotificationPreferences>(
        notificationPreferenceQueryKeys.mine,
        (current) =>
          current &&
          withPreference(current, (preference) =>
            preference.category === saved.category ? saved : preference,
          ),
      );
    },
  });
}
