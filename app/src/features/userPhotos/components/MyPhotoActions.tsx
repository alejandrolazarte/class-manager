import { useState } from "react";
import { View } from "react-native";
import { isApiError } from "@/api/apiErrors";
import { MyPhoto } from "@/features/userPhotos/types";
import { pickUserPhoto, userPhotoMaximumSizeInBytes } from "@/features/userPhotos/pickUserPhoto";
import { useChangeMyPhoto, useRemoveMyPhoto } from "@/features/userPhotos/useMyPhoto";
import { userPhotoErrorCodes } from "@/features/userPhotos/userPhotoErrorCodes";
import { TranslationKey, translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { ConfirmDialog } from "@/ui/ConfirmDialog";
import { useToast } from "@/ui/ToastProvider";

const photoErrorKeys: Record<string, TranslationKey> = {
  [userPhotoErrorCodes.unsupported]: "profile.photo.unsupported",
  [userPhotoErrorCodes.tooLarge]: "profile.photo.tooLarge",
};

function photoErrorMessage(photoError: unknown): string {
  const problemCode = isApiError(photoError) ? photoError.problem.code : undefined;
  const messageKey = problemCode === undefined ? undefined : photoErrorKeys[problemCode];
  return translate(messageKey ?? "common.unexpectedError");
}

function hintKeyOf(photo: MyPhoto): TranslationKey | null {
  if (photo.awaitsGuardian) {
    return "profile.photo.awaitsGuardian";
  }
  return photo.needsGuardian ? "profile.photo.needsGuardian" : null;
}

interface MyPhotoActionsProps {
  photo: MyPhoto;
}

export function MyPhotoActions({ photo }: MyPhotoActionsProps) {
  const { showToast } = useToast();
  const changeMutation = useChangeMyPhoto();
  const removeMutation = useRemoveMyPhoto();
  const [photoError, setPhotoError] = useState<string | null>(null);
  const [isConfirmingRemoval, setIsConfirmingRemoval] = useState(false);
  const hintKey = hintKeyOf(photo);

  const changePhoto = async () => {
    setPhotoError(null);
    const file = await pickUserPhoto();
    if (file === null) {
      return;
    }
    if (file.size !== undefined && file.size > userPhotoMaximumSizeInBytes) {
      setPhotoError(translate("profile.photo.tooLarge"));
      return;
    }
    try {
      const savedPhoto = await changeMutation.mutateAsync(file);
      showToast(
        translate(
          savedPhoto.awaitsGuardian ? "profile.photo.sentToGuardian" : "profile.photo.saved",
        ),
      );
    } catch (changeError) {
      setPhotoError(photoErrorMessage(changeError));
    }
  };

  const removePhoto = async () => {
    setPhotoError(null);
    try {
      await removeMutation.mutateAsync();
      setIsConfirmingRemoval(false);
      showToast(translate("profile.photo.removed"));
    } catch (removeError) {
      setIsConfirmingRemoval(false);
      setPhotoError(photoErrorMessage(removeError));
    }
  };

  return (
    <View className="items-center gap-2">
      <View className="flex-row flex-wrap justify-center gap-2">
        <Button
          size="small"
          variant="secondary"
          icon="upload"
          label={translate(photo.url === null ? "profile.photo.upload" : "profile.photo.change")}
          isLoading={changeMutation.isPending}
          onPress={() => void changePhoto()}
        />
        {photo.url === null ? null : (
          <Button
            size="small"
            variant="dangerGhost"
            label={translate("profile.photo.remove")}
            onPress={() => setIsConfirmingRemoval(true)}
          />
        )}
      </View>
      {photoError === null && hintKey === null ? null : (
        <AppText
          variant="caption"
          tone={photoError === null ? "subtle" : "danger"}
          className="text-center"
        >
          {photoError ?? (hintKey === null ? null : translate(hintKey))}
        </AppText>
      )}
      {isConfirmingRemoval ? (
        <ConfirmDialog
          title={translate("profile.photo.removeQuestion")}
          message={translate("profile.photo.removeMessage")}
          confirmLabel={translate("profile.photo.confirmRemove")}
          onCancel={() => setIsConfirmingRemoval(false)}
          onConfirm={() => void removePhoto()}
          isConfirming={removeMutation.isPending}
        />
      ) : null}
    </View>
  );
}
