import { useQuery } from "@tanstack/react-query";
import { PhotoApprovalCard } from "@/features/userPhotos/components/PhotoApprovalCard";
import { listPhotoApprovals } from "@/features/userPhotos/userPhotoApi";
import { userPhotoQueryKeys } from "@/features/userPhotos/userPhotoQueryKeys";

export function PhotoApprovalCards() {
  const approvals = useQuery({
    queryKey: userPhotoQueryKeys.approvals,
    queryFn: listPhotoApprovals,
  });
  return (approvals.data ?? []).map((approval) => (
    <PhotoApprovalCard key={approval.id} approval={approval} />
  ));
}
