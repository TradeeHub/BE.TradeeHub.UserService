using BE.TradeeHub.UserService.Domain.Entities;
using BE.TradeeHub.UserService.GraphQL.DataLoaders;
using BE.TradeeHub.UserService.GraphQL.QueryResolvers;
using HotChocolate.Authorization;

namespace BE.TradeeHub.UserService.GraphQL.Types;

public class UserType : ObjectType<UserEntity>
{
    protected override void Configure(IObjectTypeDescriptor<UserEntity> descriptor)
    {
        descriptor.ImplementsNode()
            .IdField(u => u.Id)
            .ResolveNode((ctx, id) => ResolveByIdAsync(id, ctx.Service<UserDataLoader>(), ctx.RequestAborted));

        descriptor.Field(u => u.Id).ID();

        descriptor.Field(c => c.Staff)
            .ResolveWith<TypeResolver>(r => r.GetStaffMembers(default!, default!, default!))
            .Type<ListType<UserType>>();

        descriptor.Field(c => c.CompaniesMemberOf)
            .ResolveWith<TypeResolver>(r => r.GetCompaniesImMemberOf(default!, default!, default!))
            .Type<ListType<UserType>>();
    }
    
    private static async Task<UserEntity?> ResolveByIdAsync(Guid id, UserDataLoader dataLoader, CancellationToken ctx)
    {
        var data = await dataLoader.LoadAsync(id, ctx);
        return data.FirstOrDefault();
    }
}