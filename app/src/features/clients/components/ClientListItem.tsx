import { Pressable, Text } from "react-native";
import { formatPhoneNumberForDisplay } from "@/features/clients/phoneNumberFormatting";
import { Client } from "@/features/clients/types";

interface ClientListItemProps {
  client: Client;
  onPress: (client: Client) => void;
}

export function ClientListItem({ client, onPress }: ClientListItemProps) {
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityLabel={client.fullName}
      onPress={() => onPress(client)}
      className="gap-1 border-b border-gray-100 bg-white px-4 py-3"
    >
      <Text className="text-base font-semibold text-gray-900">{client.fullName}</Text>
      <Text className="text-sm text-gray-600">
        {formatPhoneNumberForDisplay(client.phoneNumber)}
      </Text>
      {client.notes ? (
        <Text numberOfLines={1} className="text-sm text-gray-500">
          {client.notes}
        </Text>
      ) : null}
    </Pressable>
  );
}
