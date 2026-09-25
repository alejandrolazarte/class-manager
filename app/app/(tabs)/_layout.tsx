import { Tabs } from "expo-router";
import { ColorValue, Text } from "react-native";
import { RedirectWhenSignedOut } from "@/features/authentication/components/RedirectWhenSignedOut";
import { BusinessProvider } from "@/features/business/BusinessProvider";
import { translate } from "@/i18n/translate";
import { brandColor } from "@/ui/colors";

function TabIcon({ glyph, color }: { glyph: string; color: ColorValue }) {
  return <Text style={{ color, fontSize: 20 }}>{glyph}</Text>;
}

export default function TabsLayout() {
  return (
    <RedirectWhenSignedOut>
      <BusinessProvider>
        <Tabs screenOptions={{ tabBarActiveTintColor: brandColor }}>
          <Tabs.Screen
            name="today"
            options={{
              title: translate("tabs.today"),
              headerShown: false,
              tabBarIcon: ({ color }) => <TabIcon glyph="✅" color={color} />,
            }}
          />
          <Tabs.Screen
            name="classes"
            options={{
              title: translate("tabs.classes"),
              headerShown: false,
              tabBarIcon: ({ color }) => <TabIcon glyph="📅" color={color} />,
            }}
          />
          <Tabs.Screen
            name="students"
            options={{
              title: translate("tabs.students"),
              headerShown: false,
              tabBarIcon: ({ color }) => <TabIcon glyph="👤" color={color} />,
            }}
          />
          <Tabs.Screen
            name="fees"
            options={{
              title: translate("tabs.fees"),
              headerShown: false,
              tabBarIcon: ({ color }) => <TabIcon glyph="💰" color={color} />,
            }}
          />
          <Tabs.Screen
            name="settings"
            options={{
              title: translate("tabs.settings"),
              headerShown: false,
              tabBarIcon: ({ color }) => <TabIcon glyph="⚙️" color={color} />,
            }}
          />
        </Tabs>
      </BusinessProvider>
    </RedirectWhenSignedOut>
  );
}
