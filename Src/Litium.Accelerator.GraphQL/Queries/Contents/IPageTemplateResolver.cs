using Litium.Runtime.DependencyInjection;
using Litium.Web.GraphQL;

namespace Litium.Accelerator.GraphQL.Queries.Contents
{
    [Service(ServiceType = typeof(IPageTemplateResolver<>), NamedService = true, Lifetime = DependencyLifetime.Scoped)]
    public interface IPageTemplateResolver<TReturnType> : IFieldResolver<TReturnType>
    { }
}
