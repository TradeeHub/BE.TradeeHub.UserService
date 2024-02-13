using BE.TradeeHub.UserService.Domain.Entities;
using BE.TradeeHub.UserService.GraphQL.QueryResolvers;

namespace BE.TradeeHub.UserService.GraphQL.Types;

public class UserType : ObjectType<UserEntity>
{
    protected override void Configure(IObjectTypeDescriptor<UserEntity> descriptor)
    {
        descriptor.Field(u => u.Id).ID();

        descriptor.Field(c => c.Staff)
            .ResolveWith<TypeResolver>(r => r.GetStaffMembers(default!, default!, default!))
            .Type<ListType<UserType>>();

        descriptor.Field(c => c.CompaniesMemberOf)
            .ResolveWith<TypeResolver>(r => r.GetCompaniesImMemberOf(default!, default!, default!))
            .Type<ListType<UserType>>();
    }
}