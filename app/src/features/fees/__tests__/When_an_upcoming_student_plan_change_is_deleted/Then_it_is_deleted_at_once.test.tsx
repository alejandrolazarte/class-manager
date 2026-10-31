import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { ClientFeeSection } from "@/features/fees/components/ClientFeeSection";
import { deleteStudentPlanChange, listClientPayments } from "@/features/fees/feesApi";
import { addMonths, formatMonth, monthOf } from "@/features/fees/months";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import { buildStudentPlan } from "@/testing/feeFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");

const upcomingMonth = addMonths(monthOf(), 2);
const studentPlan = buildStudentPlan({
  upcomingChanges: [
    { effectiveFrom: upcomingMonth, classPackId: null, planName: null, customPrice: null },
  ],
});
const client = buildClient({ plans: { monthlyDue: 25000, students: [studentPlan] } });

describe("When an upcoming student plan change is deleted", () => {
  beforeEach(() => {
    jest.mocked(listClientPayments).mockResolvedValue([]);
    jest.mocked(deleteStudentPlanChange).mockResolvedValue(client.plans);
  });

  it("Then it is deleted at once", async () => {
    await renderWithProviders(<ClientFeeSection client={client} />);

    await fireEvent.press(
      screen.getByRole("button", {
        name: translate("fees.client.deleteUpcomingChange", { month: formatMonth(upcomingMonth) }),
      }),
    );

    await waitFor(() =>
      expect(deleteStudentPlanChange).toHaveBeenCalledWith(studentPlan.studentId, upcomingMonth),
    );
  });
});
