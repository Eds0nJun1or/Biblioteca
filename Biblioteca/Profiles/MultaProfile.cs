using AutoMapper;
using Biblioteca.Data.Dtos.Request;
using Biblioteca.Data.Dtos.Response;
using Biblioteca.Models;

namespace Biblioteca.Profiles
{
    public class MultaProfile : Profile
    {
        public MultaProfile()
        {
            CreateMap<CreateMultaDto, Multa>();
            CreateMap<UpdateMultaDto, Multa>();
            CreateMap<Multa, ReadMultaDto>()
                .ForMember(dest => dest.Emprestimo, opt => opt.Ignore());
        }
    }
}
