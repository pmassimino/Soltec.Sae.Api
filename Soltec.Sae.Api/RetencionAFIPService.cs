using System.Data.OleDb;

namespace Soltec.Sae.Api
{
    public class RetencionAFIPService
    {         
        public RetencionAFIPService(string connectionStringBase) 
        {
            this.ConnectionStringBase = connectionStringBase;
        }
        public string ConnectionStringBase { get; set; } = "";
        private string fields = "tipo,ntra,num,pe,nu1,nu2,nu3,cod,cuit,nom,dir,loc,pos,pro,dgr,fec,fe1,re1,bi1,im1,iml,con,ref,regim ";
        public List<RetencionBase> List(string idCuenta,DateTime fecha, DateTime fechaHasta)
        {            
            string connectionString = this.ConnectionStringBase + "sae.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT " + fields +
                "FROM retgan " +                
                "WHERE  (fec BETWEEN ctod('" + fecha.ToString("MM-dd-yyy") + "')"
                + " AND ctod('" + fechaHasta.ToString("MM-dd-yyy") + "')) " 
                + " and (cod = '" + idCuenta + "' or empty('" + idCuenta + "'))"                  
                + " order by fec,cod";
            OleDbDataReader reader = command.ExecuteReader();
            List<RetencionBase> result = new List<RetencionBase>();           
            while (reader.Read())
            {                
               result.Add(Parse(reader));                
            }            
            reader.Close();
            cnn.Close();
            return result;
        }

        public RetencionBase FindOne(string id) 
        {
            string connectionString = this.ConnectionStringBase + "sae.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT " + fields +
                "FROM retgan " +
                "WHERE nu3='" + id + "'";
            
            OleDbDataReader reader = command.ExecuteReader();
            RetencionBase result = null;
            while (reader.Read())
            {
                result = Parse(reader);
            }
            reader.Close();
            cnn.Close();
            return result;
        }
        private RetencionBase Parse(OleDbDataReader reader)
        {
            RetencionBase item = new RetencionBase();
            item.Sec = "";
            item.Orden = "";
            item.Id = reader["nu3"].ToString().Trim();
            string pidTipo = reader["tipo"].ToString().Trim();
            item.Tipo = "GANANCIAS";
            item.Impuesto = "Impuesto a las Ganancias";
            item.Regimen = reader["ref"].ToString().Trim();
            if (string.IsNullOrEmpty(item.Regimen))
                //item.Regimen = "Compraventa de Cosas Muebles y Locaciones / Servicios RG 2854 AFIP";
            if (pidTipo == "02") 
            {
                item.Tipo = "IVA";
                item.Impuesto = "Impuesto al valor agregado";
            }                                
            item.FechaPase = (DateTime)reader["fec"];
            item.FechaComprobante = (DateTime)reader["fec"];
            item.FechaVencimiento = (DateTime)reader["fec"];
            item.Pe = reader["pe"].ToString().Trim() == "" ? 0 : Convert.ToInt16(reader["pe"]);
            item.Numero = reader["nu3"].ToString().Trim();
            item.NumeroComprobante = reader["con"].ToString().Trim();
            //Sujeto
            item.IdCuenta = reader["cod"].ToString().Trim();            
          
            Sujeto tmpSujeto = new Sujeto();
            tmpSujeto.Id = reader["cod"].ToString().Trim();
            tmpSujeto.Nombre = reader["nom"].ToString().Trim();            
            tmpSujeto.NumeroDocumento = reader["cuit"].ToString().Trim();
            tmpSujeto.Domicilio = reader["dir"].ToString().Trim();
            tmpSujeto.CodigoPostal = reader["pos"].ToString().Trim();
            tmpSujeto.Provincia = reader["pro"].ToString().Trim();
            tmpSujeto.Localidad = reader["loc"].ToString().Trim();
            
            item.Cuenta = tmpSujeto;
            //Importes
            item.BaseImponible = (decimal)reader["bi1"];
            item.Alicuota = (decimal)reader["re1"];
            item.Importe = (decimal)reader["im1"];
            item.Obs = reader["ref"].ToString().Trim();

            return item;
        }
      
    }
}
