using GraphQL.Types;
using Litium.Accelerator.GraphQL.Models.Carts;

namespace Litium.Accelerator.GraphQL.Types.Carts
{
    public class AddressInfoType : InputObjectGraphType<AddressInfoModel>
    {
        public AddressInfoType()
        {
            Field(x => x.Address1)
                .Description("The first address line.");

            Field(x => x.Address2, nullable: true)
                .Description("The second address line.");

            Field(x => x.CareOf, nullable: true)
                .Description("The care of address.");

            Field(x => x.City)
                .Description("The city.");

            Field(x => x.Country)
                .Description("The country.");

            Field(x => x.PhoneNumber)
                .Description("The phone number.");

            Field(x => x.ZipCode)
                .Description("The zip code.");
        }
    }
}
