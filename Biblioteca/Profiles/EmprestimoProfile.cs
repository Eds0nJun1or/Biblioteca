using AutoMapper;
using Biblioteca.Data.Dtos.Request;
using Biblioteca.Data.Dtos.Response;
using Biblioteca.Models;

public class EmprestimoProfile : Profile
{
    public EmprestimoProfile()
    {
        CreateMap<CreateEmprestimoDto, Emprestimo>();
        CreateMap<Emprestimo, ReadEmprestimoDto>();
        CreateMap<UpdateEmprestimoDto, Emprestimo>();
    }
}

