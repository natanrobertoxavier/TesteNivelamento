namespace Questao5.Application.Commands.Responses;

public class ContaResponseDto
{
    public string Id { get; set; }
    public long Numero { get; set; }
    public string Nome { get; set; }
    public long Ativa { get; set;}
    public decimal Saldo { get; set;}
    public DateTime DataHoraConsulta { get; set; }
}
