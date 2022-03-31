using AutoMapper;
using JetBrains.Annotations;
using Litium.Accelerator.ViewModels.Persons;
using Litium.Runtime.AutoMapper;
using Litium.Sales;

namespace Litium.Accelerator.ViewModels.Checkout
{
    public class CustomerDetailsViewModel : AddressViewModel, IAutoMapperConfiguration
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        [UsedImplicitly]
        void IAutoMapperConfiguration.Configure(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<CustomerDetailsViewModel, Address>()
                .ForMember(x => x.Address1, m => m.MapFrom(address => address.Address))
                .ForMember(x => x.MobilePhone, m => m.MapFrom(address => address.PhoneNumber));
        }
    }
}
