import { Screen } from "@/ui/Screen";
import { SkeletonDetail } from "@/ui/motion/SkeletonDetail";

export function SkeletonScreen() {
  return (
    <Screen>
      <SkeletonDetail className="w-full max-w-2xl self-center px-5 pt-4" />
    </Screen>
  );
}
