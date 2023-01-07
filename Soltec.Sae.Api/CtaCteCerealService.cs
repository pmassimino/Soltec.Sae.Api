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
        public List<MovCtaCteCereal> List(string idCuenta, string idCosecha, DateTime fecha) 
        {
            List<MovCtaCteCereal> result = new List<MovCtaCteCereal>();
            List<MovCtaCteCereal> tmpResult = new List<MovCtaCteCereal>();
            EntradaService entradaService = new EntradaService(this.ConnectionStringBase);            
            var entradas = entradaService.List(idCuenta, idCosecha,fecha);
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
                var cosecha = cosechaService.List().OrderBy(o => o.Id).TakeLast(15).ToList();
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
    
}
