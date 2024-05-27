using AutoMapper;
using Biblioteca.Data.Dtos.Request;
using Biblioteca.Data.Dtos.Response;
using Biblioteca.Models;

namespace Biblioteca.Profiles
{
    public class UsuarioProfile : Profile
    {
        public UsuarioProfile()
        {
          CreateMap<CreateUsuarioDto, Usuario>();
          CreateMap<Usuario, ReadUsuarioDto>();
          CreateMap<UpdateUsuarioDto, Usuario>();
        }
    }
}
