import {
  createContext,
  PropsWithChildren,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
} from "react";
import { View } from "react-native";
import { AppText } from "@/ui/AppText";
import { Icon } from "@/ui/Icon";

const toastDurationMilliseconds = 3000;

interface ToastContextValue {
  showToast: (message: string) => void;
}

const ToastContext = createContext<ToastContextValue>({ showToast: () => undefined });

export function ToastProvider({ children }: PropsWithChildren) {
  const [visibleMessage, setVisibleMessage] = useState<string | null>(null);

  useEffect(() => {
    if (visibleMessage === null) {
      return;
    }
    const timeoutId = setTimeout(() => setVisibleMessage(null), toastDurationMilliseconds);
    return () => clearTimeout(timeoutId);
  }, [visibleMessage]);

  const showToast = useCallback((message: string) => setVisibleMessage(message), []);
  const contextValue = useMemo(() => ({ showToast }), [showToast]);

  return (
    <ToastContext.Provider value={contextValue}>
      {children}
      {visibleMessage !== null ? (
        <View pointerEvents="none" className="absolute bottom-24 left-0 right-0 items-center px-4">
          <View
            accessibilityRole="alert"
            className="flex-row items-center gap-2 rounded-[14px] bg-inverse px-[18px] py-3"
          >
            <Icon name="paid" size="medium" tone="success" />
            <AppText variant="bodyStrong" tone="inverse" className="font-label">
              {visibleMessage}
            </AppText>
          </View>
        </View>
      ) : null}
    </ToastContext.Provider>
  );
}

export function useToast(): ToastContextValue {
  return useContext(ToastContext);
}
