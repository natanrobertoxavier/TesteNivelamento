using AutoMapper;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Sqlite;

namespace Questao5.Application.UseCase;

public class MovimentacaoContaUseCase : IMovimentacaoContaUseCase
{
    private readonly IDatabaseBootstrap _dataBase;
    private readonly IMapper _mapper;

    public MovimentacaoContaUseCase(
        IDatabaseBootstrap dataBase, 
        IMapper mapper)
    {
        _dataBase = dataBase;
        _mapper = mapper;
    }

    public ContaResponseDto SaldoContaCorrente(int numeroConta)
    {
        var conta = ValidarRequisicao(numeroConta, true);

        Movimentacoes movimentacoes = new Movimentacoes();

        movimentacoes.TotalCredito = _dataBase.ObterMovimentacoesCredito(conta.Id);
        movimentacoes.TotalDebito = _dataBase.ObterMovimentacoesDebito(conta.Id);

        movimentacoes.Saldo = (movimentacoes.TotalCredito - movimentacoes.TotalDebito);

        conta.Saldo = movimentacoes.Saldo;
        conta.DataHoraConsulta = DateTime.Now;

        return conta;
    }

    public ContaResponseDto MovimentarContaCorrente(MovimentacaoRequestDto request)
    {
        var conta = ValidarRequisicao(request.NumeroConta, false);

        request.ContaCorrenteId = conta.Id;

        _dataBase.InserirMovimentacao(request);

        conta = SaldoContaCorrente(request.NumeroConta);

        return conta;
    }

    private ContaResponseDto ValidarRequisicao(int numeroConta, bool consultaSaldo)
    {
        var retorno = _dataBase.ConsultarConta(numeroConta) ?? 
            throw new Exception($"Conta {numeroConta} não localizada!");

        if (!consultaSaldo && retorno.Ativa == 0)
            throw new Exception($"Conta {numeroConta} está inativa!");

        return _mapper.Map<ContaResponseDto>(retorno);
    }
}
