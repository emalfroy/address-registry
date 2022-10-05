namespace AddressRegistry.Api.BackOffice.Handlers.Sqs.Handlers
{
    using Abstractions;
    using Requests;
    using System.Collections.Generic;
    using TicketingService.Abstractions;

    public sealed class SqsAddressCorrectRejectionHandler : SqsHandler<SqsAddressCorrectRejectionRequest>
    {
        public const string Action = "CorrectAddressRejection";

        private readonly BackOfficeContext _backOfficeContext;

        public SqsAddressCorrectRejectionHandler(
            ISqsQueue sqsQueue,
            ITicketing ticketing,
            ITicketingUrl ticketingUrl,
            BackOfficeContext backOfficeContext)
            : base(sqsQueue, ticketing, ticketingUrl)
        {
            _backOfficeContext = backOfficeContext;
        }

        protected override string? WithAggregateId(SqsAddressCorrectRejectionRequest request)
        {
            var relation = _backOfficeContext
                .AddressPersistentIdStreetNamePersistentIds
                .Find(request.Request.PersistentLocalId);

            return relation?.StreetNamePersistentLocalId.ToString();
        }

        protected override IDictionary<string, string> WithTicketMetadata(string aggregateId, SqsAddressCorrectRejectionRequest sqsRequest)
        {
            return new Dictionary<string, string>
            {
                { RegistryKey, nameof(AddressRegistry) },
                { ActionKey, Action },
                { AggregateIdKey, aggregateId },
                { ObjectIdKey, sqsRequest.Request.PersistentLocalId.ToString() }
            };
        }
    }
}