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
        public List<Seccion> SeccionDolar { get; set; }
        
        public List<Factura> List(DateTime fecha, DateTime fechaHasta)
        {
            SujetoService sujetoService = new SujetoService(this.ConnectionStringBase);
            string connectionString = this.ConnectionStringBase + "sae.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT facmae.sec,facmae.orden,facmae.tipo,letra,pe,num,cae,cae_id,tipcomp,femi,fvto,facmae.cmay,scta,facmae.rem,cla,facmae.ven,tve,tep,tra,civa,sub1,sub1Imp,dto,pde,sub2," +
                "gas,facmae.int,facmae.ibru,cibru,per,facmae.fle,ot1,ret,iva1,iva2,iva3,facmae.tot,cotiz,morig,estado,integ,facmae.lote,facmae.noa,noi,obs1,obs2,fnventa,facmae.pre, " +
                "facmae.nc,facmae.tip_op,ord_ven,fac_cre," +
                "for_pag,transp,guia,nped,bultos,facmae.credito,pag,fcr,totd,nctip," +
                "cla,peri_asig," + 
                "clipro.cod,clipro.nom,clipro.dir,clipro.alt,clipro.loc,clipro.pos,clipro.provin,clipro.email,clipro.cuit,clipro.piva, " +
                "can,ume,des,pun,bon,art,facdet.tot as totdet,facdet.iva as ivadet,VAL(STR(facdet.aiva,10,2)) as aiva,dtog,punimp,VAL(STR(facdet.int,10,2)) as intdet " + 
                "FROM facmae " +
                "INNER JOIN facdet ON facmae.sec = facdet.sec AND facmae.orden = facdet.orden " +
                "INNER JOIN sec On facmae.sec = sec.cod " +
                "inner join clipro on clipro.cod = facmae.scta " + 
                "WHERE  (femi BETWEEN ctod('" + fecha.ToString("MM-dd-yyy") + "')" +                 
                " AND ctod('" + fechaHasta.ToString("MM-dd-yyy") + "')) and sec.funcion = '1' and sec.dis_iva = .f. order by femi,facmae.tipo,letra,pe,num,facdet.item";
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
        public List<FacturaView> ListView(DateTime fecha, DateTime fechaHasta)
        {
            var tmpResult = this.List(fecha,fechaHasta);
            List<FacturaView> result = new List<FacturaView>();
            
            foreach (var item in tmpResult) 
            { 
                int numitem = 0;
                foreach (var itemD in item.Detalle)
                {
                    numitem += 1;
                    var newItem = (new FacturaView
                    {
                        Sec = item.Sec,
                        Orden = item.Orden,
                        FechaPase = item.FechaPase,
                        FechaComprobante = item.FechaComprobante,
                        FechaVencimiento = item.FechaVencimiento,
                        Tipo = item.Tipo,
                        Letra = item.Letra,
                        Pe = item.Pe,
                        Numero = item.Numero,
                        Comprobante = item.Comprobante,
                        IdDivisa = item.IdDivisa,
                        Cotizacion = item.Cotizacion,
                        IdCuenta = item.IdCuenta,
                        NombreCuenta = item.Cuenta.Nombre,
                        Obs = item.Obs,
                        SubTotal = item.SubTotal,
                        Descuento = item.Detalle.Sum(s=>s.Descuento),
                        IvaGeneral = item.IvaGeneral,
                        IvaOtro = item.IvaOtro,                        
                        PercepcionIva = item.PrecepcionIva,
                        PercepcionIB = item.PrecepcionIB,
                        ImpuestoInterno = item.Detalle.Sum(s=>s.ImpInterno),                        
                        Total = item.Total,
                        Cae = item.Cae,
                        Remito = item.Remito,
                        CondVenta = item.CondVenta,
                        TipoComp = item.TipoComp,
                        IdArticulo = itemD.IdArticulo,
                        Item = numitem,
                        Concepto = itemD.Concepto,
                        Precio = itemD.Precio,
                        AlicuotaIva = itemD.AlicuotaIva,
                        Iva = itemD.Iva,
                        ImpuestoInternoItem = itemD.ImpInterno,
                        Cantidad = itemD.Cantidad,                        
                        UnidadMedida = itemD.UnidadMedida,
                        SubTotalItem = itemD.SubTotal,
                        Bonificacion = itemD.Bonificacion,
                        IdRemito = itemD.IdRemito,
                        IdCampania = item.IdCampania,
                        IdClaseVenta = item.IdClaseVenta,                        
                    }); 
                    result.Add(newItem);                    
                }
            }
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
                + " and (FACMAE.cla!='07' and FACMAE.cla!='08' and  FACMAE.cla!='09')"
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
                "facmae.ven,tve,tep,tra,civa,sub1,sub1imp,dto,pde,sub2,gas,facmae.int,facmae.ibru,cibru,per,facmae.fle,ot1,ret,iva1,iva2,iva3,facmae.tot, " + 
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
                "VAL(STR(facdet.int,10,2)) as intdet,facdet.rem " + 
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
            item.Remito = reader["rem"].ToString();
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
            item.CondVenta = reader["cla"].ToString().Trim() == "002" ? "CONTADO" : "CUENTA CORRIENTE";
            item.IdCuenta = reader["scta"].ToString().Trim();
            item.SubTotal = (decimal)reader["sub1"];
            if (item.SubTotal==0)
                item.SubTotal = (decimal)reader["sub1Imp"];
            item.PrecepcionIva = (decimal)reader["per"];
            item.PrecepcionIB = (decimal)reader["ibru"];
            
            item.Descuento = 0; //(decimal)reader["sub1"];
            item.Obs = reader["obs1"].ToString().Trim() + reader["obs2"].ToString().Trim();
            item.Remito = reader["rem"].ToString().Trim();
            item.IvaGeneral = (decimal)reader["iva1"];
            item.IvaOtro = (decimal)reader["iva2"];            
            item.Total = (decimal)reader["tot"];
            item.Cae = Convert.ToInt64(reader["cae"]);            
            item.IdDivisa = reader["morig"].ToString().Trim() == "D" || reader["morig"].ToString().Trim() == "1" ? 1:0;
            item.Cotizacion = 1;
            //Poner solo la cotizacion si es Factura en dolar o con clausula
            if (item.IdDivisa == 1 || (item.IdDivisa == 0  && this.SeccionDolar.Where(w=>w.Id ==item.Sec).Count() > 0))
                item.Cotizacion = (decimal)reader["cotiz"];
            Sujeto tmpSujeto = new Sujeto();
            tmpSujeto.Id = reader["cod"].ToString().Trim();
            tmpSujeto.Nombre = reader["nom"].ToString().Trim();
            tmpSujeto.NumeroDocumento = reader["cuit"].ToString().Trim();
            tmpSujeto.Domicilio = reader["dir"].ToString().Trim();
            tmpSujeto.CodigoPostal = reader["pos"].ToString().Trim();
            tmpSujeto.Provincia = reader["provin"].ToString().Trim();
            tmpSujeto.Localidad = reader["loc"].ToString().Trim();
            item.Cuenta = tmpSujeto;
            item.IdClaseVenta = reader["cla"].ToString().Trim();
            item.IdCampania = reader["peri_asig"].ToString().Trim();
            return item;
        }
        private DetalleFactura ParseDetalle(OleDbDataReader reader)
        {
            DetalleFactura item = new DetalleFactura();
            item.Cantidad = (decimal) reader["can"];
            item.UnidadMedida = reader["ume"].ToString().Trim();
            if (string.IsNullOrEmpty(item.UnidadMedida)) 
               item.UnidadMedida = "UN";
            item.Concepto = reader["des"].ToString().Trim();
            item.Precio = (decimal)(reader["pun"]);
            item.Descuento = (decimal)reader["Dtog"];
            item.Bonificacion = (decimal)reader["bon"];
            item.IdArticulo = reader["art"].ToString().Trim();
            item.SubTotal = (decimal)reader["totdet"];
            item.AlicuotaIva = (decimal)reader["aiva"];
            item.Iva = (decimal)reader["ivadet"];
            item.IdRemito = reader["rem"].ToString();
            try { item.ImpInterno = Convert.ToDecimal(reader["intdet"]); }catch{ }
            //if (reader["letra"].ToString().Trim() == "A")
            //{
            item.Precio = (decimal)(reader["pun"]);
            item.SubTotal = (decimal)reader["totdet"] + (decimal)reader["dtog"] - (decimal)reader["ivadet"];
            //}
            //else 
            //{
            //    try { item.Precio = (decimal)reader["punimp"]; } catch { }
                
               // item.SubTotal = (decimal)reader["totdet"];
            //}
                
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
