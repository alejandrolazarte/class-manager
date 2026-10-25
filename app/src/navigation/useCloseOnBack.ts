import { NavigationContext } from "expo-router/react-navigation";
import { useContext, useEffect, useRef } from "react";
import { Platform } from "react-native";
import { browserLayerHistory } from "@/navigation/browserLayerHistory";
import { BackDismissal, createBackDismissal } from "@/navigation/layerHistory";

const BeforeRemoveEvent = "beforeRemove";

let browserBackDismissal: BackDismissal | undefined;

function backDismissal(): BackDismissal {
  browserBackDismissal ??= createBackDismissal(browserLayerHistory);
  return browserBackDismissal;
}

export function useCloseOnBack(onClose: () => void): void {
  const navigation = useContext(NavigationContext);
  const onCloseReference = useRef(onClose);

  useEffect(() => {
    onCloseReference.current = onClose;
  });

  useEffect(() => {
    if (Platform.OS !== "web") {
      return undefined;
    }
    const layerEntry = backDismissal().open(() => onCloseReference.current());
    const stopListeningToRemoval = navigation?.addListener(BeforeRemoveEvent, layerEntry.remove);
    return () => {
      stopListeningToRemoval?.();
      layerEntry.removeAfterNavigation();
    };
  }, [navigation]);
}
