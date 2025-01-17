namespace AddressRegistry.Api.BackOffice.Handlers.Lambda.Requests
{
    using Abstractions.Requests;
    using Be.Vlaanderen.Basisregisters.Sqs.Lambda.Requests;
    using StreetName;
    using StreetName.Commands;

    public sealed class SqsLambdaAddressRetireRequest :
        SqsLambdaRequest,
        IHasBackOfficeRequest<AddressBackOfficeRetireRequest>,
        Abstractions.IHasAddressPersistentLocalId
    {
        public AddressBackOfficeRetireRequest Request { get; set; }

        public int AddressPersistentLocalId => Request.PersistentLocalId;

        /// <summary>
        /// Map to RetireAddress command
        /// </summary>
        /// <returns>RetireAddress.</returns>
        public RetireAddress ToCommand()
        {
            return new RetireAddress(
                this.StreetNamePersistentLocalId(),
                new AddressPersistentLocalId(AddressPersistentLocalId),
                Provenance);
        }
    }
}
