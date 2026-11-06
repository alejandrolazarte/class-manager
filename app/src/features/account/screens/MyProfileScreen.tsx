import { Href, useRouter } from "expo-router";
import { View } from "react-native";
import { formatBirthDateForDisplay } from "@/dates/birthDateFormatting";
import { useMyAccount } from "@/features/account/useMyAccount";
import { useMyPasskeys } from "@/features/passkeys/useMyPasskeys";
import { usePasskeySupport } from "@/features/passkeys/usePasskeySupport";
import { LegalLinks } from "@/features/legal/components/LegalLinks";
import { MyPhotoActions } from "@/features/userPhotos/components/MyPhotoActions";
import { useMyPhoto } from "@/features/userPhotos/useMyPhoto";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Avatar } from "@/ui/Avatar";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { HairlineRow } from "@/ui/HairlineRow";
import { ListRow } from "@/ui/ListRow";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SectionTitle } from "@/ui/SectionTitle";
import { SkeletonDetail } from "@/ui/motion/SkeletonDetail";

interface MyProfileScreenProps {
  editProfileRoute: Href;
  changeEmailRoute: Href;
  deleteAccountRoute: Href;
  passkeysRoute: Href;
}

export function MyProfileScreen({
  editProfileRoute,
  changeEmailRoute,
  deleteAccountRoute,
  passkeysRoute,
}: MyProfileScreenProps) {
  const router = useRouter();
  const isPasskeySupported = usePasskeySupport();
  const passkeyCount = useMyPasskeys(isPasskeySupported).data?.length ?? 0;
  const accountQuery = useMyAccount();
  const account = accountQuery.data;
  const photo = useMyPhoto().data;

  return (
    <ScrollScreen
      footer={
        <View className="pb-3">
          <LegalLinks />
        </View>
      }
      header={
        <ScreenHeader
          navigation="back"
          title={translate("profile.title")}
          navigationAction={
            <Button
              variant="ghost"
              size="medium"
              icon="edit"
              label={translate("profile.edit")}
              accessibilityLabel={translate("profile.editAccessibility")}
              onPress={() => router.push(editProfileRoute)}
            />
          }
        />
      }
    >
      {accountQuery.isPending ? <SkeletonDetail /> : null}
      {accountQuery.isError ? (
        <Banner message={translate("common.unexpectedError")}>
          <Button
            variant="secondary"
            size="medium"
            label={translate("common.retry")}
            onPress={() => accountQuery.refetch()}
          />
        </Banner>
      ) : null}
      {account ? (
        <>
          <View className="items-center gap-2 py-2">
            <Avatar name={account.fullName} size="large" photoUrl={photo?.url} />
            <AppText variant="heading" className="text-center">
              {account.fullName}
            </AppText>
            {photo === undefined ? null : <MyPhotoActions photo={photo} />}
          </View>
          <View className="gap-0.5">
            <SectionTitle title={translate("profile.personalData")} isOverline />
            <ProfileValueRow
              label={translate("profile.name.fullName")}
              value={account.fullName}
              hasDivider={Boolean(account.birthDate)}
            />
            {account.birthDate ? (
              <ProfileValueRow
                label={translate("birthDate.label")}
                value={formatBirthDateForDisplay(account.birthDate)}
                hasDivider={false}
              />
            ) : null}
          </View>
          <View className="gap-0.5">
            <SectionTitle title={translate("profile.access")} isOverline />
            <ListRow
              icon="email"
              label={translate("profile.email.title")}
              detail={translate("profile.signsInWith", { email: account.email })}
              onPress={() => router.push(changeEmailRoute)}
            />
            {isPasskeySupported ? (
              <ListRow
                icon="passkey"
                iconTone={passkeyCount > 0 ? "success" : undefined}
                label={translate("profile.passkeys.title")}
                detail={
                  passkeyCount > 0
                    ? translateCount("profile.passkeys.configured", passkeyCount)
                    : translate("profile.passkeys.detail")
                }
                onPress={() => router.push(passkeysRoute)}
              />
            ) : null}
            <ListRow
              icon="delete"
              iconTone="danger"
              label={translate("profile.delete.title")}
              isDestructive
              onPress={() => router.push(deleteAccountRoute)}
            />
          </View>
        </>
      ) : null}
    </ScrollScreen>
  );
}

interface ProfileValueRowProps {
  label: string;
  value: string;
  hasDivider: boolean;
}

function ProfileValueRow({ label, value, hasDivider }: ProfileValueRowProps) {
  return (
    <HairlineRow hasDivider={hasDivider} className="gap-0.5 py-3">
      <AppText variant="caption" tone="subtle">
        {label}
      </AppText>
      <AppText variant="body">{value}</AppText>
    </HairlineRow>
  );
}
