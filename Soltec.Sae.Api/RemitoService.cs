using System.Data.OleDb;

namespace Soltec.Sae.Api
{
    public class RemitoService
    {         
        public RemitoService(string connectionStringBase) 
        {
            this.ConnectionStringBase = connectionStringBase;
        }
        public string ConnectionStringBase { get; set; } = "";
        
        public List<Remito> List(DateTime fecha, DateTime fechaHasta)
        {
            SujetoService sujetoService = new SujetoService(this.ConnectionStringBase);
            string connectionString = this.ConnectionStringBase + "sae.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT remmae.sec,remmae.orden,remmae.tipo,letra,pe,num,num_doc,femi,fvto,scta,remmae.rem, " +
                "sub1,dto,pde,sub2,remmae.int,per,iva1,iva2,iva3,remmae.tot,obs1,obs2,clipro.cod,clipro.nom,clipro.dir,clipro.alt,clipro.loc, " +
                "clipro.pos,clipro.provin,clipro.email,clipro.cuit,clipro.piva,can,can_r,exp_tipo,des,pun,bon,art,remdet.tot as total " +
                "FROM remmae " +
                "INNER JOIN remdet ON remdet.sec = remmae.sec AND remmae.orden = remdet.orden " +
                "INNER JOIN clipro on clipro.cod = remmae.scta " +
                " WHERE  (femi BETWEEN ctod('" + fecha.ToString("MM-dd-yyy") + "')"
                + " AND ctod('" + fechaHasta.ToString("MM-dd-yyy") + "')) order by femi,remmae.tipo,letra,pe,num";
            OleDbDataReader reader = command.ExecuteReader();
            List<Remito> result = new List<Remito>();
            string idAnt = "";
            Remito item = new Remito();
            while (reader.Read())
            {
                string id = reader["sec"].ToString().Trim() + reader["orden"].ToString().Trim();
                if (idAnt != id && idAnt != "")
                {
                    result.Add(item);
                    item = new Remito();
                }
                if (idAnt != id)
                {
                    item = Parse(reader);
                }                                
                item.Detalle.Add(ParseDetalle(reader));                
                idAnt = id;                                
            }
            reader.Close();
            cnn.Close();
            return result;
        }
        public Remito FindOne(string sec,string orden) 
        {
            string connectionString = this.ConnectionStringBase + "sae.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "select sec,orden,tipo,letra,pe,num,num_doc,femi,fvto,scta,rem,cla,tve,tep,tra,sub1,dto,pde,sub2,int,per,iva1,iva2,iva3,tot,div,estado,obs1,obs2, " +
                "clipro.cod,clipro.nom,clipro.dir,clipro.alt,clipro.loc,clipro.pos,clipro.provin,clipro.email,clipro.cuit,clipro.piva " +
                "FROM remmae inner join clipro on clipro.cod = remmae.scta " +
                "WHERE  sec = '" + sec + "' and orden = '" + orden + "'";
            OleDbDataReader reader = command.ExecuteReader();
            Remito result = null;
            while (reader.Read())
            {
                result = Parse(reader);                
            }
            reader.Close();
            command.CommandText = "Select can,can_r,exp_tipo,des,pun,bon,art,tot as totd from remdet WHERE sec = '" + sec + "' and orden = '" + orden + "'";
            reader = command.ExecuteReader();
            List<DetalleRemito> detalle = new List<DetalleRemito>();
            while (reader.Read())
            {
                 var item = ParseDetalle(reader);
                 detalle.Add(item);
            }
            result.Detalle = detalle;
            cnn.Close();
            return result;
        }
        public List<RemitoView> ListInforme(DateTime fecha, DateTime fechaHasta)
        {
            SujetoService sujetoService = new SujetoService(this.ConnectionStringBase);
            string connectionString = this.ConnectionStringBase + "sae.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT remmae.sec,remmae.orden,remmae.tipo,letra,pe,num,num_doc,femi,fvto,scta,remmae.rem, " +
                "sub1,dto,pde,sub2,remmae.int,per,iva1,iva2,iva3,remmae.tot,obs1,obs2,clipro.cod,clipro.nom,clipro.dir,clipro.alt,clipro.loc, " +
                "clipro.pos,clipro.provin,clipro.email,clipro.cuit,clipro.piva,can,can_r,exp_tipo,des,pun,bon,art,remdet.tot as total " +
                "FROM remmae " +
                "INNER JOIN remdet ON remdet.sec = remmae.sec AND remmae.orden = remdet.orden " +
                " inner join clipro on clipro.cod = remmae.scta " +
                " WHERE  (femi BETWEEN ctod('" + fecha.ToString("MM-dd-yyy") + "')"
                + " AND ctod('" + fechaHasta.ToString("MM-dd-yyy") + "')) order by femi,remmae.tipo,letra,pe,num";
            OleDbDataReader reader = command.ExecuteReader();
            List<RemitoView> result = new List<RemitoView>();

            while (reader.Read())
            {
                string id = reader["sec"].ToString().Trim();
                var item = ParseRemitoView(reader);
                result.Add(item);
            }
            reader.Close();
            cnn.Close();
            return result;
        }
        private Remito Parse(OleDbDataReader reader)
        {
            Remito item = new Remito();
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
                item.Comprobante = "REMITO";
            }
            else if (item.Tipo == 2)
            {
                item.Comprobante = "REMITO DEVOLUCION";
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
        private RemitoView ParseRemitoView(OleDbDataReader reader)
        {
            RemitoView item = new RemitoView();
            item.Sec = reader["sec"].ToString().Trim();
            item.Orden = reader["orden"].ToString().Trim();
            item.Tipo = Convert.ToInt16(reader["tipo"]);
            item.Letra = reader["letra"].ToString().Trim();
            item.FechaPase = (DateTime)reader["femi"];
            item.FechaComprobante = (DateTime)reader["femi"];
            item.FechaVencimiento = (DateTime)reader["fvto"];
            if (item.Tipo == 1)
            {
                item.Comprobante = "REMITO";
            }
            else if (item.Tipo == 2)
            {
                item.Comprobante = "REMITO DEVOLUCION";
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
            
            try
            {
                item.Total = (decimal)reader["tot"];
            }
            catch
            {
            }            
            item.NombreCuenta = reader["nom"].ToString().Trim();
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
                item.SubTotal = (decimal)reader["total"];
            }
            catch { }
            return item;
        }
        private DetalleRemito ParseDetalle(OleDbDataReader reader)
        {
            DetalleRemito item = new DetalleRemito();
            item.Cantidad = (decimal) reader["can"];
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
                item.SubTotal = (decimal)reader["tot"];
            }
            catch { }

            return item;
        }
    }
    public class RemitoView 
    {
        public string Sec { get; set; }
        public string Orden { get; set; }
        public DateTime FechaPase { get; set; }
        public DateTime FechaComprobante { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public int Tipo { get; set; }
        public string Comprobante { get; set; }
        public string Letra { get; set; }
        public decimal Pe { get; set; }
        public decimal Numero { get; set; }
        
        public string IdCuenta { get; set; }
        public string NombreCuenta { get; set; }
        public string Obs { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal IvaGeneral { get; set; }
        public decimal IvaOtro { get; set; }
        public decimal ImpuestoInterno { get; set; }
        public decimal PrecepcionIva { get; set; }
        public decimal Total { get; set; }
        public string IdArticulo { get; set; }
        public string Concepto { get; set; }
        public decimal Precio { get; set; }
        public decimal Cantidad { get; set; }
        public decimal CantidadPendiente { get; set; }
        public string Estado { get; set; }
        
    }

    //public List<Remito> List(DateTime fecha, DateTime fechaHasta, bool incluyeDetalle = false)
    //{
    //    SujetoService sujetoService = new SujetoService(this.ConnectionStringBase);
    //    string connectionString = this.ConnectionStringBase + "sae.dbc";
    //    OleDbConnection cnn = new OleDbConnection(connectionString);
   //     cnn.Open();
   //     OleDbCommand command = cnn.CreateCommand();
   //     command.CommandText = "SELECT sec,orden,tipo,letra,pe,num,num_doc,femi,fvto,scta,rem,cla,tve,tep,tra,sub1,dto,pde,sub2,int,per,iva1,iva2,iva3,tot,div,estado,obs1,obs2, " +
   //         "clipro.cod,clipro.nom,clipro.dir,clipro.alt,clipro.loc,clipro.pos,clipro.provin,clipro.email,clipro.cuit,clipro.piva " +
   //         "FROM remmae inner join clipro on clipro.cod = remmae.scta WHERE  (femi BETWEEN ctod('" + fecha.ToString("MM-dd-yyy") + "')"
   //         + " AND ctod('" + fechaHasta.ToString("MM-dd-yyy") + "')) order by femi,tipo,letra,pe,num";
   //     OleDbDataReader reader = command.ExecuteReader();
   //     List<Remito> result = new List<Remito>();
   //     while (reader.Read())
   //     {
   //         var item = Parse(reader);
   //         result.Add(item);
   //     }
   //     reader.Close();
   //     if (incluyeDetalle)
   //     {
    //        foreach (var item in result)
    //        {
    //            command.CommandText = "Select can,can_r,exp_tipo,des,pun,bon,art,tot from remdet WHERE sec = '" + item.Sec + "' and orden = '" + item.Orden + "'";
    //            reader = command.ExecuteReader();
    //            List<DetalleRemito> detalle = new List<DetalleRemito>();
    //            while (reader.Read())
    //            {
    //                var newitem = ParseDetalle(reader);
    //                detalle.Add(newitem);
    //            }
     //           reader.Close();
      //          item.Detalle = detalle;
       //     }
      //  }
      //  cnn.Close();
      //  return result;
   // }
}
