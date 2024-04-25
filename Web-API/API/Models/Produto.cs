using System.ComponentModel.DataAnnotations;

namespace API.Models;

public class Produto
{

    // construtor

    public Produto()
    {
        Id = Guid.NewGuid().ToString();
        CriadoEm = DateTime.Now;
    }

    public Produto(string nome, string descricao, double valor)
    {
        Id = Guid.NewGuid().ToString();
        Nome = nome;
        Descricao = descricao;
        Valor = valor;
        CriadoEm = DateTime.Now;
    }

    // regra de negocios
    // data anotations

    public string Id { get; set; }

    [Required(ErrorMessage = "Este Campo é obrigatorio")]
    public string? Nome { get; set; }


    [MinLength(3, ErrorMessage = "Esse campo tem o tamanho minimo de 3 caracteres")]
    [MaxLength(10, ErrorMessage = "Esse campo tem o tamanho maximo de 10 caracteres")]
    public string? Descricao { get; set; }


    [Range(1, 1000, ErrorMessage = "Esse campo deve ter valo entre 1 e 1000")]
    public double Valor { get; set; }

    public DateTime CriadoEm { get; set; }
    public double Quantidade { get; set; }

}