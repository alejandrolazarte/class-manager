import { Redirect } from "expo-router";
import { backofficeRoutes } from "@/backoffice/backofficeRoutes";

export default function BackofficeIndexRoute() {
  return <Redirect href={backofficeRoutes.signIn} />;
}
