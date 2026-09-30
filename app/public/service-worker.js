const appShellCacheName = "class-manager-app-shell-v1";
const appShellPath = "/";

self.addEventListener("install", (event) => {
  event.waitUntil(
    caches
      .open(appShellCacheName)
      .then((cache) => cache.add(appShellPath))
      .then(() => self.skipWaiting()),
  );
});

self.addEventListener("activate", (event) => {
  event.waitUntil(
    caches
      .keys()
      .then((cacheNames) =>
        Promise.all(
          cacheNames
            .filter((cacheName) => cacheName !== appShellCacheName)
            .map((cacheName) => caches.delete(cacheName)),
        ),
      )
      .then(() => self.clients.claim()),
  );
});

self.addEventListener("fetch", (event) => {
  if (event.request.mode !== "navigate") {
    return;
  }
  event.respondWith(
    fetch(event.request)
      .then((response) => {
        if (response.ok) {
          const responseToCache = response.clone();
          caches.open(appShellCacheName).then((cache) => cache.put(appShellPath, responseToCache));
        }
        return response;
      })
      .catch(() => caches.match(appShellPath)),
  );
});

const defaultNotificationTitle = "Class Manager";
const notificationIconPath = "/icons/icon-192.png";
const defaultNotificationUrl = "/";

self.addEventListener("push", (event) => {
  const message = event.data ? event.data.json() : {};
  event.waitUntil(
    self.registration.showNotification(message.title || defaultNotificationTitle, {
      body: message.body,
      icon: notificationIconPath,
      lang: "es",
      data: { url: message.url || defaultNotificationUrl },
    }),
  );
});

self.addEventListener("notificationclick", (event) => {
  event.notification.close();
  const url = new URL(
    (event.notification.data && event.notification.data.url) || defaultNotificationUrl,
    self.location.origin,
  ).href;
  event.waitUntil(
    self.clients.matchAll({ type: "window", includeUncontrolled: true }).then((windows) => {
      const openWindow = windows.find((client) => client.url.startsWith(self.location.origin));
      if (openWindow === undefined) {
        return self.clients.openWindow(url);
      }
      return openWindow.focus().then((client) => client.navigate(url));
    }),
  );
});
