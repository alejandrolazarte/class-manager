import { View } from "react-native";
import { SkeletonFrame } from "@/ui/motion/SkeletonFrame";

interface SkeletonListProps {
  rowCount?: number;
  className?: string;
}

const defaultRowCount = 4;

export function SkeletonList({ rowCount = defaultRowCount, className }: SkeletonListProps) {
  return (
    <SkeletonFrame className={`gap-1 ${className ?? ""}`}>
      {Array.from({ length: rowCount }, (_, rowIndex) => (
        <View
          key={rowIndex}
          className="flex-row items-center gap-3 border-b border-border-subtle py-3.5"
        >
          <View className="h-9 w-9 rounded-full bg-muted" />
          <View className="flex-1 gap-2">
            <View className="h-2.5 w-3/4 rounded-full bg-muted" />
            <View className="h-2 w-2/5 rounded-full bg-muted" />
          </View>
        </View>
      ))}
    </SkeletonFrame>
  );
}
