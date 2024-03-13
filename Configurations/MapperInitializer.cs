using AutoMapper;
using BookshelfXchange.ViewModels.GET;
using BookshelfXchange.ViewModels.Update;

namespace BookshelfXchange.Maps
{
    public class MapperInitializer : Profile
    {
        public MapperInitializer()
        {
            CreateMap<UpdateBookViewModel, GetBookViewModel>().ReverseMap();

        }
    }
}
