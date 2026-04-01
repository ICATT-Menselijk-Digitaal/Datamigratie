using Datamigratie.Common.Services.Det.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Datamigratie.Server.Features.MigrateZaken.MigrateZaak.Pdf
{
    public interface IZaakgegevensPdfGenerator
    {
        /// <summary>
        /// Generates a PDF document containing basic zaak information
        /// </summary>
        /// <param name="zaak">The DET zaak to generate PDF for</param>
        /// <returns>PDF document as byte array</returns>
        byte[] GenerateZaakgegevensPdf(DetZaak zaak);
    }

    public class ZaakgegevensPdfGenerator : IZaakgegevensPdfGenerator
    {
        public byte[] GenerateZaakgegevensPdf(DetZaak zaak)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);

                    page.Content().Column(col =>
                    {
                        // Zaak
                        AddSection(col, "Zaak", table =>
                        {
                            Row(table, "functioneleIdentificatie", zaak.FunctioneleIdentificatie);
                            Row(table, "omschrijving", zaak.Omschrijving);
                            Row(table, "aangemaaktDoor", zaak.AangemaaktDoor);
                            Row(table, "afdeling", zaak.Afdeling);
                            Row(table, "externeIdentificatie", zaak.ExterneIdentificatie);
                            Row(table, "organisatie", zaak.Organisatie);
                            Row(table, "redenStart", zaak.RedenStart);
                            Row(table, "startdatum", FormatDate(zaak.Startdatum));
                            Row(table, "streefdatum", FormatDate(zaak.Streefdatum));
                            Row(table, "fataledatum", FormatDate(zaak.Fataledatum));
                            Row(table, "einddatum", FormatDate(zaak.Einddatum));
                            Row(table, "opschorttermijnStartdatum", FormatDate(zaak.OpschorttermijnStartdatum));
                            Row(table, "opschorttermijnEinddatum", FormatDate(zaak.OpschorttermijnEinddatum));
                            Row(table, "creatieDatumTijd", FormatDateTime(zaak.CreatieDatumTijd));
                            Row(table, "wijzigDatumTijd", FormatDateTime(zaak.WijzigDatumTijd));
                            Row(table, "ztc1MigratiedatumTijd", FormatDateTime(zaak.Ztc1MigratiedatumTijd));
                            Row(table, "zaakStatus.naam", zaak.ZaakStatus?.Naam);
                            Row(table, "resultaat.naam", zaak.Resultaat?.Naam);
                            Row(table, "open", zaak.Open.ToString());
                            Row(table, "intake", zaak.Intake.ToString());
                            Row(table, "notificeerbaar", zaak.Notificeerbaar.ToString());
                            Row(table, "geautoriseerdVoorMedewerkers", zaak.GeautoriseerdVoorMedewerkers.ToString());
                            Row(table, "heropend", zaak.Heropend.ToString());
                            Row(table, "vertrouwelijk", zaak.Vertrouwelijk.ToString());
                            Row(table, "vernietiging", zaak.Vernietiging.ToString());
                            Row(table, "procesGestart", zaak.ProcesGestart.ToString());
                            Row(table, "geolocatie", zaak.Geolocatie != null ? $"{zaak.Geolocatie.Type} {string.Join(", ", zaak.Geolocatie.Point2D ?? [])}" : null);
                        });

                        // Zaaktype
                        if (zaak.Zaaktype != null)
                        {
                            AddSection(col, "Zaaktype", table =>
                            {
                                Row(table, "functioneleIdentificatie", zaak.Zaaktype.FunctioneleIdentificatie);
                                Row(table, "naam", zaak.Zaaktype.Naam);
                                Row(table, "omschrijving", zaak.Zaaktype.Omschrijving);
                                Row(table, "actief", zaak.Zaaktype.Actief.ToString());
                            });
                        }

                        // Betaalgegevens
                        if (zaak.Betaalgegevens != null)
                        {
                            var b = zaak.Betaalgegevens;
                            AddSection(col, "Betaalgegevens", table =>
                            {
                                Row(table, "bedrag", b.Bedrag.HasValue ? $"€{b.Bedrag.Value:F2}" : null);
                                Row(table, "betaalstatus", b.Betaalstatus);
                                Row(table, "kenmerk", b.Kenmerk);
                                Row(table, "transactieDatum", FormatDate(b.TransactieDatum));
                                Row(table, "transactieId", b.TransactieId);
                                Row(table, "origineleStatusCode", b.OrigineleStatusCode);
                                Row(table, "ncerror", b.Ncerror);
                            });
                        }

                        // ArchiveerGegevens
                        if (zaak.ArchiveerGegevens != null)
                        {
                            var a = zaak.ArchiveerGegevens;
                            AddSection(col, "ArchiveerGegevens", table =>
                            {
                                Row(table, "bewaartermijnEinddatum", FormatDate(a.BewaartermijnEinddatum));
                                Row(table, "bewaartermijnWaardering", a.BewaartermijnWaardering);
                                Row(table, "overbrengenOp", FormatDate(a.OverbrengenOp));
                                Row(table, "overbrengenDoor", a.OverbrengenDoor);
                                Row(table, "overbrengenNaar", a.OverbrengenNaar);
                                Row(table, "overbrengenType", a.OverbrengenType);
                                Row(table, "selectielijstItemNaam", a.SelectielijstItemNaam);
                                Row(table, "zaaktypeNaam", a.ZaaktypeNaam);
                            });

                            if (a.OvergebrachteGegevens != null)
                            {
                                var og = a.OvergebrachteGegevens;
                                AddSection(col, "OvergebrachteGegevens", table =>
                                {
                                    Row(table, "overgebrachtOp", FormatDate(og.OvergebrachtOp));
                                    Row(table, "overgebrachtDoor", og.OvergebrachtDoor);
                                    Row(table, "overgebrachtNaar", og.OvergebrachtNaar);
                                });
                            }
                        }

                        // Kanaal
                        if (zaak.Kanaal != null)
                        {
                            AddSection(col, "Kanaal", table =>
                            {
                                Row(table, "naam", zaak.Kanaal.Naam);
                                Row(table, "omschrijving", zaak.Kanaal.Omschrijving);
                            });
                        }

                        // GekoppeldeZaken
                        if (zaak.GekoppeldeZaken != null)
                        {
                            for (var i = 0; i < zaak.GekoppeldeZaken.Count; i++)
                            {
                                var koppeling = zaak.GekoppeldeZaken[i];
                                AddSection(col, $"GekoppeldeZaak {i + 1}", table =>
                                {
                                    Row(table, "gekoppeldeZaak", koppeling.GekoppeldeZaak);
                                    Row(table, "relatietype", koppeling.Relatietype);
                                    Row(table, "dossierEigenaar", koppeling.DossierEigenaar.ToString());
                                });
                            }
                        }

                        // Notities
                        if (zaak.Notities != null)
                        {
                            for (var i = 0; i < zaak.Notities.Count; i++)
                            {
                                var n = zaak.Notities[i];
                                AddSection(col, $"Notitie {i + 1}", table =>
                                {
                                    Row(table, "medewerker", n.Medewerker);
                                    Row(table, "datumTijd", FormatDateTime(n.DatumTijd));
                                    Row(table, "notitie", n.Notitie);
                                });
                            }
                        }

                        // Betrokkenen
                        if (zaak.Betrokkenen != null)
                        {
                            for (var i = 0; i < zaak.Betrokkenen.Count; i++)
                            {
                                var bt = zaak.Betrokkenen[i];
                                AddSection(col, $"Betrokkene {i + 1}", table =>
                                {
                                    Row(table, "typeBetrokkenheid", bt.TypeBetrokkenheid);
                                    Row(table, "startdatum", FormatDate(bt.Startdatum));
                                    Row(table, "indCorrespondentie", bt.IndCorrespondentie.ToString());
                                    Row(table, "toelichting", bt.Toelichting);
                                });
                            }
                        }

                        // BagObjecten
                        if (zaak.BagObjecten != null)
                        {
                            for (var i = 0; i < zaak.BagObjecten.Count; i++)
                            {
                                var bag = zaak.BagObjecten[i];
                                AddSection(col, $"BagObject {i + 1}", table =>
                                {
                                    Row(table, "bagObjectId", bag.BagObjectId);
                                });
                            }
                        }

                        // Besluiten
                        if (zaak.Besluiten != null)
                        {
                            for (var i = 0; i < zaak.Besluiten.Count; i++)
                            {
                                var besluit = zaak.Besluiten[i];
                                AddSection(col, $"Besluit {i + 1}", table =>
                                {
                                    Row(table, "functioneleIdentificatie", besluit.FunctioneleIdentificatie);
                                    Row(table, "besluittype.naam", besluit.Besluittype.Naam);
                                    Row(table, "besluittype.omschrijving", besluit.Besluittype.Omschrijving);
                                    Row(table, "besluittype.actief", besluit.Besluittype.Actief.ToString());
                                    Row(table, "besluitDatum", FormatDate(besluit.BesluitDatum));
                                    Row(table, "ingangsdatum", FormatDate(besluit.Ingangsdatum));
                                    Row(table, "vervaldatum", FormatDate(besluit.Vervaldatum));
                                    Row(table, "publicatiedatum", FormatDate(besluit.Publicatiedatum));
                                    Row(table, "reactiedatum", FormatDate(besluit.Reactiedatum));
                                    Row(table, "toelichting", besluit.Toelichting);
                                    Row(table, "procestermijnInMaanden", besluit.ProcestermijnInMaanden?.ToString());
                                });
                            }
                        }

                        // Zaakdata
                        if (zaak.Zaakdata != null)
                        {
                            for (var i = 0; i < zaak.Zaakdata.Count; i++)
                            {
                                var zd = zaak.Zaakdata[i];
                                AddSection(col, $"Zaakdata: {zd.Naam}", table =>
                                {
                                    Row(table, "type", zd.Type);
                                    Row(table, "naam", zd.Naam);
                                    Row(table, "omschrijving", zd.Omschrijving);
                                });
                            }
                        }

                        // Taken
                        if (zaak.Taken != null)
                        {
                            for (var i = 0; i < zaak.Taken.Count; i++)
                            {
                                var taak = zaak.Taken[i];
                                AddSection(col, $"Taak: {taak.FunctioneelIdentificatie}", table =>
                                {
                                    Row(table, "functioneelIdentificatie", taak.FunctioneelIdentificatie);
                                    Row(table, "taaktype", taak.Taaktype);
                                    Row(table, "processtap", taak.Processtap);
                                    Row(table, "startdatum", FormatDate(taak.Startdatum));
                                    Row(table, "streefdatum", FormatDate(taak.Streefdatum));
                                    Row(table, "fataledatum", FormatDate(taak.Fataledatum));
                                    Row(table, "einddatum", FormatDateTime(taak.Einddatum));
                                    Row(table, "afgehandeldDoor", taak.AfgehandeldDoor);
                                    Row(table, "indicatieExternToegankelijk", taak.IndicatieExternToegankelijk.ToString());
                                    Row(table, "vestigingsnummer", taak.Vestigingsnummer);
                                    Row(table, "kvkNummer", taak.KvkNummer);
                                    Row(table, "toekenningEmail", taak.ToekenningEmail);
                                });

                                for (var j = 0; j < taak.Historie.Count; j++)
                                {
                                    var th = taak.Historie[j];
                                    AddSection(col, $"Taak {i + 1} — Historie {j + 1}", table =>
                                    {
                                        Row(table, "typeWijziging", th.TypeWijziging);
                                        Row(table, "gewijzigdDoor", th.GewijzigdDoor);
                                        Row(table, "wijzigingDatum", FormatDate(th.WijzigingDatum));
                                        Row(table, "oudeWaarde", th.OudeWaarde);
                                        Row(table, "nieuweWaarde", th.NieuweWaarde);
                                        Row(table, "toelichting", th.Toelichting);
                                    });
                                }
                            }
                        }

                        // ZaakHistorie
                        for (var i = 0; i < zaak.Historie.Count; i++)
                        {
                            var h = zaak.Historie[i];
                            AddSection(col, $"Historie {i + 1}", table =>
                            {
                                Row(table, "typeWijziging", h.TypeWijziging);
                                Row(table, "gewijzigdDoor", h.GewijzigdDoor);
                                Row(table, "wijzigingDatum", FormatDate(h.WijzigingDatum));
                                Row(table, "oudeWaarde", h.OudeWaarde);
                                Row(table, "nieuweWaarde", h.NieuweWaarde);
                                Row(table, "nieuweWaardeExtern", h.NieuweWaardeExtern);
                                Row(table, "toelichting", h.Toelichting);
                            });
                        }

                        // GekoppeldeContacten
                        if (zaak.GekoppeldeContacten != null)
                        {
                            for (var i = 0; i < zaak.GekoppeldeContacten.Count; i++)
                            {
                                var contact = zaak.GekoppeldeContacten[i];
                                AddSection(col, $"Contact {i + 1}", table =>
                                {
                                    Row(table, "functioneleIdentificatie", contact.FunctioneleIdentificatie);
                                    Row(table, "indicatieVertrouwelijk", contact.IndicatieVertrouwelijk.ToString());
                                    Row(table, "emailadres", contact.Emailadres);
                                    Row(table, "telefoonnummer", contact.Telefoonnummer);
                                    Row(table, "telefoonnummerAlternatief", contact.TelefoonnummerAlternatief);
                                    Row(table, "startdatumTijd", FormatDateTime(contact.StartdatumTijd));
                                    Row(table, "einddatumTijd", FormatDateTime(contact.EinddatumTijd));
                                    Row(table, "streefdatumTijd", FormatDateTime(contact.StreefdatumTijd));
                                    Row(table, "vraag", contact.Vraag);
                                    Row(table, "antwoord", contact.Antwoord);
                                    Row(table, "type.naam", contact.Type?.Naam);
                                    Row(table, "type.omschrijving", contact.Type?.Omschrijving);
                                    Row(table, "status.naam", contact.Status?.Naam);
                                    Row(table, "status.omschrijving", contact.Status?.Omschrijving);
                                    Row(table, "prioriteit.naam", contact.Prioriteit?.Naam);
                                    Row(table, "prioriteit.omschrijving", contact.Prioriteit?.Omschrijving);
                                    Row(table, "prioriteit.dagen", contact.Prioriteit?.Dagen.ToString());
                                    Row(table, "kanaal.omschrijving", contact.Kanaal?.Omschrijving);
                                });

                                if (contact.VoorlopigeAntwoorden != null)
                                {
                                    for (var v = 0; v < contact.VoorlopigeAntwoorden.Count; v++)
                                    {
                                        var va = contact.VoorlopigeAntwoorden[v];
                                        AddSection(col, $"Contact {i + 1} — VoorlopigAntwoord {v + 1}", table =>
                                        {
                                            Row(table, "antwoord", va.Antwoord);
                                            Row(table, "antwoordDatumTijd", FormatDateTime(va.AntwoordDatumTijd));
                                        });
                                    }
                                }

                                if (contact.BagObjecten != null)
                                {
                                    for (var b = 0; b < contact.BagObjecten.Count; b++)
                                    {
                                        var bag = contact.BagObjecten[b];
                                        AddSection(col, $"Contact {i + 1} — BagObject {b + 1}", table =>
                                        {
                                            Row(table, "bagObjectId", bag.BagObjectId);
                                        });
                                    }
                                }

                                if (contact.GekoppeldeContacten != null)
                                {
                                    for (var k = 0; k < contact.GekoppeldeContacten.Count; k++)
                                    {
                                        AddSection(col, $"Contact {i + 1} — GekoppeldContact {k + 1}", table =>
                                        {
                                            Row(table, "gekoppeldContact", contact.GekoppeldeContacten[k]);
                                        });
                                    }
                                }

                                if (contact.Historie != null)
                                {
                                    for (var h = 0; h < contact.Historie.Count; h++)
                                    {
                                        var ch = contact.Historie[h];
                                        AddSection(col, $"Contact {i + 1} — Historie {h + 1}", table =>
                                        {
                                            Row(table, "typeWijziging", ch.TypeWijziging);
                                            Row(table, "gewijzigdDoor", ch.GewijzigdDoor);
                                            Row(table, "oudeWaarde", ch.OudeWaarde);
                                            Row(table, "nieuweWaarde", ch.NieuweWaarde);
                                            Row(table, "toelichting", ch.Toelichting);
                                        });
                                    }
                                }
                            }
                        }

                        // Documenten
                        if (zaak.Documenten != null)
                        {
                            for (var i = 0; i < zaak.Documenten.Count; i++)
                            {
                                var doc = zaak.Documenten[i];
                                AddSection(col, $"Document: {doc.Titel}", table =>
                                {
                                    Row(table, "titel", doc.Titel);
                                    Row(table, "kenmerk", doc.Kenmerk);
                                    Row(table, "beschrijving", doc.Beschrijving);
                                    Row(table, "publicatieniveau", doc.Publicatieniveau);
                                    Row(table, "documentrichting", doc.Documentrichting);
                                    Row(table, "geautoriseerdVoorMedewerkers", doc.GeautoriseerdVoorMedewerkers.ToString());
                                    Row(table, "taal.naam", doc.Taal?.Naam);
                                    Row(table, "documentVorm.naam", doc.DocumentVorm?.Naam);
                                    Row(table, "ontvangstDatum", FormatDate(doc.OntvangstDatum));
                                    Row(table, "verzendDatum", FormatDate(doc.VerzendDatum));
                                    Row(table, "documentVersturenDatum", FormatDate(doc.DocumentVersturenDatum));
                                    Row(table, "documentVersturen", doc.DocumentVersturen);
                                    Row(table, "locatie", doc.Locatie);
                                    Row(table, "aanvraagDocument", doc.AanvraagDocument.ToString());
                                    Row(table, "pdfaDocumentInhoudID", doc.PdfaDocumentInhoudID?.ToString());
                                });

                                if (doc.PdfaDocumentversie != null)
                                {
                                    AddSection(col, $"Document {i + 1} — PdfaDocumentversie", table =>
                                    {
                                        Row(table, "versienummer", doc.PdfaDocumentversie.Versienummer.ToString());
                                        Row(table, "bestandsnaam", doc.PdfaDocumentversie.Bestandsnaam);
                                        Row(table, "mimetype", doc.PdfaDocumentversie.Mimetype);
                                        Row(table, "creatiedatum", FormatDate(doc.PdfaDocumentversie.Creatiedatum));
                                        Row(table, "auteur", doc.PdfaDocumentversie.Auteur);
                                        Row(table, "afzender", doc.PdfaDocumentversie.Afzender);
                                        Row(table, "documentgrootte", doc.PdfaDocumentversie.Documentgrootte?.ToString());
                                        Row(table, "compressed", doc.PdfaDocumentversie.Compressed.ToString());
                                    });
                                }

                                if (doc.Documenttype != null)
                                {
                                    AddSection(col, $"Document {i + 1} — Documenttype", table =>
                                    {
                                        Row(table, "naam", doc.Documenttype.Naam);
                                        Row(table, "omschrijving", doc.Documenttype.Omschrijving);
                                        Row(table, "documentcategorie", doc.Documenttype.Documentcategorie);
                                        Row(table, "publicatieniveau", doc.Documenttype.Publicatieniveau);
                                    });
                                }

                                AddSection(col, $"Document {i + 1} — Documentstatus", table =>
                                {
                                    Row(table, "naam", doc.Documentstatus.Naam);
                                    Row(table, "omschrijving", doc.Documentstatus.Omschrijving);
                                });

                                // DocumentVersies
                                for (var v = 0; v < doc.DocumentVersies.Count; v++)
                                {
                                    var versie = doc.DocumentVersies[v];
                                    AddSection(col, $"Document {i + 1} — Versie {versie.Versienummer}", table =>
                                    {
                                        Row(table, "versienummer", versie.Versienummer.ToString());
                                        Row(table, "bestandsnaam", versie.Bestandsnaam);
                                        Row(table, "mimetype", versie.Mimetype);
                                        Row(table, "creatiedatum", FormatDate(versie.Creatiedatum));
                                        Row(table, "auteur", versie.Auteur);
                                        Row(table, "afzender", versie.Afzender);
                                        Row(table, "documentgrootte", versie.Documentgrootte?.ToString());
                                        Row(table, "compressed", versie.Compressed.ToString());
                                    });

                                    // Ondertekeningen
                                    if (versie.Ondertekeningen != null)
                                    {
                                        for (var o = 0; o < versie.Ondertekeningen.Count; o++)
                                        {
                                            var ond = versie.Ondertekeningen[o];
                                            AddSection(col, $"Document {i + 1} Versie {versie.Versienummer} — Ondertekening {o + 1}", table =>
                                            {
                                                Row(table, "documentTitel", ond.DocumentTitel);
                                                Row(table, "ondertekenaar", ond.Ondertekenaar);
                                                Row(table, "ondertekenDatum", FormatDateTime(ond.OndertekenDatum));
                                                Row(table, "creatieDatum", FormatDate(ond.CreatieDatum));
                                                Row(table, "gemandateerd", ond.Gemandateerd.ToString());
                                                Row(table, "opmerking", ond.Opmerking);
                                            });
                                        }
                                    }
                                }

                                // DocumentMetadata
                                if (doc.DocumentMetadata != null)
                                {
                                    for (var m = 0; m < doc.DocumentMetadata.Count; m++)
                                    {
                                        var meta = doc.DocumentMetadata[m];
                                        AddSection(col, $"Document {i + 1} — Metadata {m + 1}", table =>
                                        {
                                            Row(table, "metadataElement.naam", meta.MetadataElement?.Naam);
                                            Row(table, "metadataElement.label", meta.MetadataElement?.Label);
                                            Row(table, "metadataElement.type", meta.MetadataElement?.Type);
                                            Row(table, "waarde", meta.Waarde);
                                        });
                                    }
                                }

                                // DocumentPublicaties
                                if (doc.Publicaties != null)
                                {
                                    for (var p = 0; p < doc.Publicaties.Count; p++)
                                    {
                                        var pub = doc.Publicaties[p];
                                        AddSection(col, $"Document {i + 1} — Publicatie {p + 1}", table =>
                                        {
                                            Row(table, "bestemming", pub.Bestemming);
                                            Row(table, "publicatiedatum", FormatDate(pub.Publicatiedatum));
                                        });
                                    }
                                }

                                // DocumentHistorie
                                for (var h = 0; h < doc.Historie.Count; h++)
                                {
                                    var dh = doc.Historie[h];
                                    AddSection(col, $"Document {i + 1} — Historie {h + 1}", table =>
                                    {
                                        Row(table, "typeWijziging", dh.TypeWijziging);
                                        Row(table, "gewijzigdDoor", dh.GewijzigdDoor);
                                        Row(table, "wijzigingDatum", FormatDate(dh.WijzigingDatum));
                                        Row(table, "oudeWaarde", dh.OudeWaarde);
                                        Row(table, "nieuweWaarde", dh.NieuweWaarde);
                                        Row(table, "toelichting", dh.Toelichting);
                                    });
                                }
                            }
                        }
                    });
                });
            });

            return document.GeneratePdf();
        }

        private static void AddSection(ColumnDescriptor col, string title, Action<TableDescriptor> tableContent)
        {
            col.Item().PaddingTop(8).Text(title).Bold();
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(200);
                    columns.RelativeColumn();
                });
                tableContent(table);
            });
        }

        private static void Row(TableDescriptor table, string key, string? value)
        {
            table.Cell().Text(key);
            table.Cell().Text(value ?? "-");
        }

        private static string FormatDate(DateOnly? d) => d?.ToString("dd-MM-yyyy") ?? "-";

        private static string FormatDateTime(DateTimeOffset? dt) => dt?.ToString("dd-MM-yyyy HH:mm") ?? "-";

        private static string FormatDateTime(DateTimeOffset dt) => dt.ToString("dd-MM-yyyy HH:mm");
    }
}
