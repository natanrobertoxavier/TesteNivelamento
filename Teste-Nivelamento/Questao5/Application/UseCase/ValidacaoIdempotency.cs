
using Azure.Core;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;
using Questao5.Infrastructure.Sqlite;

namespace Questao5.Application.UseCase;

public class ValidacaoIdempotency : IValidacaoIdempotency
{
    private readonly IDatabaseBootstrap _dataBase;

    public ValidacaoIdempotency(
        IDatabaseBootstrap dataBase)
    {
        _dataBase = dataBase;
    }

    public void RegistrarResultado(string key, string response)
    {
        _dataBase.AtualizarIdempotencia(key, response);
    }

    public bool Validar(string key, string request)
    {
        var retorno = _dataBase.VerificaIdempotencia(key);

        if (!retorno)
        {
            _dataBase.RegistrarIdempotencia(key, request);
            return false;
        }

        return true;
    }
}
