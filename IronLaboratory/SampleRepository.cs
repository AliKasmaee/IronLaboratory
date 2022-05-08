using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace IronLaboratory
{
    public class SampleRepository
    {
        public List<Sample> Repository { get; set; }
        public static int SampleID;

        public SampleRepository()
        {
            Repository = GetSampleRepository();
            SampleID = GetLastSampleID() + 1;
            CheckSampleID.ResetSampleID();
        }

        public List<Sample> GetSampleRepository()
        {
            List<Sample> listOfSamples = new List<Sample>();
            using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.DBConnectionString))
            {
                if (conn == null)
                {
                    throw new Exception("Connection String is Null. Set the value of Connection String in IronLaboratory->Properties-?Settings.settings");
                }
                SqlCommand sc = new SqlCommand("SELECT * FROM tblRecords", conn);
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(sc);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    Sample s = new Sample
                    {
                        PrimaryId = (Int64)row["PrimaryId"],
                        SampleId = (int)row["SampleId"],
                        Registration = row["Registration"].ToString(),
                        Material = row["Material"].ToString(),
                        Experiment = row["Experiment"].ToString(),
                        Clock = row["Clock"].ToString(),
                        OnDate = DateConversion.ConvertToPersian((DateTime)row["OnDate"]),
                        OrderId = row["OrderId"].ToString(),
                        DateAndTimeAdded = row["DateAndTimeAdded"].ToString()
                    };
                    listOfSamples.Add(s);
                }
                return listOfSamples;
            }
        }

        private int GetLastSampleID()
        {
            using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.DBConnectionString))
            {
                if (conn == null)
                {
                    throw new Exception("Connection String is Null. Set the value of Connection String in IronLaboratory->Properties-?Settings.settings");
                }
                SqlCommand sc = new SqlCommand("returnLastSampleId", conn);
                conn.Open();
                sc.CommandType = CommandType.StoredProcedure;
                return (int)sc.ExecuteScalar();
            }
        }

        public void AddNewSample(Sample newSample)
        {
            using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.DBConnectionString))
            {
                if (conn == null)
                {
                    throw new Exception("Connection String is Null. Set the value of Connection String in IronLaboratory->Properties-?Settings.settings");
                }
                else if (newSample == null)
                {
                    throw new Exception("The passed argument is null");
                }
                SqlCommand sc = new SqlCommand("recordAdd", conn);
                conn.Open();
                sc.CommandType = CommandType.StoredProcedure;

                SqlParameter param1 = new SqlParameter("@pSampleId", SqlDbType.Int);
                SqlParameter param2 = new SqlParameter("@pRegistration", SqlDbType.NVarChar);
                SqlParameter param3 = new SqlParameter("@pMaterial", SqlDbType.NVarChar);
                SqlParameter param4 = new SqlParameter("@pExperiment", SqlDbType.NVarChar);
                SqlParameter param5 = new SqlParameter("@pClock", SqlDbType.NVarChar);
                SqlParameter param6 = new SqlParameter("@pOnDate", SqlDbType.Date);
                SqlParameter param7 = new SqlParameter("@pOrderID", SqlDbType.NVarChar);

                param1.Value = newSample.SampleId;
                param2.Value = newSample.Registration;
                param3.Value = newSample.Material;
                param4.Value = newSample.Experiment;
                param5.Value = newSample.Clock;
                param6.Value = newSample.OnDate;
                param7.Value = newSample.OrderId;

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

        public void DeleteRecord(Int64 primaryId)
        {
            using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.DBConnectionString))
            {
                if (conn == null)
                {
                    throw new Exception("Connection String is Null. Set the value of Connection String in IronLaboratory->Properties-?Settings.settings");
                }
                SqlCommand sc = new SqlCommand("recordDelete", conn);
                conn.Open();
                sc.CommandType = CommandType.StoredProcedure;
                SqlParameter param1 = new SqlParameter("@pPrimaryId", SqlDbType.BigInt);
                param1.Value = primaryId;
                sc.Parameters.Add(param1);

                sc.ExecuteNonQuery();
            }
        }

        public void UpdateRecord(Sample s)
        {
            using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.DBConnectionString))
            {
                if (conn == null)
                {
                    throw new Exception("Connection String is Null. Set the value of Connection String in IronLaboratory->Properties-?Settings.settings");
                }
                SqlCommand sc = new SqlCommand("recordUpdate", conn);
                conn.Open();
                sc.CommandType = CommandType.StoredProcedure;
                SqlParameter param1 = new SqlParameter("@pPrimaryId", SqlDbType.BigInt);
                SqlParameter param2 = new SqlParameter("@pRegistration", SqlDbType.NVarChar);
                SqlParameter param3 = new SqlParameter("@pMaterial", SqlDbType.NVarChar);
                SqlParameter param4 = new SqlParameter("@pExperiment", SqlDbType.NVarChar);
                SqlParameter param5 = new SqlParameter("@pClock", SqlDbType.NVarChar);
                SqlParameter param6 = new SqlParameter("@pOnDate", SqlDbType.Date);
                SqlParameter param7 = new SqlParameter("@pOrderID", SqlDbType.NVarChar);

                param1.Value = s.PrimaryId;
                param2.Value = s.Registration;
                param3.Value = s.Material;
                param4.Value = s.Experiment;
                param5.Value = s.Clock;
                param6.Value = s.OnDate;
                param7.Value = s.OrderId;

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
    }
}
