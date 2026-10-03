import { View } from "react-native";
import { CatalogImage } from "@/features/catalogImages/components/CatalogImage";
import { CatalogImageDraftState } from "@/features/catalogImages/useCatalogImageDraft";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { IconName } from "@/ui/Icon";

interface CatalogImageFieldProps {
  image: CatalogImageDraftState;
  placeholderIcon: IconName;
}

export function CatalogImageField({ image, placeholderIcon }: CatalogImageFieldProps) {
  const hasImage = image.previewUri !== null;
  return (
    <Card className="flex-row items-center gap-3.5 p-4">
      <CatalogImage
        imageUri={image.previewUri}
        placeholderIcon={placeholderIcon}
        className="h-[76px] w-[76px] rounded-3xl"
      />
      <View className="min-w-0 flex-1 gap-2">
        <AppText variant="bodyStrong">{translate("catalogImages.photo")}</AppText>
        <View className="flex-row flex-wrap gap-2">
          <Button
            size="medium"
            icon="upload"
            label={translate(hasImage ? "catalogImages.change" : "catalogImages.upload")}
            onPress={image.pick}
          />
          {hasImage ? (
            <Button
              size="medium"
              variant="dangerOutline"
              label={translate("catalogImages.remove")}
              onPress={image.remove}
            />
          ) : null}
        </View>
        <AppText variant="caption" tone={image.errorMessage === null ? "subtle" : "danger"}>
          {image.errorMessage ?? translate("catalogImages.hint")}
        </AppText>
      </View>
    </Card>
  );
}
