import { useState } from "react";
import { Linking, View } from "react-native";
import { GoogleMapsLinkHelpSheet } from "@/features/business/components/GoogleMapsLinkHelpSheet";
import {
  BusinessLocationValues,
  hasLocationMapsUrl,
  isLocationMapsUrlValid,
  locationMapsUrlMaximumLength,
  locationNameMaximumLength,
} from "@/features/business/businessLocation";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { Icon } from "@/ui/Icon";
import { TextField } from "@/ui/TextField";
import { TextLink } from "@/ui/TextLink";

const googleMapsPlaceholder = "{googleMaps}";
const googleMapsName = "Google Maps";

interface BusinessLocationFieldsProps {
  values: BusinessLocationValues;
  onChange: (values: BusinessLocationValues) => void;
  locationNameError?: string;
  locationMapsUrlError?: string;
}

function LinkExplanation() {
  const [beforeGoogleMaps, afterGoogleMaps] = translate("businessLocation.linkHint").split(
    googleMapsPlaceholder,
  );
  return (
    <AppText variant="caption" tone="subtle">
      {beforeGoogleMaps}
      <AppText variant="link">{googleMapsName}</AppText>
      {afterGoogleMaps}
    </AppText>
  );
}

export function BusinessLocationFields({
  values,
  onChange,
  locationNameError,
  locationMapsUrlError,
}: BusinessLocationFieldsProps) {
  const [isLinkTouched, setIsLinkTouched] = useState(false);
  const [isHelpOpen, setIsHelpOpen] = useState(false);
  const hasLink = hasLocationMapsUrl(values);
  const isLinkValid = isLocationMapsUrlValid(values);
  const isValidLinkShown = hasLink && isLinkValid;
  const linkError =
    locationMapsUrlError ??
    (isLinkTouched && !isLinkValid ? translate("businessLocation.linkInvalid") : undefined);

  return (
    <View className="gap-3.5 border-t border-border pt-[18px]">
      <View className="flex-row items-center gap-2">
        <Icon name="location" size="medium" tone="primary" />
        <AppText variant="heading" accessibilityRole="header">
          {translate("businessLocation.title")}
        </AppText>
      </View>
      <View className="gap-2">
        <TextField
          label={translate("businessLocation.link")}
          isLabelHidden
          placeholder={translate("businessLocation.linkPlaceholder")}
          keyboardType="url"
          autoCapitalize="none"
          autoCorrect={false}
          maxLength={locationMapsUrlMaximumLength}
          value={values.locationMapsUrl}
          onChangeText={(locationMapsUrl) => onChange({ ...values, locationMapsUrl })}
          onBlur={() => setIsLinkTouched(true)}
          errorMessage={linkError}
          hint={isValidLinkShown ? translate("businessLocation.linkValid") : undefined}
          isHintSatisfied={isValidLinkShown}
        />
        {isValidLinkShown ? (
          <>
            <Button
              variant="secondary"
              size="medium"
              icon="openExternal"
              label={translate("businessLocation.tryLink")}
              onPress={() => Linking.openURL(values.locationMapsUrl.trim())}
            />
            <AppText variant="caption" tone="subtle">
              {translate("businessLocation.tryLinkHint")}
            </AppText>
          </>
        ) : (
          <>
            <LinkExplanation />
            <TextLink
              label={translate("businessLocation.howToCopy")}
              onPress={() => setIsHelpOpen(true)}
            />
          </>
        )}
      </View>
      <TextField
        label={translate("businessLocation.name")}
        isRequired={hasLink}
        placeholder={translate("businessLocation.namePlaceholder")}
        hint={translate("businessLocation.nameHint")}
        maxLength={locationNameMaximumLength}
        value={values.locationName}
        onChangeText={(locationName) => onChange({ ...values, locationName })}
        errorMessage={locationNameError}
      />
      {isHelpOpen ? <GoogleMapsLinkHelpSheet onClose={() => setIsHelpOpen(false)} /> : null}
    </View>
  );
}
