using System.Data.OleDb;

namespace Soltec.Sae.Api
{
    public class CertificadoService
    {
        public CertificadoService(string connectionStringBase) 
        {
            this.ConnectionStringBase = connectionStringBase;
        }
        public string ConnectionStringBase { get; set; } = "";
        public string IdSucursal { get; set; } = "01";

        public List<Certificado> List(string idCuenta , string idCosecha ,DateTime fecha) 
        {
            string connectionString = this.ConnectionStringBase + "Cereales.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT  acarreo, acopio, almacenaje, certi.analisis, b_iva1, b_iva2, base_imp1, boletin, cac, certi.cereal, cform, coe, cosecha, dto_des, dto_ori, estado, f1116a, f_calidad, f_cierre, " + 
                         "f_condi, f_disconf, f_pago, factor, flete, fpase, g_gles, gastos, certi.grado, i_gg, i_iva1, i_iva2, i_sec, i_sel, i_total, i_zar,i_aca, k_liqui, k_pendi, k_transf, k_vta, l_entrega, lugar, " +
                         "m_cal, m_sec, m_vol, m_zar, n1116a, otros, p_bruto, p_neto, pcia_des, pcia_ori, peso_bloq, productor," +
                         "produmae.rsocial as NombreProductor,produmae.n_cuit as CuitProductor,certi.proteico,certi.sec_d,certi.sec_h, certi.tar_exc," +
                         " tar_imp1, tar_sec, tarifa,Cosechas.descri as NombreCosecha, " + 
                         " tipo_cer, tot_gastos, total_ana,total_imp1, zarandeo, t_fumi, t_extras, t_mermas,cermae.descri as NombreCereal " + 
                         "FROM certi " +
                         "LEFT JOIN Produmae on produmae.codigo = certi.productor " + 
                         "LEFT JOIN Cosechas on cosechas.cod = certi.cosecha " +
                         "LEFT JOIN Cermae on cermae.cod_cer = cosechas.cereal " + 
                         "WHERE (productor = '" + idCuenta + "' OR empty('" + idCuenta + "')) and " +
                         " (cosecha = '" + idCosecha + "' OR empty('" + idCosecha + "')) and " +
                         "fpase <= ctod('" + fecha.ToString("MM-dd-yyy") + "')";

            OleDbDataReader reader = command.ExecuteReader();
            List<Certificado> result = new List<Certificado>();
            while (reader.Read())
            {
                result.Add(this.Parse(reader));
            }

            //Analisis
            reader.Close();
            foreach (var item in result) 
            {
                command.CommandText = "Select f1116a,rubro,porc_r,bon_r,reb_r,cod_rub,grado from ana_det where f1116a='" + item.Id + "'";
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    item.DetalleAnalisis.Add(this.ParseAnalisis(reader));
                }
                reader.Close();
            }
            //Romaneos
            reader.Close();
            foreach (var item in result)
            {
                command.CommandText = "Select f1116a,fecha,n_rom,k_bru,m_zar,t_zar,i_zar,p_hum,m_hum,i_sec,t_sec,m_vol,tarifa2,importe2,i_fum,i_hs,pe_cp,n_cre,c_res,num_rom,tarifa " + 
                                      "from rem_det where f1116a='" + item.Id + "'";
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    item.DetalleRomaneo.Add(this.ParseRomaneo(reader));
                }
                reader.Close();
            }

            cnn.Close();
            return result;
        }
        public Certificado FindOne(string id) 
        {

            string connectionString = this.ConnectionStringBase + "Cereales.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT  acarreo, acopio, almacenaje, certi.analisis, b_iva1, b_iva2, base_imp1, boletin, cac, certi.cereal, cform, coe, cosecha, dto_des, dto_ori, estado, f1116a, f_calidad, f_cierre, " +
                         "f_condi, f_disconf, f_pago, factor, flete, fpase, g_gles, gastos, certi.grado, i_gg, i_iva1, i_iva2, i_sec, i_sel, i_total, i_zar,i_aca, k_liqui, k_pendi, k_transf, k_vta, l_entrega, lugar, " +
                         "m_cal, m_sec, m_vol, m_zar, n1116a, otros, p_bruto, p_neto, pcia_des, pcia_ori, peso_bloq, productor," +
                         "produmae.rsocial as NombreProductor,produmae.n_cuit as CuitProductor,certi.proteico,certi.sec_d,certi.sec_h, certi.tar_exc," +
                         " tar_imp1, tar_sec, tarifa,Cosechas.descri as NombreCosecha, " +
                         " tipo_cer, tot_gastos, total_ana,total_imp1, zarandeo, t_fumi, t_extras, t_mermas,cermae.descri as NombreCereal " +
                         "FROM certi " +
                         "LEFT JOIN Produmae on produmae.codigo = certi.productor " +
                         "LEFT JOIN Cosechas on cosechas.cod = certi.cosecha " +
                         "LEFT JOIN Cermae on cermae.cod_cer = cosechas.cereal " +
                         "WHERE f1116a = '" + id + "'";

            OleDbDataReader reader = command.ExecuteReader();
            Certificado result = null;
            while (reader.Read())
            {
                result = this.Parse(reader);
            }
            //Retornar si es nulo
            if (result == null) return result;
            //Analisis
            reader.Close();
            command.CommandText = "Select f1116a,rubro,porc_r,bon_r,reb_r,cod_rub,grado from ana_det where f1116a='" + result.Id + "'";
            reader = command.ExecuteReader();
            while (reader.Read())
            {
              result.DetalleAnalisis.Add(this.ParseAnalisis(reader));
            }
            reader.Close();            
            //Romaneos
            reader.Close();
            
            command.CommandText = "Select f1116a,fecha,n_rom,k_bru,m_zar,t_zar,i_zar,p_hum,m_hum,i_sec,t_sec,m_vol,tarifa2,importe2,i_fum,i_hs,pe_cp,n_cre,c_res,num_rom,tarifa " +
                                  "from rem_det where f1116a='" + result.Id + "'";
            reader = command.ExecuteReader();
            while (reader.Read())
            {
              result.DetalleRomaneo.Add(this.ParseRomaneo(reader));
            }
            reader.Close();
            cnn.Close();
            return result;
        }
        public Int64 Total(string idCuenta, string idCosecha,DateTime fechaHasta) 
        {
            Int64 result = 0;
            string connectionString = this.ConnectionStringBase + "Cereales.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT sum(p_neto) as Total " +
                                  "FROM Certi " +
                                  "WHERE (productor = '" + idCuenta + "') and " +
                                  " (cosecha = '" + idCosecha + "' and fpase <=ctod('" + fechaHasta.ToString("MM-dd-yyy") + "'))";
                                  
            OleDbDataReader reader = command.ExecuteReader();           
            while (reader.Read())
            {
                result = reader["Total"].ToString()== ""?0:Convert.ToInt64(reader["Total"].ToString());
            }
            cnn.Close();
            return result;
        }

        private Certificado Parse(OleDbDataReader reader)
        {
            Certificado item = new Certificado();
            item.IdSucursal = this.IdSucursal;
            item.Id = reader["f1116a"].ToString().Trim();
            item.IdTransaccion = "CERTI;" + item.Id;
            item.Numero = reader["n1116a"].ToString().Trim();
            item.Fecha = (DateTime)reader["fpase"];
            item.IdCosecha = reader["cosecha"].ToString().Trim();            
            item.NombreCosecha = reader["NombreCosecha"].ToString().Trim();
            item.NombreCereal = reader["NombreCereal"].ToString().Trim();            
            item.IdCuenta = reader["productor"].ToString().Trim();
            item.Nombre = reader["NombreProductor"].ToString().Trim();            
            item.PesoBruto = Convert.ToInt64(reader["p_bruto"].ToString().Trim());
            item.MermaCalidad = Convert.ToInt32(reader["m_cal"].ToString().Trim());
            item.MermaSecado = Convert.ToInt32(reader["m_sec"].ToString().Trim());
            item.MermaVolatil = Convert.ToInt32(reader["m_vol"].ToString().Trim());
            item.MermaZaranda = Convert.ToInt32(reader["m_zar"].ToString().Trim());
            item.PesoNeto = Convert.ToInt64(reader["p_neto"].ToString().Trim());
            //Tarifas
            item.TarifaGG = (decimal)reader["g_gles"];
            item.TarifaAcarreo = (decimal)reader["acarreo"];
            item.TarifaAlmacenaje = (decimal)reader["almacenaje"];
            item.TarifaPtoExceso = (decimal)reader["tar_exc"];
            item.TarifaSecado = (decimal)reader["tar_sec"];
            item.TarifaZarandeo = (decimal)reader["zarandeo"];            
            //Importes
            item.ImporteGG = (decimal)reader["i_gg"];
            item.ImporteAcarreo = (decimal)reader["i_aca"];
            item.ImporteAnalisis = (decimal)reader["total_ana"];
            item.ImporteFlete = (decimal)reader["flete"];
            item.ImporteFumigada = (decimal)reader["t_fumi"];
            item.ImporteImp1 = (decimal)reader["total_imp1"];
            //item.ImporteImp2 = (decimal)reader["total_imp2"];
            item.ImporteOtros = (decimal)reader["tot_gastos"];
            item.ImporteSecado = (decimal)reader["i_sec"];
            item.ImporteSellado = (decimal)reader["i_sel"];
            item.ImporteZaranda = (decimal)reader["i_zar"];
            item.AlicuotaIva = 21;
            item.ImporteIva = (decimal)reader["i_iva2"] + (decimal)reader["i_iva1"];
            item.Total = (decimal)reader["i_total"];
            item.SubTotal = item.Total - item.ImporteIva;
            item.Coe = reader["COE"].ToString().Trim();
            return item;
        }
        private Analisis ParseAnalisis(OleDbDataReader reader) 
        {            
            Analisis item = new Analisis();
            item.Nombre = reader["rubro"].ToString().Trim();
            item.Valor = (decimal)reader["porc_r"];
            item.Rebaja = (decimal)reader["reb_r"];
            item.Bonificacion = (decimal)reader["bon_r"];
            return item;
        }
        private ItemRomaneo ParseRomaneo(OleDbDataReader reader)
        {
            ItemRomaneo item = new ItemRomaneo();
            item.Fecha = (DateTime)reader["fecha"];
            item.ImporteSecado = (decimal)reader["i_sec"];
            item.TarifaSecado = (decimal)reader["t_sec"];
            item.PorHumedad = (decimal)reader["p_hum"];
            item.MermaHumedad = (decimal)reader["m_hum"];            
            item.MermaZaranda = Convert.ToInt16(reader["m_zar"].ToString().Trim());
            item.TarifaZaranda = (decimal)reader["t_zar"];
            item.PesoBruto = Convert.ToInt64(reader["k_bru"].ToString().Trim());
            item.IdTransaccion = "ENTRADA;" + reader["n_rom"].ToString().Trim();
            item.NumeroCPorte = reader["pe_cp"].ToString().Trim() + reader["n_cre"].ToString().Trim();
            return item;
        }
    }
}

