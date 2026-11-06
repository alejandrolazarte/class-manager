import { MyProfileScreen } from "@/features/account/screens/MyProfileScreen";
import { routes } from "@/navigation/routes";

export default function TodayMyProfileRoute() {
  return (
    <MyProfileScreen
      editProfileRoute={routes.todayEditMyProfile}
      changeEmailRoute={routes.todayChangeMyEmail}
      deleteAccountRoute={routes.todayDeleteMyAccount}
      passkeysRoute={routes.todayMyPasskeys}
    />
  );
}
