import { useLocalSearchParams } from "expo-router";
import { ClassGroupFormScreen } from "@/features/classGroups/screens/ClassGroupFormScreen";

export default function EditClassGroupRoute() {
  const { classGroupId } = useLocalSearchParams<{ classGroupId: string }>();
  return <ClassGroupFormScreen classGroupId={classGroupId} />;
}
