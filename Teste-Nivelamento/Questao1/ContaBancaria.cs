using System.Globalization;

namespace Questao1
{
    class ContaBancaria
    {
        public ContaBancaria(
            int numero, 
            string titular)
        {
            Numero = numero;
            Titular = titular;
        }

        public ContaBancaria(
            int numero, 
            string titular, 
            double depositoInicial)
        {
            Numero = numero;
            Titular = titular;
            Saldo += depositoInicial;
        }

        public int Numero { get; set; }
        public string Titular { get; set; }
        public double Saldo { get; set; }

        public string ToString()
        {
            string saldo = string.Empty;

            if (Saldo != 0)
            {
                saldo = $"Saldo: {Saldo}";
            }

            return $"Numero da Conta: {Numero.ToString()} Titular {Titular.ToUpper()} {saldo}";
        }

        public void Deposito(double quantia)
        {
            Saldo += quantia;
        }

        public void Saque(double quantia)
        {
            Saldo -= quantia;
        }
    }
}
