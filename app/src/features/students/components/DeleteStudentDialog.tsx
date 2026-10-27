import { useState } from "react";
import { useStudentEnrollments } from "@/features/enrollments/useStudentEnrollments";
import { Student } from "@/features/students/types";
import { useDeleteStudent } from "@/features/students/useDeleteStudent";
import { translate, translateCount } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { ConfirmDialog } from "@/ui/ConfirmDialog";
import { useToast } from "@/ui/ToastProvider";

interface DeleteStudentDialogProps {
  student: Student;
  onClose: () => void;
}

export function DeleteStudentDialog({ student, onClose }: DeleteStudentDialogProps) {
  const { showToast } = useToast();
  const deleteStudentMutation = useDeleteStudent();
  const { data: enrollments } = useStudentEnrollments(student.id);
  const [hasDeleteFailed, setHasDeleteFailed] = useState(false);
  const enrollmentCount = enrollments?.length ?? 0;

  const confirmDelete = async () => {
    setHasDeleteFailed(false);
    try {
      await deleteStudentMutation.mutateAsync(student.id);
      showToast(translate("students.delete.deleted"));
      onClose();
    } catch {
      setHasDeleteFailed(true);
    }
  };

  return (
    <>
      <ConfirmDialog
        title={translate("students.delete.question", { name: student.fullName })}
        message={
          enrollmentCount > 0
            ? translateCount("students.delete.enrollments", enrollmentCount)
            : undefined
        }
        confirmLabel={translate("students.delete.confirm")}
        onCancel={onClose}
        onConfirm={confirmDelete}
        isConfirming={deleteStudentMutation.isPending}
      />
      {hasDeleteFailed ? <Banner message={translate("common.unexpectedError")} /> : null}
    </>
  );
}
