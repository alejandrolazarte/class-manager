import { fireEvent, screen } from "@testing-library/react-native";
import { FeedbackSheet } from "@/features/sessions/components/FeedbackSheet";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSessionStudent } from "@/testing/sessionFactory";

describe("When a class comment is deleted", () => {
  it("Then it is deleted after confirming", async () => {
    const onSave = jest.fn();
    await renderWithProviders(
      <FeedbackSheet
        student={buildSessionStudent({ feedback: "Muy bien" })}
        isSaving={false}
        onSave={onSave}
        onClose={jest.fn()}
      />,
    );

    await fireEvent.press(
      screen.getByRole("button", { name: translate("sessions.feedback.remove") }),
    );
    expect(onSave).not.toHaveBeenCalled();
    await fireEvent.press(screen.getByRole("button", { name: translate("common.confirmDelete") }));

    expect(onSave).toHaveBeenCalledWith(null);
  });
});
