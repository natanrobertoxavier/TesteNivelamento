using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;

namespace Questao5.Application.UseCase;

public interface IValidacaoIdempotency
{
    bool Validar(string key, string request);
    void RegistrarResultado(string key, string response);
}
