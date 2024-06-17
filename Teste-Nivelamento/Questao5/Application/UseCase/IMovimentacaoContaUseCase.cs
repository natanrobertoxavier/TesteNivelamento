using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;

namespace Questao5.Application.UseCase;

public interface IMovimentacaoContaUseCase
{
    ContaResponseDto MovimentarContaCorrente(MovimentacaoRequestDto request);
    ContaResponseDto SaldoContaCorrente(int numeroConta);
}
