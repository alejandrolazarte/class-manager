import { MyProfileScreen } from "@/features/account/screens/MyProfileScreen";
import { routes } from "@/navigation/routes";

export default function MyProfileRoute() {
  return <MyProfileScreen editProfileRoute={routes.editMyProfile} />;
}
