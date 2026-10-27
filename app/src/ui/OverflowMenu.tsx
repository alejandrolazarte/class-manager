import { useRef, useState } from "react";
import { Modal, Pressable, View, useWindowDimensions } from "react-native";
import { translate } from "@/i18n/translate";
import { useCloseOnBack } from "@/navigation/useCloseOnBack";
import { ThemeScope } from "@/theme/ThemeScope";
import { AppText } from "@/ui/AppText";
import { useFloatingShadowStyle } from "@/ui/elevation";
import { IconButton } from "@/ui/IconButton";
import { Entrance } from "@/ui/motion/Entrance";

export interface OverflowMenuItem {
  key: string;
  label: string;
  onPress: () => void;
}

interface OverflowMenuProps {
  accessibilityLabel: string;
  items: readonly OverflowMenuItem[];
}

interface MenuPosition {
  top: number;
  right: number;
}

const screenEdgeGap = 8;
const initialMenuPosition: MenuPosition = { top: screenEdgeGap, right: screenEdgeGap };

export function OverflowMenu({ accessibilityLabel, items }: OverflowMenuProps) {
  const anchorReference = useRef<View>(null);
  const { width: windowWidth } = useWindowDimensions();
  const [menuPosition, setMenuPosition] = useState<MenuPosition | null>(null);

  function open() {
    setMenuPosition(initialMenuPosition);
    anchorReference.current?.measureInWindow((left, top, width) =>
      setMenuPosition({ top, right: Math.max(screenEdgeGap, windowWidth - left - width) }),
    );
  }

  function choose(item: OverflowMenuItem) {
    setMenuPosition(null);
    item.onPress();
  }

  return (
    <View ref={anchorReference} collapsable={false}>
      <IconButton icon="more" accessibilityLabel={accessibilityLabel} onPress={open} />
      {menuPosition ? (
        <OverflowMenuPopover
          position={menuPosition}
          items={items}
          onChoose={choose}
          onClose={() => setMenuPosition(null)}
        />
      ) : null}
    </View>
  );
}

interface OverflowMenuPopoverProps {
  position: MenuPosition;
  items: readonly OverflowMenuItem[];
  onChoose: (item: OverflowMenuItem) => void;
  onClose: () => void;
}

function OverflowMenuPopover({ position, items, onChoose, onClose }: OverflowMenuPopoverProps) {
  useCloseOnBack(onClose);
  const floatingShadowStyle = useFloatingShadowStyle();
  return (
    <Modal transparent visible animationType="none" onRequestClose={onClose}>
      <ThemeScope className="flex-1">
        <Pressable
          accessibilityRole="button"
          accessibilityLabel={translate("common.close")}
          onPress={onClose}
          className="absolute inset-0"
        />
        <View className="absolute" style={position}>
          <Entrance kind="scale">
            <View
              accessibilityRole="menu"
              className="min-w-56 rounded-xl bg-surface py-2"
              style={floatingShadowStyle}
            >
              {items.map((item) => (
                <Pressable
                  key={item.key}
                  accessibilityRole="menuitem"
                  accessibilityLabel={item.label}
                  onPress={() => onChoose(item)}
                  className="px-5 py-3.5 active:bg-muted"
                >
                  <AppText variant="body">{item.label}</AppText>
                </Pressable>
              ))}
            </View>
          </Entrance>
        </View>
      </ThemeScope>
    </Modal>
  );
}
