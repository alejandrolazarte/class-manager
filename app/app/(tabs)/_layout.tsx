import { Tabs } from "expo-router";
import { RedirectWhenSignedOut } from "@/features/authentication/components/RedirectWhenSignedOut";
import { BusinessProvider } from "@/features/business/BusinessProvider";
import { translate } from "@/i18n/translate";
import { Icon } from "@/ui/Icon";

export default function TabsLayout() {
  return (
    <RedirectWhenSignedOut>
      <BusinessProvider>
        <Tabs>
          <Tabs.Screen
            name="today"
            options={{
              title: translate("tabs.today"),
              headerShown: false,
              tabBarIcon: ({ color }) => <Icon name="today" color={color} />,
            }}
          />
          <Tabs.Screen
            name="classes"
            options={{
              title: translate("tabs.classes"),
              headerShown: false,
              tabBarIcon: ({ color }) => <Icon name="classes" color={color} />,
            }}
          />
          <Tabs.Screen
            name="students"
            options={{
              title: translate("tabs.students"),
              headerShown: false,
              tabBarIcon: ({ color }) => <Icon name="students" color={color} />,
            }}
          />
          <Tabs.Screen
            name="fees"
            options={{
              title: translate("tabs.fees"),
              headerShown: false,
              tabBarIcon: ({ color }) => <Icon name="fees" color={color} />,
            }}
          />
          <Tabs.Screen
            name="settings"
            options={{
              title: translate("tabs.settings"),
              headerShown: false,
              tabBarIcon: ({ color }) => <Icon name="settings" color={color} />,
            }}
          />
        </Tabs>
      </BusinessProvider>
    </RedirectWhenSignedOut>
  );
}
