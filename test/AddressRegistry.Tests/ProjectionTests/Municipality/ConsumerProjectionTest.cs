namespace AddressRegistry.Tests.ProjectionTests.Municipality
{
    using System;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Testing;
    using Microsoft.EntityFrameworkCore;
    using Municipality = AddressRegistry.Consumer.Read.Municipality;

    public abstract class MunicipalityProjectionTest<TProjection>
        where TProjection : ConnectedProjection<Municipality.ConsumerContext>, new()
    {
        protected ConnectedProjectionTest<Municipality.ConsumerContext, TProjection> Sut { get; }

        public MunicipalityProjectionTest()
        {
            Sut = new ConnectedProjectionTest<Municipality.ConsumerContext, TProjection>(CreateContext, CreateProjection);
        }

        protected virtual Municipality.ConsumerContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<Municipality.ConsumerContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new Municipality.ConsumerContext(options);
        }

        protected abstract TProjection CreateProjection();
    }
}
