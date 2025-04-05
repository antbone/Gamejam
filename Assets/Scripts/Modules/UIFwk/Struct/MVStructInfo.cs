#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

[Serializable]
public class MVStructInfo : ScriptableObject
{
    public string invName = "";
    public string modelName = "";
    public string mvType = "";
    public string vmDataType = null;
    public string mvDataType = null;
    public string adapterName = "";
    public bool hasAdapter = false;
    public bool isValid = false;
    public string goPath = "";
    public bool isPrivate = false;
    public string eventKey = "";

    public bool isList
    {
        get
        {
            Type type = Type.GetType(mvDataType);
            if (type == null)
                return false;
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        }
    }
}