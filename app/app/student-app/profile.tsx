import { MyProfileScreen } from "@/features/account/screens/MyProfileScreen";
import { routes } from "@/navigation/routes";

export default function StudentAppProfileRoute() {
  return <MyProfileScreen editProfileRoute={routes.studentAppEditProfile} />;
}
