using System.Data.OleDb;

namespace Soltec.Sae.Api
{
    public class ReciboCtaCteService
    {         
        public ReciboCtaCteService(string connectionStringBase) 
        {
            this.ConnectionStringBase = connectionStringBase;
        }
        public string ConnectionStringBase { get; set; } = "";
        
       
      
        public List<ReciboCtaCte> List(DateTime fecha, DateTime fechaHasta)
        {
            SujetoService sujetoService = new SujetoService(this.ConnectionStringBase);
            string connectionString = this.ConnectionStringBase + "sae.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT ceop,suc,pe,num,may,scta,edi,plan,femi,ntra,obn,morig,impd,cotiz,imp_fc,imp_ac,imp_dv, " +
                "clipro.cod,clipro.nom,clipro.dir,clipro.alt,clipro.loc,clipro.pos,clipro.provin,clipro.email,clipro.cuit,clipro.piva " +                
                "FROM recma " +                
                "inner join clipro on clipro.cod = recma.scta WHERE  (femi BETWEEN ctod('" + fecha.ToString("MM-dd-yyy") + "')"
                + " AND ctod('" + fechaHasta.ToString("MM-dd-yyy") + "')) order by femi,pe,num";
            OleDbDataReader reader = command.ExecuteReader();
            List<ReciboCtaCte> result = new List<ReciboCtaCte>();            
            ReciboCtaCte item = new ReciboCtaCte();
            while (reader.Read())
            {
              
              item = Parse(reader);
                result.Add(item);
            }
            //Detalle De Valores
            command.CommandText = "SELECT pe,num,cfon,cmay,ctanom,scta,fec,bano,pla,che,orden,imp,cotiz " +
                                  "from recdva order by pe,num ";
            reader.Close();
            reader = command.ExecuteReader();
            List<DetalleValores> tmpDetalleValores = new List<DetalleValores>();
            while (reader.Read())
            {
                tmpDetalleValores.Add(ParseDetalleValores(reader));                
            }
            foreach (var tmpRecibo in result) 
            {
                tmpRecibo.DetalleValores = (from d in tmpDetalleValores where d.Pe == tmpRecibo.Pe && d.Numero == tmpRecibo.Numero select d).ToList();
            }
            //Detalle de comprobantes
            command.CommandText = "SELECT org,suc,usu,ntra,pe,num,c_pe,c_num,ref,fec,fvto,ccpte,con,tip,imp " +
                                  "from recdfa order by pe,num ";
            reader.Close();
            reader = command.ExecuteReader();
            List<DetalleComprobante> tmpDetalleComprobante = new List<DetalleComprobante>();
            while (reader.Read())
            {
                tmpDetalleComprobante.Add(ParseDetalleComprobante(reader));
            }
            foreach (var tmpRecibo in result)
            {
                tmpRecibo.DetalleComprobante = (from d in tmpDetalleComprobante where d.Pe == tmpRecibo.Pe && d.Numero == tmpRecibo.Numero select d).ToList();
            }

            reader.Close();
            cnn.Close();
            return result;
        }      
        private ReciboCtaCte Parse(OleDbDataReader reader)
        {
            ReciboCtaCte item = new ReciboCtaCte();
            item.Sec = reader["ceop"].ToString().Trim();
            //item.Id = reader["orden"].ToString().Trim();
            item.IdTipo = "1";            
            item.Fecha = (DateTime)reader["femi"];
            item.FechaVencimiento = item.Fecha;
            item.Pe = reader["pe"].ToString().Trim();
            item.Numero = reader["num"].ToString().Trim();
            item.Id = item.Numero;
            item.IdCuentaMayor = reader["may"].ToString().Trim();
            item.IdCuenta = reader["scta"].ToString().Trim();
            item.Importe = (decimal)reader["imp_dv"];
            item.Cotizacion = (decimal)reader["cotiz"];
            item.IdDivisa = reader["morig"].ToString().Trim() == "D" || reader["morig"].ToString().Trim() == "1" ? 1 : 0;
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
        private DetalleValores ParseDetalleValores(OleDbDataReader reader)
        {
            DetalleValores item = new DetalleValores();
            item.IdTipo = "1";
            item.Fecha = (DateTime)reader["fec"];
            item.FechaVencimiento = item.Fecha;
            item.Pe = reader["pe"].ToString().Trim();
            item.Numero = reader["num"].ToString().Trim();
            item.NumeroComprobante = reader["che"].ToString().Trim();
            item.IdCuentaMayor = reader["cmay"].ToString().Trim();
            item.Importe = (decimal)reader["imp"];
            item.Concepto = reader["ctanom"].ToString().Trim();
            item.Banco = reader["bano"].ToString().Trim();           
            return item;
        }
        private DetalleComprobante ParseDetalleComprobante(OleDbDataReader reader)
        {
            DetalleComprobante item = new DetalleComprobante();
            item.IdTipo = reader["tip"].ToString();
            item.Fecha = (DateTime)reader["fec"];                
            item.Pe = reader["pe"].ToString().Trim();
            item.Numero = reader["num"].ToString().Trim();
            item.PeComprobante = Convert.ToInt16(reader["c_pe"]);
            item.NumeroComprobante = Convert.ToInt64(reader["c_num"]);     
            item.IdComprobante = reader["ntra"].ToString().Trim();
            item.Importe = (decimal)reader["imp"];
            item.Concepto = reader["con"].ToString().Trim();            
            return item;
        }

    }
}
