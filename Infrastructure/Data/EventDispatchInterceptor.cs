namespace Infrastructure.Data
{
    public class EventDispatchInterceptor(IDomainEventDispatcher domainEventDispatcher)
        : SaveChangesInterceptor
    {
        private readonly IDomainEventDispatcher _domainEventDispatcher = domainEventDispatcher;

        public override async ValueTask<int> SavedChangesAsync(
            SaveChangesCompletedEventData eventData,
            int result,
            CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;
            if (context is not AppDbContext appDbContext)
                return await base.SavedChangesAsync(eventData, result, cancellationToken);

            var entitiesWithEvents = appDbContext.ChangeTracker
                .Entries<IHasDomainEvents>()
                .Select(e => e.Entity)
                .Where(e => e.DomainEvents.Any())
                .ToArray();

            await _domainEventDispatcher.DispatchAndClearEvents(entitiesWithEvents);

            return await base.SavedChangesAsync(eventData, result, cancellationToken);
        }
    }
}
