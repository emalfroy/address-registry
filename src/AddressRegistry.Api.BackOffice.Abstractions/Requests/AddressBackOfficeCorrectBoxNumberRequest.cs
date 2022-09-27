namespace AddressRegistry.Api.BackOffice.Abstractions.Requests
{
    using Newtonsoft.Json;
    using System.Runtime.Serialization;

    [DataContract(Name = "CorrigerenBusnummerAdres", Namespace = "")]
    public class AddressBackOfficeCorrectBoxNumberRequest
    {
        /// <summary>
        /// Het Busnummer van het adres.
        /// </summary>
        [DataMember(Name = "Busnummer", Order = 0)]
        [JsonProperty(Required = Required.Always)]
        public string Busnummer { get; set; }
    }
}
