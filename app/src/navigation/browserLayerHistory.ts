import { LayerHistory } from "@/navigation/layerHistory";

const LayerKeyStateField = "layerKey";
const PopStateEvent = "popstate";

export const browserLayerHistory: LayerHistory = {
  pushLayerEntry(layerKey) {
    window.history.pushState(
      { ...window.history.state, [LayerKeyStateField]: layerKey },
      "",
      window.location.href,
    );
  },
  currentLayerKey() {
    return window.history.state?.[LayerKeyStateField];
  },
  goBack(steps) {
    window.history.go(-steps);
  },
  listenToBack(listener) {
    window.addEventListener(
      PopStateEvent,
      (event) => listener(() => event.stopImmediatePropagation()),
      { capture: true },
    );
  },
};
