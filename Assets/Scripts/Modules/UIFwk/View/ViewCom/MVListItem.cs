using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif
[Serializable]
public class ListItemData
{

}
[Serializable]
public class ItemFieldInfo
{
    public string fieldName = "";
    public string viewDataTypeName = "";
    public string viewPath = "";
    public string viewTypeName = "";
    public bool isValid = false;
}
public class MVListItem : VirtualListItem
{
    public string dataTypeName = "";
    public override Type DataType => Type.GetType(dataTypeName);
    public List<ItemFieldInfo> fieldInfos = new();

    public override void OnData()
    {
        for (int i = 0; i < fieldInfos.Count; i++)
        {
            ItemFieldInfo info = fieldInfos[i];
            Transform viewTrans = transform.Find(info.viewPath);
            if (viewTrans == null)
            {
                Debug.LogError($"{info.viewPath} is null in MVListItem: " + gameObject.name);
                continue;
            }
            if (!info.isValid)
                continue;
            Type viewType = Type.GetType(info.viewTypeName);
            Component view = viewTrans.GetComponent(viewType);
            object val = null;
            FieldInfo field = data.GetType().GetField(info.fieldName);
            PropertyInfo prop = data.GetType().GetProperty(info.fieldName);
            if (field != null)
                val = field.GetValue(data);
            else if (prop != null)
                val = prop.GetValue(data);
            else
            {
                Debug.LogError($"{info.fieldName} is null in MVListItem: " + gameObject.name);
            }
            viewType.GetMethod("SetData").Invoke(view, new object[] { val });
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(MVListItem))]
    class MVListItemEditor : Editor
    {
        private MVListItem obj;
        private void OnEnable()
        {
            obj = (MVListItem)target;
        }
        private List<ItemFieldInfo> Scan(Transform root, Transform topRoot, List<ItemFieldInfo> newList)
        {
            for (int i = 0; i < root.childCount; i++)
            {
                Transform child = root.GetChild(i);
                MonoBehaviour[] comps = child.GetComponents<MonoBehaviour>();
                for (int j = 0; j < comps.Length; j++)
                {
                    MonoBehaviour com = comps[j];
                    Type type = com.GetType();
                    if (type.RecurJudge(e => e.BaseType, e => e.IsGenericType && e.GetGenericTypeDefinition() == typeof(MView<>)))
                    {
                        ItemFieldInfo info = new ItemFieldInfo()
                        {
                            viewPath = root.GetGoPath(com.gameObject),
                            viewTypeName = type.AssemblyQualifiedName,
                            viewDataTypeName = type.RecurFind(e => e.BaseType, e => e.IsGenericType && e.GetGenericTypeDefinition() == typeof(MView<>), e => e.GetGenericArguments()[0]).AssemblyQualifiedName
                        };
                        newList.Add(info);
                    }
                }
                Scan(child, topRoot, newList);
            }
            return newList;
        }
        public override void OnInspectorGUI()
        {
            GUIStyle sucessesStyle = new GUIStyle(GUI.skin.label);
            sucessesStyle.normal.textColor = new Color(49 / 255f, 147 / 255f, 18 / 255f);
            GUIStyle warnStyle = new GUIStyle(GUI.skin.label);
            warnStyle.normal.textColor = new Color(152 / 255f, 102 / 255f, 56 / 255f);
            GUIStyle errorStyle = new GUIStyle(GUI.skin.label);
            errorStyle.normal.textColor = new Color(177 / 255f, 12 / 255f, 12 / 255f);
            bool dirty = false;
            if (GUILayout.Button("Scan"))
            {
                var oldList = obj.fieldInfos;
                obj.fieldInfos.Clear();
                List<ItemFieldInfo> newList = Scan(obj.transform, obj.transform, new());
                for (int i = 0; i < newList.Count; i++)
                {
                    ItemFieldInfo info = newList[i];
                    var old = oldList.Find(x => x.viewPath == info.viewPath && x.viewTypeName == info.viewTypeName);
                    if (old != null)
                    {
                        obj.fieldInfos.Add(old);
                    }
                    else
                    {
                        obj.fieldInfos.Add(info);
                    }
                }
                dirty = true;
            }
            List<Type> dataTypes = Assembly.GetExecutingAssembly().GetTypes().Where(e => e.IsSubclassOf(typeof(ListItemData))).ToList();
            if (dataTypes.Count <= 0)
            {
                GUILayout.Label("not found any ListItemData", errorStyle, GUILayout.Width(300), GUILayout.ExpandWidth(false));
                for (int i = 0; i < obj.fieldInfos.Count; i++)
                {
                    var info = obj.fieldInfos[i];
                    GUI.enabled = false;
                    EditorGUILayout.ObjectField(obj.transform.GetGoByPath(info.viewPath).GetComponent(Type.GetType(info.viewTypeName)), Type.GetType(info.viewTypeName), true, GUILayout.Width(100), GUILayout.ExpandWidth(false));
                    GUI.enabled = true;
                    info.isValid = false;
                }
            }
            else
            {
                int dataTypeIdx = EditorGUILayout.Popup("List Data:", Mathf.Clamp(dataTypes.IndexOf(obj.DataType), 0, dataTypes.Count), dataTypes.Select(e => e.FullName).ToArray());
                if (dataTypes[dataTypeIdx] != obj.DataType)
                {
                    dirty = true;
                    obj.dataTypeName = dataTypes[dataTypeIdx].AssemblyQualifiedName;
                }
                for (int i = 0; i < obj.fieldInfos.Count; i++)
                {
                    var info = obj.fieldInfos[i];
                    GameObject go = obj.transform.GetGoByPath(info.viewPath);
                    if (go == null)
                    {
                        GUILayout.Label("target MView not found", warnStyle);
                        if (info.isValid)
                        {
                            info.isValid = false;
                            dirty = true;
                        }
                        continue;
                    }
                    EditorGUILayout.BeginHorizontal();
                    GUI.enabled = false;
                    EditorGUILayout.ObjectField(go.GetComponent(Type.GetType(info.viewTypeName)), Type.GetType(info.viewTypeName), true, GUILayout.Width(150), GUILayout.ExpandWidth(false));
                    GUI.enabled = true;
                    GUILayout.FlexibleSpace();
                    Type viewDataType = Type.GetType(info.viewDataTypeName);
                    List<string> fieldNameList = dataTypes[dataTypeIdx].GetFields().Where(e => e.FieldType == viewDataType).Select(e => e.Name).ToList();
                    List<string> propNameList = dataTypes[dataTypeIdx].GetProperties().Where(e => e.PropertyType == viewDataType).Select(e => e.Name).ToList();
                    List<string> list = fieldNameList.Concat(propNameList).ToList();
                    if (list.Count <= 0)
                    {
                        GUILayout.Label("field or property not found", errorStyle);
                        info.isValid = false;
                    }
                    else
                    {
                        int fieldNameIdx = Mathf.Clamp(list.IndexOf(info.fieldName), 0, list.Count);
                        int idx = EditorGUILayout.Popup(fieldNameIdx, list.ToArray(), GUILayout.Width(100), GUILayout.ExpandWidth(false));
                        if (fieldNameIdx != idx)
                            dirty = true;
                        info.fieldName = list[idx];
                        info.isValid = true;
                    }

                    EditorGUILayout.EndHorizontal();
                }

            }
            if (dirty)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(obj.gameObject);
                EditorUtility.SetDirty(obj);
            }
        }
    }
#endif
}