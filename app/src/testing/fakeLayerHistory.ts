import { LayerHistory } from "@/navigation/layerHistory";

interface HistoryEntry {
  page: string;
  layerKey?: string;
}

export class FakeLayerHistory implements LayerHistory {
  readonly navigationBackPresses: string[] = [];
  private entries: HistoryEntry[];
  private index = 0;
  private backListener: ((stopPropagation: () => void) => void) | undefined;
  private pendingTasks: (() => void)[] = [];

  constructor(page: string) {
    this.entries = [{ page }];
  }

  get currentPage(): string {
    return this.entries[this.index]!.page;
  }

  get length(): number {
    return this.entries.length;
  }

  pushLayerEntry(layerKey: string): void {
    this.push({ page: this.currentPage, layerKey });
  }

  currentLayerKey(): string | undefined {
    return this.entries[this.index]!.layerKey;
  }

  goBack(steps: number): void {
    this.pendingTasks.push(() => this.traverseBack(steps));
  }

  afterNavigation(task: () => void): void {
    this.pendingTasks.push(task);
  }

  runPendingTasks(): void {
    while (this.pendingTasks.length > 0) {
      this.pendingTasks.shift()!();
    }
  }

  listenToBack(listener: (stopPropagation: () => void) => void): void {
    this.backListener = listener;
  }

  navigateTo(page: string): void {
    this.push({ page });
  }

  pressBack(): void {
    this.traverseBack(1);
  }

  private traverseBack(steps: number): void {
    this.index -= steps;
    this.notifyPop();
  }

  private push(entry: HistoryEntry): void {
    this.entries = [...this.entries.slice(0, this.index + 1), entry];
    this.index = this.entries.length - 1;
  }

  private notifyPop(): void {
    let isStopped = false;
    this.backListener?.(() => {
      isStopped = true;
    });
    if (!isStopped) {
      this.navigationBackPresses.push(this.currentPage);
    }
  }
}
