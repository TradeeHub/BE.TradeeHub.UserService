using BE.TradeeHub.UserService.Domain.Entities;
using BE.TradeeHub.UserService.Domain.Interfaces.Repositories;

namespace BE.TradeeHub.UserService.GraphQL.DataLoaders;

public class CompaniesMemberOfDataLoader : GroupedDataLoader<Guid, UserEntity>
{
    private readonly IUserRepository _userRepository;

    public CompaniesMemberOfDataLoader(IBatchScheduler batchScheduler, IUserRepository userRepository, DataLoaderOptions? options = null)
        : base(batchScheduler, options)
    {
        _userRepository = userRepository;
    }

    protected override async Task<ILookup<Guid, UserEntity>> LoadGroupedBatchAsync(IReadOnlyList<Guid> staffIds, CancellationToken cancellationToken)
    {
        var companies = await _userRepository.GetStaffByIdsAsync(staffIds, cancellationToken); //returns companies where the staff belongs to

        return staffIds
            .SelectMany(staffId => 
                companies?.Where(c => c.Staff.Contains(staffId))
                    .Select(c => new { StaffId = staffId, Company = c }))
            .ToLookup(x => x.StaffId, x => x.Company);
    }
}