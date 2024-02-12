using BE.TradeeHub.UserService.Domain.Entities;
using BE.TradeeHub.UserService.Domain.Interfaces.Repositories;
using BE.TradeeHub.UserService.Infrastructure.Repository;

namespace BE.TradeeHub.UserService.GraphQL.DataLoaders;

public class StaffDataLoader : GroupedDataLoader<Guid, UserEntity>
{
    private readonly IUserRepository _userRepository;

    public StaffDataLoader(IBatchScheduler batchScheduler, IUserRepository userRepository, DataLoaderOptions? options = null)
        : base(batchScheduler, options)
    {
        _userRepository = userRepository;
    }

    protected override async Task<ILookup<Guid, UserEntity>> LoadGroupedBatchAsync(IReadOnlyList<Guid> companiesMemberOfIds, CancellationToken cancellationToken)
    {
        // so basically we need to check the the children to what company they belong to that's why we use the GetAccessToCompaniesByIds 
        // because I am already on the staff level and I need to access my parent company 
        // basically get all people who have my userid as their access aka they are staff
        var staff = await _userRepository.GetCompaniesMemberOfByIds(companiesMemberOfIds, cancellationToken); 

        // Group the properties by each customer ID
        return companiesMemberOfIds
            .SelectMany(companiesMemberOfId => 
                staff?.Where(s => s.CompaniesMemberOf.Contains(companiesMemberOfId))
                    .Select(s => new { CompaniesMemberOfId = companiesMemberOfId, Staff = s }))
            .ToLookup(x => x.CompaniesMemberOfId, x => x.Staff);
    }
}