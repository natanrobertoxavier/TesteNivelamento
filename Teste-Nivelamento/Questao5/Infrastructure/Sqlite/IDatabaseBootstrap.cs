using Questao5.Application.Commands.Requests;
using Questao5.Domain.Entities;

namespace Questao5.Infrastructure.Sqlite
{
    public interface IDatabaseBootstrap
    {
        void AtualizarIdempotencia(string key, string reponseJson);
        Conta ConsultarConta(int numeroConta);
        void InserirMovimentacao(MovimentacaoRequestDto request);
        decimal ObterMovimentacoesCredito(string id);
        decimal ObterMovimentacoesDebito(string id);
        void RegistrarIdempotencia(string key, string request);
        void Setup();
        bool VerificaIdempotencia(string key);
    }
}