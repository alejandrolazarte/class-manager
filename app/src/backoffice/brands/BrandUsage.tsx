import { View } from "react-native";
import { featureLabel } from "@/backoffice/brands/brandLabels";
import { BrandDetail } from "@/backoffice/brands/brandsApi";
import { formatMegabytes } from "@/formatting/storageSize";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { ProgressBar } from "@/ui/ProgressBar";

const branchesFeatureCode = "branches";
const studentsFeatureCode = "students";
const teamFeatureCode = "team";
const documentStorageFeatureCode = "document-storage";
const nearLimitRatio = 0.9;

interface UsageMeterProps {
  featureCode: string;
  used: number;
  limit: number | null;
}

function amountText(featureCode: string, amount: number): string | number {
  return featureCode === documentStorageFeatureCode ? formatMegabytes(amount) : amount;
}

function UsageMeter({ featureCode, used, limit }: UsageMeterProps) {
  const ratio = limit === null || limit === 0 ? 0 : used / limit;
  return (
    <View className="gap-1.5">
      <View className="flex-row gap-3">
        <AppText variant="bodyStrong" className="flex-1">
          {featureLabel(featureCode)}
        </AppText>
        <AppText tone="muted">
          {limit === null
            ? translate("backoffice.brand.usageUnlimited", {
                used: amountText(featureCode, used),
              })
            : translate("backoffice.brand.usageOf", {
                used: amountText(featureCode, used),
                limit: amountText(featureCode, limit),
              })}
        </AppText>
      </View>
      {limit === null ? null : (
        <View className="flex-row">
          <ProgressBar ratio={ratio} tone={ratio >= nearLimitRatio ? "warning" : "primary"} />
        </View>
      )}
    </View>
  );
}

export function BrandUsage({ brand }: { brand: BrandDetail }) {
  const limitOf = (featureCode: string) =>
    brand.features.find((feature) => feature.code === featureCode);
  const branchesFeature = limitOf(branchesFeatureCode);
  const hasSeveralBranches = brand.branches.length > 1;
  return (
    <View className="gap-4">
      {branchesFeature === undefined ? null : (
        <UsageMeter
          featureCode={branchesFeatureCode}
          used={brand.branches.length}
          limit={branchesFeature.limit}
        />
      )}
      {brand.branches.map((branch) => {
        const usedByFeature: Record<string, number> = {
          [studentsFeatureCode]: branch.studentCount,
          [teamFeatureCode]: branch.teamCount,
          [documentStorageFeatureCode]: branch.storageMegabytes,
        };
        return (
          <View key={branch.id} className="gap-3">
            {hasSeveralBranches ? (
              <AppText variant="label" tone="muted">
                {branch.name}
              </AppText>
            ) : null}
            {Object.entries(usedByFeature).map(([featureCode, used]) => {
              const feature = limitOf(featureCode);
              return feature === undefined ? null : (
                <UsageMeter
                  key={featureCode}
                  featureCode={featureCode}
                  used={used}
                  limit={feature.limit}
                />
              );
            })}
          </View>
        );
      })}
    </View>
  );
}

export const countedFeatureCodes: readonly string[] = [
  branchesFeatureCode,
  studentsFeatureCode,
  teamFeatureCode,
  documentStorageFeatureCode,
];
