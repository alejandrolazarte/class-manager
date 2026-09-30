import { Platform } from "react-native";

export type BrowserPushSupport = "supported" | "unsupported" | "blocked";

export interface BrowserPushSubscription {
  endpoint: string;
  p256dh: string;
  auth: string;
}

const deniedPermission = "denied";
const grantedPermission = "granted";

function hasPushApi(): boolean {
  return (
    Platform.OS === "web" &&
    typeof navigator !== "undefined" &&
    "serviceWorker" in navigator &&
    typeof window !== "undefined" &&
    "PushManager" in window &&
    "Notification" in window
  );
}

export function browserPushSupport(): BrowserPushSupport {
  if (!hasPushApi()) {
    return "unsupported";
  }
  return Notification.permission === deniedPermission ? "blocked" : "supported";
}

function toBrowserSubscription(subscription: PushSubscription): BrowserPushSubscription | null {
  const { endpoint, keys } = subscription.toJSON();
  if (endpoint === undefined || keys?.p256dh === undefined || keys.auth === undefined) {
    return null;
  }
  return { endpoint, p256dh: keys.p256dh, auth: keys.auth };
}

async function pushManager(): Promise<PushManager | null> {
  if (!hasPushApi()) {
    return null;
  }
  const registration = await navigator.serviceWorker.ready;
  return registration.pushManager;
}

function applicationServerKeyOf(publicKey: string): ArrayBuffer {
  const base64 = publicKey.replace(/-/g, "+").replace(/_/g, "/");
  const padded = base64.padEnd(Math.ceil(base64.length / 4) * 4, "=");
  return Uint8Array.from(atob(padded), (character) => character.charCodeAt(0)).buffer;
}

export async function currentBrowserSubscription(): Promise<BrowserPushSubscription | null> {
  const manager = await pushManager();
  const subscription = await manager?.getSubscription();
  return subscription ? toBrowserSubscription(subscription) : null;
}

export async function subscribeBrowser(publicKey: string): Promise<BrowserPushSubscription | null> {
  const manager = await pushManager();
  if (manager === null || (await Notification.requestPermission()) !== grantedPermission) {
    return null;
  }
  const subscription = await manager.subscribe({
    userVisibleOnly: true,
    applicationServerKey: applicationServerKeyOf(publicKey),
  });
  return toBrowserSubscription(subscription);
}

export async function unsubscribeBrowser(): Promise<string | null> {
  const manager = await pushManager();
  const subscription = await manager?.getSubscription();
  if (!subscription) {
    return null;
  }
  await subscription.unsubscribe();
  return subscription.endpoint;
}
