using AutoMapper;
using Biblioteca.Data.Dtos.Request;
using Biblioteca.Data.Dtos.Response;
using Biblioteca.Models;

public class EmprestimoProfile : Profile
{
    public EmprestimoProfile()
    {
        CreateMap<CreateEmprestimoDto, Emprestimo>();
        CreateMap<UpdateEmprestimoDto, Emprestimo>();
        CreateMap<Emprestimo, UpdateEmprestimoDto>();
        CreateMap<Emprestimo, ReadEmprestimoDto>();
    }
}