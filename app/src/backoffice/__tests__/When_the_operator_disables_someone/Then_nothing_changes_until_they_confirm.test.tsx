import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { disableOperator, getOperators } from "@/backoffice/operators/operatorsApi";
import { OperatorsScreen } from "@/backoffice/operators/OperatorsScreen";
import { translate } from "@/i18n/translate";
import { anOperatorList } from "@/testing/backofficeOperators";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

jest.mock("@/backoffice/operators/operatorsApi", () => ({
  ...jest.requireActual("@/backoffice/operators/operatorsApi"),
  getOperators: jest.fn(),
  disableOperator: jest.fn(),
}));

describe("When the operator disables someone", () => {
  it("Then nothing changes until they confirm", async () => {
    jest.mocked(getOperators).mockResolvedValue(anOperatorList());
    jest.mocked(disableOperator).mockResolvedValue({ loginId: "login-2" });
    await renderBackofficeScreen(<OperatorsScreen />);

    await fireEvent.press(
      await screen.findByRole("button", {
        name: translate("backoffice.operators.disableOf", { name: "Lectora Dos" }),
      }),
    );
    await screen.findByText(translate("backoffice.operators.disableConfirmMessage"));
    expect(disableOperator).not.toHaveBeenCalled();

    await fireEvent.press(
      screen.getByRole("button", { name: translate("backoffice.operators.disable") }),
    );

    await waitFor(() => expect(disableOperator).toHaveBeenCalledWith("login-2"));
  });
});
