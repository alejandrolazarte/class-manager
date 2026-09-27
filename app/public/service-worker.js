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
