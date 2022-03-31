using GraphQL.Types;
using Litium.Accelerator.GraphQL.Models.Carts;

namespace Litium.Accelerator.GraphQL.Types.Carts
{
    public class CustomerDetailsType : InputObjectGraphType<CustomerDetailsModel>
    {
        public CustomerDetailsType()
        {
            Field(x => x.Email)
                .Description("The customer email.");

            Field(x => x.FirstName)
                .Description("The customers first name.");

            Field(x => x.LastName)
                .Description("The customers last name.");
        }
    }
}
