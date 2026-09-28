import { useLocalSearchParams } from "expo-router";
import { MemberScreen } from "@/features/members/screens/MemberScreen";

export default function MemberRoute() {
  const { memberId } = useLocalSearchParams<{ memberId: string }>();
  return <MemberScreen memberId={memberId} />;
}
