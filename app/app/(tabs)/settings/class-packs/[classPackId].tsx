import { useLocalSearchParams } from "expo-router";
import { ClassPackFormScreen } from "@/features/classPacks/screens/ClassPackFormScreen";

export default function EditClassPackRoute() {
  const { classPackId } = useLocalSearchParams<{ classPackId: string }>();
  return <ClassPackFormScreen classPackId={classPackId} />;
}
