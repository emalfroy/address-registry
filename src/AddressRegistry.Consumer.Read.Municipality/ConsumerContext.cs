namespace AddressRegistry.Consumer.Read.Municipality
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AddressRegistry.Infrastructure;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner;
    using Microsoft.EntityFrameworkCore;
    using Projections;

    public class ConsumerContext : RunnerDbContext<ConsumerContext>
    {
        public DbSet<MunicipalityLatestItem> MunicipalityLatestItems { get; set; }

        // This needs to be here to please EF
        public ConsumerContext()
        { }

        // This needs to be DbContextOptions<T> for Autofac!
        public ConsumerContext(DbContextOptions<ConsumerContext> options)
            : base(options)
        { }

        public override string ProjectionStateSchema => Schema.ConsumerReadMunicipality;
    }

    public class ConsumerContextFactory : RunnerDbContextMigrationFactory<ConsumerContext>
    {
        public ConsumerContextFactory()
            : this("ConsumerAdmin")
        { }

        public ConsumerContextFactory(string connectionStringName)
            : base(connectionStringName, new MigrationHistoryConfiguration
            {
                Schema = Schema.ConsumerReadMunicipality,
                Table = MigrationTables.ConsumerReadMunicipality
            })
        { }

        protected override ConsumerContext CreateContext(DbContextOptions<ConsumerContext> migrationContextOptions) => new ConsumerContext(migrationContextOptions);

        public ConsumerContext Create(DbContextOptions<ConsumerContext> options) => CreateContext(options);
    }

    public static class AddressDetailExtensions
    {
        public static async Task<MunicipalityLatestItem> FindAndUpdate(
            this Func<ConsumerContext> contextFactory,
            Guid municipalityId,
            Action<MunicipalityLatestItem> updateFunc,
            CancellationToken ct)
        {
            await using var context = contextFactory();

            var municipality = await context
                .MunicipalityLatestItems
                .FindAsync(municipalityId, cancellationToken: ct);

            if (municipality == null)
                throw DatabaseItemNotFound(municipalityId);

            updateFunc(municipality);

            await context.SaveChangesAsync(ct);

            return municipality;
        }

        public static async Task<MunicipalityLatestItem> FindAndUpdate(
            this ConsumerContext context,
            Guid municipalityId,
            Action<MunicipalityLatestItem> updateFunc,
            CancellationToken ct)
        {
            var municipality = await context
                .MunicipalityLatestItems
                .FindAsync(municipalityId, cancellationToken: ct);

            if (municipality == null)
                throw DatabaseItemNotFound(municipalityId);

            updateFunc(municipality);

            await context.SaveChangesAsync(ct);

            return municipality;
        }

        private static ProjectionItemNotFoundException<MunicipalityProjections> DatabaseItemNotFound(Guid municipalityId)
            => new ProjectionItemNotFoundException<MunicipalityProjections>(municipalityId.ToString("D"));
    }
}
