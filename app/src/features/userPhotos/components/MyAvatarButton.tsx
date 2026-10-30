import { Href, useRouter } from "expo-router";
import { useMyAccount } from "@/features/account/useMyAccount";
import { useMyPhoto } from "@/features/userPhotos/useMyPhoto";
import { translate } from "@/i18n/translate";
import { AvatarButton } from "@/ui/AvatarButton";

interface MyAvatarButtonProps {
  profileRoute: Href;
}

export function MyAvatarButton({ profileRoute }: MyAvatarButtonProps) {
  const router = useRouter();
  const account = useMyAccount().data;
  const photo = useMyPhoto().data;
  return account === undefined ? null : (
    <AvatarButton
      name={account.fullName}
      photoUrl={photo?.url}
      accessibilityLabel={translate("profile.title")}
      onPress={() => router.push(profileRoute)}
    />
  );
}
