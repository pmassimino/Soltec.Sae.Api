using System.ComponentModel;
using ModelContextProtocol.Server;
using Soltec.Sae.Api;

[McpServerToolType]
public class SujetosTools
{
    [McpServerTool, Description(
        "Devuelve el listado de sujetos , que puedesn ser empresas , clientes, proveedores,productores agropecuarios " +
       "su id es el id de la cuenta en SAE, y el nombre es el nombre del sujeto. "
    )]
    public static List<Sujeto> Sujeto(IConfiguration configuration )
    {
        var connectionString = configuration["ConnectionStringsSAE"];
        var service = new SujetoService(connectionString);
        var result = service.List();
        return result;
    }

    [McpServerTool, Description(
        "Devuelve el resumen de cuenta de un sujeto (cliente, proveedor o productor agropecuario): " +
        "saldo de cuenta corriente por subdiario y divisa, saldos de cosecha (granos) por sucursal, " +
        "y facturas y remitos pendientes. Usar cuando el usuario pregunte por el resumen, saldo o " +
        "estado de cuenta de un sujeto. Devuelve null si la cuenta no existe."
    )]
    public static ResumenView? ResumenCuenta(
        IConfiguration configuration, // <- inyectado por DI, no aparece como parámetro para la IA
        List<Sucursal> sucursales, // <- inyectado por DI, no aparece como parámetro para la IA
        [Description("Id de cuenta del sujeto en el sistema SAE.")] string idCuenta,
        [Description("Fecha para el cálculo de saldos de cosecha, formato MM-dd-yyyy. Vacío = hoy")] string? fecha = null)
    {
        var connectionStringBase = configuration["ConnectionStringsSAE"];
        var tipoSaldo = configuration["TipoSaldo"];
        var seccionPendiente = configuration.GetSection("SeccionPendiente").GetChildren().ToList().Select(x => new Seccion
        {
            Id = x.GetValue<string>("Id"),
            Nombre = x.GetValue<string>("Nombre"),
        }).ToList();

        var f = string.IsNullOrEmpty(fecha)
            ? DateTime.Now
            : DateTime.ParseExact(fecha, "MM-dd-yyyy", System.Globalization.CultureInfo.InvariantCulture);

        var sujetoService = new SujetoService(connectionStringBase);
        var tmpCuenta = sujetoService.FindOne(idCuenta);
        if (tmpCuenta == null)
        {
            return null;
        }

        var result = new ResumenView { Sujeto = tmpCuenta };

        // Saldos Cta. Cte.
        var ctaCteService = new CtaCteService(connectionStringBase);
        foreach (var item in tmpCuenta.Subdiarios)
        {
            int idDivisa = Convert.ToInt16(item.IdDivisa);
            var saldoVencido = ctaCteService.Saldo(idCuenta, item.Id, DateTime.Now, idDivisa, true);
            var saldo = ctaCteService.Saldo(idCuenta, item.Id, DateTime.Now, idDivisa, false);

            result.CtaCte.Add(new CtaCteView
            {
                IdCuenta = idCuenta,
                IdSubdiario = item.Id,
                Nombre = item.Nombre,
                IdDivisa = idDivisa,
                Saldo = saldo,
                SaldoVencido = saldoVencido
            });
        }

        // Saldos Cosechas
        var tmpSaldos = new List<SaldoCtaCteCereal>();
        foreach (var suc in sucursales)
        {
            var service = new CtaCteCerealService(suc.ConnectionStrings)
            {
                SaeConnectionStringBase = connectionStringBase,
                IdSucursal = suc.Id,
                TipoSaldo = tipoSaldo
            };
            tmpSaldos.AddRange(service.Saldos(idCuenta, null, f));
        }
        foreach (var item in tmpSaldos)
        {
            result.Cosechas.Add(new CosechaView
            {
                IdSucursal = item.IdSucursal,
                IdCosecha = item.IdCosecha,
                Nombre = item.NombreCosecha,
                Saldo = item.Saldo
            });
        }

        // Facturas y Remitos Pendientes
        //Facturas y Remitos Pendientes
        DateTime fechaDesde = f.AddYears(-3);
        FacturaService facturaService = new FacturaService(connectionStringBase);
        facturaService.SeccionPendiente = seccionPendiente;
        var tmpFacturasPendientes = facturaService.ListPendiente(idCuenta, fechaDesde, f);

        RemitoService remitoService = new RemitoService(connectionStringBase);
        var tmpRemitosPendientes = remitoService.ListPendiente(idCuenta, fechaDesde, f);
        result.RemitosPendientes = tmpRemitosPendientes;
        result.FacturasPendientes = tmpFacturasPendientes;

        return result;
    }
}