namespace AddressRegistry.Consumer.Read.Municipality.Projections.Bosa
{
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using Be.Vlaanderen.Basisregisters.MessageHandling.Kafka.Simple;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Latest;
    using Microsoft.Extensions.Logging;

    public class MunicipalityBosaItemConsumer
    {
        private readonly ILifetimeScope _container;
        private readonly ILoggerFactory _loggerFactory;
        private readonly KafkaOptions _options;
        private readonly MunicipalityConsumerOptions _municipalityConsumerOptions;

        public MunicipalityBosaItemConsumer(
            ILifetimeScope container,
            ILoggerFactory loggerFactory,
            KafkaOptions options,
            MunicipalityConsumerOptions municipalityConsumerOptions)
        {
            _container = container;
            _loggerFactory = loggerFactory;
            _options = options;
            _municipalityConsumerOptions = municipalityConsumerOptions;
        }

        public async Task Start(CancellationToken cancellationToken = default)
        {
            var projector = new ConnectedProjector<MunicipalityConsumerContext>(Resolve.WhenEqualToHandlerMessageType(new MunicipalityBosaItemProjections().Handlers));

            var consumerGroupId = $"{nameof(AddressRegistry)}.{nameof(MunicipalityBosaItemConsumer)}.{_municipalityConsumerOptions.Topic}{_municipalityConsumerOptions.ConsumerGroupSuffix}";
            var result = await KafkaConsumer.Consume(
                _options,
                consumerGroupId,
                _municipalityConsumerOptions.Topic,
                async message =>
                {
                    await projector.ProjectAsync(_container.Resolve<MunicipalityConsumerContext>(), message, cancellationToken);
                },
                offset: null,
                cancellationToken);

            if (!result.IsSuccess)
            {
                var logger = _loggerFactory.CreateLogger<MunicipalityBosaItemConsumer>();
                logger.LogCritical($"Consumer group {consumerGroupId} could not consume from topic {_municipalityConsumerOptions.Topic}");
            }
        }
    }
}
