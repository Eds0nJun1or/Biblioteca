using AutoMapper;
using Biblioteca.Data.Dtos.Reponse;
using Biblioteca.Data.Dtos.Request;
using Biblioteca.Models;

namespace Biblioteca.Profiles;

public class LivroProfile : Profile
{
    public LivroProfile()
    {
        CreateMap<CreateLivroDto, Livro>();
        CreateMap<UpdateLivroDto, Livro>();
        CreateMap<Livro, UpdateLivroDto>();
        CreateMap<Livro, ReadLivroDto>();
    }
}