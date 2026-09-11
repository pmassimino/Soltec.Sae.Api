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

    
   
}