import { fireEvent, screen } from "@testing-library/react-native";
import { DayScreen } from "@/features/sessions/screens/DayScreen";
import { listDaySessions } from "@/features/sessions/sessionsApi";
import { routerMock } from "@/testing/expoRouterMock";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildDaySession, sessionDate } from "@/testing/sessionFactory";

jest.mock("@/features/sessions/sessionsApi");

const privateLessonId = "0192f0f1-0000-7000-8000-000000000001";

describe("When day has a private lesson", () => {
  beforeEach(() => {
    jest.mocked(listDaySessions).mockResolvedValue([
      buildDaySession({
        kind: "Private",
        classGroupId: null,
        privateLessonId,
        classGroupName: "Tomás Pérez",
        studentNames: ["Tomás Pérez"],
        startTime: "10:00",
      }),
    ]);
  });

  it("Then its card opens the lesson", async () => {
    await renderWithProviders(<DayScreen initialDate={sessionDate} />);

    await fireEvent.press(await screen.findByRole("button", { name: "10:00 Tomás Pérez" }));

    expect(routerMock.push).toHaveBeenCalledWith(`/today/private/${privateLessonId}`);
  });
});
