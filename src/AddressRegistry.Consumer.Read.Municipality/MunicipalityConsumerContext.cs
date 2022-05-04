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

    public class MunicipalityConsumerContext : RunnerDbContext<MunicipalityConsumerContext>
    {
        public DbSet<MunicipalityLatestItem> MunicipalityLatestItems { get; set; }
        public DbSet<MunicipalityBosaItem> MunicipalityBosaItems { get; set; }

        // This needs to be here to please EF
        public MunicipalityConsumerContext()
        { }

        // This needs to be DbContextOptions<T> for Autofac!
        public MunicipalityConsumerContext(DbContextOptions<MunicipalityConsumerContext> options)
            : base(options)
        { }

        public override string ProjectionStateSchema => Schema.ConsumerReadMunicipality;
    }

    public class ConsumerContextFactory : RunnerDbContextMigrationFactory<MunicipalityConsumerContext>
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

        protected override MunicipalityConsumerContext CreateContext(DbContextOptions<MunicipalityConsumerContext> migrationContextOptions) => new MunicipalityConsumerContext(migrationContextOptions);

        public MunicipalityConsumerContext Create(DbContextOptions<MunicipalityConsumerContext> options) => CreateContext(options);
    }

    public static class AddressDetailExtensions
    {
        public static async Task<TEntity> FindAndUpdate<TEntity, TProjections>(
            this MunicipalityConsumerContext context,
            Guid municipalityId,
            Action<TEntity> updateFunc,
            CancellationToken ct) where TEntity : class
        {
            var municipality = await context.FindAsync<TEntity>(new object[] { municipalityId }, cancellationToken: ct);

            if (municipality == null)
                throw DatabaseItemNotFound<TProjections>(municipalityId);

            updateFunc(municipality);

            await context.SaveChangesAsync(ct);

            return municipality;
        }

        private static ProjectionItemNotFoundException<TProjections> DatabaseItemNotFound<TProjections>(Guid municipalityId)
            => new(municipalityId.ToString("D"));
    }
}
