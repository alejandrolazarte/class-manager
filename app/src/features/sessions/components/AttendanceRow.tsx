import { useMemo, useState } from "react";
import { Animated, PanResponder, View } from "react-native";
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
import { Icon, IconName } from "@/ui/Icon";
import { Entrance } from "@/ui/motion/Entrance";
import { PressableScale } from "@/ui/motion/PressableScale";
import { TextLink } from "@/ui/TextLink";

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
  selectedBorderClassName: string;
  selectedFillClassName: string;
  selectedIconTone: "success-foreground" | "danger-foreground";
  disabled: boolean;
  onPress: () => void;
}

const swipeActivationDistance = 10;
const rowTintClassNames: Record<AttendanceStatus, string> = {
  Present: "bg-success-soft/50",
  Absent: "bg-danger-soft/50",
};
const avatarTones: Record<AttendanceStatus | "Unmarked", AvatarTone> = {
  Present: "success",
  Absent: "danger",
  Unmarked: "muted",
};

function MarkButton({
  icon,
  accessibilityLabel,
  isSelected,
  selectedBorderClassName,
  selectedFillClassName,
  selectedIconTone,
  disabled,
  onPress,
}: MarkButtonProps) {
  return (
    <PressableScale
      accessibilityRole="button"
      accessibilityLabel={accessibilityLabel}
      accessibilityState={{ selected: isSelected, disabled }}
      disabled={disabled}
      onPress={onPress}
      className={`h-10 w-10 items-center justify-center overflow-hidden rounded-full border-[1.5px] bg-transparent ${isSelected ? selectedBorderClassName : "border-border"} ${disabled ? "opacity-40" : ""}`}
    >
      {isSelected ? (
        <Entrance kind="pop" className={`absolute inset-0 rounded-full ${selectedFillClassName}`} />
      ) : null}
      <Icon name={icon} size="medium" tone={isSelected ? selectedIconTone : "subtle-foreground"} />
    </PressableScale>
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
    <View className="-mx-2 overflow-hidden bg-muted">
      <View className="absolute inset-0 flex-row items-center justify-between px-4">
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
        <View className="flex-row items-center gap-2.5 border-b border-border-subtle bg-background px-2 py-2.5">
          {status === null ? null : (
            <Entrance
              key={status}
              kind="fade"
              className={`absolute inset-0 ${rowTintClassNames[status]}`}
            />
          )}
          <Avatar
            name={student.studentFullName}
            tone={avatarTones[status ?? "Unmarked"]}
            size="small"
          />
          <View className="min-w-0 flex-1 gap-0.5">
            <AppText variant="bodyStrong">{student.studentFullName}</AppText>
            <AppText variant="footnote" tone="subtle">
              {isOwnClient
                ? statusLabel
                : `${statusLabel} · ${translate("students.list.responsible", { name: student.clientFullName })}`}
            </AppText>
            {student.absenceNotified ? (
              <AppText variant="badge" tone="warning">
                {translate("sessions.attendance.absenceNotified")}
              </AppText>
            ) : null}
            {student.isMakeup ? (
              <AppText variant="badge" tone="primary">
                {translate("sessions.attendance.makeup")}
              </AppText>
            ) : null}
            {student.isPackBooking ? (
              <AppText variant="badge" tone="primary">
                {translate("sessions.attendance.packBooking")}
              </AppText>
            ) : null}
            {onOpenFeedback === undefined ? null : (
              <TextLink
                icon="comment"
                label={
                  student.feedback
                    ? translate("sessions.feedback.edit")
                    : translate("sessions.feedback.open")
                }
                accessibilityLabel={translate("sessions.feedback.openFor", {
                  name: student.studentFullName,
                })}
                onPress={onOpenFeedback}
              />
            )}
          </View>
          <MarkButton
            icon="present"
            accessibilityLabel={translate("sessions.attendance.presentFor", {
              name: student.studentFullName,
            })}
            isSelected={status === "Present"}
            selectedBorderClassName="border-success"
            selectedFillClassName="bg-success"
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
            selectedBorderClassName="border-danger"
            selectedFillClassName="bg-danger"
            selectedIconTone="danger-foreground"
            disabled={disabled}
            onPress={() => toggle("Absent")}
          />
        </View>
      </Animated.View>
    </View>
  );
}
