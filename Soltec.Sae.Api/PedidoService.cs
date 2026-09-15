using System.Data.OleDb;

namespace Soltec.Sae.Api
{
    public class PedidoService
    {
        public PedidoService(string connectionStringBase)
        {
            this.ConnectionStringBase = connectionStringBase;
        }
        public string ConnectionStringBase { get; set; } = "";

       public List<Pedido> List(DateTime fecha, DateTime fechaHasta, string IdCuenta = "")
{
    SujetoService sujetoService = new SujetoService(this.ConnectionStringBase);
    string connectionString = this.ConnectionStringBase + "sae.dbc";
    OleDbConnection cnn = new OleDbConnection(connectionString);
    cnn.Open();

    OleDbCommand command = cnn.CreateCommand();
    command.CommandText =
        "SELECT pedmae.sec , pedmae.orden AS orden, pedmae.tipo, pedmae.letra, pedmae.pe, pedmae.num, " +
        "pedmae.femi, pedmae.fvto, pedmae.scta, pedmae.rem, " +
        "pedmae.sub1, pedmae.dto, pedmae.pde, pedmae.sub2, pedmae.int, pedmae.per, " +
        "pedmae.iva1, pedmae.iva2, pedmae.iva3, pedmae.tot, pedmae.obs1, pedmae.obs2, " +
        "pedmae.EXP_VIA AS exp_tipo, " +
        "clipro.cod, clipro.nom, clipro.dir, clipro.alt, clipro.loc, " +
        "clipro.pos, clipro.provin, clipro.email, clipro.cuit, clipro.piva, " +
        "peddet.can, peddet.can_r, peddet.des, peddet.pun, peddet.bon, peddet.art, " +
        "peddet.tot AS totd " +
        "FROM pedmae " +
        "INNER JOIN peddet ON peddet.sec = pedmae.sec AND peddet.orden = pedmae.orden " +
        "INNER JOIN clipro ON clipro.cod = pedmae.scta " +
        "WHERE pedmae.femi BETWEEN ctod('" + fecha.ToString("MM-dd-yyyy") + "') " +
        "AND ctod('" + fechaHasta.ToString("MM-dd-yyyy") + "') " +
        "AND (pedmae.scta = '" + IdCuenta + "' or empty('" + IdCuenta + "')) " +
        "ORDER BY pedmae.femi, pedmae.tipo, pedmae.letra, pedmae.pe, pedmae.num";

    // Descomentar para depurar el SQL generado:
    // Console.WriteLine(command.CommandText);

    OleDbDataReader reader = command.ExecuteReader();
    List<Pedido> result = new List<Pedido>();
    string idAnt = "";
    Pedido item = new Pedido();

    while (reader.Read())
    {
        string id = reader["sec"].ToString().Trim() + reader["orden"].ToString().Trim();

        if (idAnt != id && idAnt != "")
        {
            result.Add(item);
            item = new Pedido();
        }

        if (idAnt != id)
        {
            item = Parse(reader);
        }

        item.Detalle.Add(ParseDetalle(reader));
        idAnt = id;
    }

    if (item.Numero != 0) result.Add(item);

    reader.Close();
    cnn.Close();
    return result;
}        private Pedido Parse(OleDbDataReader reader)
        {
            Pedido item = new Pedido();
            item.Sec = reader["sec"].ToString().Trim();
            item.Orden = reader["orden"].ToString().Trim();
            item.Tipo = Convert.ToInt16(reader["tipo"]);
            item.Letra = reader["letra"].ToString().Trim();
            item.TipoComp = reader["tipo"].ToString();
            item.FechaPase = (DateTime)reader["femi"];
            item.FechaComprobante = (DateTime)reader["femi"];
            item.FechaVencimiento = (DateTime)reader["fvto"];
            if (item.Tipo == 1)
            {
                item.Comprobante = "PEDIDO";
            }
            else if (item.Tipo == 2)
            {
                item.Comprobante = "PEDIDO DEVOLUCION";
            }

            item.Pe = reader["pe"].ToString().Trim() == "" ? 0 : Convert.ToInt16(reader["pe"]);
            item.Numero = reader["num"].ToString().Trim() == "" ? 0 : Convert.ToInt32(reader["num"]);
            item.IdCuenta = reader["scta"].ToString().Trim();

            item.PrecepcionIva = (decimal)reader["per"];
            //item.PrecepcionIB = (decimal)reader["ibru"];
            try
            {
                item.SubTotal = (decimal)reader["sub1"];
            }
            catch
            {
            }

            //item.Descuento = (decimal)reader["sub1"];
            item.Obs = reader["obs1"].ToString().Trim() + reader["obs2"].ToString().Trim();
            try
            {
                item.Total = (decimal)reader["tot"];
            }
            catch
            {
            }
            Sujeto tmpSujeto = new Sujeto();
            tmpSujeto.Id = reader["cod"].ToString().Trim();
            tmpSujeto.Nombre = reader["nom"].ToString().Trim();
            tmpSujeto.NumeroDocumento = reader["cuit"].ToString().Trim();
            tmpSujeto.Domicilio = reader["dir"].ToString().Trim();
            tmpSujeto.CodigoPostal = reader["pos"].ToString().Trim();
            tmpSujeto.Provincia = reader["provin"].ToString().Trim();
            tmpSujeto.Localidad = reader["loc"].ToString().Trim();
            item.Cuenta = tmpSujeto;
            return item;
        }
        private DetallePedido ParseDetalle(OleDbDataReader reader)
        {
            DetallePedido item = new DetallePedido();
            item.Cantidad = (decimal)reader["can"];
            item.CantidadPendiente = (decimal)reader["can_r"];
            item.Estado = reader["exp_tipo"].ToString().Trim();
            item.Concepto = reader["des"].ToString().Trim();
            try
            {
                item.Precio = (decimal)(reader["pun"]);
            }
            catch { }
            item.Descuento = (decimal)reader["bon"];
            item.IdArticulo = reader["art"].ToString();
            try
            {
                item.SubTotal = (decimal)reader["totd"];
            }
            catch { }

            return item;
        }
    }

}