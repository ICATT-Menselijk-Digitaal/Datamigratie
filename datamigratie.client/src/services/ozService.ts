import { get } from "@/utils/fetchWrapper";

export type OZResultaattype = {
  url: string;
  omschrijving: string;
  id: string;
};

export type OZZaaktype = {
  id: string;
  identificatie: string;
  resultaattypen?: OZResultaattype[];
};

export const ozService = {
  getAllZaaktypes: (): Promise<OZZaaktype[]> =>
    get<OZZaaktype[]>(`/api/oz/zaaktypen`).then((ozZaaktypes) =>
      ozZaaktypes.sort((a, b) => a.identificatie.localeCompare(b.identificatie))
    ),
  getZaaktypeById: (zaaktypeId: string): Promise<OZZaaktype> =>
    get<OZZaaktype>(`/api/oz/zaaktypen/${zaaktypeId}`)
};
