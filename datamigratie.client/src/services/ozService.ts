import { get } from "@/utils/fetchWrapper";

export type OzStatustype = {
  uuid: string;
  omschrijving: string;
  volgnummer: number;
  isEindstatus: boolean;
};

export type OzResultaattype = {
  id: string;
  omschrijving: string;
  url: string;
};

export type OZZaaktype = {
  id: string;
  identificatie: string;
  omschrijving: string;
  statustypes?: OzStatustype[];
  resultaattypen?: OzResultaattype[];

};

export const ozService = {
  getAllZaaktypes: (): Promise<OZZaaktype[]> =>
    get<OZZaaktype[]>(`/api/oz/zaaktypen`).then((ozZaaktypes) =>
      ozZaaktypes.sort((a, b) => a.identificatie.localeCompare(b.identificatie))
    ),
  getZaaktypeById: (ozZaaktypeId: string): Promise<OZZaaktype> =>
    get<OZZaaktype>(`/api/oz/zaaktypen/${ozZaaktypeId}`)
};
