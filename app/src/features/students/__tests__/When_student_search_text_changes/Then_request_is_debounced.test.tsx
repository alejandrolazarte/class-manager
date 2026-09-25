import { act, fireEvent, screen } from "@testing-library/react-native";
import { StudentListScreen } from "@/features/students/screens/StudentListScreen";
import { searchStudents } from "@/features/students/studentsApi";
import { studentSearchDebounceMilliseconds } from "@/features/students/useStudentSearch";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/students/studentsApi");

describe("When student search text changes", () => {
  beforeEach(() => {
    jest.useFakeTimers();
    jest.mocked(searchStudents).mockResolvedValue([]);
  });

  afterEach(() => {
    jest.useRealTimers();
  });

  it("Then request is debounced", async () => {
    await renderWithProviders(<StudentListScreen />);
    const searchInput = screen.getByLabelText(translate("students.list.searchPlaceholder"));

    await fireEvent.changeText(searchInput, "t");
    await fireEvent.changeText(searchInput, "to");
    await fireEvent.changeText(searchInput, "tom");
    await act(async () => {
      jest.advanceTimersByTime(studentSearchDebounceMilliseconds);
    });

    const searchedTexts = jest
      .mocked(searchStudents)
      .mock.calls.map(([searchRequest]) => searchRequest.search);
    expect(searchedTexts).toEqual(["", "tom"]);
  });
});
