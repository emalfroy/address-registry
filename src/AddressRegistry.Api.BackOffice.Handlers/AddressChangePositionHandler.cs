namespace AddressRegistry.Api.BackOffice.Handlers
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Abstractions.Requests;
    using Abstractions.Responses;
    using Address;
    using Be.Vlaanderen.Basisregisters.CommandHandling;
    using Be.Vlaanderen.Basisregisters.CommandHandling.Idempotency;
    using Consumer.Read.Municipality;
    using MediatR;
    using StreetName;
    using StreetName.Commands;

    public class AddressChangePositionHandler : BusHandler, IRequestHandler<AddressChangePositionRequest, ETagResponse>
    {
        private readonly IStreetNames _streetNames;
        private readonly BackOfficeContext _backOfficeContext;
        private readonly IdempotencyContext _idempotencyContext;
        private readonly MunicipalityConsumerContext _municipalityConsumerContext;

        public AddressChangePositionHandler(
            ICommandHandlerResolver bus,
            IStreetNames streetNames,
            BackOfficeContext backOfficeContext,
            IdempotencyContext idempotencyContext,
            MunicipalityConsumerContext municipalityConsumerContext)
            : base(bus)
        {
            _streetNames = streetNames;
            _backOfficeContext = backOfficeContext;
            _idempotencyContext = idempotencyContext;
            _municipalityConsumerContext = municipalityConsumerContext;
        }

        public async Task<ETagResponse> Handle(AddressChangePositionRequest request, CancellationToken cancellationToken)
        {
            var addressPersistentLocalId =
                new AddressPersistentLocalId(new PersistentLocalId(request.PersistentLocalId));

            var relation = _backOfficeContext.AddressPersistentIdStreetNamePersistentIds
                .Single(x => x.AddressPersistentLocalId == addressPersistentLocalId);

            var streetNamePersistentLocalId = new StreetNamePersistentLocalId(relation.StreetNamePersistentLocalId);


            var position = request.Positie.ToExtendedWkbGeometry();
            var cmd = request.ToCommand(
                streetNamePersistentLocalId,
                position,
                CreateFakeProvenance());

            await IdempotentCommandHandlerDispatch(
                _idempotencyContext,
                cmd.CreateCommandId(),
                cmd,
                request.Metadata,
                cancellationToken);

            var etag = await GetHash(
                _streetNames,
                streetNamePersistentLocalId,
                addressPersistentLocalId,
                cancellationToken);

            return new ETagResponse(etag);
        }
    }
}
