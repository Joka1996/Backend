using GraphQL.Server.Ui.Playground;
using Litium.Web.GraphQL;
using Litium.Web.Runtime;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Litium.Accelerator.GraphQL.Runtime
{
    public class EndpointRouteBuilder : IApplicationEndpointRouteBuilder
    {
        public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapGraphQLPlayground(new PlaygroundOptions()
            {
                GraphQLEndPoint = StorefrontSchema.Endpoint,
            });

            return builder;
        }
    }
}
