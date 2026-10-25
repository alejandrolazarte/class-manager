import { useQueryClient } from "@tanstack/react-query";
import { Href, useRouter } from "expo-router";
import { useState } from "react";
import { View } from "react-native";
import { formatLongDate } from "@/dates/dates";
import { removeMyPasskey } from "@/features/passkeys/passkeysApi";
import { passkeyQueryKeys } from "@/features/passkeys/passkeyQueryKeys";
import { Passkey } from "@/features/passkeys/types";
import { useMyPasskeys } from "@/features/passkeys/useMyPasskeys";
import { SettingsListScreenLayout } from "@/features/settings/components/SettingsListScreenLayout";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { ConfirmDialog } from "@/ui/ConfirmDialog";
import { HairlineRow } from "@/ui/HairlineRow";
import { useToast } from "@/ui/ToastProvider";

const isoDateLength = 10;

interface PasskeysScreenProps {
  addPasskeyRoute: Href;
}

interface PasskeyRowProps {
  passkey: Passkey;
  onRemove: () => void;
}

function PasskeyRow({ passkey, onRemove }: PasskeyRowProps) {
  return (
    <HairlineRow>
      <View className="min-w-0 flex-1 gap-0.5">
        <AppText variant="bodyStrong">{passkey.name}</AppText>
        <AppText variant="caption" tone="muted">
          {translate("profile.passkeys.addedOn", {
            date: formatLongDate(passkey.createdAt.slice(0, isoDateLength)),
          })}
        </AppText>
      </View>
      <Button
        variant="dangerGhost"
        size="small"
        label={translate("common.remove")}
        accessibilityLabel={translate("profile.passkeys.removeAccessibility", {
          name: passkey.name,
        })}
        onPress={onRemove}
      />
    </HairlineRow>
  );
}

export function PasskeysScreen({ addPasskeyRoute }: PasskeysScreenProps) {
  const router = useRouter();
  const queryClient = useQueryClient();
  const { showToast } = useToast();
  const passkeysQuery = useMyPasskeys();
  const [passkeyToRemove, setPasskeyToRemove] = useState<Passkey | null>(null);
  const [isRemoving, setIsRemoving] = useState(false);

  const removePasskey = async () => {
    if (passkeyToRemove === null) {
      return;
    }
    setIsRemoving(true);
    try {
      await removeMyPasskey(passkeyToRemove.id);
      await queryClient.invalidateQueries({ queryKey: passkeyQueryKeys.mine });
      showToast(translate("profile.passkeys.removed"));
    } catch {
      showToast(translate("common.unexpectedError"));
    } finally {
      setIsRemoving(false);
      setPasskeyToRemove(null);
    }
  };

  return (
    <>
      <SettingsListScreenLayout
        title={translate("profile.passkeys.title")}
        subtitle={translate("profile.passkeys.hint")}
        items={passkeysQuery.data ?? []}
        isPending={passkeysQuery.isPending}
        isError={passkeysQuery.isError}
        isRefetching={passkeysQuery.isRefetching}
        onRefetch={() => passkeysQuery.refetch()}
        keyExtractor={(passkey) => passkey.id}
        renderItem={(passkey) => (
          <PasskeyRow passkey={passkey} onRemove={() => setPasskeyToRemove(passkey)} />
        )}
        emptyMessage={translate("profile.passkeys.empty")}
        emptyIcon="passkey"
        newItemLabel={translate("profile.passkeys.add")}
        onNewItem={() => router.push(addPasskeyRoute)}
      />
      {passkeyToRemove ? (
        <ConfirmDialog
          title={translate("profile.passkeys.removeQuestion")}
          message={translate("profile.passkeys.removeConsequence")}
          confirmLabel={translate("common.remove")}
          onCancel={() => setPasskeyToRemove(null)}
          onConfirm={removePasskey}
          isConfirming={isRemoving}
        />
      ) : null}
    </>
  );
}
