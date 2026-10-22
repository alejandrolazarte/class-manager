import { useQueryClient } from "@tanstack/react-query";
import { useRouter } from "expo-router";
import {
  createContext,
  PropsWithChildren,
  useCallback,
  useContext,
  useEffect,
  useState,
} from "react";
import { PlanOfferSheet } from "@/features/subscriptions/components/PlanOfferSheet";
import {
  endedPlanOffer,
  featurePlanOffer,
  PlanOffer,
  planOfferOfRefusal,
} from "@/features/subscriptions/planOffer";
import { useSubscription } from "@/features/subscriptions/useSubscription";
import { planTabs, routes } from "@/navigation/routes";
import { useCurrentTab } from "@/navigation/useCurrentTab";

type ShowPlanOffer = (offer: PlanOffer) => void;

const PlanOfferContext = createContext<ShowPlanOffer | null>(null);

const mutationErrorAction = "error";

export function PlanOfferProvider({ children }: PropsWithChildren) {
  const router = useRouter();
  const tab = useCurrentTab(planTabs);
  const queryClient = useQueryClient();
  const [offer, setOffer] = useState<PlanOffer | null>(null);

  useEffect(
    () =>
      queryClient.getMutationCache().subscribe((event) => {
        if (event.type !== "updated" || event.action.type !== mutationErrorAction) {
          return;
        }
        const refusalOffer = planOfferOfRefusal(event.action.error);
        if (refusalOffer !== null) {
          setOffer(refusalOffer);
        }
      }),
    [queryClient],
  );

  const seePlans = () => {
    setOffer(null);
    router.push(routes.plan(tab));
  };

  return (
    <PlanOfferContext.Provider value={setOffer}>
      {children}
      {offer ? (
        <PlanOfferSheet offer={offer} onSeePlans={seePlans} onClose={() => setOffer(null)} />
      ) : null}
    </PlanOfferContext.Provider>
  );
}

function useShowPlanOffer(): ShowPlanOffer {
  const showPlanOffer = useContext(PlanOfferContext);
  if (showPlanOffer === null) {
    throw new Error("useOfferPlan must be used inside PlanOfferProvider");
  }
  return showPlanOffer;
}

export function useOfferPlan(): (featureCode: string) => void {
  const showPlanOffer = useShowPlanOffer();
  const { isActive } = useSubscription();
  return useCallback(
    (featureCode: string) =>
      showPlanOffer(isActive ? featurePlanOffer(featureCode) : endedPlanOffer),
    [showPlanOffer, isActive],
  );
}

export function usePlanAllowing(
  featureCode: string,
  isAllowed: boolean,
): (action: () => void) => () => void {
  const offerPlan = useOfferPlan();
  const { isActive } = useSubscription();
  return (action) => (isActive && isAllowed ? action : () => offerPlan(featureCode));
}
