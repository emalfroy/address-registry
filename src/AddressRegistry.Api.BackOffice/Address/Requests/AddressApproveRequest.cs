namespace AddressRegistry.Api.BackOffice.Address.Requests
{
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    [DataContract(Name = "GoedkeurenAdres", Namespace = "")]
    public class AddressApproveRequest
    {
        /// <summary>
        /// De unieke en persistente identificator van het adres.
        /// </summary>
        [DataMember(Name = "PersistentLocalId", Order = 0)]
        [JsonProperty(Required = Required.Always)]
        public int PersistentLocalId { get; set; }
    }
}