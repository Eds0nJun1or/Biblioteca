using AutoMapper;
using Biblioteca.Data.Dtos;
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