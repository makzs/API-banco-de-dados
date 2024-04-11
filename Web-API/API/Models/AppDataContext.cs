using Microsoft.EntityFrameworkCore;

namespace API.Models;

// classe que representa o EntityFrameworkCore na aplicação : Code First
public class AppDataContext : DbContext
{
    // representação das classes que vao virar tabelas no banco de dados
    public DbSet<Produto> Produtos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // configuração da conexao com banco de dados
        optionsBuilder.UseSqlite("Data Source=app.db");
    }
}