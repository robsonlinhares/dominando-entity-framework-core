namespace Curso.Domain
{
    public class Funcionario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string CPF { get; set; }
        public int DepartamentoId { get; set; }
        public virtual Departamento Departamento { get; set; }
        public string RG { get; set; }
    }
}