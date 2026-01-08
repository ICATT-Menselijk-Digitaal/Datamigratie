import { get } from "@/utils/fetchWrapper";

export type DETResultaat = {
  naam: string;
};

export type DETResultaattype = {
  resultaat: DETResultaat;
};

export type DETZaaktype = {
  actief: boolean;
  naam: string;
  omschrijving: string;
  functioneleIdentificatie: string;
  closedZakenCount?: number;
  resultaten?: DETResultaattype[];
};

export const detService = {
  getAllZaaktypes: (): Promise<DETZaaktype[]> =>
    get<DETZaaktype[]>(`/api/det/zaaktypen`).then((detZaaktypes) =>
      detZaaktypes.sort((a, b) => a.naam.localeCompare(b.naam))
    ),
  getZaaktypeById: (detZaaktypeId: string): Promise<DETZaaktype> =>
    get<DETZaaktype>(`/api/det/zaaktypen/${detZaaktypeId}`)
};
