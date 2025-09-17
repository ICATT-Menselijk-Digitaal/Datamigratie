import { get } from "@/utils/fetchWrapper";

export type DETZaaktype = {
  actief: boolean;
  naam: string;
  omschrijving: string;
  functioneleIdentificatie: string;
  closedZaken?: number;
};

export const detService = {
  getAllZaaktypes: (): Promise<DETZaaktype[]> => get<DETZaaktype[]>(`/api/det/zaaktypen`),
  getZaaktypeByFunctioneleIdentificatie: (functioneleIdentificatie: string): Promise<DETZaaktype> =>
    get<DETZaaktype>(`/api/det/zaaktypen/${functioneleIdentificatie}`)
};
