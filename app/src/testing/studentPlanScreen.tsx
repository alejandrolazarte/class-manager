import { getClassBalance, listClassPacks } from "@/features/classPacks/classPacksApi";
import { getClient } from "@/features/clients/clientsApi";
import { ClassBalance } from "@/features/classPacks/types";
import { StudentPlanScreen } from "@/features/fees/screens/StudentPlanScreen";
import { buildClassBalance, buildClassPack } from "@/testing/classPackFactory";
import { buildClient } from "@/testing/clientFactory";
import { buildStudentPlan } from "@/testing/feeFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

export const studentPlanOfScreen = buildStudentPlan();
export const clientOfStudentPlanScreen = buildClient({
  plans: { monthlyDue: 25000, students: [studentPlanOfScreen] },
});

export async function renderStudentPlanScreen(
  classBalance: ClassBalance = buildClassBalance({ purchases: [] }),
) {
  jest.mocked(getClient).mockResolvedValue(clientOfStudentPlanScreen);
  jest.mocked(getClassBalance).mockResolvedValue(classBalance);
  jest.mocked(listClassPacks).mockResolvedValue([
    buildClassPack({
      id: studentPlanOfScreen.classPackId!,
      name: "Cuota mensual",
      classCount: null,
      validityMonths: null,
      renewsMonthly: true,
    }),
  ]);
  await renderWithProviders(
    <StudentPlanScreen
      clientId={clientOfStudentPlanScreen.id}
      studentId={studentPlanOfScreen.studentId}
    />,
  );
}
