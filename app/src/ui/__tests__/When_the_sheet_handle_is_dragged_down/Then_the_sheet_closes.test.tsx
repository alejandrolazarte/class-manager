import { fireEvent, render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { AppText } from "@/ui/AppText";
import { BottomSheet } from "@/ui/BottomSheet";

jest.mock("@/ui/motion/useReducedMotion", () => ({ useReducedMotion: () => true }));

const dragStartTime = 1000;
const dragEndTime = 1400;
const dragStartY = 500;
const dragEndY = 700;

function touchHistoryAt(pageY: number, timeStamp: number) {
  return {
    numberActiveTouches: 1,
    indexOfSingleActiveTouch: 0,
    mostRecentTimeStamp: timeStamp,
    touchBank: [
      {
        touchActive: true,
        startPageX: 0,
        startPageY: dragStartY,
        startTimeStamp: dragStartTime,
        currentPageX: 0,
        currentPageY: pageY,
        currentTimeStamp: timeStamp,
        previousPageX: 0,
        previousPageY: dragStartY,
        previousTimeStamp: dragStartTime,
      },
    ],
  };
}

describe("When the sheet handle is dragged down", () => {
  it("Then the sheet closes", async () => {
    const onClose = jest.fn();
    await render(
      <ThemeProvider initialColorSchemePreference="light">
        <BottomSheet onClose={onClose}>
          <AppText>Apariencia</AppText>
        </BottomSheet>
      </ThemeProvider>,
    );
    const handle = screen.getByTestId("bottom-sheet-drag-handle");

    await fireEvent(handle, "responderGrant", {
      touchHistory: touchHistoryAt(dragStartY, dragStartTime),
    });
    await fireEvent(handle, "responderMove", {
      touchHistory: touchHistoryAt(dragEndY, dragEndTime),
    });
    await fireEvent(handle, "responderRelease", {
      touchHistory: { ...touchHistoryAt(dragEndY, dragEndTime), numberActiveTouches: 0 },
    });

    expect(onClose).toHaveBeenCalled();
  });
});
