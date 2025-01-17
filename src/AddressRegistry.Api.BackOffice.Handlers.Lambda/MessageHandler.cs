namespace AddressRegistry.Api.BackOffice.Handlers.Lambda
{
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using Be.Vlaanderen.Basisregisters.Aws.Lambda;
    using Be.Vlaanderen.Basisregisters.Sqs.Requests;
    using MediatR;
    using Requests;
    using Sqs.Requests;

    public sealed class MessageHandler : IMessageHandler
    {
        private readonly ILifetimeScope _container;

        public MessageHandler(ILifetimeScope container)
        {
            _container = container;
        }

        public async Task HandleMessage(object? messageData, MessageMetadata messageMetadata, CancellationToken cancellationToken)
        {
            messageMetadata.Logger?.LogInformation($"Handling message {messageData?.GetType().Name}");

            if (messageData is not SqsRequest sqsRequest)
            {
                messageMetadata.Logger?.LogInformation($"Unable to cast {nameof(messageData)} as {nameof(sqsRequest)}.");
                return;
            }

            await using var lifetimeScope = _container.BeginLifetimeScope();
            var mediator = lifetimeScope.Resolve<IMediator>();

            switch (sqsRequest)
            {
                case SqsAddressApproveRequest request:
                    await mediator.Send(new SqsLambdaAddressApproveRequest
                    {
                        Request = request.Request,
                        TicketId = request.TicketId,
                        MessageGroupId = messageMetadata.MessageGroupId!,
                        IfMatchHeaderValue = request.IfMatchHeaderValue,
                        Metadata = request.Metadata,
                        Provenance = request.ProvenanceData.ToProvenance()
                    }, cancellationToken);
                    break;
                case SqsAddressCorrectApprovalRequest request:
                    await mediator.Send(new SqsLambdaAddressCorrectApprovalRequest
                    {
                        Request = request.Request,
                        TicketId = request.TicketId,
                        MessageGroupId = messageMetadata.MessageGroupId!,
                        IfMatchHeaderValue = request.IfMatchHeaderValue,
                        Metadata = request.Metadata,
                        Provenance = request.ProvenanceData.ToProvenance()
                    }, cancellationToken);
                    break;
                case SqsAddressChangePositionRequest request:
                    await mediator.Send(new SqsLambdaAddressChangePositionRequest
                    {
                        Request = request.Request,
                        TicketId = request.TicketId,
                        AddressPersistentLocalId = request.PersistentLocalId,
                        MessageGroupId = messageMetadata.MessageGroupId!,
                        IfMatchHeaderValue = request.IfMatchHeaderValue,
                        Metadata = request.Metadata,
                        Provenance = request.ProvenanceData.ToProvenance()
                    }, cancellationToken);
                    break;
                case SqsAddressChangePostalCodeRequest request:
                    await mediator.Send(new SqsLambdaAddressChangePostalCodeRequest
                    {
                        Request = request.Request,
                        TicketId = request.TicketId,
                        AddressPersistentLocalId = request.PersistentLocalId,
                        MessageGroupId = messageMetadata.MessageGroupId!,
                        IfMatchHeaderValue = request.IfMatchHeaderValue,
                        Metadata = request.Metadata,
                        Provenance = request.ProvenanceData.ToProvenance()
                    }, cancellationToken);
                    break;
                case SqsAddressCorrectHouseNumberRequest request:
                    await mediator.Send(new SqsLambdaAddressCorrectHouseNumberRequest
                    {
                        Request = request.Request,
                        TicketId = request.TicketId,
                        AddressPersistentLocalId = request.PersistentLocalId,
                        MessageGroupId = messageMetadata.MessageGroupId!,
                        IfMatchHeaderValue = request.IfMatchHeaderValue,
                        Metadata = request.Metadata,
                        Provenance = request.ProvenanceData.ToProvenance()
                    }, cancellationToken);
                    break;
                case SqsAddressCorrectPositionRequest request:
                    await mediator.Send(new SqsLambdaAddressCorrectPositionRequest
                    {
                        Request = request.Request,
                        TicketId = request.TicketId,
                        AddressPersistentLocalId = request.PersistentLocalId,
                        MessageGroupId = messageMetadata.MessageGroupId!,
                        IfMatchHeaderValue = request.IfMatchHeaderValue,
                        Metadata = request.Metadata,
                        Provenance = request.ProvenanceData.ToProvenance()
                    }, cancellationToken);
                    break;
                case SqsAddressCorrectPostalCodeRequest request:
                    await mediator.Send(new SqsLambdaAddressCorrectPostalCodeRequest
                    {
                        Request = request.Request,
                        TicketId = request.TicketId,
                        AddressPersistentLocalId = request.PersistentLocalId,
                        MessageGroupId = messageMetadata.MessageGroupId!,
                        IfMatchHeaderValue = request.IfMatchHeaderValue,
                        Metadata = request.Metadata,
                        Provenance = request.ProvenanceData.ToProvenance()
                    }, cancellationToken);
                    break;
                case SqsAddressCorrectRejectionRequest request:
                    await mediator.Send(new SqsLambdaAddressCorrectRejectionRequest
                    {
                        Request = request.Request,
                        TicketId = request.TicketId,
                        MessageGroupId = messageMetadata.MessageGroupId!,
                        IfMatchHeaderValue = request.IfMatchHeaderValue,
                        Metadata = request.Metadata,
                        Provenance = request.ProvenanceData.ToProvenance()
                    }, cancellationToken);
                    break;
                case SqsAddressDeregulateRequest request:
                    await mediator.Send(new SqsLambdaAddressDeregulateRequest
                    {
                        Request = request.Request,
                        TicketId = request.TicketId,
                        MessageGroupId = messageMetadata.MessageGroupId!,
                        IfMatchHeaderValue = request.IfMatchHeaderValue,
                        Metadata = request.Metadata,
                        Provenance = request.ProvenanceData.ToProvenance()
                    }, cancellationToken);
                    break;
                case SqsAddressProposeRequest request:
                    await mediator.Send(new SqsLambdaAddressProposeRequest
                    {
                        Request = request.Request,
                        TicketId = request.TicketId,
                        MessageGroupId = messageMetadata.MessageGroupId!,
                        Metadata = request.Metadata,
                        Provenance = request.ProvenanceData.ToProvenance()
                    }, cancellationToken);
                    break;
                case SqsAddressRegularizeRequest request:
                    await mediator.Send(new SqsLambdaAddressRegularizeRequest
                    {
                        Request = request.Request,
                        TicketId = request.TicketId,
                        MessageGroupId = messageMetadata.MessageGroupId!,
                        IfMatchHeaderValue = request.IfMatchHeaderValue,
                        Metadata = request.Metadata,
                        Provenance = request.ProvenanceData.ToProvenance()
                    }, cancellationToken);
                    break;
                case SqsAddressRejectRequest request:
                    await mediator.Send(new SqsLambdaAddressRejectRequest
                    {
                        Request = request.Request,
                        TicketId = request.TicketId,
                        MessageGroupId = messageMetadata.MessageGroupId!,
                        IfMatchHeaderValue = request.IfMatchHeaderValue,
                        Metadata = request.Metadata,
                        Provenance = request.ProvenanceData.ToProvenance()
                    }, cancellationToken);
                    break;
                case SqsAddressRemoveRequest request:
                    await mediator.Send(new SqsLambdaAddressRemoveRequest
                    {
                        Request = request.Request,
                        TicketId = request.TicketId,
                        MessageGroupId = messageMetadata.MessageGroupId!,
                        IfMatchHeaderValue = request.IfMatchHeaderValue,
                        Metadata = request.Metadata,
                        Provenance = request.ProvenanceData.ToProvenance()
                    }, cancellationToken);
                    break;
                case SqsAddressRetireRequest request:
                    await mediator.Send(new SqsLambdaAddressRetireRequest
                    {
                        Request = request.Request,
                        TicketId = request.TicketId,
                        MessageGroupId = messageMetadata.MessageGroupId!,
                        IfMatchHeaderValue = request.IfMatchHeaderValue,
                        Metadata = request.Metadata,
                        Provenance = request.ProvenanceData.ToProvenance()
                    }, cancellationToken);
                    break;
                case SqsAddressCorrectRetirementRequest request:
                    await mediator.Send(new SqsLambdaAddressCorrectRetirementRequest
                    {
                        Request = request.Request,
                        TicketId = request.TicketId,
                        MessageGroupId = messageMetadata.MessageGroupId!,
                        IfMatchHeaderValue = request.IfMatchHeaderValue,
                        Metadata = request.Metadata,
                        Provenance = request.ProvenanceData.ToProvenance()
                    }, cancellationToken);
                    break;
                case SqsAddressCorrectBoxNumberRequest request:
                    await mediator.Send(new SqsLambdaAddressCorrectBoxNumberRequest
                    {
                        Request = request.Request,
                        TicketId = request.TicketId,
                        AddressPersistentLocalId = request.PersistentLocalId,
                        MessageGroupId = messageMetadata.MessageGroupId!,
                        IfMatchHeaderValue = request.IfMatchHeaderValue,
                        Metadata = request.Metadata,
                        Provenance = request.ProvenanceData.ToProvenance()
                    }, cancellationToken);
                    break;
                default:
                    throw new NotImplementedException(
                        $"{sqsRequest.GetType().Name} has no corresponding SqsLambdaRequest defined.");
            }
        }
    }
}
