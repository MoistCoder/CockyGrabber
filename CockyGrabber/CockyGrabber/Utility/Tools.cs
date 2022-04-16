using System.Data.SQLite;

namespace CockyGrabber.Utility
{
    internal static class Tools
    {
        /*
         * GARBAGE CODE:
         * (keeping just in case)
        public static T Validate<T>(T obj)
        {
            return obj;
        }
        /// <summary>
        /// Checks if a string is empty or null
        /// </summary>
        public static string ValidateString(byte[] x) => x == null || x.Length == 0 || Encoding.Default.GetString(x) == string.Empty ? null : Encoding.Default.GetString(x);
        */

        public static bool ColumnExists(this SQLiteConnection conn, string table, string column)
        {
            using (var _cmd = conn.CreateCommand())
            {
                _cmd.CommandText = $"SELECT COUNT(*) AS CNTREC FROM pragma_table_info('{table}') WHERE name='{column}'";
                return (long)_cmd.ExecuteScalar() == 1;
            }
        }
    }
}
