using System.Data.OleDb;

namespace Soltec.Sae.Api
{
    public class MayorService
    {         
        public MayorService(string connectionStringBase) 
        {
            this.ConnectionStringBase = connectionStringBase;
        }
        public string ConnectionStringBase { get; set; } = "";
        
       
      
        public List<Mayor> List(DateTime fecha, DateTime fechaHasta)
        {
            SujetoService sujetoService = new SujetoService(this.ConnectionStringBase);
            string connectionString = this.ConnectionStringBase + "sae.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT suc,ntra,item,per,fpas,fvto,pvta,fcom,ccpte,pvta,ncpte,cmay,scta,con,tramay.tip,imp,can,div,cotiz,obn,ncom2,clipro.nom as NombreSujeto,ctasmg.nom as NombreCuentaMayor " + 
                "FROM tramay " + 
                "LEFT JOIN clipro ON clipro.cod = tramay.scta " + 
                "left join ctasmg on ctasmg.cod = tramay.cmay " + 
                "WHERE  (fpas BETWEEN ctod('" + fecha.ToString("MM-dd-yyy") + "')"
                + " AND ctod('" + fechaHasta.ToString("MM-dd-yyy") + "')) " +
                "order by fpas,suc,ntra,tramay.tip";
            OleDbDataReader reader = command.ExecuteReader();
            List<Mayor> result = new List<Mayor>();
            string idAnt = "";
            Mayor item = new Mayor();
            int i = 0;
            while (reader.Read())
            {
                string id = reader["suc"].ToString().Trim() + reader["ntra"].ToString().Trim();
                if (idAnt != id && idAnt != "")
                {
                    result.Add(item);
                    item = new Mayor();
                    i = 0;
                }
                if (idAnt != id)
                {
                    item = Parse(reader);
                }
                i++;
                var detalle = ParseDetalle(reader);
                detalle.Item = i;
                item.Detalle.Add(detalle);
                idAnt = id;
            }
            if (item.Numero != 0) result.Add(item);
            reader.Close();
            cnn.Close();
            return result;
        }
        public List<Diario> ListDiario(DateTime fecha, DateTime fechaHasta) 
        {
            List<Diario> result = new List<Diario>();
            var tmpresult = this.List(fecha, fechaHasta);
            foreach (var item in tmpresult) 
            {
                foreach (var detalle in item.Detalle) 
                {
                    Diario itemDiario = new Diario();
                    itemDiario.Fecha = item.Fecha;
                    itemDiario.FechaComprobante = item.FechaComprobante;
                    itemDiario.FechaVencimiento = detalle.FechaVenc;
                    itemDiario.IdSeccion = detalle.IdSeccion;
                    itemDiario.IdSucursal = detalle.IdSucursal;
                    itemDiario.Concepto = detalle.Concepto;
                    itemDiario.IdTransaccion = detalle.IdTransaccion;
                    itemDiario.Numero = item.Numero;
                    itemDiario.Pe = item.Pe;
                    itemDiario.Origen = item.Origen;
                    itemDiario.Debe = detalle.Debe;
                    itemDiario.Haber = detalle.Haber;
                    itemDiario.IdTipo = detalle.IdTipo;
                    itemDiario.IdCuentaMayor = detalle.IdCuentaMayor;
                    itemDiario.IdCuenta = detalle.IdCuenta;
                    itemDiario.NombreSujeto = detalle.NombreSujeto;
                    itemDiario.Item = detalle.Item;
                    itemDiario.IdComprobante = item.IdComprobante;
                    itemDiario.NombreCuentaMayor = detalle.NombreCuentaMayor;
                    result.Add(itemDiario);
                }
            }
            return result;
        }

        private Mayor Parse(OleDbDataReader reader)
        {
            Mayor item = new Mayor();
            item.IdSeccion = "";
            item.IdSucursal = reader["suc"].ToString().Trim();
            item.IdTransaccion = reader["ntra"].ToString().Trim();            
            item.Fecha = (DateTime)reader["fpas"];
            item.FechaComprobante = (DateTime)reader["fcom"];
            item.FechaVencimiento = (DateTime)reader["fvto"];            
            item.Pe = reader["pvta"].ToString().Trim() == "" ? 0 : Convert.ToInt16(reader["pvta"]);
            item.Numero = reader["ncpte"].ToString().Trim() == "" ? 0 : Convert.ToInt32(reader["ncpte"]);
            item.Concepto = reader["con"].ToString().Trim();
            item.IdComprobante = reader["ccpte"].ToString().Trim();

            return item;
        }
        private DetalleMayor ParseDetalle(OleDbDataReader reader)
        {

            DetalleMayor item = new DetalleMayor();
            item.IdSucursal = reader["suc"].ToString().Trim();
            item.IdTransaccion = reader["ntra"].ToString().Trim();
            item.Cantidad = (decimal)reader["can"];
            item.Concepto = reader["con"].ToString().Trim();
            decimal importe = (decimal)(reader["imp"]);
            string tipo = reader["tip"].ToString();
            item.IdTipo = tipo;
            if (tipo == "1")
            {
                item.Debe = importe;
            }
            else
            {
                item.Haber = importe;
            }
            item.IdCuentaMayor = reader["cmay"].ToString().Trim();
            item.NombreCuentaMayor = reader["NombreCuentaMayor"].ToString().Trim();
            //item.Item = Convert.ToInt32(reader["item"].ToString());
            //sujeto
            Sujeto tmpSujeto = new Sujeto();
            tmpSujeto.Id = reader["scta"].ToString().Trim();
            tmpSujeto.Nombre = reader["NombreSujeto"].ToString().Trim();
            if (tmpSujeto.Id != null) { 
               item.NombreSujeto = tmpSujeto.Nombre; 
               item.IdCuenta = tmpSujeto.Id; }
            return item;
        }
      
    }
}
