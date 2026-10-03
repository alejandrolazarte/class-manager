import { useMemo, useState } from "react";
import { Animated, PanResponder, Pressable, View } from "react-native";
import {
  attendanceStatusForSwipe,
  attendanceSwipeLimit,
  attendanceSwipeThreshold,
} from "@/features/sessions/attendanceSwipe";
import { AttendanceStatus, SessionStudent } from "@/features/sessions/types";
import { normalizeStudentName } from "@/features/students/studentSchema";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Avatar, AvatarTone } from "@/ui/Avatar";
import { useElevationStyle } from "@/ui/elevation";
import { Icon, IconName } from "@/ui/Icon";

type AttendanceRowStudent = Omit<
  SessionStudent,
  "feedback" | "absenceNotified" | "isMakeup" | "isPackBooking"
> & {
  feedback?: string | null;
  absenceNotified?: boolean;
  isMakeup?: boolean;
  isPackBooking?: boolean;
};

interface AttendanceRowProps {
  student: AttendanceRowStudent;
  status: AttendanceStatus | null;
  disabled: boolean;
  isSwipeEnabled?: boolean;
  onChangeStatus: (status: AttendanceStatus | null) => void;
  onOpenFeedback?: () => void;
}

interface MarkButtonProps {
  icon: IconName;
  accessibilityLabel: string;
  isSelected: boolean;
  selectedClassName: string;
  selectedIconTone: "success-foreground" | "danger-foreground";
  disabled: boolean;
  onPress: () => void;
}

const swipeActivationDistance = 10;
const avatarTones: Record<AttendanceStatus | "Unmarked", AvatarTone> = {
  Present: "success",
  Absent: "danger",
  Unmarked: "muted",
};

function MarkButton({
  icon,
  accessibilityLabel,
  isSelected,
  selectedClassName,
  selectedIconTone,
  disabled,
  onPress,
}: MarkButtonProps) {
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityLabel={accessibilityLabel}
      accessibilityState={{ selected: isSelected, disabled }}
      disabled={disabled}
      onPress={onPress}
      className={`h-11 w-11 items-center justify-center rounded-full border-[1.5px] ${isSelected ? selectedClassName : "border-border bg-transparent"} ${disabled ? "opacity-40" : ""}`}
    >
      <Icon name={icon} tone={isSelected ? selectedIconTone : "subtle-foreground"} />
    </Pressable>
  );
}

export function AttendanceRow({
  student,
  status,
  disabled,
  isSwipeEnabled = true,
  onChangeStatus,
  onOpenFeedback,
}: AttendanceRowProps) {
  const elevationStyle = useElevationStyle();
  const [horizontalOffset] = useState(() => new Animated.Value(0));
  const canSwipe = isSwipeEnabled && !disabled;

  const panResponder = useMemo(
    () =>
      PanResponder.create({
        onMoveShouldSetPanResponder: (_, gesture) =>
          canSwipe &&
          Math.abs(gesture.dx) > swipeActivationDistance &&
          Math.abs(gesture.dx) > Math.abs(gesture.dy),
        onPanResponderTerminationRequest: () => false,
        onPanResponderMove: (_, gesture) =>
          horizontalOffset.setValue(
            Math.max(-attendanceSwipeLimit, Math.min(attendanceSwipeLimit, gesture.dx)),
          ),
        onPanResponderRelease: (_, gesture) => {
          const swipedStatus = attendanceStatusForSwipe(gesture.dx);
          if (swipedStatus !== null) {
            onChangeStatus(swipedStatus);
          }
          Animated.spring(horizontalOffset, { toValue: 0, useNativeDriver: true }).start();
        },
        onPanResponderTerminate: () =>
          Animated.spring(horizontalOffset, { toValue: 0, useNativeDriver: true }).start(),
      }),
    [canSwipe, horizontalOffset, onChangeStatus],
  );

  const isOwnClient =
    normalizeStudentName(student.studentFullName) === normalizeStudentName(student.clientFullName);
  const statusLabel =
    status === "Present"
      ? translate("sessions.attendance.present")
      : status === "Absent"
        ? translate("sessions.attendance.absent")
        : translate("sessions.attendance.unmarked");
  const toggle = (selectedStatus: AttendanceStatus) =>
    onChangeStatus(status === selectedStatus ? null : selectedStatus);
  const presentHintOpacity = horizontalOffset.interpolate({
    inputRange: [0, attendanceSwipeThreshold],
    outputRange: [0, 1],
    extrapolate: "clamp",
  });
  const absentHintOpacity = horizontalOffset.interpolate({
    inputRange: [-attendanceSwipeThreshold, 0],
    outputRange: [1, 0],
    extrapolate: "clamp",
  });

  return (
    <View className="overflow-hidden rounded-[18px] bg-muted">
      <View className="absolute inset-0 flex-row items-center justify-between px-5">
        <Animated.View style={{ opacity: presentHintOpacity }}>
          <View className="flex-row items-center gap-1.5">
            <Icon name="present" tone="success" />
            <AppText variant="link" tone="success">
              {translate("sessions.attendance.present")}
            </AppText>
          </View>
        </Animated.View>
        <Animated.View style={{ opacity: absentHintOpacity }}>
          <View className="flex-row items-center gap-1.5">
            <AppText variant="link" tone="danger">
              {translate("sessions.attendance.absent")}
            </AppText>
            <Icon name="absent" tone="danger" />
          </View>
        </Animated.View>
      </View>
      <Animated.View
        style={{ transform: [{ translateX: horizontalOffset }] }}
        {...panResponder.panHandlers}
      >
        <View
          style={elevationStyle}
          className="flex-row items-center gap-3 rounded-[18px] bg-surface py-3 pl-3.5 pr-3"
        >
          <Avatar name={student.studentFullName} tone={avatarTones[status ?? "Unmarked"]} />
          <View className="min-w-0 flex-1 gap-0.5">
            <AppText variant="bodyStrong">{student.studentFullName}</AppText>
            <AppText variant="caption" tone="subtle">
              {isOwnClient
                ? statusLabel
                : translate("students.list.responsible", { name: student.clientFullName })}
            </AppText>
            {student.absenceNotified ? (
              <AppText variant="caption" tone="warning" className="font-label">
                {translate("sessions.attendance.absenceNotified")}
              </AppText>
            ) : null}
            {student.isMakeup ? (
              <AppText variant="caption" tone="primary" className="font-label">
                {translate("sessions.attendance.makeup")}
              </AppText>
            ) : null}
            {student.isPackBooking ? (
              <AppText variant="caption" tone="primary" className="font-label">
                {translate("sessions.attendance.packBooking")}
              </AppText>
            ) : null}
            {onOpenFeedback === undefined ? null : (
              <Pressable
                accessibilityRole="button"
                accessibilityLabel={translate("sessions.feedback.openFor", {
                  name: student.studentFullName,
                })}
                onPress={onOpenFeedback}
                hitSlop={8}
                className="flex-row items-center gap-1 self-start active:opacity-70"
              >
                <Icon name="comment" size="small" tone="primary" />
                <AppText variant="link" tone="primary" numberOfLines={1} className="flex-shrink">
                  {student.feedback
                    ? translate("sessions.feedback.edit")
                    : translate("sessions.feedback.open")}
                </AppText>
              </Pressable>
            )}
          </View>
          <MarkButton
            icon="present"
            accessibilityLabel={translate("sessions.attendance.presentFor", {
              name: student.studentFullName,
            })}
            isSelected={status === "Present"}
            selectedClassName="border-success bg-success"
            selectedIconTone="success-foreground"
            disabled={disabled}
            onPress={() => toggle("Present")}
          />
          <MarkButton
            icon="absent"
            accessibilityLabel={translate("sessions.attendance.absentFor", {
              name: student.studentFullName,
            })}
            isSelected={status === "Absent"}
            selectedClassName="border-danger bg-danger"
            selectedIconTone="danger-foreground"
            disabled={disabled}
            onPress={() => toggle("Absent")}
          />
        </View>
      </Animated.View>
    </View>
  );
}
