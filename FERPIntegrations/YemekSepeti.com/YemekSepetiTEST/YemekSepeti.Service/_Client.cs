using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using YemekSepeti.Service.YSServiceReference;

namespace YemekSepeti.Service
{
    public partial class YemekSepetiClient
    {
        EndpointAddress RemoteAddress;
        AuthHeader Auth;
        IntegrationSoap client;
        public YemekSepetiClient(string Username = "ultimate", string Password = "36zn7mjn",
            string Address = "http://messaging.yemeksepeti.com/messagingwebservice/integration.asmx")
        {
            RemoteAddress = new EndpointAddress(Address);
            Auth = new AuthHeader() { UserName = Username, Password = Password };
            client = new IntegrationSoapClient(new System.ServiceModel.BasicHttpBinding(), RemoteAddress);
        }

    }
    public static class Map
    {
        /// <summary>
        /// Fills properties of a class from a row of a DataTable where the name of the property matches the column name from that DataTable.
        /// It does this for each row in the DataTable, returning a List of classes.
        /// </summary>
        /// <typeparam name="T">The class type that is to be returned.</typeparam>
        /// <param name="Table">DataTable to fill from.</param>
        /// <returns>A list of ClassType with its properties set to the data from the matching columns from the DataTable.</returns>
        public static IList<T> DatatableToClass<T>(DataTable Table) where T : class, new()
        {
            if (!IsValidDatatable(Table))
                return new List<T>();

            Type classType = typeof(T);
            IList<PropertyInfo> propertyList = classType.GetProperties();

            // Parameter class has no public properties.
            if (propertyList.Count == 0)
                return new List<T>();

            List<string> columnNames = Table.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToList();

            List<T> result = new List<T>();
            try
            {
                foreach (DataRow row in Table.Rows)
                {
                    T classObject = new T();
                    foreach (PropertyInfo property in propertyList)
                    {
                        if (property != null && property.CanWrite)   // Make sure property isn't read only
                        {
                            if (columnNames.Contains(property.Name))  // If property is a column name
                            {
                                if (row[property.Name] != System.DBNull.Value)   // Don't copy over DBNull
                                {
                                    object propertyValue = System.Convert.ChangeType(
                                            row[property.Name],
                                            property.PropertyType
                                        );
                                    property.SetValue(classObject, propertyValue, null);
                                }
                            }
                        }
                    }
                    result.Add(classObject);
                }
                return result;
            }
            catch
            {
                return new List<T>();
            }
        }

        public static bool IsValidDatatable(DataTable Table, bool IgnoreRows = false)
        {
            if (Table == null) return false;
            if (Table.Columns.Count == 0) return false;
            if (!IgnoreRows && Table.Rows.Count == 0) return false;
            return true;
        }
    }
}