import { useRouter } from "expo-router";
import { useState } from "react";
import { isApiError } from "@/api/apiErrors";
import { MeasurementRecordForm } from "@/features/measurements/components/MeasurementRecordForm";
import { RecordMeasurementRequest } from "@/features/measurements/types";
import {
  useRecordStudentAppMeasurement,
  useStudentAppMeasurements,
} from "@/features/measurements/useMeasurements";
import { SettingsFormScreenLayout } from "@/features/settings/components/SettingsFormScreenLayout";
import { SettingsItemState } from "@/features/settings/components/SettingsItemState";
import { SubmissionFailure, toSubmissionFailure } from "@/features/settings/submissionFailure";
import { translate } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { useToast } from "@/ui/ToastProvider";

const conflictStatus = 409;

interface StudentAppRecordMeasurementScreenProps {
  studentId: string;
}

export function StudentAppRecordMeasurementScreen({
  studentId,
}: StudentAppRecordMeasurementScreenProps) {
  const router = useRouter();
  const { showToast } = useToast();
  const measurements = useStudentAppMeasurements();
  const recordMutation = useRecordStudentAppMeasurement();
  const [selectedStudentId, setSelectedStudentId] = useState(studentId);
  const [submissionFailure, setSubmissionFailure] = useState<SubmissionFailure | null>(null);
  const [lastSubmission, setLastSubmission] = useState<
    [string, string, RecordMeasurementRequest] | null
  >(null);

  if (measurements.data === undefined) {
    return (
      <SettingsItemState
        navigation="close"
        isPending={measurements.isPending}
        isError={measurements.isError}
        notFoundMessage={translate("student.loadError")}
        onRetry={() => measurements.refetch()}
      />
    );
  }

  const students = measurements.data.students;
  const recordableOf = (candidateId: string) =>
    students
      .find((student) => student.studentId === candidateId)
      ?.measurements.filter((item) => item.measurement.isRecordedByStudents) ?? [];
  const studentsWhoRecord = students
    .filter((student) => recordableOf(student.studentId).length > 0)
    .map((student) => ({ id: student.studentId, fullName: student.fullName }));

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
      showToast(translate("student.measurements.recorded"));
      router.back();
    } catch (saveError) {
      if (isApiError(saveError) && saveError.status === conflictStatus) {
        showToast(translate("student.measurements.onlyTeam"));
        return;
      }
      setSubmissionFailure(toSubmissionFailure(saveError));
    }
  };

  return (
    <SettingsFormScreenLayout
      title={translate("student.measurements.record")}
      submissionFailure={submissionFailure}
      onRetry={() => (lastSubmission ? save(...lastSubmission) : undefined)}
    >
      <MeasurementRecordForm
        key={selectedStudentId}
        students={studentsWhoRecord}
        initialStudentId={selectedStudentId}
        measurements={recordableOf(selectedStudentId).map((item) => item.measurement)}
        studentMeasurementsOf={recordableOf}
        onStudentChange={setSelectedStudentId}
        isSaving={recordMutation.isPending}
        measurementHint={translate("student.measurements.recordHint")}
        notice={<Banner tone="info" message={translate("student.measurements.recordNotice")} />}
        onSubmit={save}
      />
    </SettingsFormScreenLayout>
  );
}
