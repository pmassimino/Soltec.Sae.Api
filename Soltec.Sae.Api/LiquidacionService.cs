using System.Data.OleDb;

namespace Soltec.Sae.Api
{
    public class LiquidacionService
    {
        public LiquidacionService(string connectionStringBase)
        {
            this.ConnectionStringBase = connectionStringBase;
        }
        public string ConnectionStringBase { get; set; } = "";
        public string IdSucursal { get; set; } = "01";

        public List<Liquidacion> List(string idCuenta , string idCosecha,DateTime fecha)
        {
            string connectionString = this.ConnectionStringBase + "Cereales.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT id_tip_liq, p_neto, f1116b, n1116b, f_auto, fecha, f_vto, pre_ref, str(val(liqui.grado),1,0) as Grado, factor, iv_bru, comis, flete, imp_comi, t_fle," +
                                  "impgen1, targen1, imp_gan, imp_iva2, imp_dgr,neto1, final, r4250, produ, cose, puerto, origen, n1116a, f1116a, pre_ope, canje," +
                                  "tipform, bolvta, sellado, cac, id_tipoliq, id_tipoajuste, coe, coe_ajustado, valorgrado,id_tipogrado, liqui.proteico, proced, " +
                                  "produmae.rsocial as NombreProductor,produmae.n_cuit as CuitProductor, " +
                                  "Cosechas.descri as NombreCosecha," +
                                  "cermae.cod_cer as idCereal,cermae.descri as NombreCereal " +
                                  "FROM liqui " +
                                  "LEFT JOIN Produmae on produmae.codigo = liqui.produ " +
                                  "LEFT JOIN Cosechas on cosechas.cod = liqui.cose " +
                                  "LEFT JOIN Cermae on cermae.cod_cer = cosechas.cereal " +
                                  "WHERE (produ = '" + idCuenta + "' OR empty('" + idCuenta + "')) and " +
                                  " (cose = '" + idCosecha + "' OR empty('" + idCosecha + "')) and " + 
                                   "fecha <= ctod('" + fecha.ToString("MM-dd-yyy") + "')"; 

            OleDbDataReader reader = command.ExecuteReader();
            List<Liquidacion> result = new List<Liquidacion>();
            while (reader.Read())
            {
                result.Add(this.Parse(reader));
            }

            //Deducciones
            reader.Close();
            foreach (var item in result)
            {
                command.CommandText = "Select id_liqui,id_concepto,nombre,alicuota,cantidad,base,alicuotaiva,importe,id_afip from deduccionliqui where id_liqui='" + item.Id + "'";
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    item.Deducciones.Add(this.ParseDeduccion(reader));
                }
                reader.Close();
            }
            //Retenciones
            reader.Close();
            foreach (var item in result)
            {
                command.CommandText = "Select id_liqui,id_concepto,nombre,base,alicuota,importe,id_afip " +
                                      "from retencionliqui where id_liqui='" + item.Id + "'";
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    item.Retenciones.Add(this.ParseRetencion(reader));
                }
                reader.Close();
            }
            //Certificados
            reader.Close();
            foreach (var item in result)
            {
                command.CommandText = "Select id_liqui,id_certi,fecha,numero,pesoneto,tipo,grado,valorgrado,factor,obs " +
                                      "from certiliqui where id_liqui='" + item.Id + "'";
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    item.Certificados.Add(this.ParseCertificado(reader));
                }
                reader.Close();
            }

            cnn.Close();
            return result;
        }        
        public Liquidacion FindOne(string id)
        {
            string connectionString = this.ConnectionStringBase + "Cereales.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT id_tip_liq, p_neto, f1116b, n1116b, f_auto, fecha, f_vto, pre_ref, str(val(liqui.grado),1,0) as Grado, factor, iv_bru, comis, flete, imp_comi, t_fle," +
                                  "impgen1, targen1, imp_gan, imp_iva2, imp_dgr,neto1, final, r4250, produ, cose, puerto, origen, n1116a, f1116a, pre_ope, canje," +
                                  "tipform, bolvta, sellado, cac, id_tipoliq, id_tipoajuste, coe, coe_ajustado, valorgrado,id_tipogrado, liqui.proteico, proced, " +
                                  "produmae.rsocial as NombreProductor,produmae.n_cuit as CuitProductor, " +
                                  "Cosechas.descri as NombreCosecha," +
                                  "cermae.cod_cer as idCereal,cermae.descri as NombreCereal " +
                                  "FROM liqui " +
                                  "LEFT JOIN Produmae on produmae.codigo = liqui.produ " +
                                  "LEFT JOIN Cosechas on cosechas.cod = liqui.cose " +
                                  "LEFT JOIN Cermae on cermae.cod_cer = cosechas.cereal " +
                                  "WHERE f1116b = '" + id + "'";

            OleDbDataReader reader = command.ExecuteReader();
            Liquidacion result = null;
            while (reader.Read())
            {
                result = this.Parse(reader);
            }
            //si no encuentra retornar
            if (result == null) return result;
            //Deducciones
            reader.Close();
            command.CommandText = "Select id_liqui,id_concepto,nombre,alicuota,cantidad,base,alicuotaiva,importe,id_afip from deduccionliqui where id_liqui='" + result.Id + "'";
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Deducciones.Add(this.ParseDeduccion(reader));
            }
            reader.Close();
            //Retenciones
            reader.Close();
           
            command.CommandText = "Select id_liqui,id_concepto,nombre,base,alicuota,importe,id_afip " +
                                      "from retencionliqui where id_liqui='" + result.Id + "'";
             reader = command.ExecuteReader();
             while (reader.Read())
               {
                result.Retenciones.Add(this.ParseRetencion(reader));
               }
             reader.Close();            
             //Certificados
             reader.Close();

            command.CommandText = "Select id_liqui,id_certi,fecha,numero,pesoneto,tipo,grado,valorgrado,factor,obs " +
                                   "from certiliqui where id_liqui='" + result.Id + "'";
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                    result.Certificados.Add(this.ParseCertificado(reader));
             }
            reader.Close();    
        cnn.Close();
        return result;
}
        public Int64 Total(string idCuenta, string idCosecha, DateTime fechaHasta)
        {
            Int64 result = 0;
            string connectionString = this.ConnectionStringBase + "Cereales.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT sum(p_neto) as Total " +
                                  "FROM Liqui " +
                                  "WHERE (produ = '" + idCuenta + "') and " +
                                  " (cose = '" + idCosecha + "' and fecha <=ctod('" + fechaHasta.ToString("MM-dd-yyy") + "')) and " +
                                  "(id_tip_liq = 1 or id_tip_liq = 3)";
            OleDbDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                result = reader["Total"].ToString() == "" ? 0 : Convert.ToInt64(reader["Total"].ToString());
            }
            cnn.Close();
            return result;
        }



        private Liquidacion Parse(OleDbDataReader reader)
        {
            Liquidacion item = new Liquidacion();
            item.IdSucursal = this.IdSucursal;
            item.Id = reader["f1116b"].ToString().Trim();
            item.IdTransaccion = "LIQUI;" + item.Id;
            item.IdTipo = Convert.ToInt16(reader["id_tip_liq"].ToString().Trim());
            item.Numero = reader["n1116b"].ToString().Trim();
            item.Fecha = (DateTime)reader["fecha"];
            item.FechaVencimiento = (DateTime)reader["f_vto"];
            item.IdCosecha = reader["cose"].ToString().Trim();
            item.NombreCosecha = reader["NombreCosecha"].ToString().Trim();
            item.NombreCereal = reader["NombreCereal"].ToString().Trim();         
            item.IdCuenta = reader["produ"].ToString().Trim();            
            item.Nombre = reader["NombreProductor"].ToString().Trim();
            item.PesoNeto = Convert.ToInt64(reader["p_neto"].ToString().Trim());
            item.Precio = (decimal)reader["pre_ref"];
            item.PrecioOperacion = (decimal)reader["pre_ope"];            
            item.NumeroCertificado = reader["f1116a"].ToString().Trim();
            item.Procedencia = reader["proced"].ToString().Trim();
            item.Puerto = "ROSARIO";
            item.TipoOperacion = reader["id_tipoliq"].ToString().Trim() == "1" ? "Compra Venta de Granos" : "Consignacion de Granos";
            item.Tipo = (bool)reader["canje"] == true ? "CANJE" : "";
            item.Actividad = "ACOPIADOR - CONSIGNATARIO";
            item.Coe = reader["coe"].ToString().Trim();
            item.CoeAjustado = reader["coe_ajustado"].ToString().Trim();
            item.Grado = String.IsNullOrEmpty(reader["Grado"].ToString().Trim()) ? 0 : Convert.ToInt16(reader["Grado"].ToString().Trim());
            item.Factor = (decimal)reader["factor"];
            item.ImporteIva = (decimal)reader["iv_bru"];            
            item.TarifaComision = (decimal)reader["comis"]; 
            item.TarifaFlete = (decimal)reader["flete"];
            item.ImporteComision = (decimal)reader["imp_comi"];
            item.ImporteFlete = (decimal)reader["t_fle"];
            //Reforma Para Coop Cruz Alta - Flete Como Deducción
            if (item.ImporteFlete == 0 && (decimal)reader["impgen1"] != 0) 
            {
                item.TarifaFlete = (decimal)(decimal)reader["targen1"];
                item.ImporteFlete = (decimal)reader["impgen1"];
            }
            item.RetGan = (decimal)reader["imp_gan"];
            item.RetIva = (decimal)reader["imp_iva2"];
            item.RetIb = (decimal)reader["imp_dgr"];
            item.ImporteFinal = (decimal)reader["neto1"];
            item.ImporteNeto = (decimal)reader["final"];
            item.ImporteRg2300 = (decimal)reader["r4250"];            
            return item;
        }
        private Retencion ParseRetencion(OleDbDataReader reader) 
        {            
            Retencion item = new Retencion();
            item.Nombre = reader["nombre"].ToString().Trim();
            item.Alicuota = (decimal)reader["Alicuota"];
            item.Codigo = reader["id_concepto"].ToString().Trim();                        
            item.Importe = (decimal)reader["Importe"];
            return item;
        }
        private Deduccion ParseDeduccion(OleDbDataReader reader)
        {
            Deduccion item = new Deduccion();
            item.Nombre = reader["nombre"].ToString().Trim();
            item.Alicuota = (decimal)reader["Alicuota"];
            item.Codigo = reader["id_concepto"].ToString().Trim();
            item.AlicuotaIva = (decimal)reader["AlicuotaIva"];
            item.ImporteBase = (decimal)reader["Base"];
            try
            {
                item.Importe = (decimal)reader["Importe"];
            }
            catch 
            {
            }
            return item;
        }
        private CertificadoLiquidacion ParseCertificado(OleDbDataReader reader)
        {
            CertificadoLiquidacion item = new CertificadoLiquidacion();
            item.Fecha = (DateTime)reader["fecha"];
            item.PesoNeto = Convert.ToInt64(reader["PesoNeto"]);
            item.Numero = Convert.ToInt64(reader["Numero"].ToString());
            item.Tipo = reader["Tipo"].ToString().Trim();
            item.Factor = (decimal)reader["Factor"];
            item.Grado = Convert.ToInt32(reader["Grado"].ToString());            
            return item;
        }
    }
}

