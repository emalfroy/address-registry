namespace AddressRegistry.Consumer.Read.Municipality.Projections
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class MunicipalityLanguagesBase
    {
        public const string OfficialLanguagesBackingPropertyName = nameof(OfficialLanguagesAsString);

        private string? OfficialLanguagesAsString { get; set; }

        public IReadOnlyCollection<string> OfficialLanguages
        {
            get => GetDeserializedOfficialLanguages();
            set => OfficialLanguagesAsString = JsonConvert.SerializeObject(value);
        }

        public void AddOfficialLanguage(string language)
        {
            var languages = GetDeserializedOfficialLanguages();
            languages.Add(language);
            OfficialLanguages = languages;
        }

        public void RemoveOfficialLanguage(string language)
        {
            var languages = GetDeserializedOfficialLanguages();
            languages.Remove(language);
            OfficialLanguages = languages;
        }

        private List<string> GetDeserializedOfficialLanguages()
        {
            return string.IsNullOrEmpty(OfficialLanguagesAsString)
                ? new List<string>()
                : JsonConvert.DeserializeObject<List<string>>(OfficialLanguagesAsString) ?? new List<string>();
        }
    }
}
