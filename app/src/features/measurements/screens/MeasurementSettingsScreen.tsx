import { useRouter } from "expo-router";
import { View } from "react-native";
import { measurementSummary } from "@/features/measurements/measurementFormatting";
import { Measurement } from "@/features/measurements/types";
import {
  useDeleteMeasurement,
  useMeasurementDefinitions,
  useRestoreMeasurement,
  useUpdateMeasurement,
} from "@/features/measurements/useMeasurements";
import { SettingsListScreenLayout } from "@/features/settings/components/SettingsListScreenLayout";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { AppText } from "@/ui/AppText";
import { HairlineRow } from "@/ui/HairlineRow";
import { IconButton } from "@/ui/IconButton";
import { ToggleSwitch } from "@/ui/ToggleSwitch";
import { useToast } from "@/ui/ToastProvider";

interface MeasurementRowProps {
  measurement: Measurement;
  onChange: (measurement: Measurement) => void;
  onDelete: (measurement: Measurement) => void;
}

function MeasurementRow({ measurement, onChange, onDelete }: MeasurementRowProps) {
  return (
    <HairlineRow className="gap-1 py-3">
      <View className="flex-row items-start gap-3">
        <View className="min-w-0 flex-1 gap-0.5">
          <AppText variant="bodyStrong">{measurement.name}</AppText>
          <AppText variant="caption" tone="subtle">
            {measurementSummary(measurement)}
          </AppText>
        </View>
        <IconButton
          icon="delete"
          tone="muted-foreground"
          accessibilityLabel={translate("measurements.delete", { name: measurement.name })}
          onPress={() => onDelete(measurement)}
        />
      </View>
      <ToggleSwitch
        label={translate("measurements.visibleToStudents")}
        accessibilityLabel={translate("measurements.visibleToStudentsAccessibility", {
          name: measurement.name,
        })}
        value={measurement.isVisibleToStudents}
        onValueChange={(isVisibleToStudents) =>
          onChange({
            ...measurement,
            isVisibleToStudents,
            isRecordedByStudents: isVisibleToStudents && measurement.isRecordedByStudents,
          })
        }
      />
      {measurement.isVisibleToStudents ? (
        <ToggleSwitch
          label={translate("measurements.recordedByStudentsBadge")}
          accessibilityLabel={translate("measurements.recordedByStudentsAccessibility", {
            name: measurement.name,
          })}
          value={measurement.isRecordedByStudents}
          onValueChange={(isRecordedByStudents) =>
            onChange({ ...measurement, isRecordedByStudents })
          }
        />
      ) : null}
    </HairlineRow>
  );
}

export function MeasurementSettingsScreen() {
  const router = useRouter();
  const { showToast } = useToast();
  const {
    data: measurements = [],
    isPending,
    isError,
    isRefetching,
    refetch,
  } = useMeasurementDefinitions();
  const updateMutation = useUpdateMeasurement();
  const deleteMutation = useDeleteMeasurement();
  const restoreMutation = useRestoreMeasurement();

  const changeMeasurement = (measurement: Measurement) => {
    updateMutation
      .mutateAsync(measurement)
      .catch(() => showToast(translate("common.unexpectedError")));
  };

  const deleteMeasurement = async (measurement: Measurement) => {
    try {
      await deleteMutation.mutateAsync(measurement.id);
      showToast(translate("measurements.deleted"), {
        label: translate("common.undo"),
        onPress: () => {
          restoreMutation
            .mutateAsync(measurement.id)
            .catch(() => showToast(translate("common.undoFailed")));
        },
      });
    } catch {
      showToast(translate("common.unexpectedError"));
    }
  };

  return (
    <SettingsListScreenLayout
      title={translate("measurements.title")}
      notice={
        <AppText variant="caption" tone="subtle">
          {translate("measurements.hint")}
        </AppText>
      }
      items={measurements}
      isPending={isPending}
      isError={isError}
      isRefetching={isRefetching}
      onRefetch={() => refetch()}
      keyExtractor={(measurement) => measurement.id}
      renderItem={(measurement) => (
        <MeasurementRow
          measurement={measurement}
          onChange={changeMeasurement}
          onDelete={deleteMeasurement}
        />
      )}
      emptyMessage={translate("measurements.empty")}
      emptyIcon="trendingUp"
      newItemLabel={translate("measurements.new")}
      onNewItem={() => router.push(routes.newMeasurement)}
    />
  );
}
