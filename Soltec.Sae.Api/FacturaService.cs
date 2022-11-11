using System.Data.OleDb;

namespace Soltec.Sae.Api
{
    public class FacturaService
    {         
        public FacturaService(string connectionStringBase) 
        {
            this.ConnectionStringBase = connectionStringBase;
        }
        public string ConnectionStringBase { get; set; } = "";
        
       
        public List<Factura> ListBack(DateTime fecha ,DateTime fechaHasta) 
        {
            SujetoService sujetoService = new SujetoService(this.ConnectionStringBase);
            string connectionString = this.ConnectionStringBase + "sae.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT sec,orden,tipo,letra,pe,num,cae,cae_id,tipcomp,femi,fvto,cmay,scta,facmae.rem,cla,facmae.ven,tve,tep,tra,civa,sub1,dto,pde,sub2," +
                "                 gas,int,facmae.ibru,cibru,per,facmae.fle,ot1,ret,iva1,iva2,iva3,tot,cotiz,morig,estado,integ,lote,noa,noi,obs1,obs2,fnventa,pre,nc,tip_op,ord_ven,fac_cre," +
                "for_pag,transp,guia,nped,bultos,facmae.credito,pag,fcr,div,totd,nctip," +
                "clipro.cod,clipro.nom,clipro.dir,clipro.alt,clipro.loc,clipro.pos,clipro.provin,clipro.email,clipro.cuit,clipro.piva FROM facmae inner join clipro on clipro.cod = facmae.scta WHERE  (femi BETWEEN ctod('" + fecha.ToString("MM-dd-yyy") + "')"
                + " AND ctod('" + fechaHasta.ToString("MM-dd-yyy") + "')) order by femi,tipo,letra,pe,num";
            OleDbDataReader reader = command.ExecuteReader();
            List<Factura> result = new List<Factura>();
            while (reader.Read())
            {
                var item = Parse(reader);                
                result.Add(item);
            }            
            cnn.Close();
            return result;
        }
        public List<Factura> List(DateTime fecha, DateTime fechaHasta)
        {
            SujetoService sujetoService = new SujetoService(this.ConnectionStringBase);
            string connectionString = this.ConnectionStringBase + "sae.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT facmae.sec,facmae.orden,facmae.tipo,letra,pe,num,cae,cae_id,tipcomp,femi,fvto,cmay,scta,facmae.rem,cla,facmae.ven,tve,tep,tra,civa,sub1,dto,pde,sub2," +
                "gas,facmae.int,facmae.ibru,cibru,per,facmae.fle,ot1,ret,iva1,iva2,iva3,facmae.tot,cotiz,morig,estado,integ,facmae.lote,facmae.noa,noi,obs1,obs2,fnventa,facmae.pre, " +
                "facmae.nc,facmae.tip_op,ord_ven,fac_cre," +
                "for_pag,transp,guia,nped,bultos,facmae.credito,pag,fcr,totd,nctip," +
                "clipro.cod,clipro.nom,clipro.dir,clipro.alt,clipro.loc,clipro.pos,clipro.provin,clipro.email,clipro.cuit,clipro.piva, " +
                "can,des,pun,bon,art,facdet.tot as totdet,facdet.iva as ivadet,VAL(STR(facdet.aiva,10,2)) as aiva,dtog,punimp,VAL(STR(facdet.int,10,2)) as intdet " + 
                "FROM facmae " +
                "INNER JOIN facdet ON facmae.sec = facdet.sec AND facmae.orden = facdet.orden " + 
                "inner join clipro on clipro.cod = facmae.scta WHERE  (femi BETWEEN ctod('" + fecha.ToString("MM-dd-yyy") + "')"
                + " AND ctod('" + fechaHasta.ToString("MM-dd-yyy") + "')) order by femi,facmae.tipo,letra,pe,num";
            OleDbDataReader reader = command.ExecuteReader();
            List<Factura> result = new List<Factura>();
            string idAnt = "";
            Factura item = new Factura();
            while (reader.Read())
            {
                string id = reader["sec"].ToString().Trim() + reader["orden"].ToString().Trim();
                if (idAnt != id && idAnt != "")
                {
                    result.Add(item);
                    item = new Factura();
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
        }
        public List<DocumentoPendienteView> ListPendiente(string idCuenta,DateTime fecha, DateTime fechaHasta)
        {
            SujetoService sujetoService = new SujetoService(this.ConnectionStringBase);
            string connectionString = this.ConnectionStringBase + "sae.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT facmae.sec,facmae.orden,facmae.tipo,letra,pe,num,cae,cae_id,tipcomp,femi,fvto,cmay,scta,facmae.rem,cla,facmae.ven,tve,tep,tra,civa,sub1,dto,pde,sub2," +
                "gas,facmae.int,facmae.ibru,cibru,per,facmae.fle,ot1,ret,iva1,iva2,iva3,facmae.tot,cotiz,morig,estado,integ,facmae.lote,facmae.noa,noi,obs1,obs2,fnventa,facmae.pre, " +
                "facmae.nc,facmae.tip_op,ord_ven,fac_cre," +
                "for_pag,transp,guia,nped,bultos,facmae.credito,pag,fcr,totd,nctip," +
                "clipro.cod,clipro.nom,clipro.dir,clipro.alt,clipro.loc,clipro.pos,clipro.provin,clipro.email,clipro.cuit,clipro.piva, " +
                "can,des,pun,bon,art,facdet.tot as totdet,facdet.iva as ivadet,VAL(STR(facdet.aiva,10,2)) as aiva,dtog,punimp,VAL(STR(facdet.int,10,2)) as intdet,can_r " +
                "FROM facmae " +
                "INNER JOIN facdet ON facmae.sec = facdet.sec AND facmae.orden = facdet.orden " +
                "inner join clipro on clipro.cod = facmae.scta "   +
                "LEFT JOIN artgen ON artgen.cod = facdet.art "  +
                "WHERE  (femi BETWEEN ctod('" + fecha.ToString("MM-dd-yyy") + "')"
                + " AND ctod('" + fechaHasta.ToString("MM-dd-yyy") + "')) " 
                + " and (scta = '" + idCuenta + "' or empty('" + idCuenta + "'))"  
                + " And facmae.tipo = 1 And facdet.can_r > 0 And facdet.exp_tipo = 'C' and artgen.exime = .f. "                 
                + " order by femi,facmae.tipo,letra,pe,num";
            OleDbDataReader reader = command.ExecuteReader();
            List<DocumentoPendienteView> result = new List<DocumentoPendienteView>();           
            while (reader.Read())
            {                
               result.Add(ParsePendiente(reader));                
            }            
            reader.Close();
            cnn.Close();
            return result;
        }

        public Factura FindOne(string sec,string orden) 
        {
            string connectionString = this.ConnectionStringBase + "sae.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT facmae.sec,facmae.orden,facmae.tipo,letra,pe,num,cae,cae_id,tipcomp,femi,fvto,cmay,scta,facmae.rem,cla, " +
                "facmae.ven,tve,tep,tra,civa,sub1,dto,pde,sub2,gas,facmae.int,facmae.ibru,cibru,per,facmae.fle,ot1,ret,iva1,iva2,iva3,facmae.tot, " + 
                "cotiz,morig,estado,integ,facmae.lote,facmae.noa,noi,obs1,obs2,fnventa,facmae.tip_op,ord_ven,fac_cre,for_pag,transp,guia,nped,bultos, " +
                "facmae.credito,pag,fcr,div,totd,nctip,clipro.cod,clipro.nom,clipro.dir,clipro.alt,clipro.loc,clipro.pos,clipro.provin,clipro.email, "+ 
                "clipro.cuit,clipro.piva " +
                "FROM FACMAE " +                 
                "inner join clipro on clipro.cod = facmae.scta " +
                "WHERE  sec = '" + sec + "' and orden = '" + orden + "'";
            OleDbDataReader reader = command.ExecuteReader();
            Factura result = null;
            while (reader.Read())
            {
                result = Parse(reader);                
            }
            reader.Close();
            command.CommandText = "Select facmae.letra,can,des,pun,bon,art,facdet.tot as totdet,facdet.iva as ivadet,VAL(STR(facdet.aiva,10,2)) as aiva,dtog,punimp," + 
                "VAL(STR(facdet.int,10,2)) as intdet " + 
                "from facdet " +
                "inner join facmae on facmae.sec = facdet.sec and facmae.orden = facdet.orden " + 
                "WHERE facdet.sec = '" + sec + "' and facdet.orden = '" + orden + "'";
            reader = command.ExecuteReader();
            List<DetalleFactura> detalle = new List<DetalleFactura>();
            while (reader.Read())
            {
                 var item = ParseDetalle(reader);
                 detalle.Add(item);
            }
            result.Detalle = detalle;
            cnn.Close();
            return result;
        }
        private Factura Parse(OleDbDataReader reader)
        {
            Factura item = new Factura();
            item.Sec = reader["sec"].ToString().Trim();
            item.Orden = reader["orden"].ToString().Trim();
            item.Tipo = Convert.ToInt16(reader["tipo"]);
            item.Letra = reader["letra"].ToString().Trim();
            item.TipoComp = reader["tipcomp"].ToString();
            item.FechaPase = (DateTime)reader["femi"];
            item.FechaComprobante = (DateTime)reader["femi"];
            item.FechaVencimiento = (DateTime)reader["fvto"];
            if (item.Tipo == 1)
            {
                item.Comprobante = "FACTURA";
            }
            else if (item.Tipo == 2)
            {
                item.Comprobante = "NOTA DE CREDITO";
            }
            else if (item.Tipo == 3)
            {
                item.Comprobante = "NOTA DE DEBITO";
            }
            else if (item.Tipo == 4)
            {
                item.Comprobante = "TICKET";
            }
            item.Pe = reader["pe"].ToString().Trim() == "" ? 0 : Convert.ToInt16(reader["pe"]);
            item.Numero = reader["num"].ToString().Trim() == "" ? 0 : Convert.ToInt32(reader["num"]);
            item.IdCuenta = reader["scta"].ToString().Trim();
            item.SubTotal = (decimal)reader["sub1"];
            item.PrecepcionIva = (decimal)reader["per"];
            item.PrecepcionIB = (decimal)reader["ibru"];
            item.SubTotal = (decimal)reader["sub1"];
            item.Descuento = (decimal)reader["sub1"];
            item.Obs = reader["obs1"].ToString().Trim() + reader["obs2"].ToString().Trim();
            item.Remito = reader["rem"].ToString().Trim();
            if (item.Letra == "A")
            {
                item.IvaGeneral = (decimal)reader["iva1"];
                item.IvaOtro = (decimal)reader["iva2"];
            }
            item.Total = (decimal)reader["tot"];
            item.Cae = Convert.ToInt64(reader["cae"]);
            item.Cotizacion = (decimal)reader["cotiz"];
            item.IdDivisa = reader["morig"].ToString().Trim() == "D" || reader["morig"].ToString().Trim() == "1" ? 1:0;
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
        private DetalleFactura ParseDetalle(OleDbDataReader reader)
        {
            DetalleFactura item = new DetalleFactura();
            item.Cantidad = (decimal) reader["can"];
            item.Concepto = reader["des"].ToString().Trim();
            item.Precio = (decimal)(reader["pun"]);
            item.Descuento = (decimal)reader["bon"];
            item.IdArticulo = reader["art"].ToString();
            item.SubTotal = (decimal)reader["totdet"];
            item.AlicuotaIva = (decimal)reader["aiva"];
            item.Iva = (decimal)reader["ivadet"];
            //try { item.ImpInterno = Convert.ToDecimal(reader["intdet"]); }catch{ }
            if (reader["letra"].ToString().Trim() == "A")
            {
                item.Precio = (decimal)(reader["pun"]);
                item.SubTotal = (decimal)reader["totdet"] + (decimal)reader["dtog"] - (decimal)reader["ivadet"];
            }
            else 
            {
                try { item.Precio = (decimal)reader["punimp"]; } catch { }
                
                item.SubTotal = (decimal)reader["totdet"];
            }
                
            return item;
        }
        private DocumentoPendienteView ParsePendiente(OleDbDataReader reader)
        {
            DocumentoPendienteView item = new DocumentoPendienteView();
            item.Sec = reader["sec"].ToString().Trim();
            item.Orden = reader["orden"].ToString().Trim();
            item.Tipo = Convert.ToInt16(reader["tipo"]);
            item.Letra = reader["letra"].ToString().Trim();            
            item.FechaPase = (DateTime)reader["femi"];
            item.FechaComprobante = (DateTime)reader["femi"];
            item.FechaVencimiento = (DateTime)reader["fvto"];
            if (item.Tipo == 1)
            {
                item.Comprobante = "FACTURA";
            }
            else if (item.Tipo == 2)
            {
                item.Comprobante = "NOTA DE CREDITO";
            }
            else if (item.Tipo == 3)
            {
                item.Comprobante = "NOTA DE DEBITO";
            }
            else if (item.Tipo == 4)
            {
                item.Comprobante = "TICKET";
            }
            item.Pe = reader["pe"].ToString().Trim() == "" ? 0 : Convert.ToInt16(reader["pe"]);
            item.Numero = reader["num"].ToString().Trim() == "" ? 0 : Convert.ToInt32(reader["num"]);
            item.IdCuenta = reader["scta"].ToString().Trim();
            item.IdArticulo = reader["art"].ToString().Trim();
            item.Nombre = reader["nom"].ToString().Trim();
            item.Cantidad = (decimal)reader["can"];
            item.NombreArticulo = reader["des"].ToString().Trim();
            item.CantidadPendiente = (decimal)reader["can_r"];
            return item;
        }
    }
}
