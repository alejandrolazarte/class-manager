import { Linking, Pressable, View } from "react-native";
import { StudentAppClassMaterial } from "@/features/studentApp/types";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Card } from "@/ui/Card";
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
    <View className="gap-3">
      <SectionTitle title={translate("student.materials.title")} />
      {materials.map((material) => (
        <Pressable
          key={material.classGroupId}
          accessibilityRole="link"
          accessibilityLabel={translate("student.materials.open", {
            className: material.className,
          })}
          onPress={() => Linking.openURL(material.url)}
          className="active:opacity-70"
        >
          <Card className="flex-row items-center gap-3 px-4 py-3.5">
            <View className="h-10 w-10 items-center justify-center rounded-full bg-primary-soft">
              <Icon name="material" tone="primary" />
            </View>
            <View className="min-w-0 flex-1 gap-0.5">
              <AppText variant="bodyStrong">{material.className}</AppText>
              <AppText variant="link" tone="primary">
                {translate("student.materials.see")}
              </AppText>
            </View>
            <Icon name="next" size="small" tone="primary" />
          </Card>
        </Pressable>
      ))}
    </View>
  );
}
