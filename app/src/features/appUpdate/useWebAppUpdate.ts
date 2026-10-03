import { useEffect, useState } from "react";
import { Platform } from "react-native";
import { isNewerVersionPublished, loadedEntryScriptPath } from "@/features/appUpdate/webAppVersion";

const visibleDocumentState = "visible";
const visibilityChangeEvent = "visibilitychange";
const periodicCheckMilliseconds = 15 * 60 * 1000;

export function useWebAppUpdate(): boolean {
  const [isUpdateAvailable, setIsUpdateAvailable] = useState(false);

  useEffect(() => {
    if (Platform.OS !== "web" || isUpdateAvailable) {
      return;
    }
    const currentEntryScriptPath = loadedEntryScriptPath();
    if (currentEntryScriptPath === null) {
      return;
    }
    let isSubscribed = true;
    const checkForUpdate = () => {
      if (document.visibilityState !== visibleDocumentState) {
        return;
      }
      isNewerVersionPublished(currentEntryScriptPath).then((isNewer) => {
        if (isNewer && isSubscribed) {
          setIsUpdateAvailable(true);
        }
      });
    };
    document.addEventListener(visibilityChangeEvent, checkForUpdate);
    const intervalId = setInterval(checkForUpdate, periodicCheckMilliseconds);
    return () => {
      isSubscribed = false;
      document.removeEventListener(visibilityChangeEvent, checkForUpdate);
      clearInterval(intervalId);
    };
  }, [isUpdateAvailable]);

  return isUpdateAvailable;
}
