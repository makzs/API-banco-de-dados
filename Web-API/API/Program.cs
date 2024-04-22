
using API.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
// registrar o serviço de banco de dados
builder.Services.AddDbContext<AppDataContext>();
var app = builder.Build();
var dbContext = new AppDataContext();

List<Produto> produtos =
[
    new Produto("Arroz", "pacote de arroz 2kg", 20.50),
    new Produto("feijao", "pacote de feijao 1kg", 13.20),
    new Produto("Batata", "saco de 1kg de batata", 15.00),
    new Produto("Frango", "pacote de 500g de frango", 18.85),
];

app.MapGet("/", () => "API de produtos");

// listar
app.MapGet("/produto/listar", ([FromServices] AppDataContext contexto) =>
    {
        if (contexto.Produtos.Any())
        {
            return Results.Ok(contexto.Produtos.ToList());
        }

        return Results.NotFound("Não existem produtos na tabela!");
    });

// buscar
app.MapGet("/produto/buscar/{nome}", ([FromRoute] string nome) =>
    {
        for (int i = 0; i < produtos.Count; i++)
        {
            if (produtos[i].Nome == nome)
            {
                // retornar o produto encontrado 
                return Results.Ok(produtos[i]);
            }

        }
        // caso nao encontre o produto
        return Results.NotFound("Produto nao encontrado");
    }
);

// adiciona um produto no banco de dados
app.MapPost("/produto/cadastrar", ([FromBody] Produto novoProduto,
    [FromServices] AppDataContext contexto) =>
    {
        if (novoProduto.Nome is null || novoProduto.Descricao is null)
        {
            return Results.BadRequest("campos invalidos.");
        }

        // adicionar objeto no banco de dados
        contexto.Produtos.Add(novoProduto);
        contexto.SaveChanges();

        return Results.Created("Produto adicionado com sucesso! ", novoProduto);
    });

// alterar produto da lista
app.MapPut("/produto/atualizar/{Nome}", ([FromRoute] string nome, [FromBody] Produto produtoAtualizado) =>
{

    Produto? produtoExistente = produtos.FirstOrDefault(p => p.Nome == nome);

    if (produtoExistente is null)
    {
        return Results.NotFound("Nome requisitado nao encontrado na lista de produtos");
    }

    produtoExistente.Nome = produtoAtualizado.Nome;
    produtoExistente.Descricao = produtoAtualizado.Descricao;
    produtoExistente.Valor = produtoAtualizado.Valor;

    return Results.Ok($"Produto {produtoExistente.Nome} alterado com sucesso!");
});

// deletar produto da lista
app.MapDelete("/produto/deletar/{nome}", (string nome,
    [FromServices] AppDataContext contexto) =>
    {
        // Busca o produto pelo nome no banco de dados
        var produtoParaDeletar = contexto.Produtos.FirstOrDefault(p => p.Nome == nome);

        // Se o produto não for encontrado, retorna um erro 404 (Not Found)
        if (produtoParaDeletar == null)
        {
            return Results.NotFound("Produto não encontrado.");
        }

        // Remove o produto do contexto e salva as mudanças no banco de dados
        contexto.Produtos.Remove(produtoParaDeletar);
        contexto.SaveChanges();

        return Results.Ok("Produto deletado com sucesso!");
    });

// alterar parcialmente um produto
app.MapPatch("/produto/patch/{Nome}/{patch}", ([FromRoute] string nome, [FromRoute] string patch, [FromBody] Produto produtoAtualizado) =>
{

    Produto? produtoExistente = produtos.FirstOrDefault(p => p.Nome == nome);

    if (produtoExistente is null)
    {
        return Results.BadRequest("campos invalidos.");
    }

    switch (patch)
    {
        case "nome":
            produtoExistente.Nome = produtoAtualizado.Nome;
            return Results.Ok($"Nome do produto atualizado para: {produtoExistente.Nome}");
        case "descricao":
            produtoExistente.Descricao = produtoAtualizado.Descricao;
            return Results.Ok($"Descrição do produto atualizada para: {produtoExistente.Descricao}");
        case "valor":
            produtoExistente.Valor = produtoAtualizado.Valor;
            return Results.Ok($"Valor do produto atualizado para: {produtoExistente.Valor}");
        default:
            return Results.BadRequest("Campo 'patch' inválido. Escolha entre 'nome', 'descricao' ou 'valor'.");
    }
});



app.Run();

