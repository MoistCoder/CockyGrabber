using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace CockyGrabber.Utility
{
    public sealed class DynamicJsonConverter : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            return type == typeof(object) ? new DynamicJsonObject(dictionary) : null;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get { return new ReadOnlyCollection<Type>(new List<Type>(new[] { typeof(object) })); }
        }

        #region Nested type: DynamicJsonObject

        private sealed class DynamicJsonObject : DynamicObject
        {
            private readonly IDictionary<string, object> _dictionary;

            public DynamicJsonObject(IDictionary<string, object> dictionary)
            {
                if (dictionary == null)
                    throw new ArgumentNullException("dictionary");
                _dictionary = dictionary;
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder("{ ");
                ToString(sb);
                sb.Append("}");
                return sb.ToString();
            }

            private void ToString(StringBuilder sb)
            {
                bool firstDict = true;
                foreach (var pair in _dictionary)
                {
                    if (!firstDict)
                        sb.Append(",");
                    firstDict = false;
                    sb.Append("\"" + pair.Key + "\": ");

                    if (pair.Value is string)
                    {
                        sb.Append("\"" + pair.Value.ToString() + "\"");
                    }
                    else if (pair.Value is Dictionary<string, object>)
                    {
                        sb.Append((new DynamicJsonObject(pair.Value as Dictionary<string, object>).ToString()));
                    }
                    else if (pair.Value is ArrayList)
                    {
                        if ((pair.Value as ArrayList).Count > 1)
                            sb.Append("[");
                        bool firstAL = true;
                        foreach (var item in pair.Value as ArrayList)
                        {
                            if (!firstAL)
                                sb.Append(",");
                            firstAL = false;
                            if (item is string)
                                sb.Append("\"" + item + "\"");
                            else if (item is IDictionary<string, object>)
                                sb.Append((new DynamicJsonObject(item as Dictionary<string, object>).ToString()));
                            else
                            {
                                sb.Append("ERROR");
                            }
                        }
                        if ((pair.Value as ArrayList).Count > 1)
                            sb.Append("]");
                    }
                    else
                    {
                        sb.Append("ERROR");
                    }
                }
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                if (!_dictionary.TryGetValue(binder.Name, out result))
                {
                    // return null to avoid exception.  caller can check for null this way...
                    result = null;
                    return true;
                }

                result = WrapResultObject(result);
                return true;
            }

            public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
            {
                if (indexes.Length == 1 && indexes[0] != null)
                {
                    if (!_dictionary.TryGetValue(indexes[0].ToString(), out result))
                    {
                        // return null to avoid exception.  caller can check for null this way...
                        result = null;
                        return true;
                    }

                    result = WrapResultObject(result);
                    return true;
                }

                return base.TryGetIndex(binder, indexes, out result);
            }

            private static object WrapResultObject(object result)
            {
                IDictionary<string, object> dictionary = result as IDictionary<string, object>;
                if (dictionary != null)
                    return new DynamicJsonObject(dictionary);

                ArrayList arrayList = result as ArrayList;
                if (arrayList != null && arrayList.Count > 0)
                {
                    return arrayList[0] is IDictionary<string, object>
                        ? new List<object>(arrayList.Cast<IDictionary<string, object>>().Select(x => new DynamicJsonObject(x)))
                        : new List<object>(arrayList.Cast<object>());
                }

                return result;
            }
            private static object _WrapResultObject(object result)
            {
                var dictionary = result as IDictionary<string, object>;
                if (dictionary != null)
                    return new DynamicJsonObject(dictionary);

                var arrayList = result as ArrayList;
                if (arrayList != null && arrayList.Count > 0)
                {
                    return arrayList[0] is IDictionary<string, object>
                        ? new List<object>(arrayList.Cast<IDictionary<string, object>>().Select(x => new DynamicJsonObject(x)))
                        : new List<object>(arrayList.Cast<object>());
                }

                return result;
            }
        }

        #endregion
    }
}