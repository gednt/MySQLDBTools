﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using DBTools.Model;
using System.Dynamic;

namespace DBToolsDll
{
    /// <summary>
    /// DBTools is a Mysql Library to manipulate data in MySql Databases
    /// </summary>
    public class DBTools
    {
        #region private variables
        private string host;

        private string uid;

        private string password;

        private string database;

        private string query;

        private string error;

        private string table;
        private string port;

        private string _connectionString;

        private List<MySqlParameter> mySqlParameters;
        public int count;
        #endregion

        #region public getters and setters
        public DBTools()
        {
            //STANDARD TCP/IP Port
            port = "3306";
        }
        /// <summary>
        /// It is used to place the server address of the database <br></br>
        /// Example(localhost:3306 or localhost:4001) <br></br>
        /// It is in this formatting.
        /// Host:Port
        /// </summary>
        public string Host
        {
            get
            {
                return this.host;
            }
            set
            {
                this.host = value;
            }
        }
        /// <summary>
        /// It is used to place the userName of your database <br></br>
        /// Example(root or admin). <br></br>
        /// It is in this formatting
        /// user
        /// </summary>
        public string Uid
        {
            get
            {
                return this.uid;
            }
            set
            {
                this.uid = value;
            }
        }
        /// <summary>
        /// It is used to place the password of the database <br></br>
        ///
        /// </summary>
        public string Password
        {
            get
            {

                return this.password;
            }
            set
            {
                this.password = value;
            }
        }
        /// <summary>
        /// It is used to set the Database name <br></br>
        ///
        /// </summary>
        public string Database
        {
            get
            {
                return this.database;
            }
            set
            {
                this.database = value;
            }
        }
        /// <summary>
        /// It is used to set the Query <br></br>
        ///
        /// </summary>
        public string Query
        {
            get
            {
                return this.query;
            }
            set
            {
                this.query = value;
            }
        }

        public string Error
        {
            get
            {
                return this.error;
            }
            set
            {
                this.error = value;
            }
        }

        public string Table
        {
            get
            {
                return this.table;
            }
            set
            {
                this.table = value;
            }
        }

        public int Count
        {
            get
            {
                return this.count;
            }
            set
            {
                this.count = value;
            }
        }
        /// <summary>
        /// Specify the port of the database: Standard 3306
        ///
        /// </summary>
        public string Port { get => port; set => port = value; }
        public string ConnectionString
        {


            get
            {
                _connectionString = String.Format("Server={0};Port={1};Database={2};Uid={3};Pwd = {4};", host, port, database, uid, password);
                return _connectionString;

            }

        }

        public List<MySqlParameter> MySqlParameters { get => mySqlParameters; set => mySqlParameters = value; }
        #endregion

        #region legacy getters and setters
        public void setHost(string host)
        {
            this.Host = host;
        }

        public string getHost()
        {
            return this.Host;
        }

        public void setUid(string uid)
        {
            this.Uid = uid;
        }

        public string getUid()
        {
            return this.Uid;
        }

        public void setPassword(string password)
        {
            this.Password = password;
        }

        public string getPassword()
        {
            return this.Password;
        }

        public void setQuery(string query)
        {
            this.Query = query;
        }

        public string getQuery()
        {
            return this.Query;
        }

        public void setDataBase(string database)
        {
            this.Database = database;
        }

        public string getDatabase()
        {
            return this.Database;
        }
        #endregion
        #region Public Day One Methods
        /// <summary>
        /// Executes the Query in the database.<br/>
        /// This method is deprecated, use <see cref="MySQLExecuteQuery(string)"/> instead, for better security and compliance with the standards.
        /// </summary>
        /// 
        [Obsolete("This method is deprecated and should use MySqlExecuteQuery instead", false)]
        public void mySQLExecuteQuery()
        {

            MySqlConnection mySqlConnection = new MySqlConnection(ConnectionString);
            mySqlConnection.Open();
            try
            {
                MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                mySqlCommand.CommandText = this.getQuery();
                mySqlCommand.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                this.Error = ex.ToString();
            }
            finally
            {
                bool flag = mySqlConnection.State == ConnectionState.Open;
                if (flag)
                {
                    mySqlConnection.Close();
                }
            }
        }

        /// <summary>
        /// Retrieves the DataView Representation in the database.<br/>
        /// This method is deprecated
        /// </summary>
        /// <returns></returns>
        /// 
        [Obsolete("This Method is deprecated, use RetrieveObjectMySQL or RetrieveDataMySQL instead", false)]
        public DataView retrieveDataMySQL()
        {
            DataView defaultView = new DataView();
            try
            {

                MySqlCommand mySqlCommand = new MySqlCommand();
                MySqlConnection mySqlConnection = new MySqlConnection(ConnectionString);
                //Verifica se a conexão foi aberta com sucesso
                try
                {
                    mySqlConnection.Open();
                    mySqlCommand = mySqlConnection.CreateCommand();
                    mySqlCommand.CommandText = this.getQuery();
                    MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(mySqlCommand);
                    DataSet dataSet = new DataSet();
                    mySqlDataAdapter.Fill(dataSet);
                    this.Count = dataSet.Tables.Count;
                    defaultView = dataSet.Tables[0].DefaultView;
                    mySqlConnection.Close();
                }
                catch (MySqlException e)
                {
                    Error = e.ToString();

                }



            }
            catch (Exception)
            {
                DataSet dataSet2 = new DataSet();
                defaultView = dataSet2.Tables[0].DefaultView;
            }
            return defaultView;
        }
        #endregion

        #region Public improved methods
        /// <summary>
        /// Returns a list of <see cref="GenericObject"/> that can be used in a variety of scenarios
        /// </summary>
        /// <returns></returns>
        public List<GenericObject> RetrieveObjectMySQL()
        {
            using (MySql.Data.MySqlClient.MySqlConnection conn = new MySqlConnection(this.ConnectionString))
            {
                MySqlCommand command = new MySqlCommand(this.Query, conn);
                if (MySqlParameters != null)
                    command.Parameters.AddRange(MySqlParameters.ToArray());
                MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(command);
                DataSet dataSet = new DataSet();
                mySqlDataAdapter.Fill(dataSet);
                List<GenericObject> lstObject = new List<GenericObject>();

                List<String> columns = new List<string>();
                List<String> types = new List<string>();
                DataView values = dataSet.Tables[0].DefaultView;
                int cont = 0;
                foreach (var column in values.Table.Columns)
                {
                    columns.Add(column.ToString());
                    types.Add(dataSet.Tables[0].Columns[columns[cont]].DataType.Name);
                    cont++;

                }

                for (cont = 0; cont < values.Count; cont++)
                {
                    lstObject.Add(new GenericObject
                    {
                        columns = columns.ToArray(),
                        types = types.ToArray(),
                        values = values[cont].Row.ItemArray
                        //  valuesString = values[cont].DataView
                    });
                }

                return lstObject;
            }

        }


        /// <summary>
        /// Retrieves a list of entities of the database, available to be parsed to any object when needed.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public List<ExpandoObject> RetrieveObjectMySQL_Entity(Type obj)
        {
            using (MySql.Data.MySqlClient.MySqlConnection conn = new MySqlConnection(this.ConnectionString))
            {
              
                List<ExpandoObject> lstObject = new List<ExpandoObject>();



                try
                {
                    MySqlCommand command = new MySqlCommand(this.Query, conn);
                    if (MySqlParameters != null)
                        command.Parameters.AddRange(MySqlParameters.ToArray());
                    MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(command);
                    DataSet dataSet = new DataSet();
                    mySqlDataAdapter.Fill(dataSet);

                    DataView values = dataSet.Tables[0].DefaultView;

                    foreach (DataRow dr in values.Table.Rows)
                    {
                        lstObject.Add(new ExpandoObject { });

                        IDictionary<string, object> dictionary = (IDictionary<string, object>)lstObject[lstObject.Count - 1];


                        foreach (DataColumn column in dr.Table.Columns)
                        {
                            Console.WriteLine(column.ColumnName);

                            dictionary.Add(column.ColumnName, dr[column.ColumnName]);


                        }

                    }
                }
                catch(MySqlException e)
                {
                    Error = e.Message;
                }
               


                return lstObject;
            }

        }

        /// <summary>
        /// Executes a sql query and, if sucessful, returns a void string, if error, returns the error message<br/>
        /// Concatenating the query is not recommendable, use <see cref="MySqlParameters"/> to pass your parameters before executing this command.<br/>
        /// </summary>
        /// 
        public void MySQLExecuteQuery(String query = "")
        {

            using (MySqlConnection mySqlConnection = new MySqlConnection(ConnectionString))
            {
                mySqlConnection.Open();
                try
                {
                    MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                    mySqlCommand.CommandText = this.getQuery();
                    if (MySqlParameters != null)
                        mySqlCommand.Parameters.AddRange(MySqlParameters.ToArray());
                    mySqlCommand.ExecuteNonQuery();
                }
                catch (MySqlException ex)
                {
                    this.Error = ex.ToString();
                }
                mySqlConnection.Close();
                //finally
                //{
                //    bool flag = mySqlConnection.State == ConnectionState.Open;
                //    if (flag)
                //    {
                //        mySqlConnection.Close();
                //    }
                //}
            }

        }



        /// <summary>
        /// Retrieves the DataView Representation in the database.<br/>
        /// Concatenating the query is not recommendable, use <see cref="MySqlParameters"/> to pass your parameters before executing this command.<br/>
        /// </summary>
        /// <returns></returns>
        public DataView RetrieveDataMySQL(String query = "")
        {
            DataView defaultView = new DataView();
            try
            {

                using (MySql.Data.MySqlClient.MySqlConnection conn = new MySqlConnection(this.ConnectionString))
                {
                    try
                    {
                        MySqlCommand mySqlCommand = new MySqlCommand(query != "" ? query : this.Query, conn);
                        if (mySqlParameters != null)
                            mySqlCommand.Parameters.AddRange(MySqlParameters.ToArray());
                        MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(mySqlCommand);
                        DataSet dataSet = new DataSet();
                        mySqlDataAdapter.Fill(dataSet);
                        this.Count = dataSet.Tables.Count;
                        defaultView = dataSet.Tables[0].DefaultView;
                    }
                    catch (MySqlException e)
                    {
                        Error = e.ToString();

                    }

                }



            }
            catch (Exception e)
            {
                DataSet dataSet2 = new DataSet();
                defaultView = dataSet2.Tables[0].DefaultView;
            }
            return defaultView;
        }


        #endregion
    }
}
