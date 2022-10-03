namespace AddressRegistry.Api.BackOffice.Abstractions.Requests
{
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    [DataContract(Name = "CorrigeerOpheffingAdres", Namespace = "")]
    public class AddressBackOfficeCorrectRetirementRequest
    {
        /// <summary>
        /// De unieke en persistente identificator van het adres.
        /// </summary>
        [DataMember(Name = "PersistentLocalId", Order = 0)]
        [JsonProperty(Required = Required.Always)]
        public int PersistentLocalId { get; set; }
    }
}
