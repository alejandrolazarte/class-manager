import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { createBranch, listBranches } from "@/features/branches/branchesApi";
import { branchQueryKeys } from "@/features/branches/branchQueryKeys";
import { CreateBranchRequest } from "@/features/branches/types";

export function useBranches() {
  return useQuery({ queryKey: branchQueryKeys.all, queryFn: listBranches });
}

export function useCreateBranch() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (request: CreateBranchRequest) => createBranch(request),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: branchQueryKeys.all }),
  });
}
