import {
  createContext,
  PropsWithChildren,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
} from "react";
import { Pressable, View } from "react-native";
import { AppText } from "@/ui/AppText";
import { Icon } from "@/ui/Icon";

const toastDurationMilliseconds = 3000;
const toastWithActionDurationMilliseconds = 6000;

export interface ToastAction {
  label: string;
  onPress: () => void;
}

interface VisibleToast {
  message: string;
  action?: ToastAction;
}

interface ToastContextValue {
  showToast: (message: string, action?: ToastAction) => void;
}

const ToastContext = createContext<ToastContextValue>({ showToast: () => undefined });

export function ToastProvider({ children }: PropsWithChildren) {
  const [visibleToast, setVisibleToast] = useState<VisibleToast | null>(null);

  useEffect(() => {
    if (visibleToast === null) {
      return;
    }
    const timeoutId = setTimeout(
      () => setVisibleToast(null),
      visibleToast.action ? toastWithActionDurationMilliseconds : toastDurationMilliseconds,
    );
    return () => clearTimeout(timeoutId);
  }, [visibleToast]);

  const showToast = useCallback(
    (message: string, action?: ToastAction) => setVisibleToast({ message, action }),
    [],
  );
  const runAction = (action: ToastAction) => {
    setVisibleToast(null);
    action.onPress();
  };
  const contextValue = useMemo(() => ({ showToast }), [showToast]);

  return (
    <ToastContext.Provider value={contextValue}>
      {children}
      {visibleToast !== null ? (
        <View
          pointerEvents={visibleToast.action ? "box-none" : "none"}
          className="absolute bottom-24 left-0 right-0 items-center px-4"
        >
          <View
            accessibilityRole="alert"
            className="flex-row items-center gap-2 rounded-[14px] bg-inverse px-[18px] py-3"
          >
            <Icon name="paid" size="medium" tone="success" />
            <AppText variant="bodyStrong" tone="inverse" className="font-label">
              {visibleToast.message}
            </AppText>
            {visibleToast.action ? (
              <ToastActionButton action={visibleToast.action} onPress={runAction} />
            ) : null}
          </View>
        </View>
      ) : null}
    </ToastContext.Provider>
  );
}

interface ToastActionButtonProps {
  action: ToastAction;
  onPress: (action: ToastAction) => void;
}

function ToastActionButton({ action, onPress }: ToastActionButtonProps) {
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityLabel={action.label}
      hitSlop={12}
      className="ml-2 rounded-lg px-2 py-1 active:opacity-70"
      onPress={() => onPress(action)}
    >
      <AppText variant="bodyStrong" tone="inverse" className="font-label uppercase">
        {action.label}
      </AppText>
    </Pressable>
  );
}

export function useToast(): ToastContextValue {
  return useContext(ToastContext);
}
