namespace AddressRegistry.Tests.ProjectionTests.Municipality
{
    using System.Threading.Tasks;
    using AddressRegistry.Consumer.Read.Municipality.Projections;
    using AutoFixture;
    using Be.Vlaanderen.Basisregisters.GrAr.Contracts.MunicipalityRegistry;
    using FluentAssertions;
    using global::AutoFixture;
    using Xunit;

    public class MunicipalityProjectionsTests : MunicipalityProjectionTest<MunicipalityProjections>
    {
        private readonly Fixture _fixture;

        public MunicipalityProjectionsTests()
        {
            _fixture = new Fixture();
            _fixture.Customize(new InfrastructureCustomization());
            _fixture.Customize(new WithFixedStreetNameId());
            _fixture.Customize(new WithFixedStreetNamePersistentLocalId());
        }

        [Fact]
        public Task MunicipalityWasRegistered()
        {
            var registered = _fixture.Create<MunicipalityWasRegistered>();

            return Sut
                .Given(registered)
                .Then(async ct =>
                {
                    var expected = await ct.MunicipalityLatestItems.FindAsync(registered.MunicipalityId);
                    expected.Should().NotBeNull();
                    expected.MunicipalityId.Should().Be(registered.MunicipalityId);
                });
        }

        protected override MunicipalityProjections CreateProjection()
            => new MunicipalityProjections();
    }
}
