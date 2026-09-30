import MaterialIcons from "@expo/vector-icons/MaterialIcons";
import { ComponentProps } from "react";
import { ColorValue } from "react-native";
import { ThemeColorToken } from "@/theme/themeColorTokens";
import { useTheme } from "@/theme/useTheme";

type MaterialIconsGlyph = ComponentProps<typeof MaterialIcons>["name"];

const iconGlyphs = {
  today: "today",
  classes: "calendar-month",
  monthView: "calendar-view-month",
  weekView: "view-week",
  students: "group",
  fees: "payments",
  settings: "settings",
  brand: "pool",
  add: "add",
  remove: "remove",
  previous: "chevron-left",
  next: "chevron-right",
  back: "arrow-back",
  close: "close",
  expand: "expand-more",
  collapse: "expand-less",
  present: "check",
  absent: "close",
  allPresent: "done-all",
  paid: "check-circle",
  cancelled: "event-busy",
  schedule: "schedule",
  substitute: "swap-horiz",
  roles: "admin-panel-settings",
  notYet: "hourglass-top",
  enroll: "person-add",
  privateLesson: "person",
  unenroll: "person-remove",
  edit: "edit",
  monthlyFee: "sell",
  instructors: "sports",
  business: "storefront",
  classPacks: "confirmation-number",
  appearance: "contrast",
  palette: "palette",
  signOut: "logout",
  password: "lock-reset",
  search: "search",
  call: "call",
  whatsApp: "chat",
  celebration: "celebration",
  cash: "payments",
  transfer: "account-balance",
  card: "credit-card",
  otherMethod: "more-horiz",
  warning: "warning-amber",
  error: "error-outline",
  delete: "delete-outline",
  importExport: "import-export",
  info: "info-outline",
  download: "file-download",
  upload: "file-upload",
  products: "shopping-bag",
  orders: "receipt-long",
  delivered: "inventory",
  refund: "undo",
  home: "home",
  selected: "radio-button-checked",
  unselected: "radio-button-unchecked",
  noClasses: "event-available",
  photo: "image",
  streak: "local-fire-department",
  comment: "chat-bubble-outline",
  notifications: "notifications",
  announcement: "campaign",
  achievements: "emoji-events",
  flag: "flag",
  firstMilestone: "looks-one",
  trendingUp: "trending-up",
  verified: "verified",
  militaryTech: "military-tech",
  locked: "lock-outline",
  welcome: "play-circle-outline",
} as const satisfies Record<string, MaterialIconsGlyph>;

const iconSizes = {
  small: 16,
  medium: 20,
  large: 22,
  extraLarge: 24,
  huge: 40,
  hero: 60,
} as const;

export type IconName = keyof typeof iconGlyphs;

export type IconSize = keyof typeof iconSizes;

interface IconProps {
  name: IconName;
  size?: IconSize;
  tone?: ThemeColorToken;
  color?: ColorValue;
  accessibilityLabel?: string;
  testID?: string;
}

export function Icon({
  name,
  size = "large",
  tone = "foreground",
  color,
  accessibilityLabel,
  testID,
}: IconProps) {
  const { colors } = useTheme();
  const isDecorative = accessibilityLabel === undefined;
  return (
    <MaterialIcons
      name={iconGlyphs[name]}
      size={iconSizes[size]}
      color={color ?? colors[tone]}
      testID={testID}
      accessible={!isDecorative}
      accessibilityRole={isDecorative ? undefined : "image"}
      accessibilityLabel={accessibilityLabel}
      importantForAccessibility={isDecorative ? "no-hide-descendants" : "yes"}
      aria-hidden={isDecorative}
    />
  );
}
