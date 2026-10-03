import { Pressable, ScrollView, View } from "react-native";
import { previewUriOf } from "@/features/catalogImages/catalogImageDraft";
import { CatalogImage } from "@/features/catalogImages/components/CatalogImage";
import { CatalogImageDraftState } from "@/features/catalogImages/useCatalogImageDraft";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { IconButton } from "@/ui/IconButton";
import { IconName } from "@/ui/Icon";

interface CatalogImageFieldProps {
  images: CatalogImageDraftState;
  placeholderIcon: IconName;
}

export function CatalogImageField({ images, placeholderIcon }: CatalogImageFieldProps) {
  const { items } = images.draft;
  return (
    <Card className="gap-3 p-4">
      <AppText variant="bodyStrong">{translate("catalogImages.photos")}</AppText>
      {items.length === 0 ? (
        <CatalogImage
          imageUri={null}
          placeholderIcon={placeholderIcon}
          className="h-[76px] w-[76px] rounded-3xl"
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
                    className="h-[76px] w-[76px] rounded-3xl"
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
      <View className="flex-row">
        <Button
          size="medium"
          icon="upload"
          label={translate("catalogImages.add")}
          onPress={images.pick}
        />
      </View>
      <AppText variant="caption" tone={images.errorMessage === null ? "subtle" : "danger"}>
        {images.errorMessage ?? translate("catalogImages.hint")}
      </AppText>
    </Card>
  );
}
