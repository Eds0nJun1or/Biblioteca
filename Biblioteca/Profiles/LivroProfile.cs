using AutoMapper;
using Biblioteca.Data.Dtos.Reponse;
using Biblioteca.Data.Dtos.Request;
using Biblioteca.Models;

namespace Biblioteca.Profiles;

public class LivroProfile : Profile
{
    public LivroProfile()
    {
        CreateMap<CreateLivroDto, livro>();
        CreateMap<UpdateLivroDto, livro>();
        CreateMap<livro, UpdateLivroDto>();
        CreateMap<livro, ReadLivroDto>();
    }
}