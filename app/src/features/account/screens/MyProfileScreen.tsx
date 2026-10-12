import { Href, useRouter } from "expo-router";
import { Pressable, View } from "react-native";
import { formatBirthDateForDisplay } from "@/features/students/birthDateFormatting";
import { useMyAccount } from "@/features/account/useMyAccount";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Avatar } from "@/ui/Avatar";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { HairlineRow } from "@/ui/HairlineRow";
import { Icon } from "@/ui/Icon";
import { ListRow } from "@/ui/ListRow";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SectionTitle } from "@/ui/SectionTitle";
import { SkeletonDetail } from "@/ui/motion/SkeletonDetail";

interface MyProfileScreenProps {
  editProfileRoute: Href;
  changeEmailRoute: Href;
}

export function MyProfileScreen({ editProfileRoute, changeEmailRoute }: MyProfileScreenProps) {
  const router = useRouter();
  const accountQuery = useMyAccount();
  const account = accountQuery.data;

  return (
    <ScrollScreen
      header={
        <ScreenHeader
          navigation="back"
          title={translate("profile.title")}
          navigationAction={
            <Pressable
              accessibilityRole="button"
              accessibilityLabel={translate("profile.editAccessibility")}
              onPress={() => router.push(editProfileRoute)}
              className="flex-row items-center gap-1 rounded-full px-3 py-2.5 active:bg-muted"
            >
              <Icon name="edit" size="medium" tone="primary" />
              <AppText variant="bodyStrong" tone="primary">
                {translate("profile.edit")}
              </AppText>
            </Pressable>
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
            <Avatar name={account.fullName} size="large" />
            <AppText variant="heading" className="text-center">
              {account.fullName}
            </AppText>
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
