using System;
using System.Linq;
using Curso.Domain;
using Microsoft.EntityFrameworkCore;

namespace DominandoEFCore
{
    class Program
    {
        static void Main(string[] args)
        {
            //EnsureCreatedAndDeleted();
            //HealthCheckBancoDeDados();

            //warmup
            // new Curso.Data.ApplicationContext().Departamentos.AsNoTracking().Any();
            // _count = 0;
            // GerenciarEstadoDaConexao(false);
            // _count = 0;
            // GerenciarEstadoDaConexao(true);

            //SqlInjection();

            //MigracoesPendentes();

            //AplicarMigracaoEmTempoDeExecucao();

            //TodasMigracoes();

            //MigracoesJaAplicadas();

            //ScriptGeralDoBancoDeDados();

            //CarregamentoAdiantado();

            //CarregamentoExplicito();
            CarregamentoLento();
        }

        static void CarregamentoLento()
        {
            using var db = new Curso.Data.ApplicationContext();
            SetupTiposCarregamentos(db);

            //db.ChangeTracker.LazyLoadingEnabled = false;

            var departamentos = db
                .Departamentos
                .ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine("------------------------------------");
                Console.WriteLine($"Departamentos: {departamento.Descricao}");

                if (departamento.Funcionarios?.Any() ?? false)
                {
                    foreach (var funcionario in departamento.Funcionarios)
                    {
                        Console.WriteLine($"\tFuncionario: {funcionario.Nome}");
                    }
                }
                else
                {
                    Console.WriteLine($"\tNenhum funcionario encontrado!");
                }
            }
        }

        static void CarregamentoExplicito()
        {
            using var db = new Curso.Data.ApplicationContext();
            SetupTiposCarregamentos(db);

            var departamentos = db
                .Departamentos.ToList();

            foreach (var departamento in departamentos)
            {

                if (departamento.Id == 2)
                {
                    //db.Entry(departamento).Collection(p => p.Funcionarios).Load();
                    db.Entry(departamento).Collection(p => p.Funcionarios).Query().Where(p => p.Id > 2).ToList();
                }
                Console.WriteLine("------------------------------------");
                Console.WriteLine($"Departamentos: {departamento.Descricao}");

                if (departamento.Funcionarios?.Any() ?? false)
                {
                    foreach (var funcionario in departamento.Funcionarios)
                    {
                        Console.WriteLine($"\tFuncionario: {funcionario.Nome}");
                    }
                }
                else
                {
                    Console.WriteLine($"\tNenhum funcionario encontrado!");
                }
            }
        }

        static void CarregamentoAdiantado()
        {
            using var db = new Curso.Data.ApplicationContext();
            SetupTiposCarregamentos(db);

            var departamentos = db
                .Departamentos
                .Include(p => p.Funcionarios);

            foreach (var departamento in departamentos)
            {
                Console.WriteLine("------------------------------------");
                Console.WriteLine($"Departamentos: {departamento.Descricao}");

                if (departamento.Funcionarios?.Any() ?? false)
                {
                    foreach (var funcionario in departamento.Funcionarios)
                    {
                        Console.WriteLine($"\tFuncionario: {funcionario.Nome}");
                    }
                }
                else
                {
                    Console.WriteLine($"\tNenhum funcionario encontrado!");
                }
            }
        }

        static void SetupTiposCarregamentos(Curso.Data.ApplicationContext db)
        {
            if (!db.Departamentos.Any())
            {
                db.Departamentos.AddRange(
                    new Curso.Domain.Departamento
                    {
                        Descricao = "Departamento 01",
                        Funcionarios = new System.Collections.Generic.List<Funcionario>
                        {
                            new Funcionario
                            {
                                Nome = "Robson Linhares",
                                CPF = "9999999999",
                                RG = "34453222"
                            }
                        }
                    },
                    new Departamento
                    {
                        Descricao = "Departamento 02",
                        Funcionarios = new System.Collections.Generic.List<Funcionario>
                        {
                            new Funcionario
                            {
                                Nome = "Paula Lima",
                                CPF = "9999955555",
                                RG = "34433332"
                            },
                            new Funcionario
                            {
                                Nome = "Alice Linhares",
                                CPF = "101010303030",
                                RG = "2233333"
                            },
                        }
                    });

                db.SaveChanges();
                db.ChangeTracker.Clear();
            }
        }

        static void ScriptGeralDoBancoDeDados()
        {
            using var db = new Curso.Data.ApplicationContext();
            var script = db.Database.GenerateCreateScript();

            Console.WriteLine(script);
        }

        static void MigracoesJaAplicadas()
        {
            using var db = new Curso.Data.ApplicationContext();
            var migracoes = db.Database.GetAppliedMigrations();

            Console.WriteLine($"Total: {migracoes.Count()}");

            foreach (var migracao in migracoes)
            {
                Console.WriteLine($"Migracao: {migracao}");
            }
        }

        static void TodasMigracoes()
        {
            using var db = new Curso.Data.ApplicationContext();
            var migracoes = db.Database.GetMigrations();

            Console.WriteLine($"Total: {migracoes.Count()}");

            foreach (var migracao in migracoes)
            {
                Console.WriteLine($"Migracao: {migracao}");
            }
        }

        static void AplicarMigracaoEmTempoDeExecucao()
        {
            using var db = new Curso.Data.ApplicationContext();

            db.Database.Migrate();
        }

        static void MigracoesPendentes()
        {
            using var db = new Curso.Data.ApplicationContext();

            var migracoesPendentes = db.Database.GetPendingMigrations();

            Console.WriteLine($"Total: {migracoesPendentes.Count()}");

            foreach (var migracao in migracoesPendentes)
            {
                Console.WriteLine($"Migração: {migracao}");
            }
        }

        static void SqlInjection()
        {
            using var db = new Curso.Data.ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            db.Departamentos.AddRange(
                new Curso.Domain.Departamento
                {
                    Descricao = "Departamento 01"
                },
                new Curso.Domain.Departamento
                {
                    Descricao = "Departamento 02"
                });
            db.SaveChanges();

            var descricao = "Teste 'or 1='1";
            db.Database.ExecuteSqlRaw($"update departamentos set descricao='AtaqueSqlInjection' where descricao='{descricao}'");
            foreach (var departamento in db.Departamentos.AsNoTracking())
            {
                Console.WriteLine($"Id: {departamento.Id}, Descricao: {departamento.Descricao}");
            }
        }

        static void ExecuteSQL()
        {
            using var db = new Curso.Data.ApplicationContext();

            //primeira Opção
            using (var cmd = db.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "SELECT 1";
                cmd.ExecuteNonQuery();
            }

            //segunda Opção
            var descricao = "TESTE";
            db.Database.ExecuteSqlRaw("update departamentos set descricao={0} where id=1", descricao);

            //terceira Opção         
            db.Database.ExecuteSqlInterpolated($"update departamentos set descricao={descricao} where id=1");
        }

        static int _count;
        static void GerenciarEstadoDaConexao(bool gerenciarEstadoConexao)
        {
            using var db = new Curso.Data.ApplicationContext();
            var time = System.Diagnostics.Stopwatch.StartNew();

            var conexao = db.Database.GetDbConnection();

            conexao.StateChange += (_, __) => ++_count;

            if (gerenciarEstadoConexao)
            {
                conexao.Open();
            }

            for (var i = 0; i < 200; i++)
            {
                db.Departamentos.AsNoTracking().Any();
            }

            time.Stop();

            var mensagem = $"Tempo: {time.Elapsed.ToString()}, {gerenciarEstadoConexao}, Contador: {_count}";

            Console.WriteLine(mensagem);
        }

        static void HealthCheckBancoDeDados()
        {
            using var db = new Curso.Data.ApplicationContext();
            var canConnect = db.Database.CanConnect();

            //nova forma
            if (canConnect)
                Console.WriteLine("Posso me Conectar");
            else
                Console.WriteLine("Não Posso me Conectar");

            //antiga forma
            // try
            // {
            //     //1
            //     var connection = db.Database.GetDbConnection();
            //     connection.Open();

            //     //2
            //     db.Departamentos.Any();


            //     Console.WriteLine("Posso me Conectar");
            // }
            // catch (Exception)
            // {

            // }
        }

        static void EnsureCreatedAndDeleted()
        {
            using var db = new Curso.Data.ApplicationContext();
            db.Database.EnsureCreated();
            //db.Database.EnsureDeleted();
        }
    }
}
