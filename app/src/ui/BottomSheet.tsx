import { PropsWithChildren, ReactNode, useMemo, useState } from "react";
import {
  Animated,
  Modal,
  PanResponder,
  Pressable,
  ScrollView,
  View,
  useWindowDimensions,
} from "react-native";
import { SafeAreaView } from "react-native-safe-area-context";
import { translate } from "@/i18n/translate";
import { useCloseOnBack } from "@/navigation/useCloseOnBack";
import { ThemeScope } from "@/theme/ThemeScope";
import { Entrance } from "@/ui/motion/Entrance";
import { motionDurations } from "@/ui/motion/motion";
import { useReducedMotion } from "@/ui/motion/useReducedMotion";
import {
  resistedSheetDragDistance,
  sheetDragActivationDistance,
  shouldCloseDraggedSheet,
} from "@/ui/sheetDrag";

interface BottomSheetProps extends PropsWithChildren {
  onClose: () => void;
  header?: ReactNode;
}

const backdropFadeDistanceRatio = 0.6;

export function BottomSheet({ onClose, header, children }: BottomSheetProps) {
  useCloseOnBack(onClose);
  const isReducedMotion = useReducedMotion();
  const { height: windowHeight } = useWindowDimensions();
  const [dragOffset] = useState(() => new Animated.Value(0));

  const dragHandlers = useMemo(() => {
    const settleOpen = () =>
      Animated.spring(dragOffset, { toValue: 0, useNativeDriver: true }).start();
    const closeAfterDrag = () => {
      if (isReducedMotion) {
        onClose();
        return;
      }
      Animated.timing(dragOffset, {
        toValue: windowHeight,
        duration: motionDurations.sheet,
        useNativeDriver: true,
      }).start(() => onClose());
    };
    return PanResponder.create({
      onStartShouldSetPanResponder: () => true,
      onMoveShouldSetPanResponder: (_, gesture) =>
        Math.abs(gesture.dy) > sheetDragActivationDistance,
      onPanResponderTerminationRequest: () => false,
      onPanResponderMove: (_, gesture) =>
        dragOffset.setValue(resistedSheetDragDistance(gesture.dy)),
      onPanResponderRelease: (_, gesture) => {
        if (shouldCloseDraggedSheet(gesture.dy, gesture.vy)) {
          closeAfterDrag();
          return;
        }
        settleOpen();
      },
      onPanResponderTerminate: settleOpen,
    }).panHandlers;
  }, [dragOffset, isReducedMotion, onClose, windowHeight]);

  const backdropOpacity = dragOffset.interpolate({
    inputRange: [0, windowHeight * backdropFadeDistanceRatio],
    outputRange: [1, 0],
    extrapolate: "clamp",
  });

  return (
    <Modal transparent visible animationType="none" onRequestClose={onClose}>
      <ThemeScope className="flex-1 justify-end">
        <Entrance kind="fade" className="absolute inset-0">
          <Animated.View style={{ flex: 1, opacity: backdropOpacity }}>
            <Pressable
              accessibilityRole="button"
              accessibilityLabel={translate("common.close")}
              onPress={onClose}
              className="absolute inset-0 bg-inverse/45"
            />
          </Animated.View>
        </Entrance>
        <Entrance kind="slideUp" className="max-h-[92%] w-full max-w-2xl self-center">
          <Animated.View style={{ flexShrink: 1, transform: [{ translateY: dragOffset }] }}>
            <SafeAreaView
              edges={["bottom"]}
              className="w-full shrink overflow-hidden rounded-t-[20px] bg-background"
            >
              <View
                testID="bottom-sheet-drag-handle"
                className="h-6 w-full items-center justify-center"
                {...dragHandlers}
              >
                <View className="h-[5px] w-10 rounded-full bg-border-strong" />
              </View>
              {header}
              <ScrollView
                contentContainerClassName="gap-3.5 px-5 pb-7 pt-1.5"
                keyboardShouldPersistTaps="handled"
              >
                {children}
              </ScrollView>
            </SafeAreaView>
          </Animated.View>
        </Entrance>
      </ThemeScope>
    </Modal>
  );
}
