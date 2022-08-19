namespace AddressRegistry.StreetName
{
    public interface IMunicipalities
    {
        Municipality? Get(MunicipalityId municipalityId);
    }

    public class Municipality
    {
        public byte[]? ExtendedWkbGeometry { get; }

        public Municipality(byte[]? extendedWkbGeometry)
        {
            ExtendedWkbGeometry = extendedWkbGeometry;
        }
    }
}
