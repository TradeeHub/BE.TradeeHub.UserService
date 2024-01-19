using BE.TradeeHub.UserService.Infrastructure.DbObjects;
using BE.TradeeHub.UserService.Infrastructure.Repository;
using MongoDB.Bson;

namespace BE.TradeeHub.UserService.GraphQL.DataLoaders;

public class CompaniesMemberOfDataLoader : GroupedDataLoader<Guid, UserDbObject>
{
    private readonly UserRepository _userRepository;

    public CompaniesMemberOfDataLoader(IBatchScheduler batchScheduler, UserRepository userRepository, DataLoaderOptions? options = null)
        : base(batchScheduler, options)
    {
        _userRepository = userRepository;
    }

    protected override async Task<ILookup<Guid, UserDbObject>> LoadGroupedBatchAsync(IReadOnlyList<Guid> staffIds, CancellationToken cancellationToken)
    {
        var companies = await _userRepository.GetStaffByIds(staffIds, cancellationToken); //returns companies where the staff belongs to

        return staffIds
            .SelectMany(staffId => 
                companies?.Where(c => c.Staff.Contains(staffId))
                    .Select(c => new { StaffId = staffId, Company = c }))
            .ToLookup(x => x.StaffId, x => x.Company);
    }
}