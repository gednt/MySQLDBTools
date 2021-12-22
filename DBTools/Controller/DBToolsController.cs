using DBTools.Model;
using DBToolsDll;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBTools.Controller
{
    public class DBToolsController : Interfaces.IDBTools
    {
        public DBToolsDll.DBTools DBTools = new DBToolsDll.DBTools();

        public DBToolsController(DBToolsDll.DBTools dbTools)
        {
            DBTools = dbTools;
        }

        public DataView RetrieveDataSQL()
        {
            return DBTools.RetrieveDataMySQL();
        }

        public List<GenericObject> RetrieveObjectSQL()
        {
            return DBTools.RetrieveObjectMySQL();
        }

        public void SqlExecuteQuery(String query = "")
        {
            DBTools.MySQLExecuteQuery();
        }
    }
}
