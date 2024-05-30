using AutoMapper;
using Biblioteca.Data.Dtos.Request;
using Biblioteca.Data.Dtos.Response;
using Biblioteca.Models;

namespace Biblioteca.Profiles
{
    public class ExemplarProfile : Profile
    {
        public ExemplarProfile()
        {
            CreateMap<Exemplar, ReadExemplarDto>();
            CreateMap<CreateExemplarDto, Exemplar>();
        }
    }
}
