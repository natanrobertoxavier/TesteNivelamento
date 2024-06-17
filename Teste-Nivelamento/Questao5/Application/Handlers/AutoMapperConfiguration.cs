using AutoMapper;
using Questao5.Application.Commands.Responses;
using Questao5.Domain.Entities;

namespace Questao5.Application.Handlers;

public class AutoMapperConfiguration : Profile
{
    public AutoMapperConfiguration()
    {
        CreateMap<Conta, ContaResponseDto>()
            .ReverseMap();
    }
}
