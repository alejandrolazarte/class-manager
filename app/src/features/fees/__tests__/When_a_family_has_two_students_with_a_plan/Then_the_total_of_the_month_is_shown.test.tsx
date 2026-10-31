import { screen } from "@testing-library/react-native";
import { ClientFeeSection } from "@/features/fees/components/ClientFeeSection";
import { listClientPayments } from "@/features/fees/feesApi";
import { formatMoney } from "@/formatting/money";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import { buildStudentPlan } from "@/testing/feeFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");

const client = buildClient({
  plans: {
    monthlyDue: 50000,
    students: [
      buildStudentPlan(),
      buildStudentPlan({
        studentId: "0192f0c4-0000-7000-8000-0000000057d2",
        studentFullName: "Lucía Pérez",
      }),
    ],
  },
});

describe("When a family has two students with a plan", () => {
  beforeEach(() => {
    jest.mocked(listClientPayments).mockResolvedValue([]);
  });

  it("Then the total of the month is shown", async () => {
    await renderWithProviders(<ClientFeeSection client={client} />);

    expect(
      screen.getByText(translate("fees.client.monthTotal", { amount: formatMoney(50000, "ARS") })),
    ).toBeOnTheScreen();
  });
});
