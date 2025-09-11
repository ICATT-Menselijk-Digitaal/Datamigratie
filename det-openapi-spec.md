```yaml
---
openapi: 3.1.0
components:
  schemas:
    AanvullijstDataElement:
      type: object
      required:
      - waarden
      allOf:
      - $ref: "#/components/schemas/DataElement"
      properties:
        waarden:
          type: array
          items:
            $ref: "#/components/schemas/AanvullijstRecord"
          description: Aanvullijst records
    AanvullijstRecord:
      type: object
      required:
      - itemIdentificatie
      properties:
        recordNummer:
          type: integer
          format: int32
          description: Record nummer
        itemIdentificatie:
          type: string
          description: Item identificatie
        itemWaarde:
          type: string
          description: Item waarde
    Adres:
      type: object
      required:
      - type
      - buitenlandsAdres
      - land
      properties:
        type:
          $ref: "#/components/schemas/AdresType"
          type: string
          description: Type adres
        straatnaam:
          type: string
          maxLength: 64
          description: Straatnaam
        postcode:
          type: string
          maxLength: 6
          description: Postcode
        plaatsnaam:
          type: string
          maxLength: 128
          description: Plaatsnaam
        huisletter:
          type: string
          maxLength: 10
          description: Huisletter
        huisnummer:
          type: integer
          format: int32
          description: Huisnummer
        huisnummertoevoeging:
          type: string
          maxLength: 10
          description: Huisnummertoevoeging
        huisnummeraanduiding:
          type: string
          maxLength: 64
          description: Huisnummeraanduiding
        adresbuitenland1:
          type: string
          maxLength: 128
          description: Adres buitenland 1
        adresbuitenland2:
          type: string
          maxLength: 128
          description: Adres buitenland 2
        adresbuitenland3:
          type: string
          maxLength: 128
          description: Adres buitenland 3
        buitenlandsAdres:
          type: boolean
          description: Indicatie adres buitenland
        land:
          $ref: "#/components/schemas/Land"
          type: object
          description: Land
    AdresType:
      type: string
      enum:
      - correspondentie_adres
      - inschrijf_adres_persoon
      - verblijf_adres_persoon
      - post_adres_bedrijf
      - vestigings_adres_bedrijf
    Afdeling:
      type: object
      required:
      - naam
      - omschrijving
      - actief
      - medewerkers
      properties:
        naam:
          type: string
          uniqueItems: true
          maxLength: 128
          description: Naam van afdeling
        code:
          type: string
          maxLength: 128
          description: Interne code van afdeling
        omschrijving:
          type: string
          description: Omschrijving van afdeling
        afdelingshoofd:
          type: string
          maxLength: 128
          description: Gebruikersnaam van hoofd van afdeling
        emailadres:
          type: string
          maxLength: 64
          pattern: E-mailadres van afdeling
        actief:
          type: boolean
          description: Is afdeling actief
        medewerkers:
          type: array
          uniqueItems: true
          items:
            type: string
          description: Gebruikersnamen van medewerkers in afdeling
    AfdelingOverzicht:
      type: object
      required:
      - naam
      properties:
        naam:
          type: string
          uniqueItems: true
          maxLength: 128
          description: Naam van afdeling
    ArchiveerGegevens:
      type: object
      properties:
        reviewTermijnEinddatum:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: ""
        bewaartermijnEinddatum:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Datum waarop de bewaartermijn afloopt
        bewaartermijnWaardering:
          type: string
          description: Waardering van de bewaartermijn
          $ref: "#/components/schemas/BewaartermijnWaardering"
        overbrengenOp:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Datum waarop de zaak overgebracht moet worden
        overbrengenNaar:
          type: string
          maxLength: 255
          description: Locatie/instantie waarnaar zaak overgebracht moet worden
        overbrengenDoor:
          type: string
          maxLength: 128
          description: Naam medewerker die zaak op overbrengen zet
        overgebrachteGegevens:
          type: object
          description: ""
          $ref: "#/components/schemas/OvergebrachteGegevens"
        beperkingOpenbaarheid:
          type: boolean
          description: Aanduiding beperking openbaarheid
        beperkingOpenbaarheidReden:
          type: string
          description: Reden beperking openbaarheid
        beperkingOpenbaarheidVanaf:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Beperking openbaarheid datum vanaf
        beperkingOpenbaarheidTotEnMet:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Beperking openbaarheid datum tot en met
        zaaktypeNaam:
          type: string
          maxLength: 255
          description: "Naam van het zaaktype (zaaktype: naam_algemeen)"
        selectielijstItemNaam:
          type: string
          maxLength: 255
          description: ""
        overbrengenType:
          type: string
          description: Type overbrengen
          $ref: "#/components/schemas/OverbrengenType"
    AuthenticatieDoelgroep:
      type: string
      enum:
      - burger
      - bedrijf
    AuthenticatieNiveau:
      type: string
      enum:
      - none
      - digid_1
      - digid_2
      - digid_3
      - digid_4
      - eherkenning_1
      - eherkenning_2
      - eherkenning_3
      - eherkenning_4
      - eherkenning_5
      - eherkenning_6
    BAGObject:
      type: object
      required:
      - bagObjectId
      properties:
        bagObjectId:
          type: string
          maxLength: 128
          description: Identificatie van BAG object
    Bedrijf:
      type: object
      required:
      - inSurceance
      - failliet
      - ingangsdatum
      - vestigingstype
      allOf:
      - $ref: "#/components/schemas/Subject"
      properties:
        kvkNummer:
          type: string
          minLength: 8
          maxLength: 8
          description: Kamer van koophandel (KvK) nummer
        vestigingsnummer:
          type: string
          minLength: 12
          maxLength: 12
          description: Vestigingsnummer
        buitenlandsHandelsregisternummer:
          type: string
          maxLength: 255
          description: Buitenlands handelsregisternummer
        bedrijfsnaam:
          type: string
          maxLength: 128
          description: De volledige bedrijfsnaam zoals bekend bij de KvK
        vennootschapsnaam:
          type: string
          maxLength: 255
          description: Vennootschapsnaam
        statutaireZetel:
          type: string
          maxLength: 64
          description: Plaats van de statutaire zetel
        datumVestiging:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Datum vestiging onderneming
        datumOpheffing:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Datum opheffing onderneming
        datumVoortzetting:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Datum voortzetting onderneming
        faxnummer:
          type: string
          maxLength: 20
          description: Telefaxnummer
        aantalWerknemers:
          type: integer
          format: int32
          description: Totaal aantal werknemers
        inSurceance:
          type: boolean
          description: Indicatie surseance van betaling
        failliet:
          type: boolean
          description: Indicatie van faillisement
        rsinummer:
          type: string
          maxLength: 20
          description: RSI nummer onderneming
        vestigingsstatus:
          type: string
          description: Status vestiging
          $ref: "#/components/schemas/Vestigingsstatus"
        ingangsdatum:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Ingangsdatum geldigheid record vanaf
        mutatiedatum:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Datum waarop record voor het laatst is gewijzigd
        vestigingstype:
          $ref: "#/components/schemas/Vestigingstype"
          type: string
          description: Vestigingstype
        hoofdactiviteit:
          type: object
          description: Hoofdactiviteit
          $ref: "#/components/schemas/Hoofdactiviteit"
        rechtsvorm:
          type: object
          description: Rechtsvorm
          $ref: "#/components/schemas/Rechtsvorm"
        nevenactiviteiten:
          type: array
          uniqueItems: true
          items:
            $ref: "#/components/schemas/Nevenactiviteit"
          description: Nevenactiviteiten
        contactpersonen:
          type: array
          items:
            $ref: "#/components/schemas/Contactpersoon"
    Besluit:
      type: object
      required:
      - functioneleIdentificatie
      - besluittype
      - besluitDatum
      - berekenVervaldatum
      properties:
        functioneleIdentificatie:
          type: string
          maxLength: 128
          description: Functionele identificatie van het besluit
        besluittype:
          $ref: "#/components/schemas/Besluittype"
          type: object
          description: Besluittype
        functioneleIdentificatieDocument:
          type: string
          maxLength: 36
          description: Functionele identificatie van aan besluit gerelateerd document
        documenttype:
          type: object
          description: Aan besluit gerelateerd documenttype
          $ref: "#/components/schemas/Documenttype"
        besluitDatum:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Datum waarop besluit is aangemaakt
        vervaldatum:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Datum waarop het besluit vervalt
        ingangsdatum:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Datum vanaf wanneer besluit geldig is
        reactiedatum:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Uiterlijke reactie datum
        publicatiedatum:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Publicatie  datum van het beluit
        berekenVervaldatum:
          type: boolean
          description: Geeft aan of vervaldatum moet worden berekend op het moment
            dat zaak beeindigd wordt
        toelichting:
          type: string
          description: Toelichting op besluit
        procestermijnInMaanden:
          type: integer
          format: int32
          description: Procestermijn in maanden voor berekening van vervaldatum
    Besluitcategorie:
      type: object
      allOf:
      - $ref: "#/components/schemas/Referentie"
    Besluittype:
      type: object
      required:
      - besluitcategorie
      - reactietermijnInDagen
      - publicatieIndicatie
      allOf:
      - $ref: "#/components/schemas/Referentie"
      properties:
        besluitcategorie:
          $ref: "#/components/schemas/Besluitcategorie"
          type: object
          description: Besluit categorie
        reactietermijnInDagen:
          type: integer
          format: int32
          description: Reactietermijn in dagen
        publicatieIndicatie:
          type: boolean
          description: Publicatie indicatie
        publicatietekst:
          type: string
          description: Publicatietekst
        publicatietermijnInDagen:
          type: integer
          format: int32
          description: Publicatie termijn in dagen
    Betaalgegevens:
      type: object
      properties:
        transactieId:
          type: string
          maxLength: 64
          description: transactieId
        kenmerk:
          type: string
          maxLength: 64
          description: Een betalingskenmerk
        bedrag:
          type: number
          description: Het bedrag
          multipleOf: 1.0E-9
          maximum: 9999999999
        transactieDatum:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Datum van de transactie
        ncerror:
          type: string
          maxLength: 64
          description: Foutcode
        origineleStatusCode:
          type: string
          maxLength: 64
          description: Originele statuscode van de betaling
        betaalstatus:
          type: string
          description: Transactiestatus van de betaling
          $ref: "#/components/schemas/Betaalstatus"
    Betaalstatus:
      type: string
      enum:
      - geslaagd
      - niet_geslaagd
      - in_behandeling
      - geannuleerd
    BewaartermijnEenheid:
      type: string
      enum:
      - maanden
      - jaren
    BewaartermijnWaardering:
      type: string
      enum:
      - bewaar
      - vernietig
    BooleanDataElement:
      type: object
      allOf:
      - $ref: "#/components/schemas/DataElement"
      properties:
        waarde:
          type: boolean
          description: Boolean waarde data element
    BooleanDomeinObjectAttribuutWaarde:
      type: object
      required:
      - waarde
      allOf:
      - $ref: "#/components/schemas/DomeinObjectAttribuutWaarde"
      properties:
        waarde:
          type: boolean
          description: Boolean waarde
    BooleanReferentietabelRecordAttribuut:
      type: object
      required:
      - waarde
      allOf:
      - $ref: "#/components/schemas/ReferentietabelRecordAttribuut"
      properties:
        waarde:
          type: boolean
          description: Boolean waarde
    BurgerlijkeStaat:
      type: object
      required:
      - gbaCode
      allOf:
      - $ref: "#/components/schemas/Referentie"
      properties:
        gbaCode:
          type: string
          maxLength: 1
          description: GBA landcode
    CalendarDataElement:
      type: object
      allOf:
      - $ref: "#/components/schemas/DataElement"
      properties:
        waarde:
          type: string
          format: date-time
          examples:
          - 2022-03-10T12:15:50-04:00
          description: Calendar waarde data element
    Categorie:
      type: object
      allOf:
      - $ref: "#/components/schemas/Referentie"
    ComplexDataElement:
      type: object
      allOf:
      - $ref: "#/components/schemas/DataElement"
      properties:
        waarde:
          type: string
          description: Complexe waarde data element
    Contact:
      type: object
      required:
      - functioneleIdentificatie
      - aangemaaktDoor
      - indicatieVertrouwelijk
      - verwijderbaar
      - inVernietiging
      - startdatumTijd
      properties:
        functioneleIdentificatie:
          type: string
          maxLength: 128
          description: Functionele identificatie
        aangemaaktDoor:
          type: string
          maxLength: 128
          description: Volledige naam van aanmaker van contact
        antwoord:
          type: string
          description: Definitief gegeven antwoord
        vraag:
          type: string
          description: Gestelde vraag
        telefoonnummer:
          type: string
          maxLength: 20
          description: Telefoonnummer doorgegeven door melder/aanvrager
        telefoonnummerAlternatief:
          type: string
          maxLength: 20
          description: Alternatief telefoonnummer doorgegeven door melder/aanvrager
        emailadres:
          type: string
          maxLength: 128
          description: Emailadres doorgegeven door melder/aanvrager
        indicatieVertrouwelijk:
          type: boolean
          description: Indicatie vertrouwelijk
        verwijderbaar:
          type: boolean
          description: Indicatie of contact verwijderbaar is
        inVernietiging:
          type: boolean
          description: Indicatie of contact in vernietiging is
        aanvrager:
          type: object
          description: Aanvrager
          $ref: "#/components/schemas/Subject"
        bagObjecten:
          type: array
          items:
            $ref: "#/components/schemas/BAGObject"
          description: BAG Objecten gerelateerd aan contact
        prioriteit:
          type: object
          description: Prioriteit van contact
          $ref: "#/components/schemas/ContactPrioriteit"
        status:
          type: object
          description: Status van contact
          $ref: "#/components/schemas/ContactStatus"
        type:
          type: object
          description: Type contact
          $ref: "#/components/schemas/Contacttype"
        startdatumTijd:
          type: string
          format: date-time
          examples:
          - 2022-03-10T12:15:50-04:00
          description: Startdatum/tijd contact
        streefdatumTijd:
          type: string
          format: date-time
          examples:
          - 2022-03-10T12:15:50-04:00
          description: Streefdatum/tijd contact
        einddatumTijd:
          type: string
          format: date-time
          examples:
          - 2022-03-10T12:15:50-04:00
          description: Einddatum/tijd contact
        kanaal:
          type: object
          description: Kanaal via welke contact is binnengekomen
          $ref: "#/components/schemas/Kanaal"
        kennisbankItemLabel:
          type: string
          maxLength: 255
          description: Label KennisbankProduct uit Loket
        groep:
          type: string
          maxLength: 128
          description: Groep waaraan het contact is gekoppeld
        afdeling:
          type: string
          maxLength: 128
          description: Afdeling waaraan het contact is gekoppeld
        behandelaar:
          type: string
          maxLength: 64
          description: Behandelaar waaraan het contact is gekoppeld
        historie:
          type: array
          items:
            $ref: "#/components/schemas/ContactHistorie"
          description: Contact historie
        voorlopigeAntwoorden:
          type: array
          items:
            $ref: "#/components/schemas/VoorlopigAntwoord"
          description: Voorlopige antwoorden
        gekoppeldeContacten:
          type: array
          items:
            type: string
          description: Functionele identificatie van gekoppelde contacten
        organisatie:
          type: object
          description: Organisatie behorende bij contact
          $ref: "#/components/schemas/Organisatie"
    ContactHistorie:
      type: object
      allOf:
      - $ref: "#/components/schemas/History"
      properties:
        typeWijziging:
          type: string
          description: Type wijziging
          $ref: "#/components/schemas/ContactHistorieTypeWijziging"
    ContactHistorieTypeWijziging:
      type: string
      enum:
      - aanvrager
      - overdragen
      - status
      - prioriteit
      - kanaal
      - streefdatum
      - contacttype
      - vertrouwelijkheid
      - vraag
      - antwoord
      - email_verzonden
      - emailadres
      - telefoonnummer
      - telefoonnummer_alternatief
      - bag_koppeling
      - dsr_koppeling
      - contact_koppeling
      - zaak_koppeling
    ContactOverzicht:
      type: object
      required:
      - functioneleIdentificatie
      properties:
        functioneleIdentificatie:
          type: string
          maxLength: 128
          description: Functionele identificatie
    ContactPrioriteit:
      type: object
      required:
      - dagen
      allOf:
      - $ref: "#/components/schemas/Referentie"
      properties:
        dagen:
          type: integer
          format: int32
          description: Aantal dagen maximale doorlooptijd
    ContactStatus:
      type: object
      allOf:
      - $ref: "#/components/schemas/Referentie"
      properties:
        type:
          type: string
          description: Type status
          $ref: "#/components/schemas/ContactStatusType"
    ContactStatusType:
      type: string
      enum:
      - nieuw
      - in_behandeling
      - afgehandeld
    Contactpersoon:
      type: object
      properties:
        naam:
          type: string
          maxLength: 128
          description: Naam
        geslacht:
          type: string
          description: Geslachtsaanduiding
          $ref: "#/components/schemas/Geslacht"
        emailadres:
          type: string
          maxLength: 128
          description: Emailadres
        telefoonnummer:
          type: string
          maxLength: 20
          description: Telefoonnummer
        faxnummer:
          type: string
          maxLength: 20
          description: Telefaxnummer
        functie:
          type: string
          maxLength: 64
          description: Functie
    Contacttype:
      type: object
      allOf:
      - $ref: "#/components/schemas/Referentie"
    DataElement:
      type: object
      required:
      - type
      - naam
      oneOf:
      - $ref: "#/components/schemas/AanvullijstDataElement"
      - $ref: "#/components/schemas/BooleanDataElement"
      - $ref: "#/components/schemas/CalendarDataElement"
      - $ref: "#/components/schemas/ComplexDataElement"
      - $ref: "#/components/schemas/DecimaalDataElement"
      - $ref: "#/components/schemas/DecimalenDataElement"
      - $ref: "#/components/schemas/DocumentDataElement"
      - $ref: "#/components/schemas/OptiesDataElement"
      - $ref: "#/components/schemas/SelectDocumentDataElement"
      - $ref: "#/components/schemas/StringDataElement"
      - $ref: "#/components/schemas/StringsDataElement"
      discriminator:
        propertyName: type
        mapping:
          aanvullijst: "#/components/schemas/AanvullijstDataElement"
          boolean: "#/components/schemas/BooleanDataElement"
          calendar: "#/components/schemas/CalendarDataElement"
          datummettijdstip: "#/components/schemas/CalendarDataElement"
          adresgegevens: "#/components/schemas/ComplexDataElement"
          afstand: "#/components/schemas/ComplexDataElement"
          generieke_afspraak: "#/components/schemas/ComplexDataElement"
          geo_informatie: "#/components/schemas/ComplexDataElement"
          optie: "#/components/schemas/ComplexDataElement"
          referentietabel_record: "#/components/schemas/ComplexDataElement"
          digitale_notificaties: "#/components/schemas/ComplexDataElement"
          zaak_besluit: "#/components/schemas/ComplexDataElement"
          decimaal: "#/components/schemas/DecimaalDataElement"
          decimalen: "#/components/schemas/DecimalenDataElement"
          zaak_documenten: "#/components/schemas/DocumentDataElement"
          opties: "#/components/schemas/OptiesDataElement"
          select_documents: "#/components/schemas/SelectDocumentDataElement"
          string: "#/components/schemas/StringDataElement"
          strings: "#/components/schemas/StringsDataElement"
      properties:
        type:
          $ref: "#/components/schemas/DataElementType"
          type: string
          description: Type data element
        naam:
          type: string
          maxLength: 255
          description: Naam van data element
        omschrijving:
          type: string
          description: Omschrijving of verklarende waarde voor data element
    DataElementType:
      type: string
      enum:
      - string
      - optie
      - aanvullijst
      - adresgegevens
      - afstand
      - boolean
      - calendar
      - datummettijdstip
      - decimaal
      - decimalen
      - zaak_documenten
      - generieke_afspraak
      - geo_informatie
      - opties
      - referentietabel_record
      - select_documents
      - strings
      - digitale_notificaties
      - zaak_besluit
    DatumDomeinObjectAttribuutWaarde:
      type: object
      required:
      - waarde
      allOf:
      - $ref: "#/components/schemas/DomeinObjectAttribuutWaarde"
      properties:
        waarde:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Datum waarde
    DatumReferentietabelRecordAttribuut:
      type: object
      required:
      - waarde
      allOf:
      - $ref: "#/components/schemas/ReferentietabelRecordAttribuut"
      properties:
        waarde:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Datum waarde
    DatumTijdDomeinObjectAttribuutWaarde:
      type: object
      required:
      - waarde
      allOf:
      - $ref: "#/components/schemas/DomeinObjectAttribuutWaarde"
      properties:
        waarde:
          type: string
          format: date-time
          examples:
          - 2022-03-10T12:15:50-04:00
          description: Datum/Tijd waarde
    DatumTijdReferentietabelRecordAttribuut:
      type: object
      required:
      - waarde
      allOf:
      - $ref: "#/components/schemas/ReferentietabelRecordAttribuut"
      properties:
        waarde:
          type: string
          format: date-time
          examples:
          - 2022-03-10T12:15:50-04:00
          description: Datum/Tijd waarde
    DecimaalDataElement:
      type: object
      allOf:
      - $ref: "#/components/schemas/DataElement"
      properties:
        waarde:
          type: number
          description: Decimaal waarde data element
        formattering:
          type: string
          description: Manier waarop nummer geformatteerd moet worden
          $ref: "#/components/schemas/NummerFormattering"
    DecimaalDomeinObjectAttribuutWaarde:
      type: object
      required:
      - waarde
      allOf:
      - $ref: "#/components/schemas/DomeinObjectAttribuutWaarde"
      properties:
        waarde:
          type: number
          description: Decimaal waarde
    DecimaalReferentietabelRecordAttribuut:
      type: object
      required:
      - waarde
      allOf:
      - $ref: "#/components/schemas/ReferentietabelRecordAttribuut"
      properties:
        waarde:
          type: number
          description: Decimaal waarde
    DecimalenDataElement:
      type: object
      allOf:
      - $ref: "#/components/schemas/DataElement"
      properties:
        waarden:
          type: array
          uniqueItems: true
          items:
            type: number
          description: Lijst van decimalen
    Document:
      type: object
      required:
      - functioneleIdentificatie
      - documenttype
      - documentStatus
      - titel
      - creatieDatumTijd
      - publicatieniveau
      - documentVersturen
      - aanvraagDocument
      - documentrichting
      - documentversies
      - historie
      - geautoriseerdVoorMedewerkers
      - converterenNaarPdfa
      - locked
      properties:
        functioneleIdentificatie:
          type: string
          maxLength: 36
          description: Functionele identificatie van document
        documentVorm:
          type: object
          description: Vorm van document
          $ref: "#/components/schemas/DocumentVorm"
        documenttype:
          $ref: "#/components/schemas/Documenttype"
          type: object
          description: Type document
        documentStatus:
          $ref: "#/components/schemas/DocumentStatus"
          type: object
          description: Status van document
        titel:
          type: string
          maxLength: 255
          description: Titel van document
        kenmerk:
          type: string
          maxLength: 128
          description: Kenmerk van document
        creatieDatumTijd:
          type: string
          format: date-time
          examples:
          - 2022-03-10T12:15:50-04:00
          description: Tijdstip waarop document is aangemaakt
        wijzigDatumTijd:
          type: string
          format: date-time
          examples:
          - 2022-03-10T12:15:50-04:00
          description: Meest recente tijdstip waarop document is gewijzigd
        publicatieniveau:
          $ref: "#/components/schemas/DocumentPublicatieniveau"
          type: string
          description: Document publicatieniveau
        documentVersturen:
          $ref: "#/components/schemas/DocumentVersturen"
          type: string
          description: Document verstuur gegevens
        documentVersturenDatum:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Datum van versturen document via mail
        aanvraagDocument:
          type: boolean
          description: Is dit document een aanvraag document
        ontvangstDatum:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Datum ontvangst document
        verzendDatum:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Datum verzenden document
        documentrichting:
          $ref: "#/components/schemas/DocumentRichting"
          type: string
          description: Document richting
        locatie:
          type: string
          maxLength: 128
          description: Locatie van document
        beschrijving:
          type: string
          description: "Toelichting, trefwoorden etc"
        lockEigenaarId:
          type: string
          maxLength: 64
          description: Gebruikersnaam van medewerker die document heeft gelocked
        lockDatumTijd:
          type: string
          format: date-time
          examples:
          - 2022-03-10T12:15:50-04:00
          description: Datum en tijd van locken van document
        documentversies:
          type: array
          items:
            $ref: "#/components/schemas/Documentversie"
          description: Versies van document
        documentMetadata:
          type: array
          items:
            $ref: "#/components/schemas/DocumentMetadata"
          description: Document metadata
        taak:
          type: object
          description: Taak waaraan document is gerelateerd
          $ref: "#/components/schemas/Taak"
        historie:
          type: array
          items:
            $ref: "#/components/schemas/Documenthistorie"
          description: Document historie
        publicaties:
          type: array
          items:
            $ref: "#/components/schemas/DocumentPublicatie"
          description: Document publicaties
        pdfaDocumentversie:
          type: object
          description: Document versie van PDFA versie van document
          $ref: "#/components/schemas/Documentversie"
        taal:
          type: object
          description: Taal van document
          $ref: "#/components/schemas/Taal"
        geautoriseerdeMedewerkers:
          type: array
          uniqueItems: true
          items:
            type: string
          description: Gebruikersnamen van medewerkers welke zijn geautoriseerd voor
            document
        geautoriseerdVoorMedewerkers:
          type: boolean
          description: Betreft dit een document met specifieke medewerker autorisatie
        converterenNaarPdfa:
          type: boolean
          description: Indicatie of het document omgezet moet worden naar pdfa
        locked:
          type: boolean
          description: Is document gelocked
    DocumentDataElement:
      type: object
      required:
      - waarden
      allOf:
      - $ref: "#/components/schemas/DataElement"
      properties:
        waarden:
          type: array
          items:
            type: string
          description: Lijst van zaak document bestandsnamen
    DocumentMetadata:
      type: object
      properties:
        metadataElement:
          type: object
          description: ID van metadata element
          $ref: "#/components/schemas/MetadataElement"
        waarde:
          type: string
          description: Waarde van metadata element
    DocumentMetadataElementType:
      type: string
      enum:
      - tekst
      - numeriek
      - datum
    DocumentOndertekening:
      type: object
      required:
      - documentTitel
      - ondertekenaar
      - ondertekenDatum
      - creatieDatum
      - gemandateerd
      properties:
        documentTitel:
          type: string
          maxLength: 256
          description: Titel van document bij ondertekening
        ondertekenaar:
          type: string
          maxLength: 128
          description: Ondertekenaar van document versie
        ondertekenDatum:
          type: string
          format: date-time
          examples:
          - 2022-03-10T12:15:50-04:00
          description: Datum waarop document versie werd ondertekend
        creatieDatum:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Datum waarop ondertekening van document versie werd aangemaakt
        opmerking:
          type: string
          description: Opmerkingen over ondertekening van document versie
        gemandateerd:
          type: boolean
          description: Ondertekening is wel of niet gemandateerd
    DocumentPublicatie:
      type: object
      required:
      - bestemming
      - publicatiedatum
      properties:
        bestemming:
          type: string
          description: Bestemming van publicatie. Altijd gelijk aan 'berichtenbox'
          default: berichtenbox
        publicatiedatum:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Datum waarop document versie is gepubliceerd
    DocumentPublicatieniveau:
      type: string
      enum:
      - extern
      - intern
      - vertrouwelijk
    DocumentRichting:
      type: string
      enum:
      - inkomend
      - intern
      - uitgaand
    DocumentStatus:
      type: object
      allOf:
      - $ref: "#/components/schemas/Referentie"
    DocumentTag:
      type: object
      allOf:
      - $ref: "#/components/schemas/Referentie"
    DocumentTitel:
      type: object
      allOf:
      - $ref: "#/components/schemas/Referentie"
    DocumentVersturen:
      type: string
      enum:
      - moet_niet_verstuurd_worden
      - is_verstuurd
    DocumentVorm:
      type: object
      allOf:
      - $ref: "#/components/schemas/Referentie"
    Documenthistorie:
      type: object
      allOf:
      - $ref: "#/components/schemas/History"
      properties:
        typeWijziging:
          type: string
          maxLength: 64
          description: Type wijziging
    Documenttype:
      type: object
      required:
      - publicatieniveau
      allOf:
      - $ref: "#/components/schemas/Referentie"
      properties:
        documentcategorie:
          type: string
          maxLength: 64
          description: Aanduiding documentcategorie
        publicatieniveau:
          $ref: "#/components/schemas/DocumentPublicatieniveau"
          type: string
          description: Default publicatieniveau
    Documentversie:
      type: object
      required:
      - versienummer
      - documentInhoudID
      - creatiedatum
      - bestandsnaam
      - mimetype
      - compressed
      properties:
        versienummer:
          type: integer
          format: int32
          description: Versienummer van document versie
        documentInhoudID:
          type: integer
          format: int64
          description: ID van document inhoud
        creatiedatum:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Datum waarop document versie werd aangemaakt
        auteur:
          type: string
          maxLength: 128
          description: Naam van auteur
        afzender:
          type: string
          maxLength: 128
          description: Naam van afzender
        bestandsnaam:
          type: string
          maxLength: 255
          description: Bestandsnaam van document versie
        mimetype:
          type: string
          maxLength: 255
          description: Mimetype van document versie
        ondertekeningen:
          type: array
          items:
            $ref: "#/components/schemas/DocumentOndertekening"
          description: Ondertekeningen van document vdersie
        documentgrootte:
          type: integer
          format: int64
          description: Grootte van document in bytes
        compressed:
          type: boolean
          description: Indicatie of de Blob gecomprimeerd is opgeslagen
    Domein:
      type: object
      allOf:
      - $ref: "#/components/schemas/Referentie"
      properties:
        aantalObjecten:
          type: integer
          format: int64
          description: Aantal objecten in domein
    DomeinObject:
      type: object
      required:
      - identifier
      - naam
      - aangemaaktDoor
      - aangemaaktOp
      - attributen
      properties:
        identifier:
          type: integer
          format: int64
          description: Unieke identificatie van het domein object
        naam:
          type: string
          maxLength: 128
          description: Naam van type domein object
        omschrijving:
          type: string
          description: Omschrijving van type domein object
        aangemaaktDoor:
          type: string
          maxLength: 64
          description: Gebruikersnaam van medewerker die domein object heeft aangemaakt
        aangemaaktOp:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Datum waarop domein object is aangemaakt
        laatstGewijzigdDoor:
          type: string
          maxLength: 64
          description: Gebruikersnaam van medewerker die domein object als laatste
            heeft gewijzigd
        laatstGewijzigdOp:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Datum waarop domein object de laatste keer is gewijzigd
        attributen:
          type: array
          items:
            $ref: "#/components/schemas/DomeinObjectAttribuut"
          description: Attributen gerelateerd aan domein object
        koppelingen:
          type: array
          items:
            $ref: "#/components/schemas/DomeinObjectKoppeling"
          description: "Zaken, contacten, personen, bedrijven of BAG objecten waaraan\
            \ domein object gekoppeld is"
        historie:
          type: array
          items:
            $ref: "#/components/schemas/DomeinObjectHistorie"
          description: Historie van domein object
    DomeinObjectAttribuut:
      type: object
      required:
      - naam
      - waarden
      properties:
        naam:
          type: string
          maxLength: 128
          description: Naam van domein object attribuut
        omschrijving:
          type: string
          description: Omschrijving van domein object attribuut
        waarden:
          type: array
          items:
            $ref: "#/components/schemas/DomeinObjectAttribuutWaarde"
          description: Waarden van domein object attribuut
    DomeinObjectAttribuutWaarde:
      type: object
      required:
      - type
      - volgnummer
      oneOf:
      - $ref: "#/components/schemas/BooleanDomeinObjectAttribuutWaarde"
      - $ref: "#/components/schemas/DatumDomeinObjectAttribuutWaarde"
      - $ref: "#/components/schemas/DatumTijdDomeinObjectAttribuutWaarde"
      - $ref: "#/components/schemas/DecimaalDomeinObjectAttribuutWaarde"
      - $ref: "#/components/schemas/GeoDomeinObjectAttribuutWaarde"
      - $ref: "#/components/schemas/MemoDomeinObjectAttribuutWaarde"
      - $ref: "#/components/schemas/NummerDomeinObjectAttribuutWaarde"
      - $ref: "#/components/schemas/StringDomeinObjectAttribuutWaarde"
      - $ref: "#/components/schemas/DomeinObjectDomeinObjectAttribuutWaarde"
      - $ref: "#/components/schemas/ReferentietabelRecordDomeinObjectAttribuutWaarde"
      discriminator:
        propertyName: type
        mapping:
          boolean: "#/components/schemas/BooleanDomeinObjectAttribuutWaarde"
          datum: "#/components/schemas/DatumDomeinObjectAttribuutWaarde"
          datum_tijd: "#/components/schemas/DatumTijdDomeinObjectAttribuutWaarde"
          decimaal: "#/components/schemas/DecimaalDomeinObjectAttribuutWaarde"
          geo: "#/components/schemas/GeoDomeinObjectAttribuutWaarde"
          memo: "#/components/schemas/MemoDomeinObjectAttribuutWaarde"
          nummer: "#/components/schemas/NummerDomeinObjectAttribuutWaarde"
          string: "#/components/schemas/StringDomeinObjectAttribuutWaarde"
          domein_object: "#/components/schemas/DomeinObjectDomeinObjectAttribuutWaarde"
          referentietabel_record: "#/components/schemas/ReferentietabelRecordDomeinObjectAttribuutWaarde"
      properties:
        type:
          $ref: "#/components/schemas/DomeinObjectAttribuutWaardeType"
          type: string
          description: Type attribuut waarde
        volgnummer:
          type: integer
          format: int32
          description: Positie van deze waarde tussen andere waarden voor attribuut
    DomeinObjectAttribuutWaardeType:
      type: string
      enum:
      - boolean
      - datum
      - datum_tijd
      - decimaal
      - geo
      - memo
      - nummer
      - string
      - domein_object
      - referentietabel_record
    DomeinObjectDomeinObjectAttribuutWaarde:
      type: object
      required:
      - waarde
      allOf:
      - $ref: "#/components/schemas/DomeinObjectAttribuutWaarde"
      properties:
        waarde:
          type: integer
          format: int64
          description: Identificatie van ander domein object binnen hetzelfde domein
    DomeinObjectHistorie:
      type: object
      allOf:
      - $ref: "#/components/schemas/History"
      properties:
        wijziging:
          type: string
          maxLength: 128
          description: Naam gewijzigd attribut of koppeling
    DomeinObjectKoppeling:
      type: object
      required:
      - gekoppeldObjectType
      - gekoppeldObjectId
      properties:
        gekoppeldObjectType:
          $ref: "#/components/schemas/DomeinObjectKoppelingType"
          type: string
          description: Type object waaraan het domein object gekoppeld is
        gekoppeldObjectId:
          type: string
          maxLength: 64
          description: Id van object waaraan domein object gekoppeld is
    DomeinObjectKoppelingType:
      type: string
      enum:
      - zaak
      - contact
      - bag_object
      - persoon
      - bedrijf
    Fout:
      type: object
      required:
      - melding
      properties:
        melding:
          type: string
          description: Foutmelding
    Functie:
      type: object
      required:
      - naam
      - omschrijving
      - actief
      properties:
        naam:
          type: string
          maxLength: 128
          description: Naam van functie
        omschrijving:
          type: string
          description: Omschrijving van functie
        actief:
          type: boolean
          description: Is functie actief
    GeoDomeinObjectAttribuutWaarde:
      type: object
      required:
      - waarde
      allOf:
      - $ref: "#/components/schemas/DomeinObjectAttribuutWaarde"
      properties:
        waarde:
          type: string
          description: Well-known text geografische informatie
    GeoReferentietabelRecordAttribuut:
      type: object
      required:
      - waarde
      allOf:
      - $ref: "#/components/schemas/ReferentietabelRecordAttribuut"
      properties:
        waarde:
          type: string
          description: Well-known text geografische informatie
    Geometry:
      type: object
      required:
      - type
      oneOf:
      - $ref: "#/components/schemas/Point"
      - $ref: "#/components/schemas/LineString"
      - $ref: "#/components/schemas/Polygon"
      - $ref: "#/components/schemas/MultiPolygon"
      - $ref: "#/components/schemas/GeometryCollection"
      description: "GeoJSON Geometry object in coördinatensysteem: RD-NEW (EPSG-code:\
        \ 28992)"
      discriminator:
        propertyName: type
        mapping:
          Point: "#/components/schemas/Point"
          LineString: "#/components/schemas/LineString"
          Polygon: "#/components/schemas/Polygon"
          MultiPolygon: "#/components/schemas/MultiPolygon"
          GeometryCollection: "#/components/schemas/GeometryCollection"
      properties:
        type:
          $ref: "#/components/schemas/GeometryType"
          type: string
          description: Geometry type
    GeometryCollection:
      type: object
      required:
      - type
      - geometries
      description: GeoJSON GeometryCollection Geometry object
      properties:
        type:
          $ref: "#/components/schemas/GeometryType"
          type: string
          description: Geometry type
          default: GeometryCollection
        geometries:
          type: array
          items:
            $ref: "#/components/schemas/Geometry"
    GeometryType:
      type: string
      enum:
      - Point
      - LineString
      - Polygon
      - MultiPolygon
      - GeometryCollection
    Geslacht:
      type: string
      enum:
      - man
      - vrouw
      - onbekend
    GeslachtMedewerker:
      type: string
      enum:
      - man
      - vrouw
    Groep:
      type: object
      required:
      - naam
      - omschrijving
      - actief
      - medewerkers
      properties:
        naam:
          type: string
          uniqueItems: true
          maxLength: 128
          description: Naam van groep
        code:
          type: string
          maxLength: 128
          description: Interne code van groep
        omschrijving:
          type: string
          description: Omschrijving van groep
        groepshoofd:
          type: string
          maxLength: 128
          description: Gebruikersnaam van hoofd van groep
        emailadres:
          type: string
          maxLength: 64
          pattern: E-mailadres van groep
        actief:
          type: boolean
          description: Is groep actief
        medewerkers:
          type: array
          uniqueItems: true
          items:
            type: string
          description: Gebruikersnamen van medewerkers in groep
    GroepOverzicht:
      type: object
      required:
      - naam
      properties:
        naam:
          type: string
          uniqueItems: true
          maxLength: 128
          description: Naam van groep
    HandelingInitiator:
      type: object
      allOf:
      - $ref: "#/components/schemas/Referentie"
    History:
      type: object
      properties:
        wijzigingDatum:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Datum waarop wijziging heeft plaatsgevonden
        gewijzigdDoor:
          type: string
          maxLength: 128
          description: Volledige naam van gebruiker die wijziging heeft doorgevoerd
        oudeWaarde:
          type: string
          description: Oude waarde
        nieuweWaarde:
          type: string
          description: Nieuwe waarde
        toelichting:
          type: string
          description: Toelichting of reden wijziging
    Hoofdactiviteit:
      type: object
      required:
      - code
      allOf:
      - $ref: "#/components/schemas/Referentie"
      properties:
        code:
          type: string
          maxLength: 7
          description: Code
    InternExtern:
      type: string
      enum:
      - intern
      - extern
    Iv3Categorie:
      type: object
      allOf:
      - $ref: "#/components/schemas/Referentie"
      properties:
        externeCode:
          type: string
          maxLength: 64
    Kanaal:
      type: object
      allOf:
      - $ref: "#/components/schemas/Referentie"
    Land:
      type: object
      required:
      - gbaCode
      allOf:
      - $ref: "#/components/schemas/Referentie"
      properties:
        gbaCode:
          type: string
          maxLength: 10
          description: GBA landcode
    Line2D:
      type: array
      minItems: 2
      items:
        type: array
        maxItems: 2
        minItems: 2
        items:
          type: number
        description: "Position as [longitude, latitude]"
      description: A connected sequence of two or more points
    LineString:
      type: object
      required:
      - type
      - coordinates
      description: GeoJSON LineString Geometry object
      properties:
        type:
          $ref: "#/components/schemas/GeometryType"
          type: string
          description: Geometry type
          default: LineString
        coordinates:
          $ref: "#/components/schemas/Line2D"
    Medewerker:
      type: object
      required:
      - gebruikersnaam
      - volledigeNaam
      - actief
      - locked
      - geslacht
      - afdelingen
      - groepen
      - afdelingshoofdVan
      - groepshoofdVan
      - rollen
      - rechten
      properties:
        gebruikersnaam:
          type: string
          uniqueItems: true
          maxLength: 128
          description: Gebruikersnaam van medewerker
        volledigeNaam:
          type: string
          maxLength: 128
          description: Volledige naam van medewerker
        telefoonnummer:
          type: string
          maxLength: 20
          description: Telefoonnummer van medewerker
        emailadres:
          type: string
          maxLength: 64
          description: E-mailadres van medewerker
        actief:
          type: boolean
          description: Is medewerker actief
        indiensttredingDatum:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Datum indiensttreding
        uitdiensttredingDatum:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Datum uitdiensttreding
        locked:
          type: boolean
          description: Is medewerker locked
        externeNaam:
          type: string
          maxLength: 128
          description: Externe naam van medewerker
        functie:
          type: object
          description: Functie van medewerker
          $ref: "#/components/schemas/Functie"
        geslacht:
          $ref: "#/components/schemas/GeslachtMedewerker"
          type: string
          description: Geslacht medewerker
        laatsteLoginDatumTijd:
          type: string
          format: date-time
          examples:
          - 2022-03-10T12:15:50-04:00
          description: Laatste logon datum/tijd van medewerker
        opmerkingen:
          type: string
          description: Opmerkingen
        primaireAfdeling:
          type: string
          description: Naam primaire afdeling
        afdelingen:
          type: array
          uniqueItems: true
          items:
            type: string
          description: Namen van afdelingen waarin medewerker zich bevindt
        groepen:
          type: array
          uniqueItems: true
          items:
            type: string
          description: Namen van groepen waarin medewerker zich bevindt
        afdelingshoofdVan:
          type: array
          uniqueItems: true
          items:
            type: string
          description: Namen van afdelingen waar medewerker hoofd van is
        groepshoofdVan:
          type: array
          uniqueItems: true
          items:
            type: string
          description: Namen van groepen waar medewerker hoofd van is
        rollen:
          type: array
          uniqueItems: true
          items:
            $ref: "#/components/schemas/Rol"
          description: Rollen van medewerker
        rechten:
          type: array
          uniqueItems: true
          items:
            $ref: "#/components/schemas/Recht"
          description: Rechten van medewerker
    MedewerkerOverzicht:
      type: object
      required:
      - gebruikersnaam
      - volledigeNaam
      properties:
        gebruikersnaam:
          type: string
          uniqueItems: true
          maxLength: 128
          description: Gebruikersnaam van medewerker
        volledigeNaam:
          type: string
          maxLength: 128
          description: Volledige naam van medewerker
    MemoDomeinObjectAttribuutWaarde:
      type: object
      required:
      - waarde
      allOf:
      - $ref: "#/components/schemas/DomeinObjectAttribuutWaarde"
      properties:
        waarde:
          type: string
          description: Text waarde
    MemoReferentietabelRecordAttribuut:
      type: object
      required:
      - waarde
      allOf:
      - $ref: "#/components/schemas/ReferentietabelRecordAttribuut"
      properties:
        waarde:
          type: string
          description: Text waarde
    MetadataElement:
      type: object
      required:
      - label
      - type
      - indicatieVerplicht
      - indicatieVoorAlleDocumenttypes
      allOf:
      - $ref: "#/components/schemas/Referentie"
      properties:
        label:
          type: string
          maxLength: 255
          description: Label van metadata element
        type:
          $ref: "#/components/schemas/DocumentMetadataElementType"
          type: string
          description: Type van metadata element
        indicatieVerplicht:
          type: boolean
          description: Indicatie is verplicht element
        indicatieVoorAlleDocumenttypes:
          type: boolean
          description: Indicatie of het metadata element gebruikt wordt bij alle documenttypes
    MultiPolygon:
      type: object
      required:
      - type
      - coordinates
      description: GeoJSON MultiPolygon Geometry object
      properties:
        type:
          $ref: "#/components/schemas/GeometryType"
          type: string
          description: Geometry type
          default: MultiPolygon
        coordinates:
          type: array
          items:
            $ref: "#/components/schemas/Polygon2D"
    Naamgebruik:
      type: string
      enum:
      - eigen_geslachtsnaam
      - geslachtsnaam_partner_na_eigen_geslachtsnaam
      - geslachtsnaam_partner
      - geslachtsnaam_partner_voor_eigen_geslachtsnaam
    Nationaliteit:
      type: object
      required:
      - gbaCode
      allOf:
      - $ref: "#/components/schemas/Referentie"
      properties:
        redenVerkrijging:
          type: string
          maxLength: 255
          description: Reden verkrijging nationaliteit
        datumVerkrijging:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Datum verkrijging nationaliteit
        gbaCode:
          type: string
          maxLength: 10
          description: GBA code
    Nevenactiviteit:
      type: object
      allOf:
      - $ref: "#/components/schemas/Referentie"
      properties:
        code:
          type: string
          maxLength: 7
          description: Code
    NietIngezeteneAanduiding:
      type: string
      enum:
      - ingezetene
      - niet_ingezetene
      - ander_natuurlijk_persoon
    NummerDomeinObjectAttribuutWaarde:
      type: object
      required:
      - waarde
      allOf:
      - $ref: "#/components/schemas/DomeinObjectAttribuutWaarde"
      properties:
        waarde:
          type: integer
          format: int64
          description: Integer waarde
    NummerFormattering:
      type: string
      enum:
      - geheel_getal
      - geheel_getal_geformatteerd
      - twee_decimalen_geformatteerd
      - maximaal_decimalen_geformatteerd
    NummerReferentietabelRecordAttribuut:
      type: object
      required:
      - waarde
      allOf:
      - $ref: "#/components/schemas/ReferentietabelRecordAttribuut"
      properties:
        waarde:
          type: integer
          format: int64
          description: Integer waarde
    OnvolledigeDatumType:
      type: string
      enum:
      - jaar_en_maand_en_dag_onbekend
      - maand_en_dag_onbekend
      - dag_onbekend
      - volledig
    OpschortingsReden:
      type: string
      enum:
      - standaard_waarde
      - overlijden
      - emigratie
      - ministerieel_besluit
      - pl_aangelegd_in_rni
      - fout
    OptiesDataElement:
      type: object
      required:
      - waarden
      allOf:
      - $ref: "#/components/schemas/DataElement"
      properties:
        waarden:
          type: array
          items:
            type: string
          description: Lijst van opties
    Organisatie:
      type: object
      allOf:
      - $ref: "#/components/schemas/Referentie"
    OverbrengenType:
      type: string
      enum:
      - overdragen
      - overbrengen
    OvergebrachteGegevens:
      type: object
      required:
      - overgebrachtOp
      - overgebrachtDoor
      - overgebrachtNaar
      properties:
        overgebrachtOp:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Datum waarop de zaak is overgebracht
        overgebrachtDoor:
          type: string
          maxLength: 128
          description: Gebruikersnaam van medewerker die de zaak heeft overgebracht
        overgebrachtNaar:
          type: string
          maxLength: 255
          description: Locatie/instantie waarnaar de zaak is overgebracht
    Persoon:
      type: object
      required:
      - geblokkeerd
      - curateleRegister
      - inOnderzoek
      - beperkingVerstrekking
      - afnemerIndicatie
      - burgerlijkeStaat
      allOf:
      - $ref: "#/components/schemas/Subject"
      properties:
        burgerServiceNummer:
          type: string
          minLength: 9
          maxLength: 9
          description: Burgerservicenummer (BSN)
        voornamen:
          type: string
          maxLength: 200
          description: Voornamen
        voorletters:
          type: string
          maxLength: 20
          description: Voorletters
        geslachtsNaam:
          type: string
          maxLength: 200
          description: Achternaam
        voorvoegsel:
          type: string
          maxLength: 10
          description: Voorvoegsel geslachtsnaam
        geslacht:
          type: string
          description: Geslachtsaanduiding
          $ref: "#/components/schemas/Geslacht"
        aanhefAanschrijving:
          type: string
          maxLength: 64
          description: Aanhef van de aanschrijving
        adelijkeTitel:
          type: string
          maxLength: 10
          description: Adelijke titel
        preAcademischeTitel:
          type: string
          maxLength: 20
          description: Aanduiding pre academische titel
        postAcademischeTitel:
          type: string
          maxLength: 20
          description: Aanduiding post academische titel
        naamgebruik:
          type: string
          description: Aanduiding van naamgebruik
          $ref: "#/components/schemas/Naamgebruik"
        geslachtsNaamPartner:
          type: string
          maxLength: 200
          description: Achternaam partner
        voorvoegselPartner:
          type: string
          maxLength: 10
          description: Voorvoegsel geslachtsnaam partner
        geboortedatum:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Geboortedatum
        geboorteplaats:
          type: string
          maxLength: 255
          description: Geboorteplaats
        overlijdensdatum:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Overlijdensdatum
        overlijdensplaats:
          type: string
          maxLength: 255
          description: Overlijdensplaats
        aNummer:
          type: string
          maxLength: 10
          description: Gemeentelijk A-nummer
        opschortingsReden:
          type: string
          description: Reden opschorting bijhouding adresgegevens
          $ref: "#/components/schemas/OpschortingsReden"
        opschortingsDatum:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Datum opschorting bijhouding adresgegevens
        geblokkeerd:
          type: boolean
          description: Indicatie blokkering tijdens verhuizing
        curateleRegister:
          type: boolean
          description: Indicatie gezag- of curatelestelling
        inOnderzoek:
          type: boolean
          description: Indicatie of er een onderzoek naar de persoon wordt uitgevoerd
        geboortedatumVolledig:
          type: string
          description: Indicatie of de geboortedatum volledig is
          $ref: "#/components/schemas/OnvolledigeDatumType"
        overlijdensdatumVolledig:
          type: string
          description: Indicatie of datum van overlijden volledig is
          $ref: "#/components/schemas/OnvolledigeDatumType"
        beperkingVerstrekking:
          type: boolean
          description: Indicatie beperking verstrekking gegevens
        nietIngezeteneAanduiding:
          type: string
          description: Indicatie niet-ingezetene
          $ref: "#/components/schemas/NietIngezeteneAanduiding"
        afnemerIndicatie:
          type: boolean
          description: Indicatie of de persoon geabonneerd is op updates vanuit de
            makelaar
        anpIdentificatie:
          type: integer
          format: int64
          description: Ander Natuurlijk Persoon (ANP) identificatie gebruikt tijdens
            verwerking van StUF-zaken berichten
        gemeentecode:
          type: string
          maxLength: 10
          description: Gemeente waarvan persoon een inwoner is
        geboorteland:
          type: object
          description: Geboorteland
          $ref: "#/components/schemas/Land"
        overlijdensland:
          type: object
          description: Overlijdensland
          $ref: "#/components/schemas/Land"
        burgerlijkeStaat:
          $ref: "#/components/schemas/BurgerlijkeStaat"
          type: object
          description: Burgerlijke staat
        nationaliteiten:
          type: array
          items:
            $ref: "#/components/schemas/Nationaliteit"
          description: Nationaliteiten
        reisdocumenten:
          type: array
          items:
            $ref: "#/components/schemas/Reisdocument"
          description: Reisdocumenten
        relaties:
          type: array
          items:
            $ref: "#/components/schemas/Relatie"
          description: Relaties met andere personen
    Point:
      type: object
      required:
      - type
      - coordinates
      description: GeoJSON Point Geometry object
      properties:
        type:
          $ref: "#/components/schemas/GeometryType"
          type: string
          description: Geometry type
          default: Point
        coordinates:
          $ref: "#/components/schemas/Point2D"
    Point2D:
      type: array
      maxItems: 2
      minItems: 2
      items:
        type: number
      description: "Position as [longitude, latitude]"
    Polygon:
      type: object
      required:
      - type
      - coordinates
      description: GeoJSON Polygon Geometry object
      properties:
        type:
          $ref: "#/components/schemas/GeometryType"
          type: string
          description: Geometry type
          default: Polygon
        coordinates:
          $ref: "#/components/schemas/Polygon2D"
    Polygon2D:
      type: array
      items:
        type: array
        minItems: 4
        items:
          type: array
          maxItems: 2
          minItems: 2
          items:
            type: number
          description: "Position as [longitude, latitude]"
        description: A connected sequence of four or more points creating a closed
          ring
      description: The first ring is the outer boundary. Any subsequent rings represent
        holes (interior boundaries)
    Recht:
      type: object
      required:
      - naam
      - actief
      - categorie
      - operatie
      - type
      properties:
        naam:
          type: string
          uniqueItems: true
          maxLength: 255
          description: Naam van recht
        actief:
          type: boolean
          description: Is recht actief
        categorie:
          type: string
          maxLength: 255
          description: Categorie waartoe recht behoort
        operatie:
          $ref: "#/components/schemas/RechtOperatie"
          type: string
          description: Operatie waarvoor recht gebruikt wordt
        type:
          $ref: "#/components/schemas/RechtType"
          type: string
          description: Type object waarop operatie van toepassing is
        waarde:
          type: object
          description: Waarde van het recht indien flexibel recht
          $ref: "#/components/schemas/RechtWaarde"
    RechtOperatie:
      type: string
      enum:
      - tonen
      - raadplegen
      - bewerken
      - toevoegen
      - verwijderen
      - overdragen
      - starten
      - heropenen
      - verdelen
      - importeren
    RechtType:
      type: string
      enum:
      - persoonsgegevens
      - persoonsgegevens_extern
      - persoonsgegevens_handmatig
      - buitengemeentelijke_persoonsgegevens_extern
      - bedrijfsgegevens
      - bedrijfsgegevens_extern
      - bedrijfsgegevens_handmatig
      - bewaartermijn
      - contact
      - zaak
      - voorlopig_antwoord_van_contact
      - gegevens_voor_archivering
      - notities
      - vertrouwelijk_document
      - betrokkene_aan_zaak
      - tabblad_kcc
      - tabblad_werklijsten
      - tabblad_beheer
      - tabblad_overzichten
      - alle_geplande_overzichten
      - bag_object_gegevens
      - inbox_documenten
      - alle_afspraken
      - afspraak
      - beheer_rapportagemodule
      - beheer_afsprakenmodule
      - afspraken_resourcepool
      - beheer_zaaktypecatalogus
      - beheer_domeinregistratie
      - beheer_parameters
      - alle_zaaktypen
      - document
      - document_bij_afgesloten_zaak
      - domein
      - veldtoegang
      - recordmanagement
      - autorisatiemanagement
      - rapportage
      - tabblad_gebruikers
      - beheer_zgw_apis
      - beheer_referentietabellen
      - beheer_instellingen
      - beheer_berichten_monitor
      - vervaldatum_besluit
      - notitie_bij_zaak
      - notitie_bij_afgesloten_zaak
      - zaaktype_van_zaak
      - archieflijst_informatiebeheer
      - archieflijst_proceseigenaar
      - kwaliteitscontrolelijst
    RechtWaarde:
      type: object
      properties:
        type:
          type: string
          description: Type recht waarde
          $ref: "#/components/schemas/RechtWaardeType"
        objectNaam:
          type: string
          maxLength: 255
          description: Naam van object waarop 'type' van toepassing is
        waarde:
          type: string
          maxLength: 255
          description: Waarde van recht indien 'type'of 'objectNaam' niet bepaald
            kunnen worden
    RechtWaardeType:
      type: string
      enum:
      - afdeling
      - groep
      - vestiging
      - dsr_domein
      - veld
      - rapportage_categorie
      - zaaktype
    Rechtsvorm:
      type: object
      required:
      - code
      allOf:
      - $ref: "#/components/schemas/Referentie"
      properties:
        code:
          type: string
          maxLength: 2
          description: Code
        naamNhr:
          type: string
          uniqueItems: true
          description: Naam Rechtsvorm uit Nieuw HandelsRegister
    RedenOntbindingVerbintenis:
      type: string
      enum:
      - onbekend
      - vermissing_en_ander_huwelijk
      - nietig_verklaring
      - overleden
      - rechtsvermoeden_van_overlijden
      - scheiding
      - naar_vreemd_recht_anders_beeindigd
    Referentie:
      type: object
      required:
      - naam
      - actief
      properties:
        naam:
          type: string
          uniqueItems: true
          maxLength: 255
          description: Naam
        omschrijving:
          type: string
          description: Omschrijving
        actief:
          type: boolean
          description: Actief
    Referentietabel:
      type: object
      required:
      - masterDetail
      allOf:
      - $ref: "#/components/schemas/Referentie"
      properties:
        masterDetail:
          type: boolean
          description: Is referentie tabel een master/detail referentie tabel
        aantalRecords:
          type: integer
          format: int64
          description: Aantal records in tabel
    ReferentietabelRecord:
      type: object
      required:
      - naam
      - omschrijving
      - ingangsdatumGeldigheid
      - attributen
      properties:
        naam:
          type: string
          maxLength: 128
          description: Naam van record
        omschrijving:
          type: string
          maxLength: 128
          description: Omschrijving van record
        ingangsdatumGeldigheid:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: 'Datum vanaf wanneer record geldig is '
        einddatumGeldigheid:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Datum waarna record niet meer geldig is
        categorie:
          type: string
          maxLength: 64
          description: Categorie van record
        attributen:
          type: array
          items:
            $ref: "#/components/schemas/ReferentietabelRecordAttribuut"
          description: Attributen in record
    ReferentietabelRecordAttribuut:
      type: object
      required:
      - type
      - naam
      oneOf:
      - $ref: "#/components/schemas/BooleanReferentietabelRecordAttribuut"
      - $ref: "#/components/schemas/DatumReferentietabelRecordAttribuut"
      - $ref: "#/components/schemas/DatumTijdReferentietabelRecordAttribuut"
      - $ref: "#/components/schemas/DecimaalReferentietabelRecordAttribuut"
      - $ref: "#/components/schemas/GeoReferentietabelRecordAttribuut"
      - $ref: "#/components/schemas/MemoReferentietabelRecordAttribuut"
      - $ref: "#/components/schemas/NummerReferentietabelRecordAttribuut"
      - $ref: "#/components/schemas/StringReferentietabelRecordAttribuut"
      discriminator:
        propertyName: type
        mapping:
          boolean: "#/components/schemas/BooleanReferentietabelRecordAttribuut"
          datum: "#/components/schemas/DatumReferentietabelRecordAttribuut"
          datum_tijd: "#/components/schemas/DatumTijdReferentietabelRecordAttribuut"
          decimaal: "#/components/schemas/DecimaalReferentietabelRecordAttribuut"
          geo: "#/components/schemas/GeoReferentietabelRecordAttribuut"
          memo: "#/components/schemas/MemoReferentietabelRecordAttribuut"
          nummer: "#/components/schemas/NummerReferentietabelRecordAttribuut"
          string: "#/components/schemas/StringReferentietabelRecordAttribuut"
      properties:
        type:
          $ref: "#/components/schemas/ReferentietabelRecordAttribuutType"
          type: string
          description: Type attribuut
        naam:
          type: string
          maxLength: 128
          description: Naam van attribuut
        omschrijving:
          type: string
          description: Omschrijving van attribuut
    ReferentietabelRecordAttribuutType:
      type: string
      enum:
      - boolean
      - datum
      - datum_tijd
      - decimaal
      - geo
      - memo
      - nummer
      - string
    ReferentietabelRecordDomeinObjectAttribuutWaarde:
      type: object
      required:
      - waarde
      allOf:
      - $ref: "#/components/schemas/DomeinObjectAttribuutWaarde"
      properties:
        waarde:
          $ref: "#/components/schemas/ReferentietabelRecord"
          type: object
          description: Referentietabel record
    Reisdocument:
      type: object
      required:
      - gbaCode
      - indicatieOnttrekking
      allOf:
      - $ref: "#/components/schemas/Referentie"
      properties:
        gbaCode:
          type: string
          maxLength: 2
          description: GBA landcode
        indicatieOnttrekking:
          type: boolean
          description: Indicatie onttrekking aan verkeer
        autoriteitOntrekking:
          type: string
          maxLength: 255
          description: Naam autoriteit die document heeft onttrokken
        indicatieVervallen:
          type: string
          description: Indicatie onttrekking aan verkeer
          $ref: "#/components/schemas/VervallenAanduiding"
        autoriteitVervallen:
          type: string
          maxLength: 255
          description: Naam autoriteit die document heeft onttrokken
        einddatumGeldigheid:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Einddatum geldigheid
        reisdocumentnummer:
          type: string
          maxLength: 10
          description: Reisdocument nummer
        uitgiftedatum:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Uitgifte datum
        autoriteitUitgifte:
          type: string
          maxLength: 128
          description: Naam autoriteit die document heeft onttrokken
    Relatie:
      type: object
      required:
      - type
      properties:
        type:
          $ref: "#/components/schemas/RelatieType"
          type: string
          description: Aanduiding relatie
        soortVerbintenis:
          type: string
          description: Soort verbintenis
          $ref: "#/components/schemas/SoortVerbintenis"
        datumSluitingVerbintenis:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Datum sluiting van verbintenis
        plaatsSluitingVerbintenis:
          type: string
          maxLength: 128
          description: Plaats van sluiting van verbintenis
        landSluitingVerbintenis:
          type: object
          description: Land van sluiting van verbintenis
          $ref: "#/components/schemas/Land"
        datumOntbindingVerbintenis:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Datum ontbinding verbintenis
        redenOntbindingVerbintenis:
          type: string
          description: Reden ontbinding verbintenis
          $ref: "#/components/schemas/RedenOntbindingVerbintenis"
        plaatsOntbindingVerbintenis:
          type: string
          maxLength: 128
          description: Plaats van de ontbinding van verbintenis
        landOntbindingVerbintenis:
          type: object
          description: Land van ontbinding van verbintenis
          $ref: "#/components/schemas/Land"
        identifierPersoon:
          type: integer
          format: int64
          description: Interne identifier van gerelateerd persoon
    RelatieType:
      type: string
      enum:
      - is_ouder_van
      - is_kind_van
      - is_partner_van
    Resultaat:
      type: object
      required:
      - uitwisselingscode
      allOf:
      - $ref: "#/components/schemas/Referentie"
      properties:
        uitwisselingscode:
          type: string
          maxLength: 255
          description: Code welke gebruikt wordt voor het uitwisselen van zaak informatie
            naar externe systemen zoals bijvoorbeeld via StUF-ZKN-DMS
    ResultsAfdelingOverzicht:
      type: object
      required:
      - results
      - count
      - previousPage
      - nextPage
      properties:
        results:
          type: array
          items:
            $ref: "#/components/schemas/AfdelingOverzicht"
          description: Reslultaten op huidige pagina
        count:
          type: integer
          format: int64
          description: Totaal aantal
        previousPage:
          type: boolean
          description: Is er een vorige pagina
        nextPage:
          type: boolean
          description: Is er een volgende pagina
    ResultsBedrijf:
      type: object
      required:
      - results
      - count
      - previousPage
      - nextPage
      properties:
        results:
          type: array
          items:
            $ref: "#/components/schemas/Bedrijf"
          description: Reslultaten op huidige pagina
        count:
          type: integer
          format: int64
          description: Totaal aantal
        previousPage:
          type: boolean
          description: Is er een vorige pagina
        nextPage:
          type: boolean
          description: Is er een volgende pagina
    ResultsContactOverzicht:
      type: object
      required:
      - results
      - count
      - previousPage
      - nextPage
      properties:
        results:
          type: array
          items:
            $ref: "#/components/schemas/ContactOverzicht"
          description: Reslultaten op huidige pagina
        count:
          type: integer
          format: int64
          description: Totaal aantal
        previousPage:
          type: boolean
          description: Is er een vorige pagina
        nextPage:
          type: boolean
          description: Is er een volgende pagina
    ResultsDomein:
      type: object
      required:
      - results
      - count
      - previousPage
      - nextPage
      properties:
        results:
          type: array
          items:
            $ref: "#/components/schemas/Domein"
          description: Reslultaten op huidige pagina
        count:
          type: integer
          format: int64
          description: Totaal aantal
        previousPage:
          type: boolean
          description: Is er een vorige pagina
        nextPage:
          type: boolean
          description: Is er een volgende pagina
    ResultsDomeinObject:
      type: object
      required:
      - results
      - count
      - previousPage
      - nextPage
      properties:
        results:
          type: array
          items:
            $ref: "#/components/schemas/DomeinObject"
          description: Reslultaten op huidige pagina
        count:
          type: integer
          format: int64
          description: Totaal aantal
        previousPage:
          type: boolean
          description: Is er een vorige pagina
        nextPage:
          type: boolean
          description: Is er een volgende pagina
    ResultsGroepOverzicht:
      type: object
      required:
      - results
      - count
      - previousPage
      - nextPage
      properties:
        results:
          type: array
          items:
            $ref: "#/components/schemas/GroepOverzicht"
          description: Reslultaten op huidige pagina
        count:
          type: integer
          format: int64
          description: Totaal aantal
        previousPage:
          type: boolean
          description: Is er een vorige pagina
        nextPage:
          type: boolean
          description: Is er een volgende pagina
    ResultsMedewerkerOverzicht:
      type: object
      required:
      - results
      - count
      - previousPage
      - nextPage
      properties:
        results:
          type: array
          items:
            $ref: "#/components/schemas/MedewerkerOverzicht"
          description: Reslultaten op huidige pagina
        count:
          type: integer
          format: int64
          description: Totaal aantal
        previousPage:
          type: boolean
          description: Is er een vorige pagina
        nextPage:
          type: boolean
          description: Is er een volgende pagina
    ResultsReferentietabel:
      type: object
      required:
      - results
      - count
      - previousPage
      - nextPage
      properties:
        results:
          type: array
          items:
            $ref: "#/components/schemas/Referentietabel"
          description: Reslultaten op huidige pagina
        count:
          type: integer
          format: int64
          description: Totaal aantal
        previousPage:
          type: boolean
          description: Is er een vorige pagina
        nextPage:
          type: boolean
          description: Is er een volgende pagina
    ResultsReferentietabelRecord:
      type: object
      required:
      - results
      - count
      - previousPage
      - nextPage
      properties:
        results:
          type: array
          items:
            $ref: "#/components/schemas/ReferentietabelRecord"
          description: Reslultaten op huidige pagina
        count:
          type: integer
          format: int64
          description: Totaal aantal
        previousPage:
          type: boolean
          description: Is er een vorige pagina
        nextPage:
          type: boolean
          description: Is er een volgende pagina
    ResultsZaakOverzicht:
      type: object
      required:
      - results
      - count
      - previousPage
      - nextPage
      properties:
        results:
          type: array
          items:
            $ref: "#/components/schemas/ZaakOverzicht"
          description: Reslultaten op huidige pagina
        count:
          type: integer
          format: int64
          description: Totaal aantal
        previousPage:
          type: boolean
          description: Is er een vorige pagina
        nextPage:
          type: boolean
          description: Is er een volgende pagina
    ResultsZaaktypeOverzicht:
      type: object
      required:
      - results
      - count
      - previousPage
      - nextPage
      properties:
        results:
          type: array
          items:
            $ref: "#/components/schemas/ZaaktypeOverzicht"
          description: Reslultaten op huidige pagina
        count:
          type: integer
          format: int64
          description: Totaal aantal
        previousPage:
          type: boolean
          description: Is er een vorige pagina
        nextPage:
          type: boolean
          description: Is er een volgende pagina
    Ring2D:
      type: array
      minItems: 4
      items:
        type: array
        maxItems: 2
        minItems: 2
        items:
          type: number
        description: "Position as [longitude, latitude]"
      description: A connected sequence of four or more points creating a closed ring
    Rol:
      type: object
      required:
      - naam
      - omschrijving
      - actief
      properties:
        naam:
          type: string
          maxLength: 128
          description: Naam van rol
        omschrijving:
          type: string
          description: Omschrijving van rol
        actief:
          type: boolean
          description: Is rol actief
    SelectDocumentDataElement:
      type: object
      required:
      - waarden
      allOf:
      - $ref: "#/components/schemas/DataElement"
      properties:
        waarden:
          type: array
          items:
            type: string
          description: Lijst van geselecteerde documenten
    Selectielijstitem:
      type: object
      allOf:
      - $ref: "#/components/schemas/Referentie"
      properties:
        jaar:
          type: integer
          format: int32
          description: Versie (jaar) van selectielijst item
        domein:
          type: string
          maxLength: 255
          description: Naam van domein
        subdomein:
          type: string
          maxLength: 255
          description: Naam van subdomein
        bewaartermijnWaardering:
          type: string
          description: Waardering van de bewaartermijn
          $ref: "#/components/schemas/BewaartermijnWaardering"
        bewaartermijn:
          type: integer
          format: int32
          description: Bewaartermijn. Voor eenheid zie bewaartermijnEenheid
        bewaartermijnEenheid:
          type: string
          description: Bewaartermijn eenheid
          $ref: "#/components/schemas/BewaartermijnEenheid"
    SoortVerbintenis:
      type: string
      enum:
      - huwelijk
      - partnerschap
    StringDataElement:
      type: object
      allOf:
      - $ref: "#/components/schemas/DataElement"
      properties:
        waarde:
          type: string
          description: String waarde data element
    StringDomeinObjectAttribuutWaarde:
      type: object
      required:
      - waarde
      allOf:
      - $ref: "#/components/schemas/DomeinObjectAttribuutWaarde"
      properties:
        waarde:
          type: string
          description: String waarde
    StringReferentietabelRecordAttribuut:
      type: object
      required:
      - waarde
      allOf:
      - $ref: "#/components/schemas/ReferentietabelRecordAttribuut"
      properties:
        waarde:
          type: string
          description: String waarde
    StringsDataElement:
      type: object
      required:
      - waarden
      allOf:
      - $ref: "#/components/schemas/DataElement"
      properties:
        waarden:
          type: array
          items:
            type: string
          description: Lijst van string waarden
    Subject:
      type: object
      required:
      - subjecttype
      - handmatigToegevoegd
      oneOf:
      - $ref: "#/components/schemas/Persoon"
      - $ref: "#/components/schemas/Bedrijf"
      discriminator:
        propertyName: subjecttype
        mapping:
          persoon: "#/components/schemas/Persoon"
          bedrijf: "#/components/schemas/Bedrijf"
      properties:
        identifier:
          type: integer
          format: int64
          description: Interne identifier
        subjecttype:
          $ref: "#/components/schemas/Subjecttype"
          type: string
          description: Type subject (persoon of bedrijf
        telefoonnummer:
          type: string
          maxLength: 20
          description: Telefoonnummer
        telefoonnummerAlternatief:
          type: string
          maxLength: 20
          description: Alternatief telefoonnummer
        rekeningnummer:
          type: string
          maxLength: 64
          description: Bankrekeningnummer
        emailadres:
          type: string
          maxLength: 255
          description: Emailadres
        ontvangenZaakNotificaties:
          type: boolean
          description: Is het gewenst dat zaak notificaties worden ontvangen
        toestemmingZaakNotificatiesAlleenDigitaal:
          type: boolean
          description: Mogen zaak notificaties alleen digitaal worden verstuurd en
            dus niet per post
        handmatigToegevoegd:
          type: boolean
          description: Indicatie handmatig toegevoegd
        notities:
          type: array
          items:
            $ref: "#/components/schemas/SubjectNotitie"
          description: Notities
        adressen:
          type: array
          items:
            $ref: "#/components/schemas/Adres"
          description: Adressen
    SubjectNotitie:
      type: object
      required:
      - aangemaaktOp
      - aangemaaktDoor
      - titel
      - inhoud
      properties:
        ingangsdatumGeldigheid:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Datum vanaf wanneer notitie geldig is
        afdeling:
          type: string
          maxLength: 128
          description: Afdeling
        groep:
          type: string
          maxLength: 128
          description: Groep
        einddatumGeldigheid:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Datum tot wanneer notitie geldig is
        aangemaaktOp:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Datum waarop notitie is aangemaakt
        aangemaaktDoor:
          type: string
          maxLength: 128
          description: Volledige naam van medewerker die notitie heeft aangemaakt
            gevolgd door de naam van zijn primaire afdeling gescheiden door een komma
            en een spatie
        titel:
          type: string
          maxLength: 128
          description: Titel van notitie
        inhoud:
          type: string
          description: Inhoud van notitie
    Subjecttype:
      type: string
      enum:
      - persoon
      - bedrijf
    Taak:
      type: object
      required:
      - functioneelIdentificatie
      - startdatum
      - indicatieExternToegankelijk
      - taaktype
      - taaktypeOrigineel
      - historie
      properties:
        afdeling:
          type: string
          maxLength: 128
          description: Naam van afdeling waaraan taak is toegekend
        functioneelIdentificatie:
          type: string
          maxLength: 128
          description: Functionele identificatie
        groep:
          type: string
          maxLength: 128
          description: Naam van groep waaraan taak is toegekend
        behandelaar:
          type: string
          maxLength: 64
          description: Gebruikersnaam van medewerker aan wie taak is toegekend
        startdatum:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Startdatum van taak
        streefdatum:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Streefdatum van taak
        fataledatum:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Fatale datum van taak
        einddatum:
          type: string
          format: date-time
          examples:
          - 2022-03-10T12:15:50-04:00
          description: Einddatum van taak
        procesTaak:
          type: string
          maxLength: 255
          description: Referentie van externe taak
        indicatieExternToegankelijk:
          type: boolean
          description: Indicatie externe taak
        afgehandeldDoor:
          type: string
          maxLength: 255
          description: "Naam van gebruiker, afdeling of groep die taak heeft afgehandeld"
        processtap:
          type: string
          maxLength: 255
        taaktype:
          $ref: "#/components/schemas/Taaktype"
          type: string
          description: Taaktype
        taaktypeOrigineel:
          $ref: "#/components/schemas/Taaktype"
          type: string
          description: Originele taaktype waarmee taak aangemaakt is
        opschorttermijnStartdatum:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Datum vanaf taak is opgeschort
        opschorttermijnEinddatum:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Datum tot taak is opgeschort
        historie:
          type: array
          items:
            $ref: "#/components/schemas/TaakHistorie"
          description: Historie van taak
        toekenningEmail:
          type: string
          description: Emails voor notificatie/herinneringsemail voor een externe
            taak
        vestigingsnummer:
          type: string
          description: Om taak te koppelen aan vestigingsnummer
        kvkNummer:
          type: string
          description: Om taak te koppelen aan KvK nummer
        authenticatieniveau:
          type: string
          maxLength: 4
          description: Authenticatieniveau benodigd voor externen
    TaakDocument:
      type: object
      allOf:
      - $ref: "#/components/schemas/Referentie"
      properties:
        documentNaam:
          type: string
          maxLength: 255
          description: Document naam
        documentTemplate:
          type: string
          maxLength: 255
          description: Document template
        templateGroep:
          type: string
          maxLength: 255
          description: Template groep
        documenttype:
          type: object
          description: Type document
          $ref: "#/components/schemas/Documenttype"
    TaakDocumentGroep:
      type: object
      required:
      - naam
      properties:
        naam:
          type: string
          maxLength: 255
          description: Naam
        taakDocumenten:
          type: array
          items:
            $ref: "#/components/schemas/TaakDocument"
          description: Taak documenten
    TaakHistorie:
      type: object
      required:
      - typeWijziging
      allOf:
      - $ref: "#/components/schemas/History"
      properties:
        typeWijziging:
          $ref: "#/components/schemas/TaakHistorieTypeWijziging"
          type: string
          description: Type wijziging
    TaakHistorieTypeWijziging:
      type: string
      enum:
      - status
      - overdragen
      - document
      - streefdatum
      - fataledatum
      - opschorttermijn
    Taaktype:
      type: string
      enum:
      - standaard
      - iburgerzaken
      - extern
      - extern_ketenpartner
    Taal:
      type: object
      required:
      - functioneelId
      allOf:
      - $ref: "#/components/schemas/Referentie"
      properties:
        functioneelId:
          type: string
          maxLength: 3
          description: "Functionele id: Code van de taal conform de ISO 639-2/B standaard"
    ValidatieFout:
      type: object
      required:
      - message
      - field
      - value
      properties:
        message:
          type: string
          description: Validatie foutmelding
        field:
          type: string
          description: Naam veld met foutieve waarde
        value:
          type: string
          description: Foutieve waarde
    ValidatieFouten:
      type: object
      required:
      - melding
      - fouten
      properties:
        melding:
          type: string
          description: Foutmelding
        fouten:
          type: array
          items:
            $ref: "#/components/schemas/ValidatieFout"
          description: Validatie fout(en)
    VervallenAanduiding:
      type: string
      enum:
      - ingetrokken
      - van_rechtswege_vervallen
    Vestigingsstatus:
      type: string
      enum:
      - nieuw_in_handelsregister
      - nieuw_voor_profielhouder
      - vestiging_is_gewijzigd
      - vervalt_voor_profielhouder_blijft_in_handelsregister
      - opgeheven
      - opgeheven_ongedaan_gemaakt
      - opgeheven_ergens_anders_opnieuw
    Vestigingstype:
      type: string
      enum:
      - hoofdvestiging
      - nevenvestiging
    VoorlopigAntwoord:
      type: object
      required:
      - antwoord
      - antwoordDatumTijd
      properties:
        antwoord:
          type: string
          description: Voorlopig antwoord
        antwoordDatumTijd:
          type: string
          format: date-time
          examples:
          - 2022-03-10T12:15:50-04:00
          description: Datum/tijd voorlopige antwoord
        afdeling:
          type: string
          maxLength: 128
          description: Afdeling
        medewerker:
          type: string
          maxLength: 64
          description: Medewerker
    Zaak:
      type: object
      required:
      - functioneleIdentificatie
      - externeIdentificatie
      - omschrijving
      - zaaktype
      - vertrouwelijk
      - aangemaaktDoor
      - kanaal
      - creatieDatumTijd
      - streefdatum
      - zaakStatus
      - intake
      - historie
      - geautoriseerdVoorMedewerkers
      - procesGestart
      - heropend
      - open
      - vernietiging
      - notificeerbaar
      - gemigreerd
      properties:
        functioneleIdentificatie:
          type: string
          maxLength: 128
          description: Zaaknummer in e-Suite
        externeIdentificatie:
          type: string
          minLength: 5
          maxLength: 40
          description: Identificatie van zaak welke kan worden gebruikt bij koppelen
            met extern systeem
        omschrijving:
          type: string
          description: Omschrijving van zaak
        redenStart:
          type: string
          description: Reden voor starten van zaak
        zaaktype:
          $ref: "#/components/schemas/ZaaktypeOverzicht"
          type: object
          description: Zaaktype
        vertrouwelijk:
          type: boolean
          description: Is zaak vertrouwelijk
        behandelaar:
          type: string
          maxLength: 64
          description: Gebruikersnaam van behandelaar van zaak
        initiator:
          type: object
          description: Initiator van zaak
          $ref: "#/components/schemas/Subject"
        afdeling:
          type: string
          maxLength: 128
          description: Naam van afdeling waaraan zaak is toegekend
        groep:
          type: string
          maxLength: 128
          description: Naam van groep waaraan zaak is toegekend
        aangemaaktDoor:
          type: string
          maxLength: 64
          description: Gebruikersnaam van medewerker welke zaak heeft aangemaakt
        kanaal:
          $ref: "#/components/schemas/Kanaal"
          type: object
          description: Kanaal via welke zaak is binnengekomen
        creatieDatumTijd:
          type: string
          format: date-time
          examples:
          - 2022-03-10T12:15:50-04:00
          description: Tijdstip waarop zaak is aangemaakt
        wijzigDatumTijd:
          type: string
          format: date-time
          examples:
          - 2022-03-10T12:15:50-04:00
          description: Meest recente tijdstip waarop zaak is gewijzigd
        startdatum:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Datum waarop behandeling van zaak gestart is
        streefdatum:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Streefdatum van zaak
        fataledatum:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Fatale datum van zaak
        einddatum:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Einddatum van zaak
        zaakStatus:
          $ref: "#/components/schemas/Zaakstatus"
          type: object
          description: Huidige status van zaak
        resultaat:
          type: object
          description: Resultaat van zaak
          $ref: "#/components/schemas/Resultaat"
        betaalgegevens:
          type: object
          description: Betaalgegevens gerelateerd aan zaak
          $ref: "#/components/schemas/Betaalgegevens"
        intake:
          type: boolean
          description: Is zaak een intake
        archiveerGegevens:
          type: object
          description: Archiveer gegevens van zaak
          $ref: "#/components/schemas/ArchiveerGegevens"
        geolocatie:
          type: object
          description: Locatie van zaak. Coördinatenstelsel is RD_NEW (srid=28992)
          $ref: "#/components/schemas/Geometry"
        historie:
          type: array
          items:
            $ref: "#/components/schemas/ZaakHistorie"
          description: Historie van zaak
        zaakdata:
          type: array
          items:
            $ref: "#/components/schemas/DataElement"
          description: Zaak specifieke data
        organisatie:
          type: string
          description: Naam van organisatie
        opschorttermijnStartdatum:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Begindatum van een nu lopende opschorttermijn
        opschorttermijnEinddatum:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Einddatum van een nu lopende opschorttermijn
        geautoriseerdVoorMedewerkers:
          type: boolean
          description: Betreft dit een zaak met specifieke medewerker autorisatie
        geautoriseerdeMedewerkers:
          type: array
          uniqueItems: true
          items:
            type: string
          description: Gebruikersnamen van medewerkers welke zijn geautoriseerd voor
            zaak
        notities:
          type: array
          items:
            $ref: "#/components/schemas/ZaakNotitie"
          description: Notities op zaak
        procesGestart:
          type: boolean
          description: Is voor zaak afhandeling een proces gestart
        ztc1MigratiedatumTijd:
          type: string
          format: date-time
          examples:
          - 2022-03-10T12:15:50-04:00
          description: Datum tijd waarop de zaak gemigreerd is van ZTC1 naar ZTC2
        heropend:
          type: boolean
          description: Is zaak heropend nadat de zaak was beëindigd
        open:
          type: boolean
          description: Is zaak open (nog niet beëindigd)
        vernietiging:
          type: boolean
          description: Is zaak in vernietiging
        notificeerbaar:
          type: boolean
        taken:
          type: array
          items:
            $ref: "#/components/schemas/Taak"
          description: Taken gerelateerd aan zaak
        betrokkenen:
          type: array
          items:
            $ref: "#/components/schemas/ZaakBetrokkene"
          description: Betrokkenem gerelateerd aan zaak
        bagObjecten:
          type: array
          items:
            $ref: "#/components/schemas/BAGObject"
          description: BAG Objecten gerelateerd aan zaak
        gekoppeldeZaken:
          type: array
          items:
            $ref: "#/components/schemas/ZaakZaakKoppeling"
          description: Zaken gekoppeld aan zaak
        documenten:
          type: array
          items:
            $ref: "#/components/schemas/Document"
          description: Documenen gerelateerd aan zaak
        besluiten:
          type: array
          items:
            $ref: "#/components/schemas/Besluit"
          description: Besluiten gerelateerd aan zaak
        contacten:
          type: array
          items:
            type: string
          description: Functionele identificatie van contacten gerelateerd aan zaak
        gemigreerd:
          type: boolean
          description: Is zaak gemigreerd
    ZaakBetrokkene:
      type: object
      required:
      - indCorrespondentie
      - betrokkene
      - typeBetrokkenheid
      properties:
        indCorrespondentie:
          type: boolean
          description: Indicatie of correspondentie naar betrokkene gestuurd moet
            worden
        startdatum:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Startdatum betrokkenheid
        betrokkene:
          $ref: "#/components/schemas/Subject"
          type: object
          description: Betrokkene
        typeBetrokkenheid:
          $ref: "#/components/schemas/ZaakBetrokkenetype"
          type: string
          description: Type betrokkenheid
        toelichting:
          type: string
          description: Toelichting bij betrokkenheid
    ZaakBetrokkenetype:
      type: string
      enum:
      - belanghebbende
      - gemachtigde
      - medeaanvrager
      - melder
      - plaatsvervanger
      - overig
    ZaakHistorie:
      type: object
      required:
      - typeWijziging
      allOf:
      - $ref: "#/components/schemas/History"
      properties:
        typeWijziging:
          $ref: "#/components/schemas/ZaakHistorieTypeWijziging"
          type: string
          description: Type wijziging
        nieuweWaardeExtern:
          type: string
          description: Externe nieuwe waarde
    ZaakHistorieTypeWijziging:
      type: string
      enum:
      - overdragen
      - overbrengen
      - status
      - kanaal
      - streefdatum
      - startdatum
      - fataledatum
      - vertrouwelijkheid
      - zaak_omschrijving
      - reden_starten_zaak
      - verlenging_bewaartermijn
      - reviewtermijn
      - overdragen_ssa
      - overbrengen_extern
      - heropenen_zaak
      - nieuw_document
      - document_ontkoppeld
      - document_verwijderd
      - document_unlocked
      - aanvrager
      - betrokkenen
      - zaakresultaat
      - ogone_post_sale
      - ingangsdatum_besluit
      - vervaldatum_besluit
      - bewaartermijn
      - opschorttermijn
      - autorisatie
      - organisatie
      - proces_herstarten
      - betaling_verwerkt
      - bag_koppeling
      - zaak_koppeling
      - contact_koppeling
      - externe_taak
      - externe_taak_ketenpartner
      - locatie
      - notitie
      - zaaktype
      - document_status
      - metadataelement
      - documenttype
    ZaakNotitie:
      type: object
      required:
      - medewerker
      - datumTijd
      properties:
        medewerker:
          type: string
          maxLength: 40
          description: Gebruikersnaam van medewerker die notite heeft toegevoegd
        datumTijd:
          type: string
          format: date-time
          examples:
          - 2022-03-10T12:15:50-04:00
          description: Datum en tijd waarop notitie is toegevoegd
        notitie:
          type: string
          description: Inhoud van notitie
    ZaakOverzicht:
      type: object
      required:
      - functioneleIdentificatie
      - open
      - gemigreerd
      properties:
        functioneleIdentificatie:
          type: string
          maxLength: 128
          description: Zaaknummer in e-Suite
        open:
          type: boolean
          description: Is zaak open (nog niet beëindigd)
        gemigreerd:
          type: boolean
          description: Is zaak gemigreerd
    ZaakPatch:
      type: object
      required:
      - gemigreerd
      properties:
        gemigreerd:
          type: boolean
          description: Is zaak gemigreerd
    ZaakRelatietype:
      type: string
      enum:
      - hoofdzaak
      - deelzaak
      - gerelateerde_zaak
      - voorafgaande_zaak
      - vervolgzaak
    ZaakStartParameter:
      type: object
      required:
      - naam
      - type
      properties:
        naam:
          type: string
          maxLength: 255
          description: Naam
        type:
          type: string
          maxLength: 255
          description: Type
    ZaakZaakKoppeling:
      type: object
      required:
      - gekoppeldeZaak
      - relatietype
      - dossierEigenaar
      properties:
        gekoppeldeZaak:
          type: string
          maxLength: 128
          description: Zaaknummer van gekoppelde zaak
        relatietype:
          $ref: "#/components/schemas/ZaakRelatietype"
          type: string
          description: Type relatie van gekoppelde zaak
        dossierEigenaar:
          type: boolean
          description: Indicatie of zaak eigenaar is van zaakdossier voor gekoppelde
            zaak in het DMS
    Zaakstatus:
      type: object
      required:
      - uitwisselingscode
      allOf:
      - $ref: "#/components/schemas/Referentie"
      properties:
        uitwisselingscode:
          type: string
          maxLength: 255
          description: Code welke gebruikt wordt voor het uitwisselen van zaak informatie
            naar externe systemen zoals bijvoorbeeld via StUF-ZKN-DMS
        externeNaam:
          type: string
          maxLength: 255
          description: Naam van status zoals deze getoond wordt in het publieke deel
            (burger en bedrijven loket) van de e=Suite
    Zaaktype:
      type: object
      required:
      - functioneleIdentificatie
      - handelingInitiator
      - internExtern
      - categorie
      - iv3categorie
      - intake
      - beginGeldigheidDatum
      - doorlooptijdAanpassenToegestaan
      - aantalDagenVoorStreefdatumVoorEersteSignalering
      - aantalDagenVoorStreefdatumVoorTweedeSignalering
      - aantalDagenVoorFataledatumVoorEersteSignalering
      - aantalDagenVoorFataledatumVoorTweedeSignalering
      - startenProces
      - vertrouwelijk
      - geautoriseerdVoorMedewerkers
      - notificatiesVersturen
      - samenvattingDocumentNaam
      allOf:
      - $ref: "#/components/schemas/Referentie"
      properties:
        functioneleIdentificatie:
          type: string
          maxLength: 128
          description: Functionele identificatie
        handelingInitiator:
          $ref: "#/components/schemas/HandelingInitiator"
          type: object
          description: Actie die uitgevoerd wordt om zaak of proces te starten
        internExtern:
          $ref: "#/components/schemas/InternExtern"
          type: string
          description: Is zaak voor intern of extern gebruik
        categorie:
          $ref: "#/components/schemas/Categorie"
          type: object
          description: Categorie waarin zaak valt
        iv3categorie:
          $ref: "#/components/schemas/Iv3Categorie"
          type: object
          description: Informatie voor Derden Categorie waarin zaak valt
        afdeling:
          type: string
          maxLength: 255
          description: Naam van afdeling waaraan nieuwe zaak wordt toegekend
        groep:
          type: string
          maxLength: 255
          description: Naam van groep waaraan nieuwe zaak wordt toegekend
        intake:
          type: boolean
          description: Is zaak een intake
        beginGeldigheidDatum:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Datum begin geldigheid
        eindeGeldigheidDatum:
          type: string
          format: date
          examples:
          - 2022-03-10
          description: Datum einde geldigheid
        doorlooptijdGewenst:
          type: integer
          format: int32
          description: Gewenste doorlooptijd in dagen
        doorlooptijdVereist:
          type: integer
          format: int32
          description: Maximale doorlooptijd in dagen
        doorlooptijdAanpassenToegestaan:
          type: boolean
          description: Is aanpassen van doorlooptijd van zaak toegestaan
        aantalDagenVoorStreefdatumVoorEersteSignalering:
          type: integer
          format: int32
          description: Aantal dagen voorafgaand aan de streefdatum waarop de 1ste
            signalering plaats vindt
        aantalDagenVoorStreefdatumVoorTweedeSignalering:
          type: integer
          format: int32
          description: Aantal dagen voorafgaand aan de streefdatum waarop de 2de signalering
            plaats vindt
        aantalDagenVoorFataledatumVoorEersteSignalering:
          type: integer
          format: int32
          description: Aantal dagen voorafgaand aan de fataledatum waarop de 1ste
            signalering plaats vindt
        aantalDagenVoorFataledatumVoorTweedeSignalering:
          type: integer
          format: int32
          description: Aantal dagen voorafgaand aan de fataledatum waarop de 2de signalering
            plaats vindt
        status:
          type: object
          description: Initiele status van nieuwe zaak
          $ref: "#/components/schemas/Zaakstatus"
        archiveringReviewPeriode:
          type: integer
          format: int32
          description: Archivering reviewperiode in weken
        startenProces:
          type: boolean
          description: Moet proces gestart worden bij aanmaken van nieuwe zaak
        proces:
          type: string
          maxLength: 128
          description: Functionele identificatie van proces dat wordt gestart
        startformulier:
          type: string
          maxLength: 128
          description: Functionele identificatie van zaak startformulier
        startformulierVersie:
          type: integer
          format: int64
          description: Versie nummer van zaak startformulier
        vertrouwelijk:
          type: boolean
          description: Is zaak vertrouwelijk
        authenticaties:
          type: array
          items:
            $ref: "#/components/schemas/ZaaktypeAuthenticatie"
          description: Mogelijke authenticatie methoden bij aanmaken nieuwe zaak
        geautoriseerdVoorMedewerkers:
          type: boolean
          description: Betreft dit een zaak met specifieke medewerker autorisatie
        geautoriseerdeMedewerkers:
          type: array
          uniqueItems: true
          items:
            type: string
          description: Gebruikersnamen van medewerkers welke zijn geautoriseerd voor
            zaak
        notificatiesVersturen:
          type: boolean
          description: Moet er notificaties worden verstuurd voor de zaak
        statussen:
          type: array
          items:
            $ref: "#/components/schemas/Zaakstatus"
          description: Mogelijke statussen van zaak
        resultaten:
          type: array
          items:
            $ref: "#/components/schemas/ZaaktypeResultaat"
          description: Mogelijke resultaten van zaak
        besluiten:
          type: array
          items:
            $ref: "#/components/schemas/ZaaktypeBesluittype"
          description: Mogelijke besluiten bij zaak
        documenttypen:
          type: array
          items:
            $ref: "#/components/schemas/ZaaktypeDocumenttype"
          description: Mogelijke documenttypen van documenten bij zaak
        documentTags:
          type: array
          items:
            $ref: "#/components/schemas/DocumentTag"
          description: Tags welke gezet kunnen worden op documenten bij zaak
        gekoppeldeZaaktypen:
          type: array
          items:
            $ref: "#/components/schemas/ZaaktypeOverzicht"
          description: Zaaktypen waarvan zaken gekoppeld kunnen worden aan zaak
        taakDocumentGroepen:
          type: array
          items:
            $ref: "#/components/schemas/TaakDocumentGroep"
          description: Taakdocument groepen
        samenvattingDocumentNaam:
          type: string
          maxLength: 255
          description: Naam voor het samenvatting document
        zaakStartParameters:
          type: array
          items:
            $ref: "#/components/schemas/ZaakStartParameter"
          description: Zaak start parameters
        productaanvraagtype:
          type: string
          maxLength: 255
          description: Productaanvraagtype
    ZaaktypeAuthenticatie:
      type: object
      required:
      - doelgroep
      - niveau
      properties:
        doelgroep:
          $ref: "#/components/schemas/AuthenticatieDoelgroep"
          type: string
          description: Doelgroep van authenticatie
        niveau:
          $ref: "#/components/schemas/AuthenticatieNiveau"
          type: string
          description: Niveau van authenticatie
    ZaaktypeBesluittype:
      type: object
      required:
      - besluittype
      properties:
        besluittype:
          $ref: "#/components/schemas/Besluittype"
          type: object
          description: Type besluit
        documenttype:
          type: object
          description: Documenttype van document met besluit
          $ref: "#/components/schemas/Documenttype"
        procestermijn:
          type: integer
          format: int32
          description: Procestermijn in maanden
    ZaaktypeDocumenttype:
      type: object
      required:
      - documenttype
      properties:
        documenttype:
          $ref: "#/components/schemas/Documenttype"
          type: object
          description: Type document
        dspcode:
          type: string
          maxLength: 255
          description: DSP code
        titels:
          type: array
          items:
            $ref: "#/components/schemas/DocumentTitel"
          description: Mogelijke voorgedefinieerde document titels
    ZaaktypeOverzicht:
      type: object
      required:
      - functioneleIdentificatie
      allOf:
      - $ref: "#/components/schemas/Referentie"
      properties:
        functioneleIdentificatie:
          type: string
          maxLength: 128
          description: Functionele identificatie
    ZaaktypeResultaat:
      type: object
      required:
      - resultaat
      - selectielijstitem
      properties:
        resultaat:
          $ref: "#/components/schemas/Resultaat"
          type: object
          description: Resultaat van zaak
        selectielijstitem:
          $ref: "#/components/schemas/Selectielijstitem"
          type: object
          description: Selectielijst item
        bewaartermijnWaardering:
          type: string
          description: Waardering van de bewaartermijn
          $ref: "#/components/schemas/BewaartermijnWaardering"
        bewaartermijn:
          type: integer
          format: int32
          description: Bewaartermijn. Voor eenheid zie bewaartermijnEenheid
        bewaartermijnEenheid:
          type: string
          description: Bewaartermijn eenheid
          $ref: "#/components/schemas/BewaartermijnEenheid"
  securitySchemes:
    ApiKeyAuth:
      type: apiKey
      description: API key for authentication
      name: X-API-KEY
      in: header
info:
  contact:
    email: andy.verberne@atos.net
    name: Andy Verberne
  title: e-Suite data extractie API
  version: 0.12.0
security:
- ApiKeyAuth: []
paths:
  /afdelingen:
    get:
      summary: Lijst van afdeling overzichten opvragen
      operationId: afdeling_list
      parameters:
      - name: page
        in: query
        schema:
          type: integer
          format: int32
          description: Gewenste pagina; start bij 0
          default: 0
      - name: page_size
        in: query
        schema:
          type: integer
          format: int32
          description: Maximum aantal items per pagina
          default: 100
      responses:
        "401":
          description: Unauthorized
        "500":
          description: Internal Server Error
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Fout"
        "200":
          description: OK
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/ResultsAfdelingOverzicht"
        "400":
          description: Bad Request
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/ValidatieFouten"
      tags:
      - Afdelingen
  /afdelingen/{naam}:
    get:
      summary: Een specifieke afdeling opvragen
      operationId: afdeling_read
      parameters:
      - name: naam
        in: path
        required: true
        schema:
          type: string
      responses:
        "401":
          description: Unauthorized
        "500":
          description: Internal Server Error
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Fout"
        "200":
          description: OK
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Afdeling"
        "404":
          description: Not Found
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Fout"
      tags:
      - Afdelingen
  /bedrijven:
    get:
      summary: Bedrijven opvragen op basis van KvK nummer en of vestigingsnummer
      operationId: bedrijf_list
      parameters:
      - name: kvk_nummer
        in: query
        schema:
          type: string
          minLength: 8
          maxLength: 8
          description: KvK nummer
      - name: page
        in: query
        schema:
          type: integer
          format: int32
          description: Gewenste pagina; start bij 0
          default: 0
      - name: page_size
        in: query
        schema:
          type: integer
          format: int32
          description: Maximum aantal items per pagina
          default: 100
      - name: vestigingsnummer
        in: query
        schema:
          type: string
          minLength: 12
          maxLength: 12
          description: vestigingsnummer
      responses:
        "401":
          description: Unauthorized
        "500":
          description: Internal Server Error
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Fout"
        "200":
          description: OK
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/ResultsBedrijf"
        "400":
          description: Bad Request
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/ValidatieFouten"
      tags:
      - Bedrijven
  /bedrijven/{identifier}:
    get:
      summary: Een specifiek bedrijf opvragen op basis van interne identifier
      operationId: bedrijf_read
      parameters:
      - name: identifier
        in: path
        required: true
        schema:
          type: integer
          format: int64
      responses:
        "401":
          description: Unauthorized
        "500":
          description: Internal Server Error
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Fout"
        "200":
          description: OK
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Bedrijf"
        "404":
          description: Not Found
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Fout"
      tags:
      - Bedrijven
  /contacten:
    get:
      summary: Lijst van contact overzichten opvragen
      operationId: contact_list
      parameters:
      - name: page
        in: query
        schema:
          type: integer
          format: int32
          description: Gewenste pagina; start bij 0
          default: 0
      - name: page_size
        in: query
        schema:
          type: integer
          format: int32
          description: Maximum aantal items per pagina
          default: 100
      responses:
        "401":
          description: Unauthorized
        "500":
          description: Internal Server Error
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Fout"
        "200":
          description: OK
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/ResultsContactOverzicht"
        "400":
          description: Bad Request
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/ValidatieFouten"
      tags:
      - Contacten
  /contacten/{functionele_Identificatie}:
    get:
      summary: Een specifiek contact opvragen
      operationId: contact_read
      parameters:
      - name: functionele_Identificatie
        in: path
        required: true
        schema:
          type: string
      responses:
        "401":
          description: Unauthorized
        "500":
          description: Internal Server Error
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Fout"
        "200":
          description: OK
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Contact"
        "404":
          description: Not Found
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Fout"
      tags:
      - Contacten
  /documenten/inhoud/{documentInhoudID}:
    get:
      summary: Inhoud van een document ophalen
      operationId: document_inhoud_read
      parameters:
      - name: documentInhoudID
        in: path
        required: true
        schema:
          type: integer
          format: int64
      responses:
        "401":
          description: Unauthorized
        "500":
          description: Internal Server Error
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Fout"
        "200":
          description: OK
          content:
            application/octet-stream:
              schema:
                type: string
                format: binary
                description: Inhoud van het document
        "404":
          description: Not Found
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Fout"
      tags:
      - Documenten
  /domeinen:
    get:
      summary: Lijst van domeinen opvragen
      operationId: domein_list
      parameters:
      - name: page
        in: query
        schema:
          type: integer
          format: int32
          description: Gewenste pagina; start bij 0
          default: 0
      - name: page_size
        in: query
        schema:
          type: integer
          format: int32
          description: Maximum aantal items per pagina
          default: 100
      responses:
        "401":
          description: Unauthorized
        "500":
          description: Internal Server Error
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Fout"
        "200":
          description: OK
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/ResultsDomein"
        "400":
          description: Bad Request
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/ValidatieFouten"
      tags:
      - Domeinen
  /domeinen/{domein_naam}/objecten:
    get:
      summary: Objecten van een domein opvragen
      operationId: domein_object_list
      parameters:
      - name: domein_naam
        in: path
        required: true
        schema:
          type: string
      - name: page
        in: query
        schema:
          type: integer
          format: int32
          description: Gewenste pagina; start bij 0
          default: 0
      - name: page_size
        in: query
        schema:
          type: integer
          format: int32
          description: Maximum aantal items per pagina
          default: 100
      responses:
        "401":
          description: Unauthorized
        "500":
          description: Internal Server Error
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Fout"
        "200":
          description: OK
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/ResultsDomeinObject"
        "400":
          description: Bad Request
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/ValidatieFouten"
      tags:
      - Domeinen
  /domeinen/{naam}:
    get:
      summary: Een specifiek domein opvragen
      operationId: domein_read
      parameters:
      - name: naam
        in: path
        required: true
        schema:
          type: string
      responses:
        "401":
          description: Unauthorized
        "500":
          description: Internal Server Error
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Fout"
        "200":
          description: OK
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Domein"
        "404":
          description: Not Found
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Fout"
      tags:
      - Domeinen
  /groepen:
    get:
      summary: Lijst van groep overzichten opvragen
      operationId: groep_list
      parameters:
      - name: page
        in: query
        schema:
          type: integer
          format: int32
          description: Gewenste pagina; start bij 0
          default: 0
      - name: page_size
        in: query
        schema:
          type: integer
          format: int32
          description: Maximum aantal items per pagina
          default: 100
      responses:
        "401":
          description: Unauthorized
        "500":
          description: Internal Server Error
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Fout"
        "200":
          description: OK
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/ResultsGroepOverzicht"
        "400":
          description: Bad Request
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/ValidatieFouten"
      tags:
      - Groepen
  /groepen/{naam}:
    get:
      summary: Een specifieke groep opvragen
      operationId: groep_read
      parameters:
      - name: naam
        in: path
        required: true
        schema:
          type: string
      responses:
        "401":
          description: Unauthorized
        "500":
          description: Internal Server Error
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Fout"
        "200":
          description: OK
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Groep"
        "404":
          description: Not Found
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Fout"
      tags:
      - Groepen
  /medewerkers:
    get:
      summary: Lijst van medewerker overzichten opvragen
      operationId: medewerker_list
      parameters:
      - name: page
        in: query
        schema:
          type: integer
          format: int32
          description: Gewenste pagina; start bij 0
          default: 0
      - name: page_size
        in: query
        schema:
          type: integer
          format: int32
          description: Maximum aantal items per pagina
          default: 100
      responses:
        "401":
          description: Unauthorized
        "500":
          description: Internal Server Error
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Fout"
        "200":
          description: OK
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/ResultsMedewerkerOverzicht"
        "400":
          description: Bad Request
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/ValidatieFouten"
      tags:
      - Medewerkers
  /medewerkers/{gebruikersnaam}:
    get:
      summary: Een specifieke medewerker opvragen
      operationId: medewerker_read
      parameters:
      - name: gebruikersnaam
        in: path
        required: true
        schema:
          type: string
      responses:
        "401":
          description: Unauthorized
        "500":
          description: Internal Server Error
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Fout"
        "200":
          description: OK
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Medewerker"
        "404":
          description: Not Found
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Fout"
      tags:
      - Medewerkers
  /personen:
    get:
      summary: Persoon opvragen op basis van BSN
      operationId: persoon_read_bsn
      parameters:
      - name: bsn
        in: query
        schema:
          type: string
          minLength: 9
          maxLength: 9
          description: Burgerservicenummer
        required: true
      responses:
        "401":
          description: Unauthorized
        "500":
          description: Internal Server Error
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Fout"
        "200":
          description: OK
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Persoon"
        "400":
          description: Bad Request
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/ValidatieFouten"
        "404":
          description: Not Found
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Fout"
      tags:
      - Personen
  /personen/{identifier}:
    get:
      summary: Een specifiek persoon opvragen op basis van interne identifier
      operationId: persoon_read
      parameters:
      - name: identifier
        in: path
        required: true
        schema:
          type: integer
          format: int64
      responses:
        "401":
          description: Unauthorized
        "500":
          description: Internal Server Error
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Fout"
        "200":
          description: OK
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Persoon"
        "404":
          description: Not Found
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Fout"
      tags:
      - Personen
  /referentietabellen:
    get:
      summary: Lijst van referentietabellen opvragen
      operationId: referentietabel_list
      parameters:
      - name: page
        in: query
        schema:
          type: integer
          format: int32
          description: Gewenste pagina; start bij 0
          default: 0
      - name: page_size
        in: query
        schema:
          type: integer
          format: int32
          description: Maximum aantal items per pagina
          default: 100
      responses:
        "401":
          description: Unauthorized
        "500":
          description: Internal Server Error
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Fout"
        "200":
          description: OK
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/ResultsReferentietabel"
        "400":
          description: Bad Request
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/ValidatieFouten"
      tags:
      - Referentietabellen
  /referentietabellen/{naam}:
    get:
      summary: Een specifieke referentietabel opvragen
      operationId: referentietabel_read
      parameters:
      - name: naam
        in: path
        required: true
        schema:
          type: string
      responses:
        "401":
          description: Unauthorized
        "500":
          description: Internal Server Error
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Fout"
        "200":
          description: OK
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Referentietabel"
        "404":
          description: Not Found
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Fout"
      tags:
      - Referentietabellen
  /referentietabellen/{referentietabel_naam}/records:
    get:
      summary: Records van een referentietabel opvragen
      operationId: referentietabel_record_list
      parameters:
      - name: referentietabel_naam
        in: path
        required: true
        schema:
          type: string
      - name: page
        in: query
        schema:
          type: integer
          format: int32
          description: Gewenste pagina; start bij 0
          default: 0
      - name: page_size
        in: query
        schema:
          type: integer
          format: int32
          description: Maximum aantal items per pagina
          default: 100
      responses:
        "401":
          description: Unauthorized
        "500":
          description: Internal Server Error
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Fout"
        "200":
          description: OK
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/ResultsReferentietabelRecord"
        "400":
          description: Bad Request
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/ValidatieFouten"
      tags:
      - Referentietabellen
  /zaaktypen:
    get:
      summary: Lijst van zaaktypen opvragen
      operationId: zaaktype_list
      parameters:
      - name: page
        in: query
        schema:
          type: integer
          format: int32
          description: Gewenste pagina; start bij 0
          default: 0
      - name: page_size
        in: query
        schema:
          type: integer
          format: int32
          description: Maximum aantal items per pagina
          default: 100
      responses:
        "401":
          description: Unauthorized
        "500":
          description: Internal Server Error
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Fout"
        "200":
          description: OK
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/ResultsZaaktypeOverzicht"
        "400":
          description: Bad Request
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/ValidatieFouten"
      tags:
      - Zaaktypen
  /zaaktypen/{naam}:
    get:
      summary: Een specifiek zaaktype opvragen op basis van naam
      operationId: zaaktype_read
      parameters:
      - name: naam
        in: path
        required: true
        schema:
          type: string
      responses:
        "401":
          description: Unauthorized
        "500":
          description: Internal Server Error
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Fout"
        "200":
          description: OK
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Zaaktype"
        "404":
          description: Not Found
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Fout"
      tags:
      - Zaaktypen
  /zaken:
    get:
      summary: Lijst van zaak overzichten van afgeronde zaken opvragen. Indien parameter
        'inclusiefOpen=true' wordt meegegeven dan bevat de lijst ook openstaande zaken.
      operationId: zaak_list
      parameters:
      - name: inclusiefOpen
        in: query
        schema:
          type: boolean
          description: Inclusief open zaken
          default: false
      - name: page
        in: query
        schema:
          type: integer
          format: int32
          description: Gewenste pagina; start bij 0
          default: 0
      - name: page_size
        in: query
        schema:
          type: integer
          format: int32
          description: Maximum aantal items per pagina
          default: 100
      - name: zaaktype
        in: query
        schema:
          type: string
          maxLength: 255
          description: Zaaktype naam
        required: true
      responses:
        "401":
          description: Unauthorized
        "500":
          description: Internal Server Error
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Fout"
        "200":
          description: OK
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/ResultsZaakOverzicht"
        "400":
          description: Bad Request
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/ValidatieFouten"
      tags:
      - Zaken
  /zaken/{functionele_Identificatie}:
    get:
      summary: Een specifieke zaak opvragen op basis van de functiele identificatie
      operationId: zaak_read
      parameters:
      - name: functionele_Identificatie
        in: path
        required: true
        schema:
          type: string
      responses:
        "401":
          description: Unauthorized
        "500":
          description: Internal Server Error
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Fout"
        "200":
          description: OK
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Zaak"
        "404":
          description: Not Found
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Fout"
      tags:
      - Zaken
    patch:
      summary: Voor een specifieke zaak aangeven of deze is gemigreerd
      operationId: zaak_patch
      parameters:
      - name: functionele_Identificatie
        in: path
        required: true
        schema:
          type: string
      requestBody:
        required: true
        content:
          application/json:
            schema:
              $ref: "#/components/schemas/ZaakPatch"
      responses:
        "401":
          description: Unauthorized
        "500":
          description: Internal Server Error
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Fout"
        "204":
          description: No Content
        "404":
          description: Not Found
          content:
            application/json:
              schema:
                $ref: "#/components/schemas/Fout"
      tags:
      - Zaken
```