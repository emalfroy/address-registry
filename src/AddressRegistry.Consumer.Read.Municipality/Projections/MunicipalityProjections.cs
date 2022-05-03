namespace AddressRegistry.Consumer.Read.Municipality.Projections
{
    using System;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.GrAr.Contracts.MunicipalityRegistry;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.Utilities.HexByteConvertor;
    using NodaTime.Text;

    public class MunicipalityProjections : ConnectedProjection<ConsumerContext>
    {
        public MunicipalityProjections()
        {
            When<MunicipalityWasRegistered>(async (context, message, ct) =>
            {
                var timestamp = InstantPattern.General.Parse(message.Provenance.Timestamp).Value;

                var municipality = new MunicipalityLatestItem(
                    new Guid(message.MunicipalityId),
                    message.NisCode,
                    timestamp);

                await context.MunicipalityLatestItems.AddAsync(municipality, ct);
                await context.SaveChangesAsync(ct);
            });

            When<MunicipalityBecameCurrent>(async (contextFactory, message, ct) =>
            {
                await contextFactory.FindAndUpdate(new Guid(message.MunicipalityId), municipality =>
                {
                    municipality.Status = MunicipalityStatus.Current;
                }, ct);
            });

            When<MunicipalityWasCorrectedToCurrent>(async (contextFactory, message, ct) =>
            {
                await contextFactory.FindAndUpdate(new Guid(message.MunicipalityId), municipality =>
                {
                    municipality.Status = MunicipalityStatus.Current;
                }, ct);
            });

            When<MunicipalityWasCorrectedToRetired>(async (contextFactory, message, ct) =>
            {
                await contextFactory.FindAndUpdate(new Guid(message.MunicipalityId), municipality =>
                {
                    municipality.Status = MunicipalityStatus.Retired;
                }, ct);
            });

            When<MunicipalityWasRetired>(async (contextFactory, message, ct) =>
            {
                await contextFactory.FindAndUpdate(new Guid(message.MunicipalityId), municipality =>
                {
                    municipality.Status = MunicipalityStatus.Retired;
                }, ct);
            });

            When<MunicipalityWasDrawn>(async (contextFactory, message, ct) =>
            {
                await contextFactory.FindAndUpdate(new Guid(message.MunicipalityId), municipality =>
                {
                    municipality.ExtendedWkbGeometry = message.ExtendedWkbGeometry.ToByteArray();
                }, ct);
            });

            When<MunicipalityGeometryWasCorrectedToCleared>(async (contextFactory, message, ct) =>
            {
                await contextFactory.FindAndUpdate(new Guid(message.MunicipalityId), municipality =>
                {
                    municipality.ExtendedWkbGeometry = null;
                }, ct);
            });

            When<MunicipalityGeometryWasCleared>(async (contextFactory, message, ct) =>
            {
                await contextFactory.FindAndUpdate(new Guid(message.MunicipalityId), municipality =>
                {
                    municipality.ExtendedWkbGeometry = null;
                }, ct);
            });

            When<MunicipalityGeometryWasCorrected>(async (contextFactory, message, ct) =>
            {
                await contextFactory.FindAndUpdate(new Guid(message.MunicipalityId), municipality =>
                {
                    municipality.ExtendedWkbGeometry = message.ExtendedWkbGeometry.ToByteArray();
                }, ct);
            });

            When<MunicipalityWasNamed>(async (contextFactory, message, ct) =>
            {
                await contextFactory.FindAndUpdate(new Guid(message.MunicipalityId), municipality =>
                {
                    var taal = StringToTaal(message.Language);
                    SetMunicipalityName(taal, municipality, null);
                }, ct);
            });

            When<MunicipalityNameWasCleared>(async (contextFactory, message, ct) =>
            {
                await contextFactory.FindAndUpdate(new Guid(message.MunicipalityId), municipality =>
                {
                    var taal = StringToTaal(message.Language);
                    SetMunicipalityName(taal, municipality, null);
                }, ct);
            });

            When<MunicipalityNameWasCorrected>(async (contextFactory, message, ct) =>
            {
                await contextFactory.FindAndUpdate(new Guid(message.MunicipalityId), municipality =>
                {
                    var taal = StringToTaal(message.Language);
                    SetMunicipalityName(taal, municipality, message.Name);
                }, ct);
            });

            When<MunicipalityNameWasCorrectedToCleared>(async (contextFactory, message, ct) =>
            {
                await contextFactory.FindAndUpdate(new Guid(message.MunicipalityId), municipality =>
                {
                    var taal = StringToTaal(message.Language);
                    SetMunicipalityName(taal, municipality, null);
                }, ct);
            });

            When<MunicipalityNisCodeWasCorrected>(async (contextFactory, message, ct) =>
            {
                await contextFactory.FindAndUpdate(new Guid(message.MunicipalityId), municipality =>
                {
                    municipality.NisCode = message.NisCode;
                }, ct);
            });

            When<MunicipalityNisCodeWasDefined>(async (contextFactory, message, ct) =>
            {
                await contextFactory.FindAndUpdate(new Guid(message.MunicipalityId), municipality =>
                {
                    municipality.NisCode = message.NisCode;
                }, ct);
            });

            When<MunicipalityOfficialLanguageWasAdded>(async (contextFactory, message, ct) =>
            {
                await contextFactory.FindAndUpdate(new Guid(message.MunicipalityId), municipality =>
                {
                    municipality.OfficialLanguages.Add(message.Language);
                }, ct);
            });

            When<MunicipalityOfficialLanguageWasRemoved>(async (contextFactory, message, ct) =>
            {
                await contextFactory.FindAndUpdate(new Guid(message.MunicipalityId), municipality =>
                {
                    var language = municipality.OfficialLanguages.FirstOrDefault(x => x == message.Language);

                    if (language != null)
                    {
                        municipality.OfficialLanguages.Remove(language);
                    }
                }, ct);
            });
        }

        private static Taal StringToTaal(string taal) => taal switch
        {
            "nl" => Taal.NL,
            "de" => Taal.DE,
            "fr" => Taal.FR,
            "en" => Taal.EN,
            _ => throw new ArgumentOutOfRangeException(nameof(taal), taal, null)
        };

        private static void SetMunicipalityName(Taal taal, MunicipalityLatestItem municipality, string? name)
        {
            switch (taal)
            {
                case Taal.NL:
                    municipality.NameDutch = name;
                    break;
                case Taal.DE:
                    municipality.NameGerman = name;
                    break;
                case Taal.FR:
                    municipality.NameFrench = name;
                    break;
                case Taal.EN:
                    municipality.NameEnglish = name;
                    break;
            }
        }
    }
}
