import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { getFamilyNews, markFamilyNewsSeen } from "@/features/family/familyApi";
import { familyQueryKeys } from "@/features/family/familyQueryKeys";
import { FamilyNews } from "@/features/family/types";

export function useFamilyNews() {
  return useQuery({ queryKey: familyQueryKeys.news(), queryFn: getFamilyNews });
}

export function useMarkFamilyNewsSeen() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: markFamilyNewsSeen,
    onSuccess: () =>
      queryClient.setQueryData<FamilyNews>(
        familyQueryKeys.news(),
        (news) => news && { ...news, unreadCount: 0 },
      ),
  });
}
