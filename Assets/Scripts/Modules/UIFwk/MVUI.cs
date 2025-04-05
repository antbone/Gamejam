using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class MVUI : MonoBehaviour
{
    public const string InfoPath = "Assets/ScriptableObjects/UI_gen/";
    private int fromPanelId = -1;
    public int FromID => fromPanelId;
    public object Data => VController.Ins.GetPanelData(fromPanelId);
    public List<MVStructInfo> structInfos = new();
    private List<IMVStruct> structs = new();
    [SerializeField]
    private List<MVTriggerInfo> triggerInfos = new();
    private List<IMVTrigger> triggers = new();
    public List<MVUI> children = new();
    private bool isInited = false;
    private bool isEnter = false;
    public bool isEnable = false;
    private bool isReallyEnter = false;
    void OnEnable()
    {
        isEnable = true;
        ExecuteEnter();
    }
    void OnDisable()
    {
        isEnable = false;
        ExecuteEnter();
    }
    public void BindPanel(int id)
    {
        this.fromPanelId = id;
        this.children.ForEach(e => e.BindPanel(id));
    }
    public void SendVMsg(string message)
    {
        this.structs.ForEach(e => e.OnReceiveMsg(message));
        children.ForEach(e => e.SendVMsg(message));
    }

    private void Init()
    {
        if (isInited)
            return;
        isInited = true;
        structs = new();
        for (int i = 0; i < structInfos.Count; i++)
        {
            MVStructInfo info = structInfos[i];
            if (!info || !info.isValid)
                continue;
            Type invType = Type.GetType(info.invName);
            object invIns = invType.RecurFind(e => e.BaseType, e => e.GetProperty("Ins")).GetValue(null);
            object vm = invType.GetField(info.modelName).GetValue(invIns);
            Transform mvTrans = transform.Find(info.goPath);
            if (mvTrans == null)
            {
                Debug.LogError($"{info.goPath} is null in MVUI: " + gameObject.name);
                continue;
            }
            object mv = mvTrans.GetComponent(Type.GetType(info.mvType));
            Type mvDataType = Type.GetType(info.mvDataType);
            Type vmDataType = Type.GetType(info.vmDataType);
            IMVStruct ins;
            if (info.isPrivate)
            {
                ins = Activator.CreateInstance(typeof(VPrivateStruct<>).MakeGenericType(mvDataType), this, mv, info.eventKey) as IMVStruct;
            }
            else if (info.hasAdapter)
            {
                object adapter = invType.GetField(info.adapterName).GetValue(invIns);
                ins = Activator.CreateInstance(typeof(MVStructWithAdapter<,>).MakeGenericType(mvDataType, vmDataType), mv, vm, adapter) as IMVStruct;
            }
            else
            {
                ins = Activator.CreateInstance(typeof(MVStruct<>).MakeGenericType(vmDataType), mv, vm) as IMVStruct;
            }
            structs.Add(ins);
        }
        triggers = new();
        for (int i = 0; i < triggerInfos.Count; i++)
        {
            GameObject go = transform.GetGoByPath(triggerInfos[i].goPath);
            if (go == null)
                continue;
            IMVTrigger trigger = go.GetComponent(Type.GetType(triggerInfos[i].triggerType)) as IMVTrigger;
            if (trigger == null)
                continue;
            triggers.Add(trigger);
        }
    }
    public void SetEnter()
    {
        isEnter = true;
        ExecuteEnter();
        children.ForEach(e => e.SetEnter());
    }
    private void Enter()
    {
        Init();
        for (int i = 0; i < structs.Count; i++)
        {
            if (structs[i] != null)
                structs[i].OnEnter();
        }
    }
    public void SetExit()
    {
        isEnter = false;
        ExecuteEnter();
        children.ForEach(e => e.SetExit());
    }
    private void Exit()
    {
        Init();
        for (int i = 0; i < structs.Count; i++)
        {
            if (structs[i] != null)
                structs[i].OnExit();
        }
    }
    private void ExecuteEnter()
    {
        if (!isReallyEnter && isEnter && isEnable)
        {
            Enter();
            isReallyEnter = true;
        }
        else if (isReallyEnter && (!isEnable || !isEnter))
        {
            Exit();
            isReallyEnter = false;
        }
    }
    public void OnUpdate()
    {
        for (int i = 0; i < structs.Count; i++)
        {
            if (structs[i] != null)
                structs[i].OnUpdate();
        }
        children.ForEach(e => e.OnUpdate());
    }
    public void Clear()
    {
        structs.Clear();
        triggers.Clear();
        children.ForEach(e => e.Clear());
    }
    public void OnReset()
    {
        Init();
        triggers.ForEach(e => e.OnReset());
        children.ForEach(e => e.OnReset());
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MVUI))]
    class MVUIEditor : Editor
    {
        private SerializedProperty infos;
        private MVUI obj;
        private void OnEnable()
        {

            obj = (MVUI)target;
            infos = serializedObject.FindProperty("infos");
        }
        private void ScanRecursive(Transform cur, Transform root)
        {
            for (int i = 0; i < cur.childCount; i++)
            {
                Transform child = cur.GetChild(i);
                MonoBehaviour[] monos = child.GetComponents<MonoBehaviour>();
                for (int j = 0; j < monos.Length; j++)
                {
                    MVStructOp op = monos[j] as MVStructOp;
                    IMVTrigger trigger = monos[j] as IMVTrigger;
                    if (op != null)
                    {
                        MVStructInfo info = op.structInfo;
                        if (info != null)
                        {
                            obj.structInfos.Add(info);
                            info.goPath = root.GetGoPath(op.gameObject);
                        }
                    }
                    else if (trigger != null)
                    {
                        obj.triggerInfos.Add(new MVTriggerInfo()
                        {
                            goPath = root.GetGoPath(monos[j].gameObject),
                            triggerType = monos[j].GetType().AssemblyQualifiedName,
                            key = trigger.Key
                        });
                    }
                }

                MVUI mvui = child.GetComponent<MVUI>();
                if (mvui == null)
                    ScanRecursive(cur.GetChild(i), root);
                else
                {
                    obj.children.Add(mvui);
                }
            }
        }
        private void Scan()
        {
            obj.structInfos.Clear();
            obj.triggerInfos.Clear();
            obj.children.Clear();
            ScanRecursive(obj.transform, obj.transform);
            for (int i = 0; i < obj.structInfos.Count; i++)
            {
                MVStructInfo info = obj.structInfos[i];
                string goPath = info.goPath;
                string resMsg = AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(info), obj.gameObject.name + "_" + i + "_TEMP.asset");
                info.goPath = goPath;
                if (resMsg != "")
                {
                    Debug.LogError(resMsg);
                }
                string path = AssetDatabase.GetAssetPath(info);
                AssetDatabase.WriteImportSettingsIfDirty(path);
                EditorUtility.SetDirty(info);
            }
            for (int i = 0; i < obj.structInfos.Count; i++)
            {
                MVStructInfo info = obj.structInfos[i];
                string goPath = info.goPath;
                string resMsg = AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(info), obj.gameObject.name + "_" + i + ".asset");
                info.goPath = goPath;
                if (resMsg != "")
                {
                    Debug.LogError(resMsg);
                }
                string path = AssetDatabase.GetAssetPath(info);
                AssetDatabase.WriteImportSettingsIfDirty(path);
                EditorUtility.SetDirty(info);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(obj.gameObject);

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
                dirty = true;
                Scan();
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("【MVStruct List】：");
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            for (int i = 0; i < obj.structInfos.Count; i++)
            {
                MVStructInfo info = obj.structInfos[i];
                if (info == null)
                {
                    EditorGUILayout.LabelField("NULL MVStructInfo!", errorStyle);
                    continue;
                }
                GameObject go = obj.transform.GetGoByPath(info.goPath);
                GUI.enabled = false;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(go, typeof(GameObject), true, GUILayout.Width(100), GUILayout.ExpandWidth(false));
                GUILayout.FlexibleSpace();
                GUI.enabled = true;
                if (!info.isValid)
                {
                    GUILayout.Label("【Error】 MVStructInfo is incomplete!", warnStyle, GUILayout.ExpandWidth(false));
                }
                else
                {
                    Type mvType = Type.GetType(info.mvType);
                    Type mvDataType = Type.GetType(info.mvDataType);
                    Type vmDataType = Type.GetType(info.vmDataType);
                    if (info.isPrivate && mvType != null && mvDataType != null)
                        GUILayout.Label($"【{mvType.Name}】 drived by 【{info.eventKey} 】({mvDataType.Name})", sucessesStyle, GUILayout.ExpandWidth(false));
                    else if (info.hasAdapter && mvType != null && mvDataType != null)
                        GUILayout.Label($"【{mvType.Name}】 binded by 【{info.invName}.{info.modelName} | {info.adapterName}】({mvDataType.Name})", sucessesStyle, GUILayout.ExpandWidth(false));
                    else if (mvType != null && mvDataType != null)
                        GUILayout.Label($"【{mvType.Name}】 binded by 【{info.invName}.{info.modelName}】({mvDataType.Name})", sucessesStyle, GUILayout.ExpandWidth(false));
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("【MVTrigger List】：");
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            for (int i = 0; i < obj.triggerInfos.Count; i++)
            {
                MVTriggerInfo info = obj.triggerInfos[i];
                GUI.enabled = false;
                EditorGUILayout.BeginHorizontal();
                GameObject go = obj.transform.GetGoByPath(info.goPath);
                string typeName = Type.GetType(info.triggerType).Name;
                EditorGUILayout.ObjectField(go, typeof(GameObject), true, GUILayout.Width(100), GUILayout.ExpandWidth(false));
                GUILayout.FlexibleSpace();
                GUI.enabled = true;
                GUILayout.Label($"【{typeName}】 for 【{info.key}】");
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("【MVUI Child】：");
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            for (int i = 0; i < obj.children.Count; i++)
            {
                MVUI info = obj.children[i];
                GUI.enabled = false;
                EditorGUILayout.BeginHorizontal();
                GameObject go = info.gameObject;
                EditorGUILayout.ObjectField(go, typeof(GameObject), true, GUILayout.Width(100), GUILayout.ExpandWidth(false));
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
            }
            if (dirty)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(obj.gameObject);
            }
        }
    }
#endif
}