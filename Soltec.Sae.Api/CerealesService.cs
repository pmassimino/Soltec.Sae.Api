using System.Data.OleDb;

namespace Soltec.Sae.Api
{
    public class CerealesService
    {
        public CerealesService(string connectionStringBase) 
        {
            this.ConnectionStringBase = connectionStringBase;
        }        
        public string ConnectionStringBase { get; set; } = "";
        public string SaeConnectionStringBase { get; set; } = "";
        public List<Sucursal> Sucursales { get;set ; }
        public List<ItemPosicionFisica> PosicionFisica(DateTime fecha, DateTime  fechaHasta)
        {
            var result = new List<ItemPosicionFisica>();
            //Saldo Planta
            foreach (var suc in Sucursales)
            {
                //Existencia en Planta
                MovPlantaCerealService movPlantaService = new MovPlantaCerealService(suc.ConnectionStrings);
                var tmpSaldoPlanta = movPlantaService.Saldo("", "", fecha, fechaHasta);
                Int64 saldoSoja = tmpSaldoPlanta.Where(w => w.NombreCereal.Trim().Contains("SOJA")).Sum(s => s.Saldo);
                Int64 saldoTrigo = tmpSaldoPlanta.Where(w => w.NombreCereal.Trim().Contains("TRIGO")).Sum(s => s.Saldo);
                Int64 saldoMaiz = tmpSaldoPlanta.Where(w => w.NombreCereal.Trim().Contains("MAIZ")).Sum(s => s.Saldo);
                Int64 saldoGirasol = tmpSaldoPlanta.Where(w => w.NombreCereal.Trim().Contains("GIRASOL")).Sum(s => s.Saldo);
                var item = new ItemPosicionFisica { Concepto = "Existencia en Planta", Soja = saldoSoja, Maiz = saldoMaiz, Trigo = saldoTrigo, Girasol = saldoGirasol };
                result.Add(item);
                //Existencia de Productores
                CtaCteCerealService ctaCteCerealService = new CtaCteCerealService(suc.ConnectionStrings);
                ctaCteCerealService.SaeConnectionStringBase = this.SaeConnectionStringBase;
                ctaCteCerealService.ConnectionStringBase = this.ConnectionStringBase;
                var tmpSaldoCtaCte = ctaCteCerealService.Saldos("", "", fechaHasta);
                saldoSoja = tmpSaldoCtaCte.Where(w => w.NombreCereal.Trim().Contains("SOJA")).Sum(s => s.Saldo);
                saldoTrigo = tmpSaldoCtaCte.Where(w => w.NombreCereal.Trim().Contains("TRIGO")).Sum(s => s.Saldo);
                saldoMaiz = tmpSaldoCtaCte.Where(w => w.NombreCereal.Trim().Contains("MAIZ")).Sum(s => s.Saldo);
                saldoGirasol = tmpSaldoCtaCte.Where(w => w.NombreCereal.Trim().Contains("GIRASOL")).Sum(s => s.Saldo);
                item = new ItemPosicionFisica { Concepto = "Pendiente Liquidar Productores", Soja = saldoSoja, Maiz = saldoMaiz, Trigo = saldoTrigo, Girasol = saldoGirasol };
                result.Add(item);
                //Pendiente de Fijar
                ContratoService contratoService = new ContratoService(suc.ConnectionStrings);
                var tmpContratoAFijar = contratoService.ListEstado("", "", fecha, fechaHasta, "", "A FIJAR");
                saldoSoja = tmpContratoAFijar.Where(w => w.NombreCereal.Trim().Contains("SOJA")).Sum(s => s.PesoPendienteFijar);
                saldoTrigo = tmpContratoAFijar.Where(w => w.NombreCereal.Trim().Contains("TRIGO")).Sum(s => s.PesoPendienteFijar);
                saldoMaiz = tmpContratoAFijar.Where(w => w.NombreCereal.Trim().Contains("MAIZ")).Sum(s => s.PesoPendienteFijar);
                saldoGirasol = tmpContratoAFijar.Where(w => w.NombreCereal.Trim().Contains("GIRASOL")).Sum(s => s.PesoPendienteFijar);
                item = new ItemPosicionFisica { Concepto = "Pendiente Fijar Contratos", Soja = saldoSoja, Maiz = saldoMaiz, Trigo = saldoTrigo, Girasol = saldoGirasol };
                result.Add(item);

            }
            return result;
        }
     
    }

   
}

