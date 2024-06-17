using System.ComponentModel.DataAnnotations;

namespace Questao5.Application.Commands.Requests;

public class MovimentacaoRequestDto
{
    [Required(ErrorMessage = "Número da conta é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "Número da conta não pode ser zero")]
    public int NumeroConta { get; set; }

    [Required(ErrorMessage = "Tipo de movimentação é obrigatório")]
    [RegularExpression("^[CD]$", ErrorMessage = "Tipo de movimentação deve ser 'C' para Crédito ou 'D' para Débito")]
    public char TipoMovimentacao { get; set; }

    [Required(ErrorMessage = "Valor da movimentação é obrigatório")]
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Valor da movimentação deve ser maior que zero")]
    public decimal ValorMovimentacao { get; set; }

    public string? ContaCorrenteId { get; set; }
}
