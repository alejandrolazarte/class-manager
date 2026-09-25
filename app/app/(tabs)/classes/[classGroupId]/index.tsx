import { useLocalSearchParams } from "expo-router";
import { ClassGroupDetailScreen } from "@/features/enrollments/screens/ClassGroupDetailScreen";

export default function ClassGroupDetailRoute() {
  const { classGroupId } = useLocalSearchParams<{ classGroupId: string }>();
  return <ClassGroupDetailScreen classGroupId={classGroupId} />;
}
