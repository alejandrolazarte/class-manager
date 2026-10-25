export interface LayerHistory {
  pushLayerEntry(layerKey: string): void;
  currentLayerKey(): string | undefined;
  goBack(steps: number): void;
  afterNavigation(task: () => void): void;
  listenToBack(listener: (stopPropagation: () => void) => void): void;
}

interface OpenLayer {
  key: string;
  isOpen: boolean;
  close: () => void;
}

export interface LayerEntry {
  remove(): void;
  removeAfterNavigation(): void;
}

export interface BackDismissal {
  open(close: () => void): LayerEntry;
}

export function createBackDismissal(history: LayerHistory): BackDismissal {
  const layersWithEntries: OpenLayer[] = [];
  let pendingOwnBacks = 0;
  let nextLayerNumber = 0;

  const areEntriesOnTop = () =>
    history.currentLayerKey() === layersWithEntries[layersWithEntries.length - 1]?.key;

  history.listenToBack((stopPropagation) => {
    if (pendingOwnBacks > 0) {
      pendingOwnBacks -= 1;
      stopPropagation();
      return;
    }
    const topLayer = layersWithEntries.pop();
    if (topLayer === undefined) {
      return;
    }
    stopPropagation();
    topLayer.isOpen = false;
    topLayer.close();
  });

  const release = (layer: OpenLayer) => {
    if (!layer.isOpen) {
      return;
    }
    layer.isOpen = false;
    if (!layersWithEntries.includes(layer)) {
      return;
    }
    if (!areEntriesOnTop()) {
      layersWithEntries.length = 0;
      return;
    }
    let steps = 0;
    while (
      layersWithEntries.length > 0 &&
      !layersWithEntries[layersWithEntries.length - 1]!.isOpen
    ) {
      layersWithEntries.pop();
      steps += 1;
    }
    if (steps > 0) {
      pendingOwnBacks += 1;
      history.goBack(steps);
    }
  };

  return {
    open(close) {
      if (layersWithEntries.length > 0 && !areEntriesOnTop()) {
        layersWithEntries.length = 0;
      }
      nextLayerNumber += 1;
      const layer: OpenLayer = { key: `layer-${nextLayerNumber}`, isOpen: true, close };
      history.pushLayerEntry(layer.key);
      layersWithEntries.push(layer);
      return {
        remove: () => release(layer),
        removeAfterNavigation: () => history.afterNavigation(() => release(layer)),
      };
    },
  };
}
