using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;




#if UNITY_EDITOR
using UnityEditor;
#endif
public class MVStructOp : MonoBehaviour
{
    public MVStructInfo structInfo;
#if UNITY_EDITOR
    [CustomEditor(typeof(MVStructOp))]
    class MVStructOpEditor : Editor
    {
        private SerializedProperty structInfoField;

        private MVStructOp obj;
        STInfo<List<string>> invNameList = new(() =>
        {
            List<string> res = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                t.BaseType.IsGenericType
                && t.BaseType.GetGenericTypeDefinition() == typeof(VMInventory<>))
            .Select(t => t.Name).ToList();
            res.Insert(0, "this (private)");
            return res;
        });
        List<string> invList => invNameList;
        int invIdx = 0;
        Type viewType => coms[comIndex].GetType();
        int modelIndex = 0;
        int comIndex = 0;
        int adaptIdx = 0;
        List<MonoBehaviour> coms = new();
        const string errorPath = "#Error#";
        const string noneAdapt = "#None#";
        const string noneMV = "#None#";
        enum EStructEditState
        {
            NoMatch = 1,
            NoMatchWithAdapter = 2,
            Match = 3,
            MatchWithAdapter = 4,
            Incomplete = 5,
            PrivateMode = 6,
        }
        private void OnEnable()
        {
            structInfoField = serializedObject.FindProperty("structInfo");
            obj = (MVStructOp)target;
        }
        public override void OnInspectorGUI()
        {
            MVStructInfo structInfo = structInfoField.objectReferenceValue as MVStructInfo;
            bool dirty = false;
            bool isChanged = false;
            if (structInfo != null)
            {
                MVUI root = obj.transform.RecurFind(e => e.parent, e => e.GetComponent<MVUI>());
                GUI.enabled = false;
                EditorGUILayout.ObjectField("MVUI", root, typeof(GameObject), true);
                GUI.enabled = true;
                GUIStyle sucessesStyle = new GUIStyle(GUI.skin.label);
                sucessesStyle.normal.textColor = new Color(49 / 255f, 147 / 255f, 18 / 255f);
                GUIStyle warnStyle = new GUIStyle(GUI.skin.label);
                warnStyle.normal.textColor = new Color(152 / 255f, 102 / 255f, 56 / 255f);
                GUIStyle errorStyle = new GUIStyle(GUI.skin.label);
                errorStyle.normal.textColor = new Color(177 / 255f, 12 / 255f, 12 / 255f);
                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
                Type mvDataType = null;
                Type vmDataType = null;
                List<string> adapters = new();
                EStructEditState flag = EStructEditState.NoMatch;
                // 自定义字段的绘制逻辑
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField("StructInfo:", GUILayout.Width(70), GUILayout.ExpandWidth(false));

                Component[] comps = obj.GetComponents(typeof(MonoBehaviour));
                coms = comps.ToList().FindAll(e => e.GetType().BaseType.IsGenericType && e.GetType().BaseType.GetGenericTypeDefinition() == typeof(MView<>)).Select(e => e as MonoBehaviour).ToList();

                if (coms.Count <= 0)
                {
                    structInfo.mvType = noneMV;
                    structInfo.isValid = false;
                    GUILayout.Label("not found MView in gameObject", errorStyle, GUILayout.Width(300), GUILayout.ExpandWidth(false));
                    flag = EStructEditState.Incomplete;
                }
                else
                {
                    comIndex = Mathf.Clamp(coms.FindIndex(e => e.GetType().AssemblyQualifiedName == structInfo.mvType), 0, coms.Count);
                    int idx1 = EditorGUILayout.Popup(comIndex, coms.Select(e => e.GetType().Name).ToArray(), GUILayout.Width(100));
                    isChanged = isChanged || comIndex != idx1;
                    comIndex = idx1;
                    Type t = coms[comIndex].GetType();
                    while (t != null && !(t.IsGenericType && t.GetGenericTypeDefinition() == typeof(MView<>))) t = t.BaseType;
                    structInfo.mvType = coms[comIndex].GetType().AssemblyQualifiedName;
                    mvDataType = t.GetGenericArguments()[0];
                    structInfo.mvDataType = mvDataType.AssemblyQualifiedName;
                    List<string> invList = invNameList;
                    invIdx = structInfo.isPrivate ? 0 : Mathf.Clamp(invList.FindIndex(e => e == structInfo.invName), 1, invList.Count - 1);
                    int idx2 = EditorGUILayout.Popup(invIdx, invList.ToArray(), GUILayout.Width(100), GUILayout.ExpandWidth(false));
                    isChanged = isChanged || invIdx != idx2;
                    invIdx = idx2;
                    if (invList.Count > 1 && invIdx > 0)
                    {
                        structInfo.isPrivate = false;
                        structInfo.invName = invList[invIdx];
                        List<string> modelList =
                            Type.GetType(invList[invIdx])
                            .GetFields()
                            .Where(f => f.FieldType.BaseType != null &&
                                f.FieldType.BaseType.IsGenericType &&
                                f.FieldType.BaseType.GetGenericTypeDefinition() == typeof(VMBase<>))
                            .Select(f => f.Name).ToList();
                        if (modelList.Count > 0)
                        {
                            modelIndex = Mathf.Clamp(modelList.FindIndex(e => e == structInfo.modelName), 0, modelList.Count - 1);
                            int idx3 = EditorGUILayout.Popup(modelIndex, modelList.ToArray(), GUILayout.Width(100), GUILayout.ExpandWidth(false));
                            isChanged = isChanged || idx3 != modelIndex;
                            modelIndex = idx3;
                            structInfo.modelName = modelList[modelIndex];

                            vmDataType = Type.GetType(invList[invIdx]).GetField(modelList[modelIndex]).FieldType.BaseType.GetGenericArguments()[0];
                            structInfo.vmDataType = vmDataType.AssemblyQualifiedName;
                            adapters = Type.GetType(invList[invIdx]).GetFields()
                                .Where(f => f.FieldType != null &&
                                    f.FieldType.IsGenericType &&
                                    f.FieldType.GetGenericTypeDefinition() == typeof(VMAdapter<,>) &&
                                        f.FieldType.GetGenericArguments()[0] == vmDataType &&
                                        f.FieldType.GetGenericArguments()[1] == mvDataType
                                    )
                                .Select(f => f.Name).ToList();

                            if (mvDataType != vmDataType)
                            {
                                if (adapters.Count == 0)
                                {
                                    structInfo.isValid = false;
                                    flag = EStructEditState.NoMatch;
                                    structInfo.hasAdapter = false;
                                    structInfo.adapterName = noneAdapt;
                                }
                                else
                                {
                                    structInfo.hasAdapter = true;
                                    adaptIdx = Mathf.Clamp(adapters.FindIndex(e => e == structInfo.adapterName), 0, adapters.Count);
                                    isChanged = isChanged || structInfo.adapterName != adapters[adaptIdx];
                                    structInfo.adapterName = adapters[adaptIdx];
                                    flag = EStructEditState.MatchWithAdapter;
                                    structInfo.isValid = true;
                                }
                            }
                            else
                            {
                                if (adapters.Count == 0)
                                {
                                    structInfo.hasAdapter = false;
                                    structInfo.adapterName = noneAdapt;
                                }
                                structInfo.isValid = true;
                                flag = EStructEditState.Match;
                            }
                            if (structInfo.hasAdapter)
                            {
                                if (adapters.Count == 0)
                                {
                                    flag = EStructEditState.NoMatchWithAdapter;
                                    structInfo.isValid = false;
                                }
                                else
                                {
                                    flag = EStructEditState.MatchWithAdapter;
                                    structInfo.isValid = true;
                                }
                            }
                            else
                            {
                                if (mvDataType != vmDataType)
                                {
                                    structInfo.isValid = false;
                                    flag = EStructEditState.NoMatch;
                                }
                                else
                                {
                                    structInfo.isValid = true;
                                    flag = EStructEditState.Match;
                                }
                            }
                        }
                        else
                        {
                            GUILayout.Label("no VM field", warnStyle, GUILayout.Width(100), GUILayout.ExpandWidth(false));
                            flag = EStructEditState.Incomplete;
                            structInfo.isValid = false;
                        }
                    }
                    else
                    {
                        structInfo.isPrivate = true;
                        structInfo.eventKey = structInfo.eventKey == "" ? "defaultKey" : structInfo.eventKey;
                        structInfo.eventKey = EditorGUILayout.TextField(structInfo.eventKey, GUILayout.Width(100));
                        flag = EStructEditState.PrivateMode;
                        structInfo.isValid = true;
                    }
                }
                EditorGUILayout.EndHorizontal();
                if (flag != EStructEditState.Incomplete)
                {

                    List<string> modelList = invIdx == 0 ? new() :
                        Type.GetType(invList[invIdx])
                        .GetFields()
                        .Where(f => f.FieldType.BaseType != null &&
                            f.FieldType.BaseType.IsGenericType &&
                            f.FieldType.BaseType.GetGenericTypeDefinition() == typeof(VMBase<>))
                        .Select(f => f.Name).ToList();
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    string extDataStr = "";
                    if (coms[comIndex] is IListView)
                    {
                        Type dataType = (coms[comIndex] as IListView).DataType;
                        if (dataType != null)
                            extDataStr = "_" + dataType.Name;
                    }
                    if (flag == EStructEditState.NoMatch)
                        GUILayout.Label("MVType:" + mvDataType.Name + " != " + "VMType:" + vmDataType.Name + extDataStr, errorStyle, GUILayout.ExpandWidth(false));
                    else if (flag == EStructEditState.NoMatchWithAdapter)
                        GUILayout.Label("VMType:" + vmDataType.Name + " | no adapter" + extDataStr, errorStyle, GUILayout.ExpandWidth(false));
                    else if (flag == EStructEditState.Match)
                        GUILayout.Label("【OK】" + viewType.Name + " => " + invList[invIdx] + "." + modelList[modelIndex] + "(" + mvDataType.Name + extDataStr + ")", sucessesStyle, GUILayout.ExpandWidth(false));
                    else if (flag == EStructEditState.MatchWithAdapter)
                        GUILayout.Label("【OK】" + viewType.Name + " => " + modelList[modelIndex] + "|" + structInfo.adapterName + "(" + mvDataType.Name + extDataStr + ")", sucessesStyle, GUILayout.ExpandWidth(false));
                    else if (flag == EStructEditState.PrivateMode)
                        GUILayout.Label("【OK】" + viewType.Name + " drived by event:【" + structInfo.eventKey + "】(" + mvDataType.Name + extDataStr + ")", sucessesStyle, GUILayout.ExpandWidth(false));

                    if (!structInfo.isPrivate && adapters.Count == 0)
                    {
                        GUILayout.Label("no vm adapter", warnStyle, GUILayout.Width(100), GUILayout.ExpandWidth(false));
                    }
                    else if (!structInfo.isPrivate)
                    {
                        if (!structInfo.hasAdapter)
                        {
                            if (GUILayout.Button("active adapter"))
                                structInfo.hasAdapter = true;
                        }
                        else if (structInfo.hasAdapter)
                        {
                            adaptIdx = Mathf.Clamp(adapters.FindIndex(e => e == structInfo.adapterName), 0, adapters.Count);
                            adaptIdx = EditorGUILayout.Popup(adaptIdx, adapters.ToArray(), GUILayout.Width(vmDataType == mvDataType ? 100 : 90), GUILayout.ExpandWidth(false));
                            structInfo.adapterName = adapters[adaptIdx];
                            if (vmDataType == mvDataType)
                                structInfo.hasAdapter = EditorGUILayout.Toggle(structInfo.hasAdapter, GUILayout.Width(10), GUILayout.ExpandWidth(false));
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Delete"))
                {
                    obj.structInfo = null;
                    AssetDatabase.DeleteAsset(MVUI.InfoPath + structInfo.name + ".asset");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    dirty = true;
                }
                if (GUILayout.Button("Clear"))
                {
                    obj.structInfo = null;
                    structInfo = null;
                    dirty = true;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("New Struct"))
                {
                    MVStructInfo newInfo = ScriptableObject.CreateInstance<MVStructInfo>();
                    obj.structInfo = newInfo;
                    AssetDatabase.CreateAsset(newInfo, MVUI.InfoPath + "StructInfo_" + newInfo.GetHashCode() + ".asset");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    structInfo = obj.structInfo;
                    dirty = true;
                }
                EditorGUILayout.EndHorizontal();
            }

            if (dirty || isChanged)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(obj.gameObject);
                EditorUtility.SetDirty(obj);
                if (structInfo)
                    EditorUtility.SetDirty(structInfo);
            }
        }
    }
#endif
}