import { createBackDismissal } from "@/navigation/layerHistory";
import { FakeLayerHistory } from "@/testing/fakeLayerHistory";

describe("When back is pressed after a sheet closed", () => {
  it("Then the app goes back to the previous screen", () => {
    const history = new FakeLayerHistory("/today");
    history.navigateTo("/settings");
    const closeSheet = jest.fn();
    const layerEntry = createBackDismissal(history).open(closeSheet);
    layerEntry.removeAfterNavigation();
    history.runPendingTasks();

    history.pressBack();

    expect(history.navigationBackPresses).toEqual(["/today"]);
    expect(closeSheet).not.toHaveBeenCalled();
  });
});
