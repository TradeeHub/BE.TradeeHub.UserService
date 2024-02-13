using BE.TradeeHub.UserService.Domain.Entities;
using BE.TradeeHub.UserService.Domain.Interfaces.Repositories;


namespace BE.TradeeHub.UserService.GraphQL.DataLoaders;

public class UserDataLoader : GroupedDataLoader<Guid, UserEntity>
{
    private readonly IUserRepository _userRepository;

    public UserDataLoader(IBatchScheduler batchScheduler, IUserRepository userRepository, DataLoaderOptions? options = null)
        : base(batchScheduler, options)
    {
        _userRepository = userRepository;
    }

    protected override async Task<ILookup<Guid, UserEntity>> LoadGroupedBatchAsync(IReadOnlyList<Guid> userIds, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetUsersByIdsAsync(userIds, cancellationToken);

        var groupedUsers = users.ToLookup(user => user.Id);

        return groupedUsers;
    }
}