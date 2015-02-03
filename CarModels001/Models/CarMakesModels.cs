using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;
using System.Data.SqlClient;

namespace CarModels001.Models
{
    public class CarMakesModels
    {
        public CarMakesModels()
        {
            thisConnection = new SqlConnection(@"Data Source=(localdb)\v11.0;Integrated Security=True");
            SqlCommand = thisConnection.CreateCommand();

            try
            {
                thisConnection.Open();
            }
            catch (SqlException ex)
            {
                if(thisConnection != null)
                    thisConnection.Close();
                throw ex;
            }

            // check if database exists
            bool bDBExist = false;
            SqlCommand.CommandText = "SELECT * FROM master.dbo.sysdatabases where name=\'" +
                DataBaseName + "\'";
            using (System.Data.SqlClient.SqlDataReader reader = SqlCommand.ExecuteReader())
            {
                bDBExist = reader.HasRows;
            }

            if (!bDBExist)
            {
                // create new
                SqlCommand.CommandText = "CREATE DATABASE " + DataBaseName;
                SqlCommand.ExecuteNonQuery();
            }

            // Database created or exists, now switching
            thisConnection.ChangeDatabase(DataBaseName);

            if (!bDBExist)
            {
                SqlCommand.CommandText = "CREATE TABLE Makes( M_ID int IDENTITY(1,1) PRIMARY KEY NOT NULL, ManName VARCHAR(50) )";
                SqlCommand.ExecuteNonQuery();

                SqlCommand.CommandText = "CREATE TABLE Models( Mod_ID int IDENTITY(1,1) PRIMARY KEY NOT NULL, ModelName VARCHAR(50), Man_ID int FOREIGN KEY REFERENCES Makes(M_ID) NOT NULL )";
                SqlCommand.ExecuteNonQuery();
            }
        }

        ~CarMakesModels()
        {
            if (thisConnection != null)
                thisConnection.Close();
        }

        public void Get(ref Dictionary<string, dynamic> dict, bool bKeysOnly)
        {
            // if not specified, return all makes
            if(dict.Count == 0)
            {
                SqlCommand.CommandText = "select ManName from Makes";
                using (var reader = SqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dict.Add(String.Format("{0}", reader[0]), new List<string>());
                    }
                }
            }

            int CurrMID;

            if (!bKeysOnly)
            {
                foreach (var i in dict)
                {
                    SqlCommand.CommandText = "select M_ID from Makes where ManName = \'" + i.Key + "\'";
                    using (var reader = SqlCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            CurrMID = (int)reader.GetValue(0);
                        }
                        else
                        {
                            break;
                        }
                    }

                    SqlCommand.CommandText = "select ModelName from Models where Man_ID = " + CurrMID.ToString();
                    using (var reader = SqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dict[i.Key].Add(String.Format("{0}", reader[0]));
                        }
                    }
                }
            }
        }

        public void Set(ref Dictionary<string, dynamic> dict)
        {
            int CurrMID;

            foreach (var i in dict)
            {
                SqlCommand.CommandText = "if not exists( select * from Makes where ManName = \'" +
                    i.Key + "\' ) INSERT into Makes( ManName ) VALUES ( \'" + i.Key + "\')";
                SqlCommand.ExecuteNonQuery();

                SqlCommand.CommandText = "select M_ID from Makes where ManName = \'" + i.Key + "\'";
                using (var reader = SqlCommand.ExecuteReader())
                {
                    reader.Read();
                    CurrMID = (int)reader.GetValue(0);
                }

                foreach (var j in i.Value)
                {
                    SqlCommand.CommandText = "if not exists( select * from Models where ModelName = \'" +
                    j + "\' and Man_ID = " + CurrMID.ToString() + ") INSERT into Models( ModelName, Man_ID ) VALUES ( \'" +
                    j + "\', " + CurrMID.ToString() + ")";
                    SqlCommand.ExecuteNonQuery();
                }
            }
        }

        public void Create(ref Dictionary<string, dynamic> dict)
        {
            // clear database
//            SqlCommand.CommandText = "DROP DATABASE " + DataBaseName;
            SqlCommand.CommandText = "DROP TABLE Models";
            SqlCommand.ExecuteNonQuery();
            SqlCommand.CommandText = "DROP TABLE Makes";
            SqlCommand.ExecuteNonQuery();

            SqlCommand.CommandText = "CREATE TABLE Makes( M_ID int IDENTITY(1,1) PRIMARY KEY NOT NULL, ManName VARCHAR(50) )";
            SqlCommand.ExecuteNonQuery();

            SqlCommand.CommandText = "CREATE TABLE Models( Mod_ID int IDENTITY(1,1) PRIMARY KEY NOT NULL, ModelName VARCHAR(50), Man_ID int FOREIGN KEY REFERENCES Makes(M_ID) NOT NULL )";
            SqlCommand.ExecuteNonQuery();

            Set(ref dict);
        }

        SqlConnection thisConnection;
        const string DataBaseName = "CarManModDB";  // DB Name
        SqlCommand SqlCommand;
    }
}