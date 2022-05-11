namespace AddressRegistry.Consumer.Read.Municipality.Projections.Bosa
{
    using System;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Be.Vlaanderen.Basisregisters.GrAr.Contracts.Common;
    using Be.Vlaanderen.Basisregisters.GrAr.Contracts.MunicipalityRegistry;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using NodaTime.Text;

    public class MunicipalityBosaItemProjections : ConnectedProjection<MunicipalityConsumerContext>
    {
        public MunicipalityBosaItemProjections()
        {
            When<MunicipalityWasRegistered>(async (context, message, ct) =>
            {
                var timestamp = InstantPattern.General.Parse(message.Provenance.Timestamp).Value;

                var municipality = new MunicipalityBosaItem(
                    new Guid(message.MunicipalityId),
                    message.NisCode,
                    timestamp);

                await context.MunicipalityBosaItems.AddAsync(municipality, ct);
                await context.SaveChangesAsync(ct);
            });

            When<MunicipalityWasNamed>(async (contextFactory, message, ct) =>
            {
                await contextFactory.FindAndUpdate<MunicipalityBosaItem, MunicipalityBosaItemProjections>(
                    new Guid(message.MunicipalityId), municipality =>
                    {
                        SetMunicipalityName(message.Language.ToTaal(), municipality, message.Name);
                        UpdateVersionTimestamp(message.Provenance, municipality);
                    }, ct);
            });

            When<MunicipalityNameWasCleared>(async (contextFactory, message, ct) =>
            {
                await contextFactory.FindAndUpdate<MunicipalityBosaItem, MunicipalityBosaItemProjections>(
                    new Guid(message.MunicipalityId), municipality =>
                    {
                        SetMunicipalityName(message.Language.ToTaal(), municipality, null);
                        UpdateVersionTimestamp(message.Provenance, municipality);
                    }, ct);
            });

            When<MunicipalityNameWasCorrected>(async (contextFactory, message, ct) =>
            {
                await contextFactory.FindAndUpdate<MunicipalityBosaItem, MunicipalityBosaItemProjections>(
                    new Guid(message.MunicipalityId), municipality =>
                    {
                        SetMunicipalityName(message.Language.ToTaal(), municipality, message.Name);
                        UpdateVersionTimestamp(message.Provenance, municipality);
                    }, ct);
            });

            When<MunicipalityNameWasCorrectedToCleared>(async (contextFactory, message, ct) =>
            {
                await contextFactory.FindAndUpdate<MunicipalityBosaItem, MunicipalityBosaItemProjections>(
                    new Guid(message.MunicipalityId), municipality =>
                    {
                        SetMunicipalityName(message.Language.ToTaal(), municipality, null);
                        UpdateVersionTimestamp(message.Provenance, municipality);
                    }, ct);
            });

            When<MunicipalityNisCodeWasDefined>(async (contextFactory, message, ct) =>
            {
                await contextFactory.FindAndUpdate<MunicipalityBosaItem, MunicipalityBosaItemProjections>(
                    new Guid(message.MunicipalityId), municipality =>
                    {
                        municipality.NisCode = message.NisCode;
                        UpdateVersionTimestamp(message.Provenance, municipality);
                    }, ct);
            });

            When<MunicipalityNisCodeWasCorrected>(async (contextFactory, message, ct) =>
            {
                await contextFactory.FindAndUpdate<MunicipalityBosaItem, MunicipalityBosaItemProjections>(
                    new Guid(message.MunicipalityId), municipality =>
                    {
                        municipality.NisCode = message.NisCode;
                        UpdateVersionTimestamp(message.Provenance, municipality);
                    }, ct);
            });

            When<MunicipalityOfficialLanguageWasAdded>(async (contextFactory, message, ct) =>
            {
                await contextFactory.FindAndUpdate<MunicipalityBosaItem, MunicipalityBosaItemProjections>(
                    new Guid(message.MunicipalityId), municipality =>
                    {
                        municipality.AddOfficialLanguage(message.Language);
                        UpdateVersionTimestamp(message.Provenance, municipality);
                    }, ct);
            });

            When<MunicipalityOfficialLanguageWasRemoved>(async (contextFactory, message, ct) =>
            {
                await contextFactory.FindAndUpdate<MunicipalityBosaItem, MunicipalityBosaItemProjections>(
                    new Guid(message.MunicipalityId), municipality =>
                    {
                        municipality.RemoveOfficialLanguage(message.Language);
                        UpdateVersionTimestamp(message.Provenance, municipality);
                    }, ct);
            });
        }

        private static void SetMunicipalityName(Taal taal, MunicipalityBosaItem municipality, string? name)
        {
            switch (taal)
            {
                case Taal.NL:
                    municipality.NameDutch = name;
                    municipality.NameDutchSearch = name.RemoveDiacritics();
                    break;
                case Taal.DE:
                    municipality.NameGerman = name;
                    municipality.NameGermanSearch = name.RemoveDiacritics();
                    break;
                case Taal.FR:
                    municipality.NameFrench = name;
                    municipality.NameFrenchSearch = name.RemoveDiacritics();
                    break;
                case Taal.EN:
                    municipality.NameEnglish = name;
                    municipality.NameEnglishSearch = name.RemoveDiacritics();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(taal), taal, null);
            }
        }

        private static void UpdateVersionTimestamp(Provenance provenance, MunicipalityBosaItem municipality)
        {
            var timestamp = InstantPattern.General.Parse(provenance.Timestamp).Value;
            municipality.VersionTimestamp = timestamp;
        }
    }
}
