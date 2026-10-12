import { Linking, View } from "react-native";
import { materialFileSizeLabel } from "@/features/classGroups/materialFileSize";
import { StudentAppClassMaterial } from "@/features/studentApp/types";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { HairlineRow } from "@/ui/HairlineRow";
import { Icon, IconName } from "@/ui/Icon";
import { IconCircle } from "@/ui/IconCircle";
import { SectionTitle } from "@/ui/SectionTitle";

interface ClassMaterialListProps {
  material: StudentAppClassMaterial;
}

interface MaterialRowProps {
  icon: IconName;
  trailingIcon: IconName;
  title: string;
  detail: string;
  accessibilityLabel: string;
  url: string;
}

function MaterialRow({
  icon,
  trailingIcon,
  title,
  detail,
  accessibilityLabel,
  url,
}: MaterialRowProps) {
  return (
    <HairlineRow accessibilityLabel={accessibilityLabel} onPress={() => Linking.openURL(url)}>
      <IconCircle icon={icon} tone="primary" />
      <View className="min-w-0 flex-1 gap-0.5">
        <AppText variant="bodyStrong">{title}</AppText>
        <AppText variant="caption" tone="muted" numberOfLines={1}>
          {detail}
        </AppText>
      </View>
      <Icon name={trailingIcon} size="small" tone="primary" />
    </HairlineRow>
  );
}

export function ClassMaterialList({ material }: ClassMaterialListProps) {
  const { className, linkUrl, file } = material;
  return (
    <View className="gap-1">
      <SectionTitle title={translate("student.materials.title")} />
      <View>
        {linkUrl === null ? null : (
          <MaterialRow
            icon="link"
            trailingIcon="openExternal"
            title={translate("student.materials.link")}
            detail={linkUrl}
            accessibilityLabel={translate("student.materials.openLink", { className })}
            url={linkUrl}
          />
        )}
        {file === null ? null : (
          <MaterialRow
            icon="document"
            trailingIcon="next"
            title={translate("student.materials.file")}
            detail={materialFileSizeLabel(file.sizeInBytes)}
            accessibilityLabel={translate("student.materials.openFile", { className })}
            url={file.url}
          />
        )}
      </View>
    </View>
  );
}
