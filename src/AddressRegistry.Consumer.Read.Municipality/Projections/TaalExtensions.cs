namespace AddressRegistry.Consumer.Read.Municipality.Projections
{
    using System;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;

    public static class TaalExtensions
    {
        public static Taal ToTaal(this string taal)
            => taal.ToLower() switch
            {
                "nl" => Taal.NL,
                "de" => Taal.DE,
                "fr" => Taal.FR,
                "en" => Taal.EN,
                _ => throw new ArgumentOutOfRangeException(nameof(taal), taal, null)
            };
    }
}
