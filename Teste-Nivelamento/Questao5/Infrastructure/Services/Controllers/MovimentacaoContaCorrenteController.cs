using Azure.Core;
using IdempotentAPI.Filters;
using Microsoft.AspNetCore.Mvc;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;
using Questao5.Application.UseCase;
using System.ComponentModel.DataAnnotations;

namespace Questao5.Infrastructure.Services.Controllers;

[Route("[controller]")]
//[Idempotent(Enabled = true)]
public class MovimentacaoContaCorrenteController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ContaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult MovimentarContaCorrente(
        [FromServices] IMovimentacaoContaUseCase useCase,
        [FromServices] IValidacaoIdempotency idempotenciaUseCase,
        [FromHeader] [Required] string idempotencyKey,
        [FromBody] [Required] MovimentacaoRequestDto request)
    {
        if (request is null)
            return BadRequest("Objeto de request não pode ser nulo");

        if (!TryValidateModel(request))
            return BadRequest(ModelState);

        string requestJson = Newtonsoft.Json.JsonConvert.SerializeObject(request);

        var idempotencyRequest = idempotenciaUseCase.Validar(idempotencyKey, requestJson);

        if (idempotencyRequest)
            return BadRequest($"Requisição {idempotencyKey} já registrada");

        var retorno =  useCase.MovimentarContaCorrente(request);

        string retornoJson = Newtonsoft.Json.JsonConvert.SerializeObject(retorno);

        idempotenciaUseCase.RegistrarResultado(idempotencyKey, retornoJson);

        return Ok(retorno);
    }

    [HttpGet("{numeroConta}/nroconta")]
    [ProducesResponseType(typeof(ContaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult SaldoContaCorrente(
        [FromServices] IMovimentacaoContaUseCase useCase,
        [FromServices] IValidacaoIdempotency idempotenciaUseCase,
        [FromHeader] [Required] string idempotencyKey,
        [FromRoute] int numeroConta)
    {
        if (numeroConta == 0)
            return BadRequest("Informe o numero da conta");

        string request = $"Consulta de saldo Conta {numeroConta} as {DateTime.Now}";

        var idempotencyRequest = idempotenciaUseCase.Validar(idempotencyKey, request);

        if (idempotencyRequest)
            return BadRequest($"Requisição {idempotencyKey} já registrada");

        var retorno =  useCase.SaldoContaCorrente(numeroConta);

        string retornoJson = Newtonsoft.Json.JsonConvert.SerializeObject(retorno);

        idempotenciaUseCase.RegistrarResultado(idempotencyKey, retornoJson);

        return Ok(retorno);
    }
}
