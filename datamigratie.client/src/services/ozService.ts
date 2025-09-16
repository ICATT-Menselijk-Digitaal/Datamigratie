import { get } from "@/utils/fetchWrapper";

export type OZZaaktype = {
  uuid: string;
  naam: string;
};

export const ozService = {
  getAllZaaktypes: (): Promise<OZZaaktype[]> => get<OZZaaktype[]>(`/api/oz/zaaktypen`)
};
