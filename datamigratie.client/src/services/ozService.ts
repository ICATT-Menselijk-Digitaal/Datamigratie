import { get } from "@/utils/fetchWrapper";

export type OZZaaktype = {
  id: string;
  identificatie: string;
};

export const ozService = {
  getAllZaaktypes: (): Promise<OZZaaktype[]> =>
    get<OZZaaktype[]>(`/api/oz/zaaktypen`).then((ozZaaktypes) =>
      ozZaaktypes.sort((a, b) => a.identificatie.localeCompare(b.identificatie))
    )
};
