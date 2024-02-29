using AutoMapper;
using BaselCoin2.DTOs.Requests;
using BaselCoin2.Models;

namespace BaselCoin2.Common
{
    public class Mapping : Profile
    {

        public Mapping()
        {
            CreateMap<ApplicationUser, UserEditRequest>()
                .ForMember(dest => dest.Password, opt => opt.Ignore());

            CreateMap<UserEditRequest, ApplicationUser>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Balance, BalanceEditRequest>();

            CreateMap<BalanceEditRequest, Balance>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<BalanceCreateRequest, Balance>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }   

    }
}
