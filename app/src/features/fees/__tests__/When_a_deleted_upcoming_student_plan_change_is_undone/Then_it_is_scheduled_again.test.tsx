import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { ClientFeeSection } from "@/features/fees/components/ClientFeeSection";
import {
  deleteStudentPlanChange,
  listClientPayments,
  setStudentPlan,
} from "@/features/fees/feesApi";
import { addMonths, formatMonth, monthOf } from "@/features/fees/months";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import { buildStudentPlan } from "@/testing/feeFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");

const upcomingMonth = addMonths(monthOf(), 2);
const studentPlan = buildStudentPlan({
  upcomingChanges: [
    {
      effectiveFrom: upcomingMonth,
      classPackId: "0192f0c4-0000-7000-8000-00000000c1a6",
      planName: "Cuota mensual",
      customPrice: 30,
    },
  ],
});
const client = buildClient({ plans: { monthlyDue: 25000, students: [studentPlan] } });

describe("When a deleted upcoming student plan change is undone", () => {
  beforeEach(() => {
    jest.mocked(listClientPayments).mockResolvedValue([]);
    jest.mocked(deleteStudentPlanChange).mockResolvedValue(client.plans);
    jest.mocked(setStudentPlan).mockResolvedValue(client.plans);
  });

  it("Then it is scheduled again", async () => {
    await renderWithProviders(<ClientFeeSection client={client} />);

    await fireEvent.press(
      screen.getByRole("button", {
        name: translate("fees.client.deleteUpcomingChange", { month: formatMonth(upcomingMonth) }),
      }),
    );
    await fireEvent.press(await screen.findByRole("button", { name: translate("common.undo") }));

    await waitFor(() =>
      expect(setStudentPlan).toHaveBeenCalledWith(studentPlan.studentId, {
        classPackId: "0192f0c4-0000-7000-8000-00000000c1a6",
        customPrice: 30,
        effectiveFrom: upcomingMonth,
      }),
    );
  });
});
