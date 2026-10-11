import { View } from "react-native";
import { SkeletonFrame } from "@/ui/motion/SkeletonFrame";

interface SkeletonDetailProps {
  blockCount?: number;
  className?: string;
}

const defaultBlockCount = 3;

export function SkeletonDetail({ blockCount = defaultBlockCount, className }: SkeletonDetailProps) {
  return (
    <SkeletonFrame className={`gap-4 ${className ?? ""}`}>
      <View className="gap-2.5 py-2">
        <View className="h-6 w-1/2 rounded-full bg-muted" />
        <View className="h-3 w-1/3 rounded-full bg-muted" />
      </View>
      {Array.from({ length: blockCount }, (_, blockIndex) => (
        <View
          key={blockIndex}
          className="gap-3 rounded-[14px] border border-border-subtle bg-surface p-4"
        >
          <View className="h-2.5 w-1/3 rounded-full bg-muted" />
          <View className="h-[46px] rounded-[10px] bg-muted" />
        </View>
      ))}
    </SkeletonFrame>
  );
}
