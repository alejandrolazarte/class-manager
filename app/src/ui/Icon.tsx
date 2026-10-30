import Ionicons from "@expo/vector-icons/Ionicons";
import { ComponentProps } from "react";
import { ColorValue } from "react-native";
import { ThemeColorToken } from "@/theme/themeColorTokens";
import { useTheme } from "@/theme/useTheme";

type IoniconsGlyph = ComponentProps<typeof Ionicons>["name"];

const iconGlyphs = {
  today: "today-outline",
  classes: "calendar-outline",
  students: "people-outline",
  fees: "wallet-outline",
  settings: "options-outline",
  brand: "water-outline",
  add: "add",
  remove: "remove",
  previous: "chevron-back",
  next: "chevron-forward",
  back: "arrow-back",
  close: "close",
  expand: "chevron-down",
  collapse: "chevron-up",
  present: "checkmark",
  checked: "checkmark",
  absent: "close",
  allPresent: "checkmark-done",
  paid: "checkmark-circle-outline",
  cancelled: "close-circle-outline",
  schedule: "time-outline",
  pickDate: "calendar-outline",
  pickTime: "time-outline",
  substitute: "swap-horizontal",
  roles: "shield-checkmark-outline",
  notYet: "hourglass-outline",
  enroll: "person-add-outline",
  privateLesson: "person-outline",
  edit: "create-outline",
  monthlyFee: "pricetag-outline",
  instructors: "school-outline",
  business: "storefront-outline",
  classPacks: "ticket-outline",
  appearance: "contrast-outline",
  language: "language-outline",
  palette: "color-palette-outline",
  signOut: "log-out-outline",
  password: "key-outline",
  passkey: "finger-print-outline",
  search: "search-outline",
  filter: "options-outline",
  call: "call-outline",
  whatsApp: "chatbubble-ellipses-outline",
  celebration: "sparkles-outline",
  cash: "cash-outline",
  transfer: "business-outline",
  card: "card-outline",
  otherMethod: "ellipsis-horizontal",
  more: "ellipsis-vertical",
  warning: "warning-outline",
  error: "alert-circle-outline",
  delete: "trash-outline",
  info: "information-circle-outline",
  download: "download-outline",
  upload: "cloud-upload-outline",
  products: "bag-handle-outline",
  orders: "receipt-outline",
  delivered: "cube-outline",
  refund: "arrow-undo-outline",
  home: "home-outline",
  noClasses: "calendar-clear-outline",
  photo: "image-outline",
  streak: "flame-outline",
  comment: "chatbubble-outline",
  notifications: "notifications-outline",
  announcement: "megaphone-outline",
  achievements: "trophy-outline",
  flag: "flag-outline",
  firstMilestone: "star-outline",
  trendingUp: "trending-up-outline",
  verified: "ribbon-outline",
  militaryTech: "medal-outline",
  locked: "lock-closed-outline",
  plan: "diamond-outline",
  welcome: "play-circle-outline",
  dayDone: "checkmark-circle-outline",
  forward: "arrow-forward",
  email: "mail-outline",
  studentApp: "phone-portrait-outline",
  findPerson: "search-outline",
  handOver: "archive-outline",
  recurringFee: "repeat-outline",
  fieldError: "alert-circle-outline",
  fieldValid: "checkmark-circle-outline",
  material: "book-outline",
  link: "link-outline",
  document: "document-text-outline",
  openExternal: "open-outline",
  pin: "pin-outline",
  location: "location-outline",
  preview: "eye-outline",
} as const satisfies Record<string, IoniconsGlyph>;

export const iconSizes = {
  tiny: 13,
  small: 16,
  medium: 20,
  large: 22,
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
    <Ionicons
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
