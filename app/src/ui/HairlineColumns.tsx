import { Children, PropsWithChildren } from "react";
import { View } from "react-native";

export function HairlineColumns({ children }: PropsWithChildren) {
  const columns = Children.toArray(children);
  return (
    <View className="flex-row border-b border-t border-border-subtle">
      {columns.map((column, index) => (
        <View
          key={index}
          className={`min-w-0 flex-1 gap-1 py-3.5 ${index === 0 ? "pr-3.5" : "border-l border-border-subtle pl-3.5"}`}
        >
          {column}
        </View>
      ))}
    </View>
  );
}
