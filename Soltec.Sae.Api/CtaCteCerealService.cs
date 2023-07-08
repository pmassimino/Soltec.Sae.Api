using System.Data;
using System.Data.OleDb;

namespace Soltec.Sae.Api
{
    public class CtaCteCerealService
    {
        public CtaCteCerealService(string connectionStringBase) 
        {
            this.ConnectionStringBase = connectionStringBase;
        }
        public string ConnectionStringBase { get; set; } = "";
        public string SaeConnectionStringBase { get; set; } = "";
        public string IdSucursal { get; set; } = "01";
        public string TipoSaldo { get; set; } = "1";
        public List<MovCtaCteCereal> List(string idCuenta, string idCosecha, DateTime fechaHasta) 
        {
            List<MovCtaCteCereal> result = new List<MovCtaCteCereal>();
            List<MovCtaCteCereal> tmpResult = new List<MovCtaCteCereal>();
            EntradaService entradaService = new EntradaService(this.ConnectionStringBase);
            DateTime fecha = DateTime.Now.AddYears(-100);
            var entradas = entradaService.List(idCuenta, idCosecha,fecha,fechaHasta);
            foreach (var item in entradas) 
            {
                MovCtaCteCereal newItem = new MovCtaCteCereal();
                newItem.FechaComprobante = item.Fecha;
                newItem.FechaPase = item.Fecha;
                newItem.FechaVencimiento = item.Fecha;
                newItem.Concepto = "ENTRADA";
                newItem.Tipo = "RETIRO";
                newItem.PesoNeto = item.PesoNetoFinal;
                newItem.Ingreso = item.PesoNetoFinal;
                newItem.IdCuenta = item.IdCuenta;
                newItem.IdCosecha = item.IdCosecha;
                newItem.IdSucursal = this.IdSucursal;
                newItem.IdTransaccion = "ENTRADA;" + this.IdSucursal + ";" + item.Numero;
                newItem.NumeroComprobante = item.NumeroCartaPorte;
                tmpResult.Add(newItem);
            }
            RetiroService retiroService = new RetiroService(this.ConnectionStringBase);
            var retiros = retiroService.List(idCuenta, idCosecha,fecha);
            foreach (var item in retiros)
            {
                MovCtaCteCereal newItem = new MovCtaCteCereal();
                newItem.FechaComprobante = item.Fecha;
                newItem.FechaPase = item.Fecha;
                newItem.FechaVencimiento = item.Fecha;
                newItem.Concepto = "RETIRO";
                newItem.PesoNeto = item.PesoNeto;
                newItem.Tipo = "RETIRO";
                newItem.Egreso = item.PesoNeto;
                newItem.IdCuenta = item.IdCuenta;
                newItem.IdCosecha = item.IdCosecha;
                newItem.IdSucursal = this.IdSucursal;
                newItem.IdTransaccion = "RETIRO;" + this.IdSucursal + ";" + item.Numero;
                newItem.NumeroComprobante = item.NumeroCPorte;
                tmpResult.Add(newItem);
            }
            LiquidacionService liquidacionService = new LiquidacionService(this.ConnectionStringBase);
            var liquidaciones = liquidacionService.List(idCuenta, idCosecha,fecha);
            foreach (var item in liquidaciones)
            {
                MovCtaCteCereal newItem = new MovCtaCteCereal();
                newItem.FechaComprobante = item.Fecha;
                newItem.FechaPase = item.Fecha;
                newItem.FechaVencimiento = item.FechaVencimiento;
                newItem.Concepto = "LIQUIDACION";
                newItem.PesoNeto = item.PesoNeto;
                if ((item.IdTipo == 1 || item.IdTipo == 3) && this.TipoSaldo == "1")
                    newItem.Egreso = item.PesoNeto;                
                newItem.Tipo = "LIQUI";                
                newItem.IdCuenta = item.IdCuenta;
                newItem.IdCosecha = item.IdCosecha;
                newItem.IdSucursal = this.IdSucursal;
                newItem.IdTransaccion = "LIQUIDACION;" + this.IdSucursal + ";" + item.Id;
                newItem.NumeroComprobante = item.Numero;
                newItem.TieneComp = true;
                tmpResult.Add(newItem);
            }
            BoletoService boletoService = new BoletoService(this.ConnectionStringBase);
            var boletos = boletoService.List(idCuenta, idCosecha,fecha);
            foreach (var item in boletos)
            {
                MovCtaCteCereal newItem = new MovCtaCteCereal();
                newItem.FechaComprobante = item.Fecha;
                newItem.FechaPase = item.Fecha;
                newItem.FechaVencimiento = item.FechaVencimiento;
                newItem.Concepto = "AUTORIZACION DE VENTA";
                newItem.PesoNeto = item.PesoNeto;
                if (this.TipoSaldo == "2")
                    newItem.Egreso = item.PesoNeto;
                newItem.Tipo = "AUTORIZACION";
                newItem.IdCuenta = item.IdCuenta;
                newItem.IdCosecha = item.IdCosecha;
                newItem.IdSucursal = this.IdSucursal;
                newItem.IdTransaccion = "AUTORIZACION;" + this.IdSucursal + ";" + item.Id;
                newItem.NumeroComprobante = item.Numero;
                newItem.TieneComp = true;
                tmpResult.Add(newItem);
            }
            CertificadoService certificadoService = new CertificadoService(this.ConnectionStringBase);
            var certificados = certificadoService.List(idCuenta, idCosecha,fecha);
            foreach (var item in certificados)
            {
                MovCtaCteCereal newItem = new MovCtaCteCereal();
                newItem.FechaComprobante = item.Fecha;
                newItem.FechaPase = item.Fecha;
                newItem.FechaVencimiento = item.FechaVencimiento;
                newItem.Concepto = "CERTIFICADO";
                newItem.PesoNeto = item.PesoNeto;                
                newItem.Tipo = "CERTI";
                newItem.IdCuenta = item.IdCuenta;
                newItem.IdCosecha = item.IdCosecha;
                newItem.IdSucursal = this.IdSucursal;
                newItem.IdTransaccion = "CERTI;" + this.IdSucursal + ";" + item.Id;
                newItem.NumeroComprobante = item.Numero;
                newItem.TieneComp = true;
                tmpResult.Add(newItem);
            }
            RTService rtService = new RTService(this.ConnectionStringBase);
            var rtTranf = rtService.List(idCuenta, "",idCosecha,fecha);
            foreach (var item in rtTranf)
            {
                MovCtaCteCereal newItem = new MovCtaCteCereal();
                newItem.FechaComprobante = item.Fecha;
                newItem.FechaPase = item.Fecha;
                newItem.FechaVencimiento = item.FechaVencimiento;
                newItem.Concepto = "TRANSFERIDO";
                newItem.PesoNeto = item.PesoNeto;
                newItem.Egreso = item.PesoNeto;
                newItem.Tipo = "RT";
                newItem.IdCuenta = item.IdCuenta;
                newItem.IdCosecha = item.IdCosecha;
                newItem.IdSucursal = this.IdSucursal;
                newItem.IdTransaccion = "RT;" + this.IdSucursal + ";" + item.Id;
                newItem.NumeroComprobante = item.Numero;
                newItem.TieneComp = true;
                tmpResult.Add(newItem);
            }            
            var rtRecibido = rtService.List("",idCuenta, idCosecha,fecha);
            foreach (var item in rtRecibido)
            {
                MovCtaCteCereal newItem = new MovCtaCteCereal();
                newItem.FechaComprobante = item.Fecha;
                newItem.FechaPase = item.Fecha;
                newItem.FechaVencimiento = item.FechaVencimiento;
                newItem.Concepto = "RECIBIDO";
                newItem.PesoNeto = item.PesoNeto;
                newItem.Ingreso = item.PesoNeto;
                newItem.Tipo = "RT";
                newItem.IdCuenta = item.IdCuenta;
                newItem.IdCosecha = item.IdCosecha;
                newItem.IdSucursal = this.IdSucursal;
                newItem.IdTransaccion = "RT;" + this.IdSucursal + ";" + item.Id;
                newItem.NumeroComprobante = item.Numero;
                newItem.TieneComp = true;
                tmpResult.Add(newItem);
            }
            //Trnasferencia Anteriores- no se usan mas
            var tranfAnt = rtService.ListTraOri(idCuenta, idCuenta, idCosecha);
            foreach (var item in tranfAnt) 
            {
                MovCtaCteCereal newItem = new MovCtaCteCereal();
                newItem.FechaComprobante = item.Fecha;
                newItem.FechaPase = item.Fecha;
                newItem.FechaVencimiento = item.FechaVencimiento;
                if (!string.IsNullOrEmpty(item.IdCuenta)) //Transferida
                {
                    newItem.IdCuenta = item.IdCuenta;
                    newItem.Concepto = "TRANSFERIDO";
                    newItem.Egreso = item.PesoNeto;
                }
                else 
                {
                    newItem.IdCuenta = item.IdCuentaDestino;
                    newItem.Concepto = "RECIBIDO";
                    newItem.Ingreso = item.PesoNeto;
                }
                newItem.PesoNeto = item.PesoNeto;
               
                newItem.Tipo = "TRAORI";
                
                newItem.IdCosecha = item.IdCosecha;
                newItem.IdSucursal = this.IdSucursal;
                newItem.IdTransaccion = "TRAORI;" + this.IdSucursal + ";" + item.Id;
                newItem.NumeroComprobante = item.Numero;
                newItem.TieneComp = true;
                tmpResult.Add(newItem);
            }
            result = tmpResult.OrderBy(o => o.FechaPase).OrderBy(o => o.Concepto).ToList();
            Int64 saldo = 0;
            foreach (var item  in result) 
            {
                saldo = item.Ingreso - item.Egreso + saldo;
                item.Saldo = saldo;
            }
            return result;
        }
        public List<SaldoCtaCteCereal> Saldos(string IdCuenta,string idCosecha,DateTime fecha) 
        {
            ProductorService produService = new ProductorService(this.ConnectionStringBase);         
            var cosechasDisp = produService.CosechaDisponible(IdCuenta,idCosecha);
            if (string.IsNullOrEmpty(idCosecha) && string.IsNullOrEmpty(IdCuenta)) 
               {
                CosechaService cosechaService = new CosechaService(this.ConnectionStringBase);
                var cosecha = cosechaService.List().OrderBy(o => o.Id).TakeLast(45).ToList();
                cosechasDisp = cosechasDisp.Where(x => cosecha.Any(c => c.Id == x.IdCosecha)).ToList();
            }
               
            List<SaldoCtaCteCereal> result = new List<SaldoCtaCteCereal>();
            foreach (var item in cosechasDisp) 
            {
                if (!string.IsNullOrEmpty(item.IdCosecha) && !string.IsNullOrEmpty(item.IdProductor))
                result.Add(this.Saldo(item.IdProductor, item.IdCosecha, fecha));
            }
            return result;
        }
        public List<SaldoCtaCteCereal> Saldos2(string IdCuenta, string idCosecha, DateTime fecha)
        {
            string consultaSQL = "SELECT EN_P_NET AS Entrada, IIF(entrada.ventadir, en_p_net, 0000000000) as Directo, 00000000000 AS Certificado, 00000000000 AS Recibido, 00000000000 AS Transferido, " +
            "00000000000 AS Liquidado, 00000000000 AS K_ANTI, 00000000000 AS Retiro, 00000000000 AS K_AUTO, EN_COSEC AS id_Cosecha,EN_PRODU as id_Produ " +
            "FROM ENTRADA WHERE en_fecha <= ctod('" + fecha.ToString("MM-dd-yyy") + "')" + " AND ENTRADA.STOCK_PLAN = .F. " +
            "union all " +
            "SELECT 00000000000 AS  Entrada,00000000000 as Directo, P_NETO AS  Certificado, 00000000000 AS K_RECI, 00000000000 AS  Transferido, " +
            "00000000000 AS  Liquidado, 00000000000 AS K_ANTI, 00000000000 AS  Retiro, 00000000000 AS K_AUTO, COSECHA AS id_Cosecha,Productor as id_Produ " +
            "FROM CERTI WHERE CONFIRMADO = .T. .and.fpase <= ctod('" + fecha.ToString("MM-dd-yyy") + "') " +
            "UNION ALL " +
            "SELECT 00000000000 AS  Entrada,00000000000 as Directo, 00000000000 AS  Certificado, 00000000000 AS K_RECI, 00000000000 AS  Transferido, " +
            "00000000000 AS  Liquidado, 00000000000 AS K_ANTI, RE_PES_NET AS  Retiro,  00000000000 AS K_AUTO, RE_COSEC AS id_Cosecha,re_produ as id_Produ " +
            "FROM RETIRO WHERE RE_TIP = 2.and.re_fecha <= ctod('" + fecha.ToString("MM-dd-yyy") + "') " +
            "union all " +
            "SELECT 00000000000 AS  Entrada,00000000000 as Directo, 00000000000 AS  Certificado, 00000000000 AS K_RECI, 00000000000 AS  Transferido, " +
            "00000000000 AS  Liquidado, 00000000000 AS K_ANTI, 00000000000 AS  Retiro,  BOL_KGS AS K_AUTO, BOL_COSEC AS id_Cosecha,bol_produ as id_Produ " +
            "FROM BOLETOS WHERE BOL_CONFI = .T. .and. bol_fec <= ctod('" + fecha.ToString("MM-dd-yyy") + "') " +
            "union all " +
            "SELECT 00000000000 AS  Entrada,00000000000 as Directo, 00000000000 AS  Certificado, 00000000000 AS K_RECI, 00000000000 AS  Transferido, " +
            "P_NETO AS  Liquidado, 00000000000 AS K_ANTI, 00000000000 AS  Retiro,  00000000000 AS K_AUTO, COSE AS id_Cosecha,produ as id_Produ " +
            "FROM LIQUI WHERE CERRADA = .T. .AND. !ANTICIPO.and.fecha <= ctod('" + fecha.ToString("MM-dd-yyy") + "') " + " AND(id_tip_liq = 1 OR id_tip_liq = 3) " +
            "union all " +
            "SELECT 00000000000 AS  Entrada,00000000000 as Directo, 00000000000 AS  Certificado, 00000000000 AS K_RECI, 00000000000 AS  Transferido, " +
            "00000000000 AS  Liquidado, P_NETO AS K_ANTI, 00000000000 AS  Retiro,  00000000000 AS K_AUTO, COSE AS id_Cosecha , produ as id_Produ " +
            "FROM LIQUI WHERE CERRADA = .T. .AND. (id_tip_liq = 1 OR id_tip_liq = 3).and.fecha <= ctod('" + fecha.ToString("MM-dd-yyy") + "') " +
            "union all " +
            "SELECT 00000000000 AS  Entrada,00000000000 as Directo, 00000000000 AS  Certificado, 00000000000 AS K_RECI, KGS AS  Transferido, " +
            "00000000000 AS  Liquidado, 00000000000 AS K_ANTI, 00000000000 AS  Retiro,  00000000000 AS K_AUTO, COS AS id_Cosecha,pro as id_Produ " +
            "FROM TRAORI WHERE fec <= ctod('" + fecha.ToString("MM-dd-yyy") + "') " +
            "UNION ALL " +
            "SELECT 00000000000 AS  Entrada,00000000000 as Directo, 00000000000 AS  Certificado, 00000000000 AS K_RECI, kilosnetos AS  Transferido, " +
            "00000000000 AS  Liquidado, 00000000000 AS K_ANTI, 00000000000 AS  Retiro,  00000000000 AS K_AUTO, id_cosecha AS id_Cosecha,id_deposi as id_Produ " +
            "FROM rettransf WHERE fecha_emi <= ctod('" + fecha.ToString("MM-dd-yyy") + "') " + " AND id_tipo_rt = 1 " +
            "union all " +
            "SELECT 00000000000 AS  Entrada,00000000000 as Directo, 00000000000 AS  Certificado, KGS AS K_RECI,  00000000000 AS  Transferido, " +
            "00000000000 AS  Liquidado, 00000000000 AS K_ANTI, 00000000000 AS  Retiro,  00000000000 AS K_AUTO, COS AS id_Cosecha,pro as id_Produ " +
            "FROM TRADES WHERE fec <= ctod('" + fecha.ToString("MM-dd-yyy") + "') " +
            "union all " +
            "SELECT 00000000000 AS  Entrada,00000000000 as Directo, 00000000000 AS  Certificado, kilosNetos AS K_RECI,  00000000000 AS  Transferido, " +
            "00000000000 AS  Liquidado, 00000000000 AS K_ANTI, 00000000000 AS  Retiro,  00000000000 AS K_AUTO, id_cosecha AS id_Cosecha,id_Receptor as id_Produ " +
            "FROM rettransf WHERE fecha_emi <= ctod('" + fecha.ToString("MM-dd-yyy") + "')" + " AND id_tipo_rt = 1 into cursor tmpMovCta";
            string connectionString = this.ConnectionStringBase + "cereales.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = consultaSQL;
            OleDbDataReader reader = command.ExecuteReader();            
            List<tmpSumaSaldo> tmpResult = new List<tmpSumaSaldo>();
            while (reader.Read())
            {
                tmpResult.Add(new tmpSumaSaldo {
                    IdCosecha = reader["id_cosecha"].ToString().Trim(),
                    IdCuenta = reader["id_produ"].ToString().Trim(),
                    TotalEntradas = Convert.ToInt64(reader["entrada"].ToString()),
                    TotalRetiros = Convert.ToInt64(reader["retiro"]),
                    TotalCertificado = Convert.ToInt64(reader["entrada"]),
                    TotalTransferido = Convert.ToInt64(reader["transferido"]),                    
                    TotalRecibido = Convert.ToInt64(reader["recibido"]),
                    TotalAutorizado = Convert.ToInt64(reader["k_auto"]),
                    TotalLiquidado = Convert.ToInt64(reader["liquidado"])
                });
            }
            var tmpresult = from r in tmpResult group r by new {r.IdCuenta,r.IdCosecha} into g 
                                 select new SaldoCtaCteCereal
                                 {
                                     IdCosecha=g.Key.IdCosecha,IdCuenta=g.Key.IdCuenta,
                                     Entregado = g.Sum(s=>s.TotalEntradas),
                                     Retirado = g.Sum(s => s.TotalRetiros),
                                     Certificado = g.Sum(s => s.TotalCertificado),
                                     Recibido = g.Sum(s => s.TotalRecibido),
                                     Transferido = g.Sum(s => s.TotalTransferido),
                                     Autorizado = g.Sum(s => s.TotalAutorizado),
                                     Liquidado = g.Sum(s => s.TotalLiquidado),
                                     Disponible = 0,
                                     Saldo = 0
                                 };

            SujetoService sujetoService = new SujetoService(this.SaeConnectionStringBase);
            var sujetos = sujetoService.List();
            CosechaService cosechaService = new CosechaService(this.ConnectionStringBase);
            var cosechas = cosechaService.List();
            var result = tmpresult.ToList();
            foreach (var item in result) 
            {
                
                var cuenta = sujetos.Where(w=>w.Id==item.IdCuenta.Trim()).FirstOrDefault();                
                if (cuenta != null)
                {
                    item.Nombre = cuenta.Nombre;
                }                
                var cosecha = cosechas.Where(w=>w.Id== item.IdCosecha.Trim()).FirstOrDefault();
                if (cosecha != null)
                {
                    item.NombreCosecha = cosecha.Nombre;
                    item.NombreCereal = cosecha.NombreCereal;
                }
                if (this.TipoSaldo == "1")
                {
                    item.Saldo = item.Entregado + item.Recibido - (item.Transferido + item.Retirado + item.Liquidado);
                }
                else
                {
                    item.Saldo = item.Entregado + item.Recibido - (item.Transferido + item.Retirado + item.Autorizado);
                }
                item.Disponible = item.Entregado + item.Recibido - (item.Transferido + item.Retirado + item.Autorizado);
            }
                                
            cnn.Close();
            return result.ToList();

        }
        public SaldoCtaCteCereal Saldo(string idCuenta, string idCosecha, DateTime fecha) 
        {           
            SaldoCtaCteCereal result = new SaldoCtaCteCereal();
            SujetoService sujetoService = new SujetoService(this.SaeConnectionStringBase);
            var cuenta = sujetoService.FindOne(idCuenta);
            result.IdCuenta = idCuenta;
            if (cuenta != null)
            {                
                result.Nombre = cuenta.Nombre;                
            }
            CosechaService cosechaService = new CosechaService(this.ConnectionStringBase);
            var cosecha = cosechaService.FindOne(idCosecha);
            result.IdCosecha = idCosecha;
            result.NombreCosecha = cosecha.Nombre;
            result.NombreCereal = cosecha.NombreCereal;
            EntradaService entradaService = new EntradaService(this.ConnectionStringBase);
            result.Entregado = entradaService.Total(idCuenta,idCosecha,fecha);
            RetiroService retiroService = new RetiroService(this.ConnectionStringBase);
            result.Retirado = retiroService.Total(idCuenta, idCosecha, fecha);
            RTService rTService = new RTService(this.ConnectionStringBase);
            result.Transferido = rTService.TotalTransferido(idCuenta, idCosecha, fecha);
            result.Recibido = rTService.TotalRecibido(idCuenta, idCosecha, fecha);
            CertificadoService certificadoService = new CertificadoService(this.ConnectionStringBase);
            result.Certificado = certificadoService.Total(idCuenta, idCosecha, fecha);
            LiquidacionService liquiService = new LiquidacionService(this.ConnectionStringBase);
            result.Liquidado = liquiService.Total(idCuenta, idCosecha, fecha);
            BoletoService boletoService = new BoletoService(this.ConnectionStringBase);
            result.Autorizado = boletoService.Total(idCuenta, idCosecha, fecha);
            result.Disponible =  result.Entregado + result.Recibido - (result.Transferido + result.Retirado + result.Autorizado);
            if (this.TipoSaldo == "1")
            {
                result.Saldo = result.Entregado + result.Recibido - (result.Transferido + result.Retirado +  result.Liquidado );
            }
            else 
            {
                result.Saldo = result.Entregado + result.Recibido - (result.Transferido + result.Retirado + result.Autorizado );
            }
                return result;            
        }
        

    }

    class tmpSumaSaldo 
    {
        public string IdCosecha { get; set; }
        public string IdCuenta { get; set; }
        public Int64 TotalEntradas { get; set; }
        public  Int64 TotalRetiros { get; set; }
        public Int64 TotalCertificado { get; set; }
        public Int64 TotalTransferido { get; set; }
        public Int64 TotalRecibido { get; set; }
        public Int64 TotalLiquidado { get; set; }
        public Int64 TotalAutorizado { get; set; }

    }
    
}
