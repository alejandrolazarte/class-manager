import { useState } from "react";
import { NativeScrollEvent, NativeSyntheticEvent, ScrollView, View } from "react-native";
import { CatalogImage as CatalogImageView } from "@/features/catalogImages/components/CatalogImage";
import { CatalogImage } from "@/features/catalogImages/types";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Icon, IconName } from "@/ui/Icon";

export type PhotoCarouselSize = "card" | "page";

interface ProductPhotoCarouselProps {
  images: CatalogImage[];
  sizeClassName: string;
  placeholderIcon?: IconName;
  size?: PhotoCarouselSize;
  isDimmed?: boolean;
}

interface PhotoDotsClassNames {
  current: string;
  other: string;
  offset: string;
  row: string;
}

const photoDotsClassNames: Record<PhotoCarouselSize, PhotoDotsClassNames> = {
  card: {
    current: "h-[5px] w-3",
    other: "h-[5px] w-[5px]",
    offset: "bottom-2",
    row: "gap-1 px-[7px] py-1",
  },
  page: {
    current: "h-[7px] w-[18px]",
    other: "h-[7px] w-[7px]",
    offset: "bottom-3.5",
    row: "gap-1.5 px-2.5 py-1.5",
  },
};

interface PhotoDotsProps {
  count: number;
  position: number;
  size: PhotoCarouselSize;
}

function PhotoDots({ count, position, size }: PhotoDotsProps) {
  const classNames = photoDotsClassNames[size];
  return (
    <View
      pointerEvents="none"
      className={`absolute left-0 right-0 items-center ${classNames.offset}`}
    >
      <View className={`flex-row rounded-full bg-background/55 ${classNames.row}`}>
        {Array.from({ length: count }, (_, index) => (
          <View
            key={index}
            className={`rounded-full ${index === position ? `bg-foreground ${classNames.current}` : `bg-foreground/50 ${classNames.other}`}`}
          />
        ))}
      </View>
    </View>
  );
}

interface PhotoBadgeProps {
  label: string;
  accessibilityLabel: string;
  icon?: IconName;
}

function PhotoBadge({ label, accessibilityLabel, icon }: PhotoBadgeProps) {
  return (
    <View
      pointerEvents="none"
      accessible
      accessibilityLabel={accessibilityLabel}
      className="absolute right-2 top-2 h-6 flex-row items-center gap-[3px] rounded-full bg-background/60 px-2"
    >
      {icon === undefined ? null : <Icon name={icon} size="small" />}
      <AppText variant="badge">{label}</AppText>
    </View>
  );
}

export function ProductPhotoCarousel({
  images,
  sizeClassName,
  placeholderIcon = "products",
  size = "card",
  isDimmed = false,
}: ProductPhotoCarouselProps) {
  const [width, setWidth] = useState(0);
  const [position, setPosition] = useState(0);
  if (images.length <= 1) {
    return (
      <CatalogImageView
        imageUri={images[0]?.url ?? null}
        placeholderIcon={placeholderIcon}
        className={`w-full ${sizeClassName}`}
        isDimmed={isDimmed}
      />
    );
  }
  const followScroll = (event: NativeSyntheticEvent<NativeScrollEvent>) => {
    if (width === 0) {
      return;
    }
    const scrolledPosition = Math.round(event.nativeEvent.contentOffset.x / width);
    setPosition(Math.min(Math.max(scrolledPosition, 0), images.length - 1));
  };
  const positionLabel = translate("family.shop.photoPosition", {
    position: position + 1,
    count: images.length,
  });
  return (
    <View
      className={`w-full ${sizeClassName} ${isDimmed ? "opacity-50" : ""}`}
      onLayout={(event) => setWidth(event.nativeEvent.layout.width)}
    >
      <ScrollView
        horizontal
        pagingEnabled
        showsHorizontalScrollIndicator={false}
        scrollEventThrottle={16}
        onScroll={followScroll}
        className="flex-1"
      >
        {images.map((image) => (
          <View key={image.id} style={{ width }}>
            <CatalogImageView
              imageUri={image.url}
              placeholderIcon={placeholderIcon}
              className="h-full w-full"
            />
          </View>
        ))}
      </ScrollView>
      <PhotoDots count={images.length} position={position} size={size} />
      {size === "card" ? (
        <PhotoBadge
          icon="photo"
          label={String(images.length)}
          accessibilityLabel={translate("family.shop.photoCount", { count: images.length })}
        />
      ) : (
        <PhotoBadge label={positionLabel} accessibilityLabel={positionLabel} />
      )}
    </View>
  );
}
