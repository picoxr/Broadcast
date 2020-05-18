using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using LitJson;

public class JsonDataUtil
{
    #region new
    public const string ErrorStr = "JsonData object";
    public static List<T> GetPOJOArray<T>(JsonData root, Type clazz)
    {
        List<T> array = new List<T>();
        if (root.Count > 1)
        {
            //array
            JsonData tmp = root;
            for (int i = 0; i < tmp.Count; ++i)
            {
                array.Add((T)Activator.CreateInstance(clazz, tmp[i]));
            }
        }
        else if (root.Count == 0)
        {
            return array;
        }
        else
        {
            //array.Add((T)Activator.CreateInstance(clazz, root));
            array.Add((T)Activator.CreateInstance(clazz, root[0]));
        }
        return array;
    }


    public static string GetString(JsonData data, string paraName)
    {
        if (null == data || string.IsNullOrEmpty(paraName))
        {
            return "";
        }
        if (!data.Keys.Contains(paraName))
        {
            return "";
        }
        string returns = data[paraName].ToString();
        return "{}".Equals(returns) ? "" : returns;
    }


    public static int GetInt(JsonData data, string paraName)
    {
        int returns = 0;
        if (null == data || string.IsNullOrEmpty(paraName))
        {
            return returns;
        }
        if (!data.Keys.Contains(paraName))
        {
            return returns;
        }
        int.TryParse(data[paraName].ToString(), out returns);
        return returns;
    }

    public static long GetLong(JsonData data, string paraName)
    {
        long returns = 0;
        if (null == data || string.IsNullOrEmpty(paraName))
        {
            return returns;
        }
        if (!data.Keys.Contains(paraName))
        {
            return returns;
        }
        long.TryParse(data[paraName].ToString(), out returns);
        return returns;
    }

    public static double GetDouble(JsonData data, String paraName)
    {
        double returns = 0;
        if (null == data || string.IsNullOrEmpty(paraName))
        {
            return returns;
        }
        if (!data.Keys.Contains(paraName))
        {
            return returns;
        }
        double.TryParse(data[paraName].ToString(), out returns);
        return returns;
    }

    public static string GetJson(JsonData data, String paraName)
    {
        string returns = "";
        if (null == data || string.IsNullOrEmpty(paraName))
        {
            return returns;
        }
        if (!data.Keys.Contains(paraName))
        {
            return returns;
        }
        return data[paraName].ToJson();
    }

    public static int GetErrorCode(string root)
    {
        int errorCode = 0;
        JsonData data = JsonMapper.ToObject(root);
        return GetInt(data, "errorCode");
    }

    private static bool IsJsonArray(string plain)
    {
        //return plain.startsWith("[") && plain.endsWith("]");
        return true;
    }

    public static bool IsSuccess(JsonData root)
    {
        return "success".Equals(root["status"].ToString());
    }
}
#endregion
