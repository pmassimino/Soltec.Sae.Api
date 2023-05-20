using System.Data;
using System.Data.OleDb;

namespace Soltec.Sae.Api
{
    public class MovPlantaCerealService
    {
        public MovPlantaCerealService(string connectionStringBase) 
        {
            this.ConnectionStringBase = connectionStringBase;
        }
        public string ConnectionStringBase { get; set; } = "";        
        public string IdSucursal { get; set; } = "01";
        public List<MovPlantaCereal> List(string idPlanta, string idCosecha, DateTime fecha) 
        {
            return null;
        }
        public List<C14View> C14(string idPlanta, string idCosecha, DateTime fecha,DateTime fechaHasta) 
        {
            List<C14View> result = new List<C14View>();
            List<C14View> tmpResult = new List<C14View>();

            CosechaService cosechaService = new CosechaService(this.ConnectionStringBase);
            var cosechas = cosechaService.List().Where(c => c.Id == idCosecha || string.IsNullOrEmpty(idCosecha)).ToList();
            PlantaService plantaService = new PlantaService(this.ConnectionStringBase);
            var plantas = plantaService.List().Where(w=>w.Id==idPlanta || string.IsNullOrEmpty(idPlanta)).ToList(); 
            EntradaService entradaService = new EntradaService(this.ConnectionStringBase);
            RetiroService retiroService = new RetiroService(this.ConnectionStringBase);
            SalidaService salidaService = new SalidaService(this.ConnectionStringBase);
            foreach (var planta in plantas)
            {
                foreach (var cosecha in cosechas)
                {
                    //Saldo Anterior
                    Int64 totalentradas = entradaService.TotalPlanta(planta.Id, cosecha.Id, DateTime.Now.AddYears(-1000), fecha.AddDays(-1));
                    Int64 totalSalidas = salidaService.TotalPlanta(planta.Id, cosecha.Id, DateTime.Now.AddYears(-1000), fecha.AddDays(-1));
                    Int64 totalRetiros = retiroService.TotalPlanta(planta.Id, cosecha.Id, DateTime.Now.AddYears(-1000), fecha.AddDays(-1));
                    Int64 saldoAnterior = totalentradas - (totalSalidas + totalRetiros);

                    totalentradas = entradaService.TotalPlanta(planta.Id, cosecha.Id, fecha, fechaHasta);
                    totalSalidas = salidaService.TotalPlanta(planta.Id, cosecha.Id, fecha, fechaHasta);
                    totalRetiros = retiroService.TotalPlanta(planta.Id, cosecha.Id, fecha, fechaHasta);
                    Int64 saldo = saldoAnterior + totalentradas - (totalSalidas + totalRetiros);
                    C14View newitem = new C14View();
                    newitem.IdPlanta = planta.Id;
                    newitem.IdCosecha = cosecha.Id;
                    newitem.Cosecha = cosecha;
                    newitem.SaldoAnterior = saldoAnterior;
                    newitem.Ingresos = totalentradas;
                    newitem.Egresos = totalRetiros + totalSalidas;
                    newitem.Saldo = saldo;
                    newitem.IdSucursal = this.IdSucursal;
                    if(newitem.Saldo != 0 || newitem.Ingresos != 0 || newitem.Egresos != 0)
                       tmpResult.Add(newitem);
                }
            }
            result = tmpResult.OrderBy(o=>o.IdPlanta).OrderBy(o=>o.Cosecha.Nombre).ToList();   
            return result;
        }

        public List<SaldoPlantaView> Saldo(string idPlanta, string idCosecha, DateTime fecha, DateTime fechaHasta)
        {
            List<SaldoPlantaView> result = new List<SaldoPlantaView>();
            List<SaldoPlantaView> tmpResult = new List<SaldoPlantaView>();

            CosechaService cosechaService = new CosechaService(this.ConnectionStringBase);
            var cosechas = cosechaService.List().Where(c => c.Id == idCosecha || string.IsNullOrEmpty(idCosecha)).ToList();
            PlantaService plantaService = new PlantaService(this.ConnectionStringBase);
            var plantas = plantaService.List().Where(w => w.Id == idPlanta || string.IsNullOrEmpty(idPlanta)).ToList();
            EntradaService entradaService = new EntradaService(this.ConnectionStringBase);
            RetiroService retiroService = new RetiroService(this.ConnectionStringBase);
            SalidaService salidaService = new SalidaService(this.ConnectionStringBase);
            foreach (var planta in plantas)
            {
                foreach (var cosecha in cosechas)
                {
                    //Saldo Anterior
                    Int64 totalentradas = entradaService.TotalPlanta(planta.Id, cosecha.Id, DateTime.Now.AddYears(-1000), fecha.AddDays(-1));
                    Int64 totalSalidas = salidaService.TotalPlanta(planta.Id, cosecha.Id, DateTime.Now.AddYears(-1000), fecha.AddDays(-1));
                    Int64 totalRetiros = retiroService.TotalPlanta(planta.Id, cosecha.Id, DateTime.Now.AddYears(-1000), fecha.AddDays(-1));
                    Int64 saldoAnterior = totalentradas - (totalSalidas + totalRetiros);

                    totalentradas = entradaService.TotalPlanta(planta.Id, cosecha.Id, fecha, fechaHasta);
                    totalSalidas = salidaService.TotalPlanta(planta.Id, cosecha.Id, fecha, fechaHasta);
                    totalRetiros = retiroService.TotalPlanta(planta.Id, cosecha.Id, fecha, fechaHasta);
                    Int64 saldo = saldoAnterior + totalentradas - (totalSalidas + totalRetiros);
                    SaldoPlantaView newitem = new SaldoPlantaView();
                    newitem.IdSucursal = this.IdSucursal;
                    newitem.IdPlanta = planta.Id;
                    newitem.IdCosecha = cosecha.Id;
                    newitem.NombreCosecha = cosecha.Nombre;
                    newitem.NombreCereal = cosecha.NombreCereal;
                    newitem.Saldo = saldo;
                    if (newitem.Saldo != 0)
                        tmpResult.Add(newitem);
                }
            }
            result = tmpResult.OrderBy(o => o.IdPlanta).OrderBy(o => o.NombreCosecha).ToList();
            return result;
        }


    }
    public class C14View 
    {
        public string IdSucursal { get; set; } = "01";
        public string IdCosecha { get; set; }
        public string IdPlanta { get; set; }
        public Cosecha Cosecha { get; set; }    
        public Int64 SaldoAnterior { get; set; }
        public Int64 Ingresos { get; set; }
        public Int64 Egresos { get; set; }
        public Int64 Saldo { get; set; }
    }
    public class SaldoPlantaView
    {
        public string IdSucursal { get; set; } = "01";
        public string IdCosecha { get; set; }
        public string IdPlanta { get; set; }
        public string NombreCosecha { get; set; } 
        public string NombreCereal { get; set; }
        public Int64 Saldo { get; set; }
    }

}
