import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { EnableOperatorScreen } from "@/backoffice/operators/EnableOperatorScreen";
import { enableOperator, getOperators } from "@/backoffice/operators/operatorsApi";
import { translate } from "@/i18n/translate";
import { anOperatorList } from "@/testing/backofficeOperators";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

jest.mock("@/backoffice/operators/operatorsApi", () => ({
  ...jest.requireActual("@/backoffice/operators/operatorsApi"),
  getOperators: jest.fn(),
  enableOperator: jest.fn(),
}));

describe("When the operator enables an access request", () => {
  it("Then nothing changes until they confirm", async () => {
    jest.mocked(getOperators).mockResolvedValue(anOperatorList());
    jest.mocked(enableOperator).mockResolvedValue({ loginId: "login-3" });
    await renderBackofficeScreen(<EnableOperatorScreen accessRequestId="request-1" />);

    await fireEvent.press(await screen.findByText("Persona Nueva"));
    await fireEvent.press(screen.getByText(translate("roles.PlatformReader")));
    await fireEvent.press(
      screen.getByRole("button", { name: translate("backoffice.enableOperator.submit") }),
    );
    await screen.findByText(
      translate("backoffice.enableOperator.confirmMessage", {
        name: "Persona Nueva",
        branch: "Laztty",
      }),
    );
    expect(enableOperator).not.toHaveBeenCalled();

    await fireEvent.press(
      screen.getByRole("button", { name: translate("backoffice.enableOperator.confirm") }),
    );

    await waitFor(() =>
      expect(enableOperator).toHaveBeenCalledWith({
        accessRequestId: "request-1",
        memberId: "member-1",
        role: "PlatformReader",
      }),
    );
  });
});
