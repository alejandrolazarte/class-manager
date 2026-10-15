import { Fragment } from "react";
import { View } from "react-native";
import { AppText } from "@/ui/AppText";
import { PressableScale } from "@/ui/motion/PressableScale";

interface FooterLink {
  label: string;
  onPress: () => void;
}

interface FooterLinksProps {
  links: readonly FooterLink[];
}

export function FooterLinks({ links }: FooterLinksProps) {
  return (
    <View className="flex-row flex-wrap items-center justify-center">
      {links.map((link, index) => (
        <Fragment key={link.label}>
          {index > 0 ? <View className="h-3 w-px bg-border" /> : null}
          <PressableScale
            accessibilityRole="link"
            onPress={link.onPress}
            hitSlop={8}
            className="px-2.5 py-1.5 active:opacity-70"
          >
            <AppText variant="caption" tone="muted">
              {link.label}
            </AppText>
          </PressableScale>
        </Fragment>
      ))}
    </View>
  );
}
