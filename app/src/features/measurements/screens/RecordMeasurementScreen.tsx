import { useRouter } from "expo-router";
import { useState } from "react";
import { useClient } from "@/features/clients/useClient";
import { MeasurementRecordForm } from "@/features/measurements/components/MeasurementRecordForm";
import { RecordMeasurementRequest } from "@/features/measurements/types";
import {
  useMeasurementDefinitions,
  useRecordMeasurement,
  useStudentMeasurements,
} from "@/features/measurements/useMeasurements";
import { SettingsFormScreenLayout } from "@/features/settings/components/SettingsFormScreenLayout";
import { SettingsItemState } from "@/features/settings/components/SettingsItemState";
import { SubmissionFailure, toSubmissionFailure } from "@/features/settings/submissionFailure";
import { translate } from "@/i18n/translate";
import { useToast } from "@/ui/ToastProvider";

interface RecordMeasurementScreenProps {
  clientId: string;
  studentId: string;
  measurementId?: string;
}

export function RecordMeasurementScreen({
  clientId,
  studentId,
  measurementId,
}: RecordMeasurementScreenProps) {
  const router = useRouter();
  const { showToast } = useToast();
  const client = useClient(clientId);
  const definitions = useMeasurementDefinitions();
  const [selectedStudentId, setSelectedStudentId] = useState(studentId);
  const studentMeasurements = useStudentMeasurements(selectedStudentId);
  const recordMutation = useRecordMeasurement();
  const [submissionFailure, setSubmissionFailure] = useState<SubmissionFailure | null>(null);
  const [lastSubmission, setLastSubmission] = useState<
    [string, string, RecordMeasurementRequest] | null
  >(null);

  const save = async (
    chosenStudentId: string,
    chosenMeasurementId: string,
    request: RecordMeasurementRequest,
  ) => {
    setLastSubmission([chosenStudentId, chosenMeasurementId, request]);
    setSubmissionFailure(null);
    try {
      await recordMutation.mutateAsync({
        studentId: chosenStudentId,
        measurementId: chosenMeasurementId,
        request,
      });
      showToast(translate("measurements.recorded"));
      router.back();
    } catch (saveError) {
      setSubmissionFailure(toSubmissionFailure(saveError));
    }
  };

  if (client.data === undefined || definitions.data === undefined) {
    return (
      <SettingsItemState
        navigation="close"
        isPending={client.isPending || definitions.isPending}
        isError={client.isError || definitions.isError}
        notFoundMessage={translate("clients.detail.notFound")}
        onRetry={() => {
          client.refetch();
          definitions.refetch();
        }}
      />
    );
  }

  return (
    <SettingsFormScreenLayout
      title={translate("measurements.recordForm.title")}
      submissionFailure={submissionFailure}
      onRetry={() => (lastSubmission ? save(...lastSubmission) : undefined)}
    >
      <MeasurementRecordForm
        students={client.data.students}
        initialStudentId={studentId}
        measurements={definitions.data}
        initialMeasurementId={measurementId}
        studentMeasurementsOf={(candidateId) =>
          candidateId === selectedStudentId ? (studentMeasurements.data ?? []) : []
        }
        onStudentChange={setSelectedStudentId}
        isSaving={recordMutation.isPending}
        onSubmit={save}
      />
    </SettingsFormScreenLayout>
  );
}
