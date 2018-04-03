using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Factories
{
    public class SqlParameterFactory
    {
        public static SqlParameter getVarCharParameter(string name, string value)
        {
            var parameter = new SqlParameter(name, SqlDbType.NVarChar,-1);
            if (value == null) parameter.Value = DBNull.Value;
            else parameter.Value = value;
            
            return parameter;
        }

        public static SqlParameter getDateParameter(string name, DateTime value)
        {
            var parameter = new SqlParameter(name, SqlDbType.DateTime);
            if (value == default(DateTime)) parameter.Value = DBNull.Value;
            else parameter.Value = value;

            return parameter;
        }

        public static SqlParameter getIntParameter(string name, int value)
        {
            var parameter = new SqlParameter(name, SqlDbType.Int);
            if (value == 0) parameter.Value = DBNull.Value;
            else parameter.Value = value;

            return parameter;
        }
    }
}
