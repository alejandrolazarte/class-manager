import { useEffect, useMemo, useState } from "react";
import { View } from "react-native";
import { isApiError } from "@/api/httpClient";
import { brandErrorCodes } from "@/features/brand/brandErrorCodes";
import { useCurrentBrand, welcomeDurationInMilliseconds } from "@/features/brand/BrandProvider";
import { BrandLogo } from "@/features/brand/components/BrandLogo";
import { BrandPreview } from "@/features/brand/components/BrandPreview";
import { BrandWelcome } from "@/features/brand/components/BrandWelcome";
import { ColorSwatchGrid } from "@/features/brand/components/ColorSwatchGrid";
import { logoMaximumSizeInBytes, pickLogoFile } from "@/features/brand/pickLogoFile";
import { Brand } from "@/features/brand/types";
import {
  useRemoveBrandLogo,
  useUpdateBrand,
  useUploadBrandLogo,
} from "@/features/brand/useBrandMutations";
import { useCurrentBusiness } from "@/features/business/CurrentBusinessProvider";
import { SettingsFormScreenLayout } from "@/features/settings/components/SettingsFormScreenLayout";
import { SubmissionFailure, toSubmissionFailure } from "@/features/settings/submissionFailure";
import { translate, TranslationKey } from "@/i18n/translate";
import { findBrandColorClash, StatusColorName } from "@/theme/brandColorClash";
import { accentBrandSwatches, mainBrandSwatches } from "@/theme/brandSwatches";
import { deriveBrandTheme } from "@/theme/deriveBrandTheme";
import { ThemePreviewScope } from "@/theme/ThemePreviewScope";
import { useTheme } from "@/theme/useTheme";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { Chip } from "@/ui/Chip";
import { LoadingScreen } from "@/ui/LoadingScreen";
import { TextField } from "@/ui/TextField";
import { ToggleSwitch } from "@/ui/ToggleSwitch";
import { useToast } from "@/ui/ToastProvider";

const hexColorPattern = /^#[0-9a-f]{6}$/i;

const clashMessageKeys: Record<StatusColorName, TranslationKey> = {
  danger: "brand.settings.clashDanger",
  warning: "brand.settings.clashWarning",
  success: "brand.settings.clashSuccess",
};

const logoErrorKeys: Record<string, TranslationKey> = {
  [brandErrorCodes.unsupportedLogo]: "brand.settings.logoUnsupported",
  [brandErrorCodes.logoTooLarge]: "brand.settings.logoTooLarge",
};

interface BrandDraft {
  brandName: string;
  themeColor: string | null;
  themeColorInput: string;
  accentColor: string | null;
  locksTheme: boolean;
}

function draftOf(brand: Brand): BrandDraft {
  return {
    brandName: brand.brandName ?? "",
    themeColor: brand.themeColor,
    themeColorInput: brand.themeColor ?? "",
    accentColor: brand.accentColor,
    locksTheme: brand.locksTheme,
  };
}

function logoErrorMessage(uploadError: unknown): string {
  const problemCode = isApiError(uploadError) ? uploadError.problem.code : undefined;
  const messageKey = problemCode === undefined ? undefined : logoErrorKeys[problemCode];
  return translate(messageKey ?? "common.unexpectedError");
}

export function BrandSettingsScreen() {
  const currentBrand = useCurrentBrand();
  const brand = currentBrand?.brand ?? null;
  return brand === null ? (
    <LoadingScreen />
  ) : (
    <BrandSettingsForm brand={brand} logoUri={currentBrand?.logoUri ?? null} />
  );
}

function BrandSettingsForm({ brand, logoUri }: { brand: Brand; logoUri: string | null }) {
  const business = useCurrentBusiness();
  const { colors, colorScheme } = useTheme();
  const { showToast } = useToast();
  const updateBrandMutation = useUpdateBrand();
  const uploadLogoMutation = useUploadBrandLogo();
  const removeLogoMutation = useRemoveBrandLogo();
  const [draft, setDraft] = useState<BrandDraft>(() => draftOf(brand));
  const [submissionFailure, setSubmissionFailure] = useState<SubmissionFailure | null>(null);
  const [logoError, setLogoError] = useState<string | null>(null);
  const [isWelcomePreviewVisible, setIsWelcomePreviewVisible] = useState(false);

  useEffect(() => {
    if (!isWelcomePreviewVisible) {
      return;
    }
    const timeout = setTimeout(
      () => setIsWelcomePreviewVisible(false),
      welcomeDurationInMilliseconds,
    );
    return () => clearTimeout(timeout);
  }, [isWelcomePreviewVisible]);

  const previewColors = useMemo(
    () =>
      draft.themeColor === null
        ? colors
        : deriveBrandTheme({ themeColor: draft.themeColor, accentColor: draft.accentColor })[
            colorScheme
          ],
    [draft.themeColor, draft.accentColor, colors, colorScheme],
  );
  const displayName = draft.brandName.trim() === "" ? business.name : draft.brandName.trim();
  const clash = draft.themeColor === null ? null : findBrandColorClash(draft.themeColor);
  const isAdjusted =
    draft.themeColor !== null &&
    colorScheme === "light" &&
    previewColors.primary !== draft.themeColor.toLowerCase();
  const isThemeColorInputValid =
    draft.themeColorInput === "" || hexColorPattern.test(draft.themeColorInput);

  const updateDraft = (changes: Partial<BrandDraft>) =>
    setDraft((current) => ({ ...current, ...changes }));
  const selectThemeColor = (themeColor: string) =>
    updateDraft({ themeColor, themeColorInput: themeColor });
  const changeThemeColorInput = (themeColorInput: string) =>
    updateDraft(
      hexColorPattern.test(themeColorInput)
        ? { themeColorInput, themeColor: themeColorInput.toLowerCase() }
        : themeColorInput === ""
          ? { themeColorInput, themeColor: null, locksTheme: false }
          : { themeColorInput },
    );
  const clearThemeColor = () =>
    updateDraft({ themeColor: null, themeColorInput: "", locksTheme: false });

  const uploadLogo = async () => {
    setLogoError(null);
    const file = await pickLogoFile();
    if (file === null) {
      return;
    }
    if (file.size !== undefined && file.size > logoMaximumSizeInBytes) {
      setLogoError(translate("brand.settings.logoTooLarge"));
      return;
    }
    try {
      await uploadLogoMutation.mutateAsync(file);
      showToast(translate("brand.settings.logoSaved"));
    } catch (uploadError) {
      setLogoError(logoErrorMessage(uploadError));
    }
  };

  const removeLogo = async () => {
    setLogoError(null);
    try {
      await removeLogoMutation.mutateAsync();
    } catch (removeError) {
      setLogoError(logoErrorMessage(removeError));
    }
  };

  const save = async () => {
    setSubmissionFailure(null);
    try {
      await updateBrandMutation.mutateAsync({
        brandName: draft.brandName.trim() === "" ? null : draft.brandName.trim(),
        themeColor: draft.themeColor,
        accentColor: draft.accentColor,
        locksTheme: draft.themeColor !== null && draft.locksTheme,
      });
      showToast(translate("brand.settings.saved"));
    } catch (saveError) {
      setSubmissionFailure(toSubmissionFailure(saveError));
    }
  };

  return (
    <View className="flex-1">
      <SettingsFormScreenLayout
        eyebrow={translate("settings.title")}
        title={translate("brand.settings.title")}
        subtitle={translate("brand.settings.subtitle")}
        submissionFailure={submissionFailure}
        onRetry={save}
      >
        <Card className="flex-row items-center gap-3.5 p-4">
          <BrandLogo displayName={displayName} logoUri={logoUri} size="medium" />
          <View className="min-w-0 flex-1 gap-2">
            <AppText variant="bodyStrong">{translate("brand.settings.logo")}</AppText>
            <View className="flex-row flex-wrap gap-2">
              <Button
                size="medium"
                icon="upload"
                label={translate(
                  logoUri === null ? "brand.settings.uploadLogo" : "brand.settings.changeLogo",
                )}
                isLoading={uploadLogoMutation.isPending}
                onPress={uploadLogo}
              />
              {logoUri === null ? null : (
                <Button
                  size="medium"
                  variant="dangerOutline"
                  label={translate("brand.settings.removeLogo")}
                  isLoading={removeLogoMutation.isPending}
                  onPress={removeLogo}
                />
              )}
            </View>
            <AppText variant="caption" tone={logoError === null ? "subtle" : "danger"}>
              {logoError ?? translate("brand.settings.logoHint")}
            </AppText>
          </View>
        </Card>
        <TextField
          label={translate("brand.settings.name")}
          hint={translate("brand.settings.nameHint")}
          placeholder={business.name}
          autoCapitalize="words"
          value={draft.brandName}
          onChangeText={(brandName) => updateDraft({ brandName })}
        />
        <Card className="gap-3 p-4">
          <View className="gap-0.5">
            <AppText variant="bodyStrong">{translate("brand.settings.mainColor")}</AppText>
            <AppText variant="caption" tone="muted">
              {translate("brand.settings.mainColorHint")}
            </AppText>
          </View>
          <ColorSwatchGrid
            colors={mainBrandSwatches}
            selectedColor={draft.themeColor}
            onSelect={selectThemeColor}
            accessibilityLabelPrefix={translate("brand.settings.mainColor")}
          />
          <TextField
            label={translate("brand.settings.hexColor")}
            autoCapitalize="none"
            autoCorrect={false}
            placeholder={mainBrandSwatches[0]}
            value={draft.themeColorInput}
            onChangeText={changeThemeColorInput}
            errorMessage={
              isThemeColorInputValid ? undefined : translate("brand.settings.hexColorInvalid")
            }
          />
          {clash === null ? null : (
            <Banner tone="warning" message={translate(clashMessageKeys[clash])} />
          )}
          {isAdjusted ? (
            <AppText variant="caption" tone="muted">
              {translate("brand.settings.adjusted")}
            </AppText>
          ) : null}
          {draft.themeColor === null ? null : (
            <Button
              variant="ghost"
              size="medium"
              label={translate("brand.settings.useAppColors")}
              onPress={clearThemeColor}
            />
          )}
        </Card>
        {draft.themeColor === null ? null : (
          <Card className="gap-3 p-4">
            <View className="gap-0.5">
              <AppText variant="bodyStrong">{translate("brand.settings.accentColor")}</AppText>
              <AppText variant="caption" tone="muted">
                {translate("brand.settings.accentColorHint")}
              </AppText>
            </View>
            <ColorSwatchGrid
              colors={accentBrandSwatches}
              selectedColor={draft.accentColor}
              onSelect={(accentColor) => updateDraft({ accentColor })}
              accessibilityLabelPrefix={translate("brand.settings.accentColor")}
            />
            <View className="flex-row">
              <Chip
                label={translate("brand.settings.noAccent")}
                isSelected={draft.accentColor === null}
                onPress={() => updateDraft({ accentColor: null })}
              />
            </View>
          </Card>
        )}
        <View className="gap-2">
          <AppText variant="label" tone="muted">
            {translate("brand.settings.preview")}
          </AppText>
          <BrandPreview colors={previewColors} />
        </View>
        {draft.themeColor === null ? null : (
          <View className="gap-1.5">
            <ToggleSwitch
              label={translate("brand.settings.lock")}
              value={draft.locksTheme}
              onValueChange={(locksTheme) => updateDraft({ locksTheme })}
            />
            <AppText variant="caption" tone="muted" className="px-1">
              {translate("brand.settings.lockHint")}
            </AppText>
          </View>
        )}
        <Button
          variant="secondary"
          icon="welcome"
          label={translate("brand.settings.showWelcome")}
          onPress={() => setIsWelcomePreviewVisible(true)}
        />
        <View className="flex-row gap-2.5">
          <View className="flex-1">
            <Button
              variant="secondary"
              label={translate("brand.settings.reset")}
              onPress={() => setDraft(draftOf(brand))}
            />
          </View>
          <View className="flex-[2]">
            <Button
              label={translate("brand.settings.save")}
              disabled={!isThemeColorInputValid}
              isLoading={updateBrandMutation.isPending}
              onPress={save}
            />
          </View>
        </View>
      </SettingsFormScreenLayout>
      {isWelcomePreviewVisible ? (
        <ThemePreviewScope colors={previewColors} className="absolute inset-0">
          <BrandWelcome
            displayName={displayName}
            logoUri={logoUri}
            onDismiss={() => setIsWelcomePreviewVisible(false)}
          />
        </ThemePreviewScope>
      ) : null}
    </View>
  );
}
