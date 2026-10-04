import { Redirect, useLocalSearchParams } from "expo-router";
import { routes } from "@/navigation/routes";

export default function LegacyStudentAppRoute() {
  const { legacyPath } = useLocalSearchParams<{ legacyPath?: string[] }>();
  const segments = legacyPath ?? [];
  return <Redirect href={[routes.studentApp, ...segments].join("/")} />;
}
