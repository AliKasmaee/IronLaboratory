using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace IronLaboratory
{
    public class DataRepository
    {
        public List<TempData> Repository { get; set; }

        public DataRepository()
        {
            Repository = GetDataRepository();
        }

        private List<TempData> GetDataRepository()
        {
            List<TempData> listOfData = new List<TempData>();
            using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.DBConnectionString))
            {
                if (conn == null)
                {
                    throw new Exception("Connection String is Null. Set the value of Connection String in IronLaboratory->Properties-?Settings.settings");
                }
                SqlCommand sc = new SqlCommand("SELECT * from tblPreSamples", conn);
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(sc);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    TempData s = new TempData
                    {
                        DataId = (int)row["DataId"],
                        Material = row["Material"].ToString(),
                        Experiment = row["Experiment"].ToString(),
                        Clock = row["Clock"].ToString(),
                        Number = (Int16)row["Number"],
                        OnHour = (Int16)row["OnHour"],
                        OnShift = row["OnShift"].ToString(),
                        TypeOf = (Int16)row["TypeOf"]
                    };
                    listOfData.Add(s);
                }
                return listOfData;
            }
        }

        public void AddNewData(TempData newData)
        {
            using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.DBConnectionString))
            {
                if (conn == null)
                {
                    throw new Exception("Connection String is Null. Set the value of Connection String in IronLaboratory->Properties-?Settings.settings");
                }
                else if (newData == null)
                {
                    throw new Exception("The passed argument is null");
                }

                SqlCommand sc = new SqlCommand("preSamplesAdd", conn);
                conn.Open();
                sc.CommandType = CommandType.StoredProcedure;
                SqlParameter param1 = new SqlParameter("@pMaterial", SqlDbType.NVarChar);
                SqlParameter param2 = new SqlParameter("@pExperiment", SqlDbType.NVarChar);
                SqlParameter param3 = new SqlParameter("@pClock", SqlDbType.NVarChar);
                SqlParameter param4 = new SqlParameter("@pNumber", SqlDbType.SmallInt);
                SqlParameter param5 = new SqlParameter("@pOnHour", SqlDbType.SmallInt);
                SqlParameter param6 = new SqlParameter("@pOnShift", SqlDbType.NVarChar);
                SqlParameter param7 = new SqlParameter("@pTypeOf", SqlDbType.SmallInt);

                param1.Value = newData.Material;
                param2.Value = newData.Experiment;
                param3.Value = newData.Clock;
                param4.Value = newData.Number;
                param5.Value = newData.OnHour;
                param6.Value = newData.OnShift;
                param7.Value = newData.TypeOf;

                sc.Parameters.Add(param1);
                sc.Parameters.Add(param2);
                sc.Parameters.Add(param3);
                sc.Parameters.Add(param4);
                sc.Parameters.Add(param5);
                sc.Parameters.Add(param6);
                sc.Parameters.Add(param7);

                sc.ExecuteNonQuery();
            }
        }

        public void DeleteData(int dId)
        {
            using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.DBConnectionString))
            {
                if (conn == null)
                {
                    throw new Exception("Connection String is Null. Set the value of Connection String in IronLaboratory->Properties-?Settings.settings");
                }
                SqlCommand sc = new SqlCommand("preSamplesDelete", conn);
                conn.Open();
                sc.CommandType = CommandType.StoredProcedure;
                SqlParameter param1 = new SqlParameter("@pDataId", SqlDbType.Int);
                param1.Value = dId;
                sc.Parameters.Add(param1);

                sc.ExecuteNonQuery();
            }
        }
    }
}
