namespace Soltec.Sae.Api
{
    public class Empresa
    {
        public string Nombre { get; set; }
        public string Cuit { get; set; }
        public string NumeroIB { get; set; }
        public string Direccion { get; set; }
        public string Localidad { get; set; }
        public string Cpostal { get; set; }
        public string Provincia { get; set; }
        public string CondIva { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string FechaIniAct { get; set; }
    }
    public class Sucursal
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string ConnectionStrings { get; set; }
    }
    public class EntityGeneric
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
    }
}
