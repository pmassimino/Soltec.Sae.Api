namespace Soltec.Sae.Api
{
    using Microsoft.EntityFrameworkCore;
 

    public class DatabaseContext : DbContext
    {
        IConfiguration configuration;
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<DetalleFactura> DetalleFacturas { get; set; }
        public DatabaseContext() 
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            this.configuration = configurationBuilder.Build();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connection = this.configuration["SqlConnectionString"];
            optionsBuilder.UseSqlServer(connection);
        }
    }
}
