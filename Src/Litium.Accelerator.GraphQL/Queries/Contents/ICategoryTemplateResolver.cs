using Litium.Runtime.DependencyInjection;
using Litium.Web.GraphQL;

namespace Litium.Accelerator.GraphQL.Queries.Contents
{
    [Service(ServiceType = typeof(ICategoryTemplateResolver<>), NamedService = true, Lifetime = DependencyLifetime.Scoped)]
    public interface ICategoryTemplateResolver<TReturnType> : IFieldResolver<TReturnType>
    { }
}
