namespace AddressRegistry.Consumer.Read.Municipality.Projections.Latest
{
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using Be.Vlaanderen.Basisregisters.MessageHandling.Kafka.Simple;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Microsoft.Extensions.Logging;

    public class MunicipalityLatestItemConsumer
    {
        private readonly ILifetimeScope _container;
        private readonly ILoggerFactory _loggerFactory;
        private readonly KafkaOptions _options;
        private readonly MunicipalityConsumerOptions _municipalityConsumerOptions;

        public MunicipalityLatestItemConsumer(
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
            var projector = new ConnectedProjector<MunicipalityConsumerContext>(Resolve.WhenEqualToHandlerMessageType(new MunicipalityLatestItemProjections().Handlers));

            var consumerGroupId = $"{nameof(AddressRegistry)}.{nameof(MunicipalityLatestItemConsumer)}.{_municipalityConsumerOptions.Topic}{_municipalityConsumerOptions.ConsumerGroupSuffix}";
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
                var logger = _loggerFactory.CreateLogger<MunicipalityLatestItemConsumer>();
                logger.LogCritical($"Consumer group {consumerGroupId} could not consume from topic {_municipalityConsumerOptions.Topic}");
            }
        }
    }
}
