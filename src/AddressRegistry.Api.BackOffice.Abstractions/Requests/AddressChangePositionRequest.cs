namespace AddressRegistry.Api.BackOffice.Abstractions.Requests
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Be.Vlaanderen.Basisregisters.GrAr.Edit.Contracts;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Converters;
    using MediatR;
    using Newtonsoft.Json;
    using Responses;
    using StreetName;
    using StreetName.Commands;

    [DataContract(Name = "WijzigenAdresPositie", Namespace = "")]
    public class AddressChangePositionRequest : IRequest<ETagResponse>
    {
        /// <summary>
        /// De unieke en persistente identificator van het adres.
        /// </summary>
        [DataMember(Name = "PersistentLocalId", Order = 0)]
        [JsonProperty(Required = Required.Always)]
        public int PersistentLocalId { get; set; }

        /// <summary>
        /// De geometriemethode van het adres.
        /// </summary>
        [DataMember(Name = "PositieGeometriemethode", Order = 4)]
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public PositieGeometrieMethode? PositieGeometrieMethode { get; set; }

        /// <summary>
        /// De specificatie van de adrespositie (optioneel).
        /// </summary>
        [DataMember(Name = "PositieSpecificatie", Order = 5)]
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public PositieSpecificatie? PositieSpecificatie { get; set; }

        /// <summary>
        /// Puntgeometrie van het adres in GML-3 formaat met Lambert 72 referentie systeem.
        /// </summary>
        [DataMember(Name = "Positie", Order = 6)]
        [JsonProperty(Required = Required.Default)]
        public string? Positie { get; set; }

        [JsonIgnore]
        public IDictionary<string, object> Metadata { get; set; }

        /// <summary>
        /// Map to ProposeAddress command
        /// </summary>
        /// <returns>ProposeAddress.</returns>
        public ChangeAddressPosition ToCommand(
            StreetNamePersistentLocalId streetNamePersistentLocalId,
            ExtendedWkbGeometry position,
            Provenance provenance)
        {
            return new ChangeAddressPosition(
                streetNamePersistentLocalId,
                new AddressPersistentLocalId(PersistentLocalId),
                PositieGeometrieMethode.Map(),
                PositieSpecificatie.Map(),
                position,
                provenance);
        }
    }
}
