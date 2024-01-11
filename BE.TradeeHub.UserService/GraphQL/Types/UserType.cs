using BE.TradeeHub.UserService.GraphQL.QueryResolvers;
using BE.TradeeHub.UserService.Infrastructure.DbObjects;
using HotChocolate.Authorization;

namespace BE.TradeeHub.UserService.GraphQL.Types;

[Authorize]
public class UserType : ObjectType<UserDbObject>
{
    protected override void Configure(IObjectTypeDescriptor<UserDbObject> descriptor)
    {
        descriptor.Field(c => c.Staff)
            .ResolveWith<TypeResolver>(r => r.GetStaffMembers(default!, default!, default!))
            .Type<ListType<UserType>>();
        
        descriptor.Field(c => c.CompaniesMemberOf)
            .ResolveWith<TypeResolver>(r => r.GetCompaniesImMemberOf(default!, default!, default!))
            .Type<ListType<UserType>>();
    }
}