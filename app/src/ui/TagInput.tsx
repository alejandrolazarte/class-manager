import { useState } from "react";
import { TextInput, View } from "react-native";
import { AppText, textVariantClassNames } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { IconButton } from "@/ui/IconButton";

export interface TagInputPreset {
  label: string;
  values: readonly string[];
}

interface TagInputProps {
  label: string;
  values: readonly string[];
  onChange: (values: string[]) => void;
  addLabel: string;
  removeLabelOf: (value: string) => string;
  duplicateMessageOf: (value: string) => string;
  placeholder?: string;
  suggestions?: readonly string[];
  presets?: readonly TagInputPreset[];
  maximumLength?: number;
  errorMessage?: string;
}

const valueSeparator = ",";
const maximumSuggestionCount = 6;

function isSameValue(first: string, second: string): boolean {
  return first.toLowerCase() === second.toLowerCase();
}

export function TagInput({
  label,
  values,
  onChange,
  addLabel,
  removeLabelOf,
  duplicateMessageOf,
  placeholder,
  suggestions = [],
  presets = [],
  maximumLength,
  errorMessage,
}: TagInputProps) {
  const [draft, setDraft] = useState("");
  const [duplicateValue, setDuplicateValue] = useState<string | null>(null);
  const trimmedDraft = draft.trim();

  const addAll = (rawValues: readonly string[]) => {
    const addedValues = [...values];
    let repeatedValue: string | null = null;
    for (const rawValue of rawValues) {
      const value = rawValue.trim();
      if (value.length === 0) {
        continue;
      }
      if (addedValues.some((existingValue) => isSameValue(existingValue, value))) {
        repeatedValue = value;
        continue;
      }
      addedValues.push(value);
    }
    setDuplicateValue(repeatedValue);
    if (addedValues.length > values.length) {
      onChange(addedValues);
    }
  };

  const add = (rawValue: string) => {
    setDraft("");
    addAll([rawValue]);
  };

  const changeDraft = (text: string) => {
    if (!text.includes(valueSeparator)) {
      setDuplicateValue(null);
      setDraft(text);
      return;
    }
    const typedValues = text.split(valueSeparator);
    setDraft(typedValues.pop() ?? "");
    addAll(typedValues);
  };

  const remove = (value: string) => {
    setDuplicateValue(null);
    onChange(values.filter((existingValue) => existingValue !== value));
  };

  const matchingSuggestions =
    trimmedDraft.length === 0
      ? []
      : suggestions
          .filter(
            (suggestion) =>
              suggestion.toLowerCase().startsWith(trimmedDraft.toLowerCase()) &&
              !isSameValue(suggestion, trimmedDraft) &&
              !values.some((existingValue) => isSameValue(existingValue, suggestion)),
          )
          .slice(0, maximumSuggestionCount);
  const showsPresets = presets.length > 0 && values.length === 0 && trimmedDraft.length === 0;

  return (
    <View className="gap-2">
      <AppText variant="label" tone="muted">
        {label}
      </AppText>
      <View className="flex-row gap-2">
        <TextInput
          accessibilityLabel={label}
          value={draft}
          onChangeText={changeDraft}
          onSubmitEditing={() => add(draft)}
          submitBehavior="submit"
          returnKeyType="done"
          placeholder={placeholder}
          maxLength={maximumLength}
          className={`h-[46px] min-w-0 flex-1 rounded-[10px] border bg-surface px-3.5 text-foreground ${textVariantClassNames.input} ${errorMessage ? "border-danger" : "border-border focus:border-primary"}`}
        />
        <Button
          size="medium"
          label={addLabel}
          onPress={() => add(draft)}
          disabled={trimmedDraft.length === 0}
        />
      </View>
      {showsPresets ? (
        <View className="flex-row flex-wrap gap-2">
          {presets.map((preset) => (
            <Button
              key={preset.label}
              variant="dashed"
              size="medium"
              icon="add"
              label={preset.label}
              onPress={() => onChange([...preset.values])}
            />
          ))}
        </View>
      ) : null}
      {matchingSuggestions.length > 0 ? (
        <View className="flex-row flex-wrap gap-2">
          {matchingSuggestions.map((suggestion) => (
            <Button
              key={suggestion}
              variant="dashed"
              size="medium"
              icon="add"
              label={suggestion}
              onPress={() => add(suggestion)}
            />
          ))}
        </View>
      ) : null}
      {duplicateValue === null ? null : (
        <AppText variant="label" tone="danger" accessibilityRole="alert">
          {duplicateMessageOf(duplicateValue)}
        </AppText>
      )}
      {errorMessage ? (
        <AppText variant="label" tone="danger" accessibilityRole="alert">
          {errorMessage}
        </AppText>
      ) : null}
      {values.length > 0 ? (
        <View className="flex-row flex-wrap gap-1.5">
          {values.map((value) => (
            <View
              key={value}
              className="h-[34px] flex-row items-center rounded-full bg-primary-soft pl-3"
            >
              <AppText variant="bodyStrong" tone="primarySoft">
                {value}
              </AppText>
              <IconButton
                variant="compact"
                icon="close"
                tone="primary-soft-foreground"
                accessibilityLabel={removeLabelOf(value)}
                onPress={() => remove(value)}
              />
            </View>
          ))}
        </View>
      ) : null}
    </View>
  );
}
