using Dapper;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Data.Sqlite;
using Questao5.Application.Commands.Requests;
using Questao5.Domain.Entities;

namespace Questao5.Infrastructure.Sqlite
{
    public class DatabaseBootstrap : IDatabaseBootstrap
    {
        private readonly DatabaseConfig databaseConfig;

        public DatabaseBootstrap(DatabaseConfig databaseConfig)
        {
            this.databaseConfig = databaseConfig;
        }

        public void Setup()
        {
            using var connection = new SqliteConnection(databaseConfig.Name);

            var table = connection.Query<string>("SELECT name FROM sqlite_master WHERE type='table' AND (name = 'contacorrente' or name = 'movimento' or name = 'idempotencia');");
            var tableName = table.FirstOrDefault();
            if (!string.IsNullOrEmpty(tableName) && (tableName == "contacorrente" || tableName == "movimento" || tableName == "idempotencia"))
                return;

            connection.Execute("CREATE TABLE contacorrente ( " +
                               "idcontacorrente TEXT(37) PRIMARY KEY," +
                               "numero INTEGER(10) NOT NULL UNIQUE," +
                               "nome TEXT(100) NOT NULL," +
                               "ativo INTEGER(1) NOT NULL default 0," +
                               "CHECK(ativo in (0, 1)) " +
                               ");");

            connection.Execute("CREATE TABLE movimento ( " +
                "idmovimento TEXT(37) PRIMARY KEY," +
                "idcontacorrente INTEGER(10) NOT NULL," +
                "datamovimento TEXT(25) NOT NULL," +
                "tipomovimento TEXT(1) NOT NULL," +
                "valor REAL NOT NULL," +
                "CHECK(tipomovimento in ('C', 'D')), " +
                "FOREIGN KEY(idcontacorrente) REFERENCES contacorrente(idcontacorrente) " +
                ");");

            connection.Execute("CREATE TABLE idempotencia (" +
                               "chave_idempotencia TEXT(37) PRIMARY KEY," +
                               "requisicao TEXT(1000)," +
                               "resultado TEXT(1000));");

            connection.Execute("INSERT INTO contacorrente(idcontacorrente, numero, nome, ativo) VALUES('B6BAFC09 -6967-ED11-A567-055DFA4A16C9', 123, 'Katherine Sanchez', 1);");
            connection.Execute("INSERT INTO contacorrente(idcontacorrente, numero, nome, ativo) VALUES('FA99D033-7067-ED11-96C6-7C5DFA4A16C9', 456, 'Eva Woodward', 1);");
            connection.Execute("INSERT INTO contacorrente(idcontacorrente, numero, nome, ativo) VALUES('382D323D-7067-ED11-8866-7D5DFA4A16C9', 789, 'Tevin Mcconnell', 1);");
            connection.Execute("INSERT INTO contacorrente(idcontacorrente, numero, nome, ativo) VALUES('F475F943-7067-ED11-A06B-7E5DFA4A16C9', 741, 'Ameena Lynn', 0);");
            connection.Execute("INSERT INTO contacorrente(idcontacorrente, numero, nome, ativo) VALUES('BCDACA4A-7067-ED11-AF81-825DFA4A16C9', 852, 'Jarrad Mckee', 0);");
            connection.Execute("INSERT INTO contacorrente(idcontacorrente, numero, nome, ativo) VALUES('D2E02051-7067-ED11-94C0-835DFA4A16C9', 963, 'Elisha Simons', 0);");
        }

        public bool VerificaIdempotencia(string key)
        {
            using var connection = new SqliteConnection(databaseConfig.Name);
            connection.Open();

            string query = @"SELECT chave_idempotencia AS CHAVE,
                            requisicao AS REQUISICAO,
                            resultado AS RESULTADO
                     FROM idempotencia
                     WHERE chave_idempotencia = @Key";

            var resultado = connection.QueryFirstOrDefault(query, new { Key = key });

            if (resultado is not null)
            {
                return true;
            }

            return false;
        }
        public void RegistrarIdempotencia(string key, string request)
        {
            using var connection = new SqliteConnection(databaseConfig.Name);
            connection.Open();

            string query = @"INSERT INTO idempotencia (chave_idempotencia, requisicao)
                 VALUES(@KEY, @REQUISICAO);";

            using var command = new SqliteCommand(query, connection);

            command.Parameters.AddWithValue("@KEY", key);
            command.Parameters.AddWithValue("@REQUISICAO", request);

            command.ExecuteNonQuery();
        }

        public Conta ConsultarConta(int numeroConta)
        {
            Conta retorno = new Conta();

            using var connection = new SqliteConnection(databaseConfig.Name);
            connection.Open();

            string query = @"SELECT idcontacorrente AS ID,
                            numero AS NUMERO,
                            nome AS NOME,
                            ativo AS ATIVO
                     FROM contacorrente
                     WHERE numero = @Numero";

            var resultado = connection.QueryFirstOrDefault(query, new { Numero = numeroConta });

            if (resultado is not null)
            {
                retorno.Id = resultado.ID;
                retorno.Numero = resultado.NUMERO;
                retorno.Nome = resultado.NOME;
                retorno.Ativa = resultado.ATIVO;

                return retorno;
            }

            return null;
        }

        public decimal ObterMovimentacoesCredito(string id)
        {
            using var connection = new SqliteConnection(databaseConfig.Name);
            connection.Open();

            string query = @"
                SELECT SUM(valor) AS TOTALCREDITO
                FROM movimento
                WHERE idcontacorrente = @CONTACORRENTEID
                AND tipomovimento = 'C';
            "; 

            var resultado = connection.QueryFirstOrDefault<decimal?>(query, new { CONTACORRENTEID = id });

            if (resultado.HasValue)
            {
                return resultado.Value;
            }

            return 0.00m;
        }

        public decimal ObterMovimentacoesDebito(string id)
        {
            using var connection = new SqliteConnection(databaseConfig.Name);
            connection.Open();

            string query = @"SELECT SUM(valor) AS TOTALDEBITO
                     FROM movimento
                WHERE idcontacorrente = @CONTACORRENTEID
                AND tipomovimento = 'D'";

            var resultado = connection.QueryFirstOrDefault<decimal?>(query, new { CONTACORRENTEID = id });

            if (resultado.HasValue)
            {
                return resultado.Value;
            }

            return 0.00m;
        }

        public void AtualizarIdempotencia(string key, string responseJson)
        {
            using var connection = new SqliteConnection(databaseConfig.Name);
            connection.Open();

            string query = @"UPDATE idempotencia SET resultado = @RESULTADO
                     WHERE chave_idempotencia = @KEY";

            connection.Execute(query, new { RESULTADO = responseJson, KEY = key });
        }

        public void InserirMovimentacao(MovimentacaoRequestDto request)
        {
            Guid newGuid = Guid.NewGuid();
            using var connection = new SqliteConnection(databaseConfig.Name);
            connection.Open();

            string query = @"INSERT INTO movimento (idmovimento, idcontacorrente, datamovimento, tipomovimento, valor)
                 VALUES(@IDMOVIMENTO, @CONTACORRENTEID, @DATAMOVIMENTO, @TIPOMOVIMENTO, @VALOR);";

            using var command = new SqliteCommand(query, connection);

            command.Parameters.AddWithValue("@IDMOVIMENTO", newGuid.ToString());
            command.Parameters.AddWithValue("@CONTACORRENTEID", request.ContaCorrenteId);
            command.Parameters.AddWithValue("@DATAMOVIMENTO", DateTime.Now);
            command.Parameters.AddWithValue("@TIPOMOVIMENTO", request.TipoMovimentacao);
            command.Parameters.AddWithValue("@VALOR", request.ValorMovimentacao);

            command.ExecuteNonQuery();
        }
    }
}
