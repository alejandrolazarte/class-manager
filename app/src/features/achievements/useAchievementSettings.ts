import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { achievementQueryKeys } from "@/features/achievements/achievementQueryKeys";
import {
  getAchievementSettings,
  updateAchievementSettings,
} from "@/features/achievements/achievementsApi";
import { AchievementSettings } from "@/features/achievements/types";

export function useAchievementSettings() {
  return useQuery({ queryKey: achievementQueryKeys.settings(), queryFn: getAchievementSettings });
}

export function useUpdateAchievementSettings() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (settings: AchievementSettings) => updateAchievementSettings(settings),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: achievementQueryKeys.all }),
  });
}
