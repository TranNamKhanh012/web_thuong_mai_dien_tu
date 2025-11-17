using AutoMapper;
using Thuong_Mai_Dien_Tu.Data;
using Thuong_Mai_Dien_Tu.ViewModels;

namespace Thuong_Mai_Dien_Tu.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() 
        {
            CreateMap<RegisterVM, KhachHang>();
                //.ForMember(kh => kh.HoTen, option => option.MapFrom(RegisterVM => RegisterVM.HoTen))
                //.ReverseMap();
        }
    }
}
