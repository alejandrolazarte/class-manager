import { View } from "react-native";
import { CatalogImage } from "@/features/catalogImages/types";
import { ProductPhotoCarousel } from "@/features/studentApp/components/ProductPhotoCarousel";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { PressableScale } from "@/ui/motion/PressableScale";

interface ClassOfferCardAction {
  label: string;
  accessibilityLabel: string;
  isSelected: boolean;
  onPress: () => void;
}

interface ClassOfferCardProps {
  name: string;
  images: CatalogImage[];
  badge: string;
  details: string;
  onOpen: () => void;
  action?: ClassOfferCardAction;
}

export function ClassOfferBadge({ label }: { label: string }) {
  return (
    <View
      pointerEvents="none"
      className="absolute left-2.5 top-2.5 h-6 flex-row items-center rounded-full bg-surface px-2"
    >
      <AppText variant="badge" tone="primarySoft">
        {label}
      </AppText>
    </View>
  );
}

export function ClassOfferCard({
  name,
  images,
  badge,
  details,
  onOpen,
  action,
}: ClassOfferCardProps) {
  return (
    <View className="w-[200px] gap-0.5">
      <PressableScale
        accessibilityRole="button"
        accessibilityLabel={name}
        onPress={onOpen}
        className="gap-0.5 active:opacity-80"
      >
        <View className="overflow-hidden rounded-[14px]">
          <ProductPhotoCarousel
            images={images}
            sizeClassName="h-[96px]"
            placeholderIcon="classPacks"
          />
          <ClassOfferBadge label={badge} />
        </View>
        <AppText variant="bodyStrong" className="mt-1.5">
          {name}
        </AppText>
        <AppText variant="footnote" tone="muted">
          {details}
        </AppText>
      </PressableScale>
      {action === undefined ? null : (
        <View className="mt-2">
          <Button
            size="medium"
            variant={action.isSelected ? "secondary" : "primary"}
            label={action.label}
            accessibilityLabel={action.accessibilityLabel}
            onPress={action.onPress}
          />
        </View>
      )}
    </View>
  );
}
