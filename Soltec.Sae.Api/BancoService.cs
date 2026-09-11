using System.Data.OleDb;

namespace Soltec.Sae.Api
{
    public class LibroBancoService
    {         
        public LibroBancoService(string connectionStringBase) 
        {
            this.ConnectionStringBase = connectionStringBase;
        }
        public string ConnectionStringBase { get; set; } = "";



        public List<LibroBanco> List(DateTime fecha, DateTime fechaHasta, string idCuentaMayor, bool filtraConciliado = false, bool conciliado = false)
        {
            SujetoService sujetoService = new SujetoService(this.ConnectionStringBase);
            string connectionString = this.ConnectionStringBase + "sae.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();

            // 1. Base de la consulta SQL
            string query = "SELECT suc,ntra,item,per,fpas,fvto,pvta,fcom,fconci,ccpte,pvta,ncpte,cmay," +
                           "t1,scta,con,tramay.tip,imp,can,div,cotiz,obn,ncom2,clipro.nom as NombreSujeto,ctasmg.nom as NombreCuentaMayor " +
                           "FROM tramay " +
                           "LEFT JOIN clipro ON clipro.cod = tramay.scta " +
                           "LEFT JOIN ctasmg on ctasmg.cod = tramay.cmay " +
                           "WHERE (fpas BETWEEN ctod('" + fecha.ToString("MM-dd-yyyy") + "')" +
                           " AND ctod('" + fechaHasta.ToString("MM-dd-yyyy") + "'))";

            // 2. Aplicar filtro por idCuentaMayor (si corresponde, aunque no estaba en tu WHERE original, es buena práctica si viene como parámetro)
            if (!string.IsNullOrEmpty(idCuentaMayor))
            {
                query += " AND cmay = '" + idCuentaMayor + "'";
            }

            // 3. Aplicar filtros de conciliación según las banderas booleanas
            if (filtraConciliado)
            {
                if (conciliado)
                {
                    // Filtra solo los conciliados (en FoxPro/OleDb se usa .T.)
                    query += " AND t1 = .T.";
                }
                else
                {
                    // Filtra solo los NO conciliados (en FoxPro/OleDb se usa .F.)
                    query += " AND t1 = .F.";
                }
            }

            // 4. Agregar el ordenamiento al final
            query += " ORDER BY fpas,suc,ntra,tramay.tip";

            command.CommandText = query;

            OleDbDataReader reader = command.ExecuteReader();
            List<LibroBanco> result = new List<LibroBanco>();
            LibroBanco item = new LibroBanco();

            while (reader.Read())
            {
                item = Parse(reader);
                result.Add(item);
            }

            reader.Close();
            cnn.Close();
            return result;
        }

        private LibroBanco Parse(OleDbDataReader reader)
        {
            LibroBanco item = new LibroBanco();
            item.IdSeccion = "";
            item.IdSucursal = reader["suc"].ToString().Trim();
            item.IdTransaccion = reader["ntra"].ToString().Trim();            
            item.Fecha = (DateTime)reader["fpas"];
            item.FechaComprobante = (DateTime)reader["fcom"];
            item.FechaVencimiento = (DateTime)reader["fvto"];
            item.FechaConciliado = (DateTime)reader["fconci"];
            item.Pe = reader["pvta"].ToString().Trim() == "" ? 0 : Convert.ToInt16(reader["pvta"]);
            item.Numero = reader["ncpte"].ToString().Trim() == "" ? 0 : Convert.ToInt32(reader["ncpte"]);
            item.Concepto = reader["con"].ToString().Trim();
            item.IdCuentaMayor = reader["cmay"].ToString().Trim();
            item.NombreCuentaMayor = reader["NombreCuentaMayor"].ToString().Trim();
            item.IdComprobante = reader["ccpte"].ToString().Trim();            
            item.Importe = (decimal)(reader["imp"]);
            item.Conciliado = (bool)reader["t1"];
            string tipo = reader["tip"].ToString();
            item.Tipo = tipo =="1"? "Ingreso" : "Egreso";
            return item;
        }
        
      
    }
}
