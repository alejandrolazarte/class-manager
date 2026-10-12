import { Pressable, ScrollView, View } from "react-native";
import { previewUriOf } from "@/features/catalogImages/catalogImageDraft";
import { CatalogImage } from "@/features/catalogImages/components/CatalogImage";
import { CatalogImageDraftState } from "@/features/catalogImages/useCatalogImageDraft";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { IconButton } from "@/ui/IconButton";
import { IconName } from "@/ui/Icon";
import { HairlineSection } from "@/ui/HairlineSection";

interface CatalogImageFieldProps {
  images: CatalogImageDraftState;
  placeholderIcon: IconName;
  maximumImageCount: number;
}

export function CatalogImageField({
  images,
  placeholderIcon,
  maximumImageCount,
}: CatalogImageFieldProps) {
  const { items } = images.draft;
  const canAddImage = items.length < maximumImageCount;
  const hint =
    maximumImageCount === 1
      ? translate("catalogImages.singleHint")
      : translate("catalogImages.hint", { maximum: maximumImageCount });
  return (
    <HairlineSection title={translate("catalogImages.photos")}>
      {items.length === 0 ? (
        <CatalogImage
          imageUri={null}
          placeholderIcon={placeholderIcon}
          className="h-[76px] w-[76px] rounded-[14px]"
        />
      ) : (
        <ScrollView
          horizontal
          showsHorizontalScrollIndicator={false}
          contentContainerClassName="gap-3"
        >
          {items.map((item, index) => {
            const position = index + 1;
            return (
              <View key={item.key} className="items-center gap-1">
                <Pressable
                  accessibilityRole="button"
                  accessibilityLabel={translate("catalogImages.makeMain", { position })}
                  accessibilityState={{ disabled: index === 0 }}
                  disabled={index === 0}
                  onPress={() => images.makeMain(item.key)}
                >
                  <CatalogImage
                    imageUri={previewUriOf(item)}
                    placeholderIcon={placeholderIcon}
                    className="h-[76px] w-[76px] rounded-[14px]"
                  />
                </Pressable>
                <View className="absolute -right-2 -top-2 rounded-full bg-surface">
                  <IconButton
                    icon="close"
                    accessibilityLabel={translate("catalogImages.remove", { position })}
                    onPress={() => images.remove(item.key)}
                  />
                </View>
                <AppText variant="caption" tone={index === 0 ? "primary" : "subtle"}>
                  {translate(index === 0 ? "catalogImages.main" : "catalogImages.tapToMakeMain")}
                </AppText>
              </View>
            );
          })}
        </ScrollView>
      )}
      {canAddImage ? (
        <View className="flex-row">
          <Button
            size="medium"
            icon="upload"
            label={translate("catalogImages.add")}
            onPress={images.pick}
          />
        </View>
      ) : null}
      <AppText variant="caption" tone={images.errorMessage === null ? "subtle" : "danger"}>
        {images.errorMessage ?? hint}
      </AppText>
    </HairlineSection>
  );
}
