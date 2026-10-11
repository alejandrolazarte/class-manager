import { Linking, View } from "react-native";
import { StudentAppClassMaterial } from "@/features/studentApp/types";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { HairlineRow } from "@/ui/HairlineRow";
import { Icon } from "@/ui/Icon";
import { SectionTitle } from "@/ui/SectionTitle";

interface ClassMaterialSectionProps {
  materials: StudentAppClassMaterial[];
}

export function ClassMaterialSection({ materials }: ClassMaterialSectionProps) {
  if (materials.length === 0) {
    return null;
  }

  return (
    <View className="gap-1">
      <SectionTitle title={translate("student.materials.title")} />
      <View>
        {materials.map((material) => (
          <HairlineRow
            key={material.classGroupId}
            accessibilityRole="link"
            accessibilityLabel={translate("student.materials.open", {
              className: material.className,
            })}
            onPress={() => Linking.openURL(material.url)}
            className="flex-row items-center gap-3"
          >
            <View className="min-w-0 flex-1 gap-0.5">
              <AppText variant="bodyStrong">{material.className}</AppText>
              <AppText variant="link" tone="primary">
                {translate("student.materials.see")}
              </AppText>
            </View>
            <Icon name="next" size="small" tone="primary" />
          </HairlineRow>
        ))}
      </View>
    </View>
  );
}
