namespace AddressRegistry.StreetName.Exceptions
{
    public class ParentAddressAlreadyExistsException : AddressRegistryException
    {
        public ParentAddressAlreadyExistsException(string houseNumber)
            : base($"Attempt to add parent address when parent address with housenumber '{houseNumber}' already exists for street.")
        { }
    }
}