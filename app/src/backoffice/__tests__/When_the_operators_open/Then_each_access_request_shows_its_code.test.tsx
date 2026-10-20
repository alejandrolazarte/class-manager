import { screen } from "@testing-library/react-native";
import { getOperators } from "@/backoffice/operators/operatorsApi";
import { OperatorsScreen } from "@/backoffice/operators/OperatorsScreen";
import { translate } from "@/i18n/translate";
import { anOperatorList } from "@/testing/backofficeOperators";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

jest.mock("@/backoffice/operators/operatorsApi", () => ({
  ...jest.requireActual("@/backoffice/operators/operatorsApi"),
  getOperators: jest.fn(),
}));

describe("When the operators open", () => {
  it("Then each access request shows its code", async () => {
    jest.mocked(getOperators).mockResolvedValue(anOperatorList());

    await renderBackofficeScreen(<OperatorsScreen />);

    expect(await screen.findByText("nueva@example.com")).toBeOnTheScreen();
    expect(
      screen.getByText(
        translate("backoffice.operators.accessRequestDetail", {
          code: "K7-3QF",
          date: "20/10/2026",
        }),
      ),
    ).toBeOnTheScreen();
    expect(screen.getByText("Lectora Dos")).toBeOnTheScreen();
  });
});
