export const stripeDashboardUrl = "https://dashboard.stripe.com/";

export function goToStripePage(url: string): void {
  globalThis.location.assign(url);
}
