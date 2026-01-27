import { get } from "@/utils/fetchWrapper";

export type DetStatus = {
  naam: string;
  omschrijving: string;
  actief: boolean;
  eind: boolean;
};

export type DetResultaat = {
  naam: string;
  actief: boolean;
  omschrijving?: string;
};

export type DetResultaattypen = {
   resultaat: DetResultaat;
};

export type DetDocumenttype = {
  naam: string;
  omschrijving?: string;
  actief: boolean;
};

export type DETZaaktype = {
  actief: boolean;
  naam: string;
  omschrijving: string;
  functioneleIdentificatie: string;
  closedZakenCount?: number;
  statuses?: DetStatus[];
  resultaten?: DetResultaattypen[];
  documenttypen?: DetDocumenttype[];
};

export const detService = {
  getAllZaaktypes: (): Promise<DETZaaktype[]> =>
    get<DETZaaktype[]>(`/api/det/zaaktypen`).then((detZaaktypes) =>
      detZaaktypes.sort((a, b) => a.naam.localeCompare(b.naam))
    ),
  getZaaktypeById: (detZaaktypeId: string): Promise<DETZaaktype> =>
    get<DETZaaktype>(`/api/det/zaaktypen/${detZaaktypeId}`)
};
