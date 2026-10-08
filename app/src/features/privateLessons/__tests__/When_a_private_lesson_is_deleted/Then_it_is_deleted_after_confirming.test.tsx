import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { deletePrivateLesson, getPrivateLesson } from "@/features/privateLessons/privateLessonsApi";
import { PrivateLessonScreen } from "@/features/privateLessons/screens/PrivateLessonScreen";
import { translate } from "@/i18n/translate";
import { buildPrivateLesson } from "@/testing/privateLessonFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/privateLessons/privateLessonsApi");

const lesson = buildPrivateLesson();

describe("When a private lesson is deleted", () => {
  beforeEach(() => {
    jest.mocked(getPrivateLesson).mockResolvedValue(lesson);
    jest.mocked(deletePrivateLesson).mockResolvedValue(undefined);
  });

  it("Then it is deleted after confirming", async () => {
    await renderWithProviders(<PrivateLessonScreen privateLessonId={lesson.id} />);

    await fireEvent.press(await screen.findByText(translate("privateLessons.delete")));
    expect(deletePrivateLesson).not.toHaveBeenCalled();
    await fireEvent.press(screen.getByRole("button", { name: translate("common.confirmDelete") }));

    await waitFor(() => expect(deletePrivateLesson).toHaveBeenCalledWith(lesson.id));
  });
});
