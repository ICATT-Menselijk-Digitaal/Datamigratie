import { get } from "@/utils/fetchWrapper";

export type DETZaaktype = {
  actief: boolean;
  naam: string;
  omschrijving: string;
  functioneleIdentificatie: string;
  closedZakenCount?: number;
};

export const detService = {
  getAllZaaktypes: (): Promise<DETZaaktype[]> =>
    get<DETZaaktype[]>(`/api/det/zaaktypen`).then((detZaaktypes) =>
      detZaaktypes.sort((a, b) => a.naam.localeCompare(b.naam))
    ),
  getZaaktypeById: (detZaaktypeId: string): Promise<DETZaaktype> =>
    get<DETZaaktype>(`/api/det/zaaktypen/${detZaaktypeId}`)
};
