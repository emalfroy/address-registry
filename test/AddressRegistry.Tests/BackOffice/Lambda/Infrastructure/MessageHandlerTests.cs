namespace AddressRegistry.Tests.BackOffice.Lambda.Infrastructure
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AddressRegistry.Api.BackOffice.Handlers.Lambda;
    using AddressRegistry.Api.BackOffice.Handlers.Lambda.Requests;
    using AddressRegistry.Api.BackOffice.Handlers.Sqs.Requests;
    using Autofac;
    using Be.Vlaanderen.Basisregisters.Aws.Lambda;
    using Be.Vlaanderen.Basisregisters.Sqs.Requests;
    using FluentAssertions;
    using global::AutoFixture;
    using MediatR;
    using Moq;
    using Xunit;
    using Xunit.Abstractions;

    public class MessageHandlerTests : BackOfficeLambdaTest
    {
        public MessageHandlerTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        { }

        [Fact]
        public async Task WhenProcessingUnknownMessage_ThenNothingIsSent()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Register(_ => mediator.Object);
            var container = containerBuilder.Build();

            var messageData = Fixture.Create<object>();
            var messageMetadata = new MessageMetadata { MessageGroupId = Fixture.Create<string>() };

            var sut = new MessageHandler(container);

            // Act
            await sut.HandleMessage(
                messageData,
                messageMetadata,
                CancellationToken.None);

            // Assert
            mediator.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task WhenProcessingSqsRequestWithoutCorrespondingSqsLambdaRequest_ThenThrowsNotImplementedException()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Register(_ => mediator.Object);
            var container = containerBuilder.Build();

            var messageData = Fixture.Create<TestSqsRequest>();
            var messageMetadata = new MessageMetadata { MessageGroupId = Fixture.Create<string>() };

            var sut = new MessageHandler(container);

            // Act
            var act = async () => await sut.HandleMessage(
                messageData,
                messageMetadata,
                CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotImplementedException>();
        }

        [Fact]
        public async Task WhenSqsAddressApproveRequest_ThenSqsLambdaAddressApproveRequestIsSent()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Register(_ => mediator.Object);
            var container = containerBuilder.Build();

            var messageData = Fixture.Create<SqsAddressApproveRequest>();
            var messageMetadata = new MessageMetadata { MessageGroupId = Fixture.Create<string>() };

            var sut = new MessageHandler(container);

            // Act
            await sut.HandleMessage(
                messageData,
                messageMetadata,
                CancellationToken.None);

            // Assert
            mediator
                .Verify(x => x.Send(It.Is<SqsLambdaAddressApproveRequest>(request =>
                    request.TicketId == messageData.TicketId
                    && request.MessageGroupId == messageMetadata.MessageGroupId
                    && request.Request == messageData.Request
                    && request.IfMatchHeaderValue == messageData.IfMatchHeaderValue
                    && request.Provenance == messageData.ProvenanceData.ToProvenance()
                    && request.Metadata == messageData.Metadata
                ), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task WhenSqsAddressCorrectApprovalRequest_ThenSqsLambdaAddressCorrectApprovalRequestIsSent()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Register(_ => mediator.Object);
            var container = containerBuilder.Build();

            var messageData = Fixture.Create<SqsAddressCorrectApprovalRequest>();
            var messageMetadata = new MessageMetadata { MessageGroupId = Fixture.Create<string>() };

            var sut = new MessageHandler(container);

            // Act
            await sut.HandleMessage(
                messageData,
                messageMetadata,
                CancellationToken.None);

            // Assert
            mediator
                .Verify(x => x.Send(It.Is<SqsLambdaAddressCorrectApprovalRequest>(request =>
                    request.TicketId == messageData.TicketId
                    && request.MessageGroupId == messageMetadata.MessageGroupId
                    && request.Request == messageData.Request
                    && request.IfMatchHeaderValue == messageData.IfMatchHeaderValue
                    && request.Provenance == messageData.ProvenanceData.ToProvenance()
                    && request.Metadata == messageData.Metadata
                ), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task WhenSqsAddressChangePositionRequest_ThenSqsLambdaAddressChangePositionRequestIsSent()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Register(_ => mediator.Object);
            var container = containerBuilder.Build();

            var messageData = Fixture.Create<SqsAddressChangePositionRequest>();
            var messageMetadata = new MessageMetadata { MessageGroupId = Fixture.Create<string>() };

            var sut = new MessageHandler(container);

            // Act
            await sut.HandleMessage(
                messageData,
                messageMetadata,
                CancellationToken.None);

            // Assert
            mediator
                .Verify(x => x.Send(It.Is<SqsLambdaAddressChangePositionRequest>(request =>
                    request.TicketId == messageData.TicketId
                    && request.AddressPersistentLocalId == messageData.PersistentLocalId
                    && request.MessageGroupId == messageMetadata.MessageGroupId
                    && request.Request == messageData.Request
                    && request.IfMatchHeaderValue == messageData.IfMatchHeaderValue
                    && request.Provenance == messageData.ProvenanceData.ToProvenance()
                    && request.Metadata == messageData.Metadata
                ), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task WhenSqsAddressChangePostalCodeRequest_ThenSqsLambdaAddressChangePostalCodeRequestIsSent()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Register(_ => mediator.Object);
            var container = containerBuilder.Build();

            var messageData = Fixture.Create<SqsAddressChangePostalCodeRequest>();
            var messageMetadata = new MessageMetadata { MessageGroupId = Fixture.Create<string>() };

            var sut = new MessageHandler(container);

            // Act
            await sut.HandleMessage(
                messageData,
                messageMetadata,
                CancellationToken.None);

            // Assert
            mediator
                .Verify(x => x.Send(It.Is<SqsLambdaAddressChangePostalCodeRequest>(request =>
                    request.TicketId == messageData.TicketId
                    && request.AddressPersistentLocalId == messageData.PersistentLocalId
                    && request.MessageGroupId == messageMetadata.MessageGroupId
                    && request.Request == messageData.Request
                    && request.IfMatchHeaderValue == messageData.IfMatchHeaderValue
                    && request.Provenance == messageData.ProvenanceData.ToProvenance()
                    && request.Metadata == messageData.Metadata
                ), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task WhenSqsAddressCorrectHouseNumberRequest_ThenSqsLambdaAddressCorrectHouseNumberRequestIsSent()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Register(_ => mediator.Object);
            var container = containerBuilder.Build();

            var messageData = Fixture.Create<SqsAddressCorrectHouseNumberRequest>();
            var messageMetadata = new MessageMetadata { MessageGroupId = Fixture.Create<string>() };

            var sut = new MessageHandler(container);

            // Act
            await sut.HandleMessage(
                messageData,
                messageMetadata,
                CancellationToken.None);

            // Assert
            mediator
                .Verify(x => x.Send(It.Is<SqsLambdaAddressCorrectHouseNumberRequest>(request =>
                    request.TicketId == messageData.TicketId
                    && request.AddressPersistentLocalId == messageData.PersistentLocalId
                    && request.MessageGroupId == messageMetadata.MessageGroupId
                    && request.Request == messageData.Request
                    && request.IfMatchHeaderValue == messageData.IfMatchHeaderValue
                    && request.Provenance == messageData.ProvenanceData.ToProvenance()
                    && request.Metadata == messageData.Metadata
                ), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task WhenSqsAddressCorrectPositionRequest_ThenSqsLambdaAddressCorrectPositionRequestIsSent()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Register(_ => mediator.Object);
            var container = containerBuilder.Build();

            var messageData = Fixture.Create<SqsAddressCorrectPositionRequest>();
            var messageMetadata = new MessageMetadata { MessageGroupId = Fixture.Create<string>() };

            var sut = new MessageHandler(container);

            // Act
            await sut.HandleMessage(
                messageData,
                messageMetadata,
                CancellationToken.None);

            // Assert
            mediator
                .Verify(x => x.Send(It.Is<SqsLambdaAddressCorrectPositionRequest>(request =>
                    request.TicketId == messageData.TicketId
                    && request.AddressPersistentLocalId == messageData.PersistentLocalId
                    && request.MessageGroupId == messageMetadata.MessageGroupId
                    && request.Request == messageData.Request
                    && request.IfMatchHeaderValue == messageData.IfMatchHeaderValue
                    && request.Provenance == messageData.ProvenanceData.ToProvenance()
                    && request.Metadata == messageData.Metadata
                ), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task WhenSqsAddressCorrectPostalCodeRequest_ThenSqsLambdaAddressCorrectPostalCodeRequestIsSent()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Register(_ => mediator.Object);
            var container = containerBuilder.Build();

            var messageData = Fixture.Create<SqsAddressCorrectPostalCodeRequest>();
            var messageMetadata = new MessageMetadata { MessageGroupId = Fixture.Create<string>() };

            var sut = new MessageHandler(container);

            // Act
            await sut.HandleMessage(
                messageData,
                messageMetadata,
                CancellationToken.None);

            // Assert
            mediator
                .Verify(x => x.Send(It.Is<SqsLambdaAddressCorrectPostalCodeRequest>(request =>
                    request.TicketId == messageData.TicketId
                    && request.AddressPersistentLocalId == messageData.PersistentLocalId
                    && request.MessageGroupId == messageMetadata.MessageGroupId
                    && request.Request == messageData.Request
                    && request.IfMatchHeaderValue == messageData.IfMatchHeaderValue
                    && request.Provenance == messageData.ProvenanceData.ToProvenance()
                    && request.Metadata == messageData.Metadata
                ), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task WhenSqsAddressDeregulateRequest_ThenSqsLambdaAddressDeregulateRequestIsSent()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Register(_ => mediator.Object);
            var container = containerBuilder.Build();

            var messageData = Fixture.Create<SqsAddressDeregulateRequest>();
            var messageMetadata = new MessageMetadata { MessageGroupId = Fixture.Create<string>() };

            var sut = new MessageHandler(container);

            // Act
            await sut.HandleMessage(
                messageData,
                messageMetadata,
                CancellationToken.None);

            // Assert
            mediator
                .Verify(x => x.Send(It.Is<SqsLambdaAddressDeregulateRequest>(request =>
                    request.TicketId == messageData.TicketId
                    && request.MessageGroupId == messageMetadata.MessageGroupId
                    && request.Request == messageData.Request
                    && request.IfMatchHeaderValue == messageData.IfMatchHeaderValue
                    && request.Provenance == messageData.ProvenanceData.ToProvenance()
                    && request.Metadata == messageData.Metadata
                ), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task WhenSqsAddressProposeRequest_ThenSqsLambdaAddressProposeRequestIsSent()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Register(_ => mediator.Object);
            var container = containerBuilder.Build();

            var messageData = Fixture.Create<SqsAddressProposeRequest>();
            var messageMetadata = new MessageMetadata { MessageGroupId = Fixture.Create<string>() };

            var sut = new MessageHandler(container);

            // Act
            await sut.HandleMessage(
                messageData,
                messageMetadata,
                CancellationToken.None);

            // Assert
            mediator
                .Verify(x => x.Send(It.Is<SqsLambdaAddressProposeRequest>(request =>
                    request.TicketId == messageData.TicketId
                    && request.MessageGroupId == messageMetadata.MessageGroupId
                    && request.Request == messageData.Request
                    && request.IfMatchHeaderValue == null
                    && request.Provenance == messageData.ProvenanceData.ToProvenance()
                    && request.Metadata == messageData.Metadata
                ), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task WhenSqsAddressRegularizeRequest_ThenSqsLambdaAddressRegularizeRequestIsSent()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Register(_ => mediator.Object);
            var container = containerBuilder.Build();

            var messageData = Fixture.Create<SqsAddressRegularizeRequest>();
            var messageMetadata = new MessageMetadata { MessageGroupId = Fixture.Create<string>() };

            var sut = new MessageHandler(container);

            // Act
            await sut.HandleMessage(
                messageData,
                messageMetadata,
                CancellationToken.None);

            // Assert
            mediator
                .Verify(x => x.Send(It.Is<SqsLambdaAddressRegularizeRequest>(request =>
                    request.TicketId == messageData.TicketId
                    && request.MessageGroupId == messageMetadata.MessageGroupId
                    && request.Request == messageData.Request
                    && request.IfMatchHeaderValue == messageData.IfMatchHeaderValue
                    && request.Provenance == messageData.ProvenanceData.ToProvenance()
                    && request.Metadata == messageData.Metadata
                ), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task WhenSqsAddressRejectRequest_ThenSqsLambdaAddressRejectRequestIsSent()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Register(_ => mediator.Object);
            var container = containerBuilder.Build();

            var messageData = Fixture.Create<SqsAddressRejectRequest>();
            var messageMetadata = new MessageMetadata { MessageGroupId = Fixture.Create<string>() };

            var sut = new MessageHandler(container);

            // Act
            await sut.HandleMessage(
                messageData,
                messageMetadata,
                CancellationToken.None);

            // Assert
            mediator
                .Verify(x => x.Send(It.Is<SqsLambdaAddressRejectRequest>(request =>
                    request.TicketId == messageData.TicketId
                    && request.MessageGroupId == messageMetadata.MessageGroupId
                    && request.Request == messageData.Request
                    && request.IfMatchHeaderValue == messageData.IfMatchHeaderValue
                    && request.Provenance == messageData.ProvenanceData.ToProvenance()
                    && request.Metadata == messageData.Metadata
                ), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task WhenSqsAddressRemoveRequest_ThenSqsLambdaAddressRemoveRequestIsSent()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Register(_ => mediator.Object);
            var container = containerBuilder.Build();

            var messageData = Fixture.Create<SqsAddressRemoveRequest>();
            var messageMetadata = new MessageMetadata { MessageGroupId = Fixture.Create<string>() };

            var sut = new MessageHandler(container);

            // Act
            await sut.HandleMessage(
                messageData,
                messageMetadata,
                CancellationToken.None);

            // Assert
            mediator
                .Verify(x => x.Send(It.Is<SqsLambdaAddressRemoveRequest>(request =>
                    request.TicketId == messageData.TicketId
                    && request.MessageGroupId == messageMetadata.MessageGroupId
                    && request.Request == messageData.Request
                    && request.IfMatchHeaderValue == messageData.IfMatchHeaderValue
                    && request.Provenance == messageData.ProvenanceData.ToProvenance()
                    && request.Metadata == messageData.Metadata
                ), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task WhenSqsAddressRetireRequest_ThenSqsLambdaAddressRetireRequestIsSent()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Register(_ => mediator.Object);
            var container = containerBuilder.Build();

            var messageData = Fixture.Create<SqsAddressRetireRequest>();
            var messageMetadata = new MessageMetadata { MessageGroupId = Fixture.Create<string>() };

            var sut = new MessageHandler(container);

            // Act
            await sut.HandleMessage(
                messageData,
                messageMetadata,
                CancellationToken.None);

            // Assert
            mediator
                .Verify(x => x.Send(It.Is<SqsLambdaAddressRetireRequest>(request =>
                    request.TicketId == messageData.TicketId
                    && request.MessageGroupId == messageMetadata.MessageGroupId
                    && request.Request == messageData.Request
                    && request.IfMatchHeaderValue == messageData.IfMatchHeaderValue
                    && request.Provenance == messageData.ProvenanceData.ToProvenance()
                    && request.Metadata == messageData.Metadata
                ), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task WhenSqsAddressCorrectRejectionRequest_ThenSqsLambdaAddressCorrectRejectionRequestIsSent()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Register(_ => mediator.Object);
            var container = containerBuilder.Build();

            var messageData = Fixture.Create<SqsAddressCorrectRejectionRequest>();
            var messageMetadata = new MessageMetadata { MessageGroupId = Fixture.Create<string>() };

            var sut = new MessageHandler(container);

            // Act
            await sut.HandleMessage(
                messageData,
                messageMetadata,
                CancellationToken.None);

            // Assert
            mediator
                .Verify(x => x.Send(It.Is<SqsLambdaAddressCorrectRejectionRequest>(request =>
                    request.TicketId == messageData.TicketId
                    && request.MessageGroupId == messageMetadata.MessageGroupId
                    && request.Request == messageData.Request
                    && request.IfMatchHeaderValue == messageData.IfMatchHeaderValue
                    && request.Provenance == messageData.ProvenanceData.ToProvenance()
                    && request.Metadata == messageData.Metadata
                ), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task WhenSqsAddressCorrectRetirementRequest_ThenSqsLambdaAddressCorrectRetirementRequestIsSent()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Register(_ => mediator.Object);
            var container = containerBuilder.Build();

            var messageData = Fixture.Create<SqsAddressCorrectRetirementRequest>();
            var messageMetadata = new MessageMetadata { MessageGroupId = Fixture.Create<string>() };

            var sut = new MessageHandler(container);

            // Act
            await sut.HandleMessage(
                messageData,
                messageMetadata,
                CancellationToken.None);

            // Assert
            mediator
                .Verify(x => x.Send(It.Is<SqsLambdaAddressCorrectRetirementRequest>(request =>
                    request.TicketId == messageData.TicketId
                    && request.MessageGroupId == messageMetadata.MessageGroupId
                    && request.Request == messageData.Request
                    && request.IfMatchHeaderValue == messageData.IfMatchHeaderValue
                    && request.Provenance == messageData.ProvenanceData.ToProvenance()
                    && request.Metadata == messageData.Metadata
                ), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task WhenSqsAddressCorrectBoxNumberRequest_ThenSqsLambdaAddressCorrectBoxNumberRequestIsSent()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Register(_ => mediator.Object);
            var container = containerBuilder.Build();

            var messageData = Fixture.Create<SqsAddressCorrectBoxNumberRequest>();
            var messageMetadata = new MessageMetadata { MessageGroupId = Fixture.Create<string>() };

            var sut = new MessageHandler(container);

            // Act
            await sut.HandleMessage(
                messageData,
                messageMetadata,
                CancellationToken.None);

            // Assert
            mediator
                .Verify(x => x.Send(It.Is<SqsLambdaAddressCorrectBoxNumberRequest>(request =>
                    request.TicketId == messageData.TicketId
                    && request.MessageGroupId == messageMetadata.MessageGroupId
                    && request.AddressPersistentLocalId == messageData.PersistentLocalId
                    && request.Request == messageData.Request
                    && request.IfMatchHeaderValue == messageData.IfMatchHeaderValue
                    && request.Provenance == messageData.ProvenanceData.ToProvenance()
                    && request.Metadata == messageData.Metadata
                ), CancellationToken.None), Times.Once);
        }
    }

    public class TestSqsRequest : SqsRequest
    { }
}
