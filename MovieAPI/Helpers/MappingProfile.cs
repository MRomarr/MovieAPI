namespace MoviesAPI.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Movie,MovieDTO>()
                .ForMember(src=>src.Poster,opt=>opt.Ignore());   
        }

    }
    
}
