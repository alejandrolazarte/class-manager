import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { PickedFile } from "@/api/fileForm";
import { changeMyPhoto, getMyPhoto, removeMyPhoto } from "@/features/userPhotos/userPhotoApi";
import { userPhotoQueryKeys } from "@/features/userPhotos/userPhotoQueryKeys";

export function useMyPhoto() {
  return useQuery({ queryKey: userPhotoQueryKeys.myPhoto, queryFn: getMyPhoto });
}

export function useChangeMyPhoto() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (file: PickedFile) => changeMyPhoto(file),
    onSuccess: (photo) => queryClient.setQueryData(userPhotoQueryKeys.myPhoto, photo),
  });
}

export function useRemoveMyPhoto() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: () => removeMyPhoto(),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: userPhotoQueryKeys.myPhoto }),
  });
}
