using System;
using System.Data.SQLite;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace CockyGrabber.Utility
{
    internal static class Tools
    {
        public static bool ColumnExists(this SQLiteConnection conn, string table, string column)
        {
            using (var _cmd = conn.CreateCommand())
            {
                _cmd.CommandText = $"SELECT COUNT(*) AS CNTREC FROM pragma_table_info('{table}') WHERE name='{column}'";
                return (long)_cmd.ExecuteScalar() == 1;
            }
        }

        /// <summary>
        /// On serializing a class with the JavaScriptConverter the dates are stored like this: \/Date(1616932619000)\/
        /// <br>This method converts all thoose wrong formated dates to plain digits</br>
        /// </summary>
        /// <param name="json">The json string</param>
        /// <returns>A json string with correctly forammted dates</returns>
        public static string CorrectSerializedDates(string json)
        {
            Regex r = new Regex(@"\\\/Date\((-?\d+)\)\\\/"); // matches all dates in the json string
            foreach (Match m in r.Matches(json))
            {
                string digits = m.Value.Replace("\\/Date(", null).Replace(")\\/", null); // Remove everything but the digits

                //json = json.Replace($"\\/Date({digits})\\/", new DateTime(long.Parse(digits)).ToString()); // Replace \/Date(1616932619000)\/ -> dd.mm.yyyy hh:mm:ss (Unused & Bugged)
                json = json.Replace($"\\/Date({digits})\\/", digits); // Replace \/Date(1616932619000)\/ -> 1616932619000
            }
            return json;
        }

        /// <summary>
        /// Serializes a browser information class to a json string
        /// </summary>
        /// <param name="data">The class with the browser information</param>
        /// <returns>A json string</returns>
        public static string BrowserInformationToJson(object data) => CorrectSerializedDates(_jsonSerializer.Serialize(data));

        private static readonly JavaScriptSerializer _jsonSerializer = new JavaScriptSerializer();
    }
}
