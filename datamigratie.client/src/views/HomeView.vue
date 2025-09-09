<template>
  <h1>Zaaktypes</h1>

  <form @submit.prevent>
    <div class="form-group">
      <label for="filter">Filter</label>

      <input type="text" id="filter" v-model.trim="search" />
    </div>
  </form>

  <p v-if="!filteredZaaktypes.length">Geen zaaktypes gevonden voor "{{ search }}".</p>

  <ul class="reset">
    <li
      v-for="{ naam, functioneleIdentificatie } in filteredZaaktypes"
      :key="functioneleIdentificatie"
    >
      <router-link
        :to="{
          name: 'zaaktype',
          params: { functioneleIdentificatie },
          ...(search && { query: { search } })
        }"
        class="button button-secondary"
        >{{ naam }} <span>&gt;</span></router-link
      >
    </li>
  </ul>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import type { Zaaktype } from "@/types/zaaktype";
import { useRoute } from "vue-router";

const route = useRoute();

const search = ref("");

const filteredZaaktypes = computed(() => {
  let result = zaaktypes;

  const query = search.value.toLowerCase();

  if (query) {
    result = zaaktypes.filter((zaaktype) => zaaktype.naam.toLowerCase().includes(query));
  }

  return result.sort((a, b) => a.naam.toLowerCase().localeCompare(b.naam.toLowerCase()));
});

onMounted(() => (search.value = String(route.query.search || "")));

const zaaktypes: Zaaktype[] = [
  {
    actief: true,
    naam: "Melding openbare ruimte",
    omschrijving: "Melding openbare ruimte",
    functioneleIdentificatie: "BP_MOR"
  },
  {
    actief: false,
    naam: "Integratie_OpenFormulieren",
    omschrijving: "Integratie_OpenFormulieren",
    functioneleIdentificatie: "Integratie_OpenFormulieren"
  },
  {
    actief: false,
    naam: "Motie behandelen",
    omschrijving: "Motie Gemeenteraad behandelen",
    functioneleIdentificatie: "EMM_MOTIE"
  },
  {
    actief: true,
    naam: "Geboorte aangeven",
    omschrijving: "Aangifte geboorte behandelen",
    functioneleIdentificatie: "DV_GA"
  },
  {
    actief: true,
    naam: "ZaaktypeMarco",
    omschrijving: "ZaaktypeMarco",
    functioneleIdentificatie: "ZaaktypeMarco"
  },
  {
    actief: true,
    naam: "Eerste inschrijving",
    omschrijving: "Eerste inschrijving in Nederland - inschrijving",
    functioneleIdentificatie: "71"
  },
  {
    actief: true,
    naam: "Aanvragen eigen verklaring rijbewijs",
    omschrijving: "Aanvragen eigen verklaring rijbewijs",
    functioneleIdentificatie: "BP_REV"
  },
  {
    actief: false,
    naam: "Toezegging behandelen",
    omschrijving: "Behandelen toezegging College aan Gemeenteraad",
    functioneleIdentificatie: "EMM_TOEZEGGING"
  },
  {
    actief: false,
    naam: "Bestuurlijke besluitvorming behandelen",
    omschrijving: "Voorstel voor bestuurlijke besluitvorming behandelen",
    functioneleIdentificatie: "OND_BBV"
  },
  {
    actief: false,
    naam: "TestzaakWIJZ652",
    omschrijving:
      "Zaaktype ten behoeve van testen Wet modernisering elektronisch bestuurlijk verkeer (WMEBV): inhoudelijke notificatie aan klant bij bericht in MijnLoket/Bedrijvenloket",
    functioneleIdentificatie: "TST_WIJZ652"
  },
  {
    actief: false,
    naam: "Advies",
    omschrijving: "Advies",
    functioneleIdentificatie: "SO_ADV"
  },
  {
    actief: false,
    naam: "Evenementen vooroverleg BPMN ZAC",
    omschrijving: "Evenementen vooroverleg BPMN ZAC",
    functioneleIdentificatie: "EMM_BPMN"
  },
  {
    actief: true,
    naam: "Issues_Testen",
    omschrijving: "Issues_Testen",
    functioneleIdentificatie: "Issues_Testen"
  },
  {
    actief: true,
    naam: "Stookontheffing",
    omschrijving: "Het beoordelen van een aanvraag voor een stookontheffing.",
    functioneleIdentificatie: "SOH"
  },
  {
    actief: true,
    naam: "GEN0Xv3_3",
    omschrijving: "GEN0Xv3.3",
    functioneleIdentificatie: "GEN0Xv3_3"
  },
  {
    actief: true,
    naam: "Deeplink",
    omschrijving: "Deeplink",
    functioneleIdentificatie: "Deeplink"
  },
  {
    actief: false,
    naam: "Bijhouden rechthebbende",
    omschrijving: "Bijhouden rechthebbende",
    functioneleIdentificatie: "2621"
  },
  {
    actief: false,
    naam: "Rotterdam Testzaaktype",
    omschrijving: "Rotterdam Testzaaktype",
    functioneleIdentificatie: "ROT_RTZ"
  },
  {
    actief: true,
    naam: "Handhaving",
    omschrijving: "Handhaving",
    functioneleIdentificatie: "Handhaving"
  },
  {
    actief: true,
    naam: "Vraag of idee indienen",
    omschrijving: "Vraag of idee indienen - test ZGW API's",
    functioneleIdentificatie: "PR_INF"
  },
  {
    actief: true,
    naam: "Incidentele ontheffingskaarten aanvragen",
    omschrijving: "Incidentele ontheffingskaarten aanvragen behandelen",
    functioneleIdentificatie: "BCO_IOA"
  },
  {
    actief: true,
    naam: "ZAC Melding kleine evenementen",
    omschrijving:
      "Voorbeeld voor e-Suite zaakafhandelcomponent (ZAC). Het behandelen van een meldingen van kleine evenementen die onder de categorie 0-evenement vallen.",
    functioneleIdentificatie: "ZAC_MKE"
  },
  {
    actief: false,
    naam: "Aanvraag afschrift burgerlijke stand",
    omschrijving: "Aanvraag uittreksel BRP Burgerzaken proces",
    functioneleIdentificatie: "245"
  },
  {
    actief: false,
    naam: "Aanvraag bewijs van in leven zijn",
    omschrijving: "Aanvraag bewijs van in leven zijn Burgerzaken proces",
    functioneleIdentificatie: "35"
  },
  {
    actief: false,
    naam: "Actualisering PL",
    omschrijving: "Actualisering PL",
    functioneleIdentificatie: "2633"
  },
  {
    actief: false,
    naam: "Adresonderzoek",
    omschrijving: "Onderzoek van adres Burgerzaken proces",
    functioneleIdentificatie: "2608"
  },
  {
    actief: false,
    naam: "Begraven",
    omschrijving: "Begraven",
    functioneleIdentificatie: "23"
  },
  {
    actief: false,
    naam: "Generieke akte / latere vermelding",
    omschrijving: "Aanvragen van generieke akte Burgerzaken proces",
    functioneleIdentificatie: "2600"
  },
  {
    actief: false,
    naam: "Hernoemen openbare ruimte",
    omschrijving: "Hernoemen openbare ruimte",
    functioneleIdentificatie: "2624"
  },
  {
    actief: false,
    naam: "Huwelijk of Partnerschap",
    omschrijving: "Huwelijk / partnerschap iBurgerzaken proces",
    functioneleIdentificatie: "115"
  },
  {
    actief: false,
    naam: "Logistiek proces rijbewijs",
    omschrijving: "Logistiek proces rijbewijs",
    functioneleIdentificatie: "2629"
  },
  {
    actief: false,
    naam: "Naamskeuze",
    omschrijving: "Aanvragen van naamskeuze Burgerzaken proces",
    functioneleIdentificatie: "2601"
  },
  {
    actief: false,
    naam: "Naturalisatie",
    omschrijving: "Naturalisatie",
    functioneleIdentificatie: "166"
  },
  {
    actief: false,
    naam: "Overlijden",
    omschrijving: "Aangeven van overlijden iBurgerzaken proces",
    functioneleIdentificatie: "182"
  },
  {
    actief: false,
    naam: "Rijbewijs uitreiken",
    omschrijving: "Rijbewijs uitreiken",
    functioneleIdentificatie: "2614"
  },
  {
    actief: false,
    naam: "Verhuizing buitenland",
    omschrijving: "Aangeven van emigratie Burgerzaken proces",
    functioneleIdentificatie: "252"
  },
  {
    actief: false,
    naam: "Vondeling",
    omschrijving: "Aangeven vondeling Burgerzaken proces",
    functioneleIdentificatie: "2604"
  },
  {
    actief: false,
    naam: "Voornaam- of Geslachtswijziging",
    omschrijving: "Aanvragen wijziging voornaam en geslacht Burgerzaken proces",
    functioneleIdentificatie: "94"
  },
  {
    actief: false,
    naam: "Beleid opstellen",
    omschrijving: "Beleid opstellen",
    functioneleIdentificatie: "zaaktype-1254"
  },
  {
    actief: false,
    naam: "Controle trouwlocatie",
    omschrijving:
      "Indien er sprake is van een vrije trouwlocatie dan moet er controle op de trouwlocatie worden uitgevoerd door de brandweer",
    functioneleIdentificatie: "ControleTrouwlocatie"
  },
  {
    actief: false,
    naam: "Omgevingsvergunning aanvragen",
    omschrijving: "Omgevingsvergunning aanvragen",
    functioneleIdentificatie: "EMM_OVG"
  },
  {
    actief: false,
    naam: "Test Enschede",
    omschrijving: "Test zaaktype Enschede",
    functioneleIdentificatie: "testEns"
  },
  {
    actief: false,
    naam: "Toets",
    omschrijving: "Demo / toetsformulier",
    functioneleIdentificatie: "DEMO"
  },
  {
    actief: false,
    naam: "Generiek werkproces Laarbeek",
    omschrijving: "Generiek werkproces Laarbeek",
    functioneleIdentificatie: "Laarbeek1"
  },
  {
    actief: false,
    naam: "Verhuizing doorgeven",
    omschrijving: "Melding verhuizing binnen nederland behandelen",
    functioneleIdentificatie: "EMM_BZVHZ"
  },
  {
    actief: false,
    naam: "Model zaaktype basispakket",
    omschrijving: "Model zaaktype basispakket",
    functioneleIdentificatie: "BP"
  },
  {
    actief: false,
    naam: "Zaak Zonder Proces",
    omschrijving: "Zaak zonder gekoppeld proces",
    functioneleIdentificatie: "ZZP"
  },
  {
    actief: false,
    naam: "Bijstand Uitkering Wijzigen",
    omschrijving: "Bijstand Uitkering Wijzigen op basis van wijzigingen in woonsituatie",
    functioneleIdentificatie: "BUW"
  },
  {
    actief: true,
    naam: "Melden schade",
    omschrijving: "Melden schade ",
    functioneleIdentificatie: "BP_SME"
  },
  {
    actief: true,
    naam: "Bestuurlijke besluitvorming Griffie",
    omschrijving: "Bestuurlijke besluitvorming Griffie",
    functioneleIdentificatie: "BBV_GR"
  },
  {
    actief: true,
    naam: "Toekomstbestendig wonen lening",
    omschrijving:
      "Generiek afhandelproces; Procestermijn onbepaald; Het beoordelen van een aanvraag voor een Toekomstbestendig wonen lening.",
    functioneleIdentificatie: "TWL"
  },
  {
    actief: true,
    naam: "Indienen bezwaarschrift",
    omschrijving: "Melding openbare ruimte",
    functioneleIdentificatie: "BP_BEZ"
  },
  {
    actief: true,
    naam: "Project- programmamanagement",
    omschrijving: "Project- programmamanagement",
    functioneleIdentificatie: "PPM"
  },
  {
    actief: true,
    naam: "Gennieuw",
    omschrijving: "Gennieuw",
    functioneleIdentificatie: "Gennieuw"
  },
  {
    actief: true,
    naam: "Gegevensbescherming; AVG",
    omschrijving: "Verzoek in het kader van de Algemene Verordening Gegevensbescherming behandelen",
    functioneleIdentificatie: "VZK_AVG"
  },
  {
    actief: false,
    naam: "000 zaaktype voor kopie GEEN EINDDATUM",
    omschrijving: "000 zaaktype voor kopie GEEN EINDDATUM",
    functioneleIdentificatie: "DOWR_GE"
  },
  {
    actief: false,
    naam: "31 Generiek Basis - IJVI",
    omschrijving: "Generiek zaaktype koppeling IJVI - Basis",
    functioneleIdentificatie: "GEN31_IJVI"
  },
  {
    actief: false,
    naam: "Aanvraag vergunninggedenkteken",
    omschrijving: "Aanvraag vergunning gedenkteken",
    functioneleIdentificatie: "2619"
  },
  {
    actief: false,
    naam: "Adoptie",
    omschrijving: "Aangeven van adoptie Burgerzaken proces",
    functioneleIdentificatie: "6"
  },
  {
    actief: false,
    naam: "Reisdocument inname",
    omschrijving: "Innemen bij vermissing reisdocument Burgerzaken proces",
    functioneleIdentificatie: "2607"
  },
  {
    actief: false,
    naam: "Rijbewijs aanvraag",
    omschrijving: "Aanvragen van rijbewijs iBurgerzaken proces",
    functioneleIdentificatie: "208"
  },
  {
    actief: false,
    naam: "Groningen Generieke zaaksturing Medium",
    omschrijving: "Groningen generieke zaaksturing (proces) met afhandeltaken",
    functioneleIdentificatie: "GroningenGeneriekMedium"
  },
  {
    actief: false,
    naam: "Subsidie aanvragen",
    omschrijving: "Subsidie aanvragen",
    functioneleIdentificatie: "EMM_SUBAV"
  },
  {
    actief: false,
    naam: "TestZaakType",
    omschrijving: "Een test zaaktype vanuit development",
    functioneleIdentificatie: "Test"
  },
  {
    actief: false,
    naam: "Verhuizen naar HVV-gebied",
    omschrijving:
      "Verhuizen naar HVV-gebied; de aangifte adreswijziging. Aanvragen HVV via aan te maken deelzaak.",
    functioneleIdentificatie: "DV_HVVAA"
  },
  {
    actief: false,
    naam: "Toezichthouden",
    omschrijving:
      "Dit bedrijfsproces betreft het vanuit bestuursrechtelijke bevoegdheid beoordelen of een situatie in overeenstemming is met geldende wet- en regelgeving, verstrekte vergunningen en meldingen van activiteiten.",
    functioneleIdentificatie: "Toezicht"
  },
  {
    actief: false,
    naam: "Emmen Generiek",
    omschrijving: "Emmen Generiek",
    functioneleIdentificatie: "EMM_GEN10"
  },
  {
    actief: true,
    naam: "T_Berichtenbox",
    omschrijving: "T_Berichtenbox",
    functioneleIdentificatie: "T_BBMO"
  },
  {
    actief: true,
    naam: "Handhavingsverzoek fysieke leefomgeving ",
    omschrijving: "Handhavingsverzoek fysieke leefomgeving ",
    functioneleIdentificatie: "EMM_HHVFL"
  },
  {
    actief: false,
    naam: "AutomatischeTesten1",
    omschrijving: "Zaaktype opgezet voor de automatische testen",
    functioneleIdentificatie: "2004"
  },
  {
    actief: true,
    naam: "Indienen klacht",
    omschrijving: "Indienen klacht",
    functioneleIdentificatie: "BP_KLA"
  },
  {
    actief: true,
    naam: "Exploitatievergunning horeca aanvragen",
    omschrijving: "Exploitatievergunning en alcoholvergunning aanvragen",
    functioneleIdentificatie: "BCO_HOV"
  },
  {
    actief: true,
    naam: "Drank en horecavergunning",
    omschrijving:
      "Een aanvraag van een vergunning of ontheffing kan zowel op basis van landelijke als lokale wetgeving plaatsvinden.",
    functioneleIdentificatie: "DHV"
  },
  {
    actief: false,
    naam: "Roermond testzaaktype",
    omschrijving: "Testzaaktype Roermond",
    functioneleIdentificatie: "RMD_TST"
  },
  {
    actief: true,
    naam: "Bestuurlijke dienstverlening besluiten - College",
    omschrijving: "Bestuurlijke dienstverlening besluiten - College",
    functioneleIdentificatie: "BBV_COL"
  },
  {
    actief: true,
    naam: "Product: Exploitatievergunning test",
    omschrijving: "Product: Drank -en horecavergunning_Rotterdam",
    functioneleIdentificatie: "TSTROT_PROD_EXP"
  },
  {
    actief: true,
    naam: "Verhuizing",
    omschrijving:
      "Specifiek afhandelproces; Procestermijn nihil; Melding verhuizing binnen Nederland behandelen",
    functioneleIdentificatie: "BP_VHZ"
  },
  {
    actief: false,
    naam: "Aanvraag bewijs van Nederlanderschap",
    omschrijving: "Aanvraag bewijs van Nederlanderschap Burgerzaken proces",
    functioneleIdentificatie: "36"
  },
  {
    actief: false,
    naam: "Aanvraag stempas",
    omschrijving: "Proces voor aanvragen stempas iBurgerzaken",
    functioneleIdentificatie: "2617"
  },
  {
    actief: false,
    naam: "Benoemen openbare ruimte",
    omschrijving: "Benoemen openbare ruimte",
    functioneleIdentificatie: "2623"
  },
  {
    actief: false,
    naam: "Benoemen woonplaats",
    omschrijving: "Benoemen woonplaats",
    functioneleIdentificatie: "2625"
  },
  {
    actief: false,
    naam: "Erkenning ongeboren vrucht",
    omschrijving: "Erkenning ongeboren vrucht",
    functioneleIdentificatie: "76"
  },
  {
    actief: false,
    naam: "Indienen toestemmingsformulier reisdocument",
    omschrijving: "Indienen toestemmingsformulier reisdocument",
    functioneleIdentificatie: "2632"
  },
  {
    actief: false,
    naam: "Optieverzoek",
    omschrijving: "Optieverzoek",
    functioneleIdentificatie: "2618"
  },
  {
    actief: false,
    naam: "Sloop",
    omschrijving: "Sloop",
    functioneleIdentificatie: "216"
  },
  {
    actief: false,
    naam: "Toevoegen brondocument iBurgerzaken",
    omschrijving: "Toevoegen brondocument iBurgerzaken",
    functioneleIdentificatie: "2620"
  },
  {
    actief: false,
    naam: "Verklaring omtrent gedrag",
    omschrijving: "Verklaring omtrent gedrag",
    functioneleIdentificatie: "253"
  },
  {
    actief: false,
    naam: "Verlengen/Afstand Graf",
    omschrijving: "Verlengen/Afstand Graf",
    functioneleIdentificatie: "101"
  },
  {
    actief: false,
    naam: "Vervangen brondocument iBurgerzaken",
    omschrijving: "Vervangen brondocument iBurgerzaken",
    functioneleIdentificatie: "2622"
  },
  {
    actief: false,
    naam: "Bijstand Uitkering Wijzigen Test",
    omschrijving: "Bijstand Uitkering Wijzigen op basis van wijzigingen in woonsituatie",
    functioneleIdentificatie: "BUW_Test"
  },
  {
    actief: true,
    naam: "Evenementenvergunning demo",
    omschrijving:
      "Demo - Een aanvraag van een vergunning of ontheffing kan zowel op basis van landelijke als lokale wetgeving plaatsvinden. T",
    functioneleIdentificatie: "EVG-Demo"
  },
  {
    actief: true,
    naam: "Liesbeth test",
    omschrijving: "Liesbeth test omschrijving",
    functioneleIdentificatie: "LTE"
  },
  {
    actief: false,
    naam: "Aanvraag AA+-evenement",
    omschrijving: "Aanvraagzaak Vergunning A of A+ -evenement aanvragen",
    functioneleIdentificatie: "BCO_AAA"
  },
  {
    actief: true,
    naam: "Toezicht fysieke leefomgeving uitvoeren",
    omschrijving: "Toezicht fysieke leefomgeving uitvoeren",
    functioneleIdentificatie: "EMM_TOEFL"
  },
  {
    actief: true,
    naam: "Manifestatie melden",
    omschrijving: "GEN0X_Prototype",
    functioneleIdentificatie: "BCO_MAM"
  },
  {
    actief: true,
    naam: "Eigenschappen test",
    omschrijving: "Zaaktype voor testen van (zaak) eigenschappen in ZGW API's",
    functioneleIdentificatie: "eigenschappen_test"
  },
  {
    actief: true,
    naam: "Huisbezoek",
    omschrijving: "Huisbezoek aanvragen en behandelen (test voor API Gateway)",
    functioneleIdentificatie: "SB_HUI"
  },
  {
    actief: false,
    naam: "Bijhouden PL",
    omschrijving: "Bijhouden PL",
    functioneleIdentificatie: "2630"
  },
  {
    actief: false,
    naam: "Echtscheiding of ontbinding partnerschap",
    omschrijving: "Aangeven van beeindiging huwelijk Burgerzaken proces",
    functioneleIdentificatie: "70"
  },
  {
    actief: false,
    naam: "Erkenning kind",
    omschrijving: "Erkenning kind",
    functioneleIdentificatie: "75"
  }
];
</script>

<style lang="scss" scoped>
form {
  max-inline-size: var(--section-width);
  margin-block-end: var(--spacing-default);
}

ul {
  display: flex;
  flex-direction: column;
}

.button {
  display: flex;
  justify-content: space-between;
  inline-size: 100%;
}
</style>
