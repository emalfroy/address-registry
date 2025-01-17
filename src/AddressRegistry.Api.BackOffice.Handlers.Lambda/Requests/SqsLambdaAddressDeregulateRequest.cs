namespace AddressRegistry.Api.BackOffice.Handlers.Lambda.Requests
{
    using Abstractions.Requests;
    using Be.Vlaanderen.Basisregisters.Sqs.Lambda.Requests;
    using StreetName;
    using StreetName.Commands;

    public sealed class SqsLambdaAddressDeregulateRequest :
        SqsLambdaRequest,
        IHasBackOfficeRequest<AddressBackOfficeDeregulateRequest>,
        Abstractions.IHasAddressPersistentLocalId
    {
        public AddressBackOfficeDeregulateRequest Request { get; set; }

        public int AddressPersistentLocalId => Request.PersistentLocalId;

        /// <summary>
        /// Map to DeregulateAddress command
        /// </summary>
        /// <returns>DeregulateAddress.</returns>
        public DeregulateAddress ToCommand()
        {
            return new DeregulateAddress(
                this.StreetNamePersistentLocalId(),
                new AddressPersistentLocalId(AddressPersistentLocalId),
                Provenance);
        }
    }
}
