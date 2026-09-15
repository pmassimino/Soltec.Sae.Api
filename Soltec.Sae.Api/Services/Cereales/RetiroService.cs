using System.Data.OleDb;

namespace Soltec.Sae.Api
{
    public class RetiroService
    {
        public RetiroService(string connectionStringBase) 
        {
            this.ConnectionStringBase = connectionStringBase;
        }
        public string ConnectionStringBase { get; set; } = "";
        public string IdSucursal { get; set; } = "01";
       
        public List<Retiro> List(string idCuenta , string idCosecha ,DateTime fecha) 
        {
            string connectionString = this.ConnectionStringBase + "Cereales.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT  re_nro, re_fecha, re_produ, re_cerea, re_tipo, re_cosec, id_clasif, re_proce, re_comp, re_pes_bru, re_tara, re_pes_net, re_obser, re_trans, re_c_res," +
                         "re_n_cre, re_tr_nom, re_desti, re_cuit_d, re_borra, re_tip, re_cierre, re_ftrans, re_envase, re_ayuda, re_reg, re_ctto, cal_alm, re_horaini, " +
                         "re_horafin, re_tarfle, id_carta_p, planta, id_destino, re_desint, venta_dir,Cosechas.descri as NombreCosecha ," +
                         "produmae.rsocial as NombreProductor,produmae.n_cuit as CuitProductor " + 
                         "FROM retiro " +
                         "LEFT JOIN Produmae on produmae.codigo = retiro.re_produ " +
                         "LEFT JOIN Cosechas on cosechas.cod = retiro.re_cosec " +
                         "WHERE(re_produ = '" + idCuenta + "' OR empty('" + idCuenta + "')) AND (re_cosec = '" + idCosecha + "' OR empty('" + idCosecha + "'))  AND(re_tip <> 1) " +
                         "and re_fecha <= ctod('" + fecha.ToString("MM-dd-yyy") + "')";
           OleDbDataReader reader = command.ExecuteReader();
            List<Retiro> result = new List<Retiro>();
            while (reader.Read())
            {
                result.Add(this.Parse(reader));
            }            
            cnn.Close();
            return result;
        }
        public Retiro FindOne(string id) 
        {
            string connectionString = this.ConnectionStringBase + "Cereales.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT  re_nro, re_fecha, re_produ, re_cerea, re_tipo, re_cosec, id_clasif, re_proce, re_comp, re_pes_bru, re_tara, re_pes_net, re_obser, re_trans, re_c_res," +
                         "re_n_cre, re_tr_nom, re_desti, re_cuit_d, re_borra, re_tip, re_cierre, re_ftrans, re_envase, re_ayuda, re_reg, re_ctto, cal_alm, re_horaini, " +
                         "re_horafin, re_tarfle, id_carta_p, planta, id_destino, re_desint, venta_dir,Cosechas.descri as NombreCosecha ," +
                         "produmae.rsocial as NombreProductor,produmae.n_cuit as CuitProductor " +
                         "FROM retiro " +
                         "LEFT JOIN Produmae on produmae.codigo = retiro.re_produ " +
                         "LEFT JOIN Cosechas on cosechas.cod = retiro.re_cosec " +
                         "WHERE(re_nro = '" + id + "')";
            OleDbDataReader reader = command.ExecuteReader();
            Retiro result = null;
            while (reader.Read())
            {
                result = this.Parse(reader);
            }
            cnn.Close();
            return result;
        }
        public Int64 Total(string idCuenta, string idCosecha, DateTime fechaHasta)
        {
            Int64 result = 0;
            string connectionString = this.ConnectionStringBase + "Cereales.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT sum(re_pes_net) as Total " +
                                  "FROM Retiro " +
                                  "WHERE (re_produ = '" + idCuenta + "') and " +
                                  " (re_cosec = '" + idCosecha + "' and re_fecha <=ctod('" + fechaHasta.ToString("MM-dd-yyy") + "')) and " +
                                  "(re_tip <> 1)";
            OleDbDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                result = reader["Total"].ToString() == "" ? 0 : Convert.ToInt64(reader["Total"].ToString());
            }
            cnn.Close();
            return result;
        }

        public Int64 TotalPlanta(string idPlanta, string idCosecha,DateTime fecha, DateTime fechaHasta)
        {
            Int64 result = 0;
            string connectionString = this.ConnectionStringBase + "Cereales.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT sum(re_pes_net) as Total " +
                                  "FROM Retiro " +
                                  "WHERE (planta = '" + idPlanta + "') and " +
                                  " (re_cosec = '" + idCosecha + "') and (re_fecha BETWEEN ctod('" + fecha.ToString("MM-dd-yyy") + "')"
                                  + " AND ctod('" + fechaHasta.ToString("MM-dd-yyy") + "'))  and " +
                                  "(noincstock = .f.)";
            OleDbDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                result = reader["Total"].ToString() == "" ? 0 : Convert.ToInt64(reader["Total"].ToString());
            }
            cnn.Close();
            return result;
        }


        private Retiro Parse(OleDbDataReader reader)
        {
            Retiro item = new Retiro();
            item.Id = reader["re_nro"].ToString().Trim();
            item.IdSucursal = this.IdSucursal;
            item.IdTransaccion = "RETIRO;" + this.IdSucursal + ";" + item.Id; 
            item.Fecha = (DateTime)reader["re_fecha"];
            item.IdCosecha = reader["re_cosec"].ToString().Trim();
            Cosecha cosecha = new Cosecha();
            cosecha.Id = reader["re_cosec"].ToString().Trim();
            cosecha.Nombre = reader["NombreCosecha"].ToString().Trim();
            item.Cosecha = cosecha;
            item.IdCuenta = reader["re_produ"].ToString().Trim();
            Sujeto cuenta = new Sujeto();
            cuenta.Id = reader["re_produ"].ToString().Trim();
            cuenta.Nombre = reader["NombreProductor"].ToString().Trim();
            cuenta.NumeroDocumento = reader["CuitProductor"].ToString().Trim();
            item.Cuenta = cuenta;            
            item.PesoNeto = Convert.ToInt64(reader["re_pes_net"].ToString().Trim());            
            item.Numero = reader["re_comp"].ToString().Trim();
            item.NumeroCPorte = reader["re_n_cre"].ToString().Trim();
            return item;
        }
    }
}
