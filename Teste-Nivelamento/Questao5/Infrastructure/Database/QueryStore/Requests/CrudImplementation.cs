namespace Questao5.Infrastructure.Database.QueryStore.Requests;

public class CrudImplementation : ICrud
{
    public string VerificaIdempotency()
    {
        return @"SELECT chave_idempotencia AS CHAVE,
                        requisicao AS REQUISICAO,
                        resultado AS RESULTADO
                FROM idempotencia
                WHERE chave_idempotencia = @_KEY";
    }
}
