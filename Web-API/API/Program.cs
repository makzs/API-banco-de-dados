
using System.ComponentModel.DataAnnotations;
using API.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
// registrar o serviço de banco de dados

// configurar a politica de CORS para liberar o acesso total
builder.Services.AddCors(
    options => options.AddPolicy("Acesso Total", configs => configs.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod())
);

builder.Services.AddDbContext<AppDataContext>();
var app = builder.Build();
var dbContext = new AppDataContext();

app.MapGet("/", () => "API de produtos");

// listar
app.MapGet("/produto/listar", ([FromServices] AppDataContext contexto) =>
{
    if (contexto.Produtos.Any())
    {
        // para lista de tabelas de banco de dados utilizamos ToList
        return Results.Ok(contexto.Produtos.ToList());
    }

    return Results.NotFound("Não existem produtos na tabela!");
});

/* buscar por nome
app.MapGet("/produto/buscar/{nome}", ([FromRoute] string nome, [FromServices] AppDataContext contexto) =>
    {
        Produto? produtoExistente = contexto.Produtos.FirstOrDefault(p => p.Nome == nome);

        if (produtoExistente is null)
        {
            return Results.NotFound("Nome requisitado nao encontrado na lista de produtos");
        }

        // caso nao encontre o produto
        return Results.Ok(produtoExistente);
    }
);
*/

// buscar por ID
app.MapGet("/produto/buscar/{id}", ([FromRoute] string id, [FromServices] AppDataContext contexto) =>
{
    // para buscar um item pela chave primaria usamos find
    Produto? produtoExistente = contexto.Produtos.Find(id);

    if (produtoExistente is null)
    {
        return Results.NotFound("Id requisitado nao encontrado na lista de produtos");
    }

    return Results.Ok(produtoExistente);
});

/* adiciona um produto no banco de dados
app.MapPost("/produto/cadastrar", ([FromBody] Produto novoProduto,
    [FromServices] AppDataContext contexto) =>
{

    if (novoProduto.Nome is null || novoProduto.Descricao is null)
    {
        return Results.BadRequest("campos invalidos.");
    }
    Produto? produtoEncontrado = contexto.Produtos.FirstOrDefault(x => x.Nome == novoProduto.Nome);

    // adicionar objeto no banco de dados
    contexto.Produtos.Add(novoProduto);
    contexto.SaveChanges();

    return Results.Created("Produto adicionado com sucesso! ", novoProduto);
});
*/

// adiciona um produto no banco de dados com regra de negocio
app.MapPost("/produto/cadastrar", ([FromBody] Produto novoProduto,
    [FromServices] AppDataContext contexto) =>
{
    // valida com a lista de erros que esta na classe Produto
    List<ValidationResult> erros = new List<ValidationResult>();
    if (!Validator.TryValidateObject(novoProduto, new ValidationContext(novoProduto), erros, true))
    {
        return Results.BadRequest(erros);
    }

    Produto? produtoEncontrado = contexto.Produtos.FirstOrDefault(x => x.Nome == novoProduto.Nome);

    if (produtoEncontrado is null)
    {
        // adicionar objeto no banco de dados
        contexto.Produtos.Add(novoProduto);
        contexto.SaveChanges();
        return Results.Created("", novoProduto);
    }

    return Results.BadRequest("Ja existe um produto com mesmo nome");

});

/* deletar produto do banco de dados por nome
app.MapDelete("/produto/deletar/{nome}", (string nome,
    [FromServices] AppDataContext contexto) =>
    {
        Produto? produtoExistente = contexto.Produtos.FirstOrDefault(p => p.Nome == nome);

        if (produtoExistente == null)
        {
            return Results.NotFound("Produto não encontrado.");
        }

        contexto.Produtos.Remove(produtoExistente);
        contexto.SaveChanges();

        return Results.Ok("Produto deletado com sucesso!");
    });
*/

// deletar produto do banco de dados por ID
app.MapDelete("/produto/deletar/{id}", (string id,
    [FromServices] AppDataContext contexto) =>
{
    Produto? produtoExistente = contexto.Produtos.Find(id);

    if (produtoExistente is null)
    {
        return Results.NotFound("Id requisitado nao encontrado na lista de produtos");
    }

    contexto.Produtos.Remove(produtoExistente);
    contexto.SaveChanges();

    return Results.Ok("Produto deletado com sucesso!");
});

/* alterar produto do banco de dados pelo nome
app.MapPut("/produto/atualizar/{Nome}", ([FromRoute] string nome, [FromBody] Produto produtoAtualizado, [FromServices] AppDataContext contexto) =>
{

    Produto? produtoExistente = contexto.Produtos.FirstOrDefault(p => p.Nome == nome);

    if (produtoExistente is null)
    {
        return Results.NotFound("Nome requisitado nao encontrado na lista de produtos");
    }

    produtoExistente.Nome = produtoAtualizado.Nome;
    produtoExistente.Descricao = produtoAtualizado.Descricao;
    produtoExistente.Valor = produtoAtualizado.Valor;

    contexto.SaveChanges();
    return Results.Ok($"Produto {produtoExistente.Nome} alterado com sucesso!");
});
*/

// alterar produto do banco de dados pelo id
app.MapPut("/produto/atualizar/{id}", ([FromRoute] string id, [FromBody] Produto produtoAtualizado, [FromServices] AppDataContext contexto) =>
{
    Produto? produtoExistente = contexto.Produtos.Find(id);

    if (produtoExistente is null)
    {
        return Results.NotFound("Id requisitado nao encontrado na lista de produtos");
    }

    produtoExistente.Nome = produtoAtualizado.Nome;
    produtoExistente.Descricao = produtoAtualizado.Descricao;
    produtoExistente.Valor = produtoAtualizado.Valor;

    contexto.Produtos.Update(produtoExistente);
    contexto.SaveChanges();
    return Results.Ok($"Produto alterado com sucesso!");
});

// alterar parcialmente um produto
app.MapPatch("/produto/patch/{Nome}/{patch}", ([FromRoute] string nome, [FromRoute] string patch, [FromBody] Produto produtoAtualizado, [FromServices] AppDataContext contexto) =>
{

    Produto? produtoExistente = contexto.Produtos.FirstOrDefault(p => p.Nome == nome);

    if (produtoExistente is null)
    {
        return Results.BadRequest("campos invalidos.");
    }

    switch (patch)
    {
        case "nome":
            produtoExistente.Nome = produtoAtualizado.Nome;
            contexto.SaveChanges();
            return Results.Ok($"Nome do produto atualizado para: {produtoExistente.Nome}");
        case "descricao":
            produtoExistente.Descricao = produtoAtualizado.Descricao;
            contexto.SaveChanges();
            return Results.Ok($"Descrição do produto atualizada para: {produtoExistente.Descricao}");
        case "valor":
            produtoExistente.Valor = produtoAtualizado.Valor;
            contexto.SaveChanges();
            return Results.Ok($"Valor do produto atualizado para: {produtoExistente.Valor}");
        default:
            return Results.BadRequest("Campo 'patch' inválido. Escolha entre 'nome', 'descricao' ou 'valor'.");

    }
});


app.UseCors("Acesso Total");
app.Run();

