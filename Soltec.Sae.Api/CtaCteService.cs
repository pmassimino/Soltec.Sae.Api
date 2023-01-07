using System.Data;
using System.Data.OleDb;

namespace Soltec.Sae.Api
{
    public class CtaCteService
    {
        public CtaCteService(string connectionStringBase) 
        {
            this.ConnectionStringBase = connectionStringBase;
        }
        public string ConnectionStringBase { get; set; } = "";
       
        public List<MovCtaCte> List(string idCuenta, string idCuentaMayor, DateTime fecha,DateTime fechaHasta,int idDivisa = 0) 
        {
            string connectionString = this.ConnectionStringBase + "sae.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT can, ccpte, cmay, con, cotiz, fcom, fpas, fvto, imp, impd, morig, ncpte, ntra, obn, org, pvta, scta, suc, tip, usu " +
                "                  FROM trasub " +
                "                  WHERE (scta = '" + idCuenta + "') AND (cmay = '" + idCuentaMayor + "') AND (fvto >= ctod('" + fecha.ToString("MM-dd-yyy") + "')" 
                                         + " AND fpas <= ctod('" + fechaHasta.ToString("MM-dd-yyy") + "')) ORDER BY fvto, org, usu, ntra";
            OleDbDataReader reader = command.ExecuteReader();
            List<MovCtaCte> result = new List<MovCtaCte>();
            while (reader.Read())
            {
                decimal importe = idDivisa == 0 ? (decimal)reader["imp"] : (decimal)reader["imd"];
                var item = new MovCtaCte
                {
                    IdCuenta = reader["scta"].ToString().Trim(),
                    Concepto = reader["con"].ToString().Trim(),
                    FechaComprobante = (DateTime)reader["fcom"],
                    FechaPase = (DateTime)reader["fpas"],
                    FechaVencimiento = (DateTime)reader["fvto"],
                    Debe = (decimal)reader["tip"] == 1 ? importe : 0,
                    Haber = (decimal)reader["tip"] == 2 ? importe : 0,
                    NumeroComprobante = reader["pvta"].ToString().Trim().PadLeft(4, '0') + "-" + reader["ncpte"].ToString().Trim().PadLeft(8, '0'),
                    Cotizacion = (decimal)reader["cotiz"],
                    ImpD = (decimal)reader["impd"],
                    Tipo = ""
                };
                //Generar Transaccion a partir del comprobante
                string pidComp = reader["ccpte"].ToString().Trim();
                ComprobanteService compService = new ComprobanteService(this.ConnectionStringBase);
                var comprobante = compService.FindOne(pidComp);
                string IdTipo = comprobante==null?"":comprobante.IdTipo;
                string report = comprobante==null?"":comprobante.Report;
                if (IdTipo == "FAC")
                {
                    string idTra = "FACTURA" + ";" + "001" + ";" + reader["NTRA"].ToString().Trim() + ";" + reader["ncpte"].ToString().Trim() + ";";
                    item.IdTransaccion = idTra;
                    item.Tipo = "FACTU";
                    item.TieneComp = true;
                }
                else if (!String.IsNullOrEmpty(report))
                {
                    string idTra = "ASIENTO" + ";" + "001" + ";" + reader["NTRA"].ToString().Trim() + ";" + reader["ncpte"].ToString().Trim() + ";" + reader["usu"].ToString().Trim();
                    item.IdTransaccion = idTra;
                    item.Tipo = "ASIENTO";
                    item.TieneComp = true;
                }
                else if (pidComp == "060")
                {
                    string tmpNumero = reader["pvta"].ToString().Trim().PadLeft(4, '0') + reader["ncpte"].ToString().Trim().PadLeft(8, '0');
                    string idTra = "LIQUI" + ";" + "001" + ";" + "0" + ";" + tmpNumero;
                    item.IdTransaccion = idTra;
                    item.Tipo = "LIQUI";
                    item.TieneComp = true;
                }
                else if (pidComp == "061")
                {
                    string tmpNumero = reader["pvta"].ToString().Trim().PadLeft(4, '0') + reader["ncpte"].ToString().Trim().PadLeft(8, '0');
                    string idTra = "CERTI" + ";" + "001" + ";" + "0" + ";" + tmpNumero;
                    item.IdTransaccion = idTra;
                    item.Tipo = "CERTI";
                    item.TieneComp = true;
                }
                else
                {
                    string idTra = "NoData" + ";" + reader["suc"].ToString().Trim() + ";" + reader["ntra"].ToString().Trim();
                    item.IdTransaccion = idTra;
                    item.TieneComp = false;
                }
                result.Add(item);
            }
            //Calcular Saldo
            decimal saldoVencido = 0;            
            saldoVencido = this.Saldo(idCuenta, idCuentaMayor, fecha.AddDays(-1),idDivisa,true);            

            decimal saldoAVencer = 0;
            decimal saldo = saldoVencido;
            //Agregar Saldo Inicial
            MovCtaCte itemSaldo  =  new MovCtaCte();
            itemSaldo.Concepto = "SaldoAnterior";
            itemSaldo.Saldo = saldo;
            itemSaldo.SaldoVencido = saldoVencido;
            itemSaldo.FechaVencimiento = fecha;
            itemSaldo.FechaComprobante = fecha;
            itemSaldo.FechaPase = fecha;
            itemSaldo.IdDivisa = idDivisa;
            itemSaldo.Cotizacion = 1;
            result.Insert(0, itemSaldo);
            int orden = 0;
            foreach (var item in result) 
            {
                saldo = item.Debe - item.Haber + saldo;
                item.Saldo = saldo;
                //Saldo Vencido                
                if (item.FechaVencimiento <= fechaHasta)
                    {
                    saldoVencido = item.Debe - item.Haber + saldoVencido;
                    item.Vencido = true;
                }
                saldoAVencer = saldo-saldoVencido;
                item.SaldoVencido = saldoVencido;
                item.SaldoAVencer = saldoAVencer;
                orden += 1;
                item.Orden = orden;
            }
            cnn.Close();
            return result;
        }
        public decimal Saldo(string idCuenta, string idCuentaMayor, DateTime Fecha,int idDivisa = 0,bool vencido = false) 
        {
            string connectionString = this.ConnectionStringBase + "sae.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            string campoFecha = vencido ? "fvto" : "fpas";
            string campoImporte = idDivisa == 0 ? "imp" : "impd";
            command.CommandText = "SELECT tip,imp FROM trasub WHERE (scta ='" +  idCuenta + "') AND (cmay ='" + idCuentaMayor + "') AND (" + campoFecha + "<= ctod('" + Fecha.ToString("MM-dd-yyy") + "'))"; //
            OleDbDataReader reader = command.ExecuteReader();
            decimal result = 0;
            while (reader.Read())
            {                 
                result += (decimal)reader["tip"]==1?(decimal)reader[campoImporte] : -(decimal)reader[campoImporte];
            }
            cnn.Close();
            return result;        }
        public List<SaldoCtaCte> Saldos(string idCuenta, string idCuentaHasta, string idCuentaMayor, DateTime fecha, int idDivisa)
        {
            
            string connectionString = this.ConnectionStringBase + "sae.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);

            //Agregar 1 dia para corregir bug
            //fecha = fecha.AddDays(1);
            // Saldo Vencido
            OleDbCommand command = cnn.CreateCommand();
            string campoFecha = "fvto"; //: "fpas";

            cnn.Open();
            command.CommandText = "SELECT scta, clipro.nom as nombre ,cmay,SUM(IIF(tip = 1, imp, - imp)) AS Saldo FROM trasub LEFT JOIN clipro ON clipro.cod = scta GROUP BY scta,nom,cmay WHERE( " + campoFecha + " <=ctod('" + fecha.ToString("MM-dd-yyyy") + "')) AND (cmay ='" + idCuentaMayor + "') AND (scta >='" + idCuenta + "') AND (scta <='" + idCuentaHasta + "') having  SUM(IIF(tip = 1, imp, - imp)) <> 0";                        
            OleDbDataReader reader = command.ExecuteReader();
            List<SaldoCtaCte> tmpResultVencido = new List<SaldoCtaCte>();
            while (reader.Read())
            {
                SaldoCtaCte item = new SaldoCtaCte();
                item.IdCuentaMayor = reader["cmay"].ToString().Trim();
                item.IdCuenta = reader["scta"].ToString().Trim();
                item.Nombre = reader["nombre"].ToString().Trim();                
                item.SaldoVencido = (decimal)reader["saldo"];
                tmpResultVencido.Add(item);
            }
            reader.Close();
            // Saldo             
            campoFecha =  "fpas";
            command.CommandText = "SELECT scta, clipro.nom as nombre ,cmay,SUM(IIF(tip = 1, imp, - imp)) AS Saldo FROM trasub LEFT JOIN clipro ON clipro.cod = scta GROUP BY scta,nom,cmay WHERE( " + campoFecha + " <=ctod('" + fecha.ToString("MM-dd-yyyy") + "')) AND (cmay ='" + idCuentaMayor + "') AND (scta >='" + idCuenta + "') AND (scta <='" + idCuentaHasta + "') having  SUM(IIF(tip = 1, imp, - imp)) <> 0";
            reader = command.ExecuteReader();
            List<SaldoCtaCte> tmpResultSaldo = new List<SaldoCtaCte>();
            while (reader.Read())
            {
                SaldoCtaCte item = new SaldoCtaCte();
                item.IdCuentaMayor = reader["cmay"].ToString().Trim();
                item.IdCuenta = reader["scta"].ToString().Trim();
                item.Nombre = reader["nombre"].ToString().Trim();
                item.Saldo = (decimal)reader["saldo"];
                tmpResultSaldo.Add(item);
            }
            reader.Close();
            //Combinar Saldos
            var result = from s in tmpResultSaldo
                                 join sv in tmpResultVencido on s.IdCuenta equals sv.IdCuenta into details
                                 select new SaldoCtaCte 
                                 { IdCuenta = s.IdCuenta, Nombre = s.Nombre, IdCuentaMayor = s.IdCuentaMayor, Saldo = s.Saldo, SaldoVencido = details.Sum(s => s.SaldoVencido), IdDivisa = s.IdDivisa };


            cnn.Close();
            return result.ToList();
        }
        public Int32 DiasDeuda(string idCuenta, string idCuentaMayor)
        {
            DateTime fecha = DateTime.Now.AddDays(-365);
            DateTime fechaHasta = DateTime.Now;
            var tmpMov = this.List(idCuenta, idCuentaMayor, fecha, fechaHasta);
            var tmpMovFinal = tmpMov.OrderByDescending(o => o.Orden).ToList();
            DateTime UltimaFecha = fecha;
            //Calcular Saldo
            var registarFecha = false;
            MovCtaCte tmpMovAnterior = null;
            foreach (var item in tmpMovFinal) 
            {                
                if (item.Saldo <= 0) 
                {
                    UltimaFecha = tmpMovAnterior == null ? DateTime.Now : tmpMovAnterior.FechaVencimiento;
                    break;
                }
                tmpMovAnterior = item;
            }
            Int32 result = (DateTime.Now - UltimaFecha).Days;           
            return result;
        }






    }
}
