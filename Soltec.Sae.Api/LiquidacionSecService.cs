using System.Data.OleDb;

namespace Soltec.Sae.Api
{
    public class LiquidacionSecService
    {
        public LiquidacionSecService(string connectionStringBase)
        {
            this.ConnectionStringBase = connectionStringBase;
        }
        public string ConnectionStringBase { get; set; } = "";
        public string IdSucursal { get; set; } = "01";

        public List<LiquidacionSec> List(string idCosecha,DateTime fecha)
        {
            string connectionString = this.ConnectionStringBase + "Cereales.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT ctto,nliq,fec,kgs,porc,imp,definitiva,iva21,retiva,retibrut," +
                                  "rg991,comision,sellado,flete,honorarios,ivacom,otros,contrato.ctocose, " +
                                  "Cosechas.descri as NombreCosecha," +
                                  "cermae.cod_cer as idCereal,cermae.descri as NombreCereal " +
                                  "FROM cttoliq " +
                                  "LEFT JOIN contrato on contrato.ctocod = cttoliq.ctto " +
                                  "LEFT JOIN Cosechas on cosechas.cod = contrato.ctocose " +
                                  "LEFT JOIN Cermae on cermae.cod_cer = cosechas.cereal " +
                                  "WHERE (contrato.ctocose = '" + idCosecha + "' OR empty('" + idCosecha + "')) and kgs > 0 ";
            OleDbDataReader reader = command.ExecuteReader();
            List<LiquidacionSec> result = new List<LiquidacionSec>();
            while (reader.Read())
            {
                result.Add(this.Parse(reader));
            }
            cnn.Close();
            return result;
        }        
        
        private LiquidacionSec Parse(OleDbDataReader reader)
        {
            LiquidacionSec item = new LiquidacionSec();
            item.IdSucursal = this.IdSucursal;
            item.Id = reader["nliq"].ToString().Trim();                        
            item.Numero = reader["nliq"].ToString().Trim();
            item.Fecha = (DateTime)reader["fec"];
            item.FechaVencimiento = (DateTime)reader["fec"];
            item.IdCosecha = reader["ctocose"].ToString().Trim();
            item.NombreCosecha = reader["NombreCosecha"].ToString().Trim();
            item.NombreCereal = reader["NombreCereal"].ToString().Trim();                     
            item.PesoNeto = Convert.ToInt64(reader["kgs"].ToString().Trim());
            decimal porc = (decimal)reader["porc"];
            
            item.ImporteIva = (decimal)reader["iva21"];
            
            
            decimal importeBruto = item.ImporteIva * 100 / (decimal)10.5;
            var precio = (importeBruto / item.PesoNeto)*100;
            item.Precio = Math.Round(precio, 2); 
            item.ImporteBruto = importeBruto;
            if (porc < 100)
                item.ImporteDeduccion = (100 - porc)*importeBruto/100;

            item.ImporteFlete = 0;
            item.RetGan = 0;
            item.RetIva = (decimal)reader["retiva"];
            try {
                item.RetIb = reader["retibrut"] as decimal? ?? item.RetIb;
            }
            catch { }
            
            item.ImporteComision = (decimal)reader["comision"];
            item.ImporteSellado = (decimal)reader["sellado"];
            item.ImporteFinal = importeBruto + item.ImporteIva - item.ImporteDeduccion;
            return item;
        }
        
    }
}

