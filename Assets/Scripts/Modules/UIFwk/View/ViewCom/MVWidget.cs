using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
[Serializable]
public class WidgetInfo
{
    public Vector2 sizeDelta;
    public Vector2 anchoredPosition;
    public Vector2 scale;
    public float angle;
}
[RequireComponent(typeof(RectTransform))]
public class MVWidget : MView<int>
{
    private STField<MVWidget, RectTransform> rt = new(e => e.GetComponent<RectTransform>());
    public List<WidgetInfo> infos = new();
    public override void SetData(int state)
    {
        if (state >= 0 && state < infos.Count)
        {
            WidgetInfo info = infos[state];
            RectTransform rect = rt.Get(this);
            rect.anchoredPosition = info.anchoredPosition;
            rect.sizeDelta = info.sizeDelta;
            rect.rotation = Quaternion.Euler(0, 0, info.angle);
            rect.localScale = new Vector3(info.scale.x, info.scale.y, 1);
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(MVWidget))]
    class MVWidgetEditor : Editor
    {
        private SerializedProperty infoList;
        private void OnEnable()
        {
            infoList = serializedObject.FindProperty("infos");
        }
        public override void OnInspectorGUI()
        {
            bool dirty = false;
            dirty = dirty || EditorGUILayout.PropertyField(infoList);
            var obj = (MVWidget)target;
            if (GUILayout.Button("Record current state"))
            {
                RectTransform rt = obj.rt.Get(obj);
                obj.infos.Add(new WidgetInfo()
                {
                    sizeDelta = rt.sizeDelta,
                    anchoredPosition = rt.anchoredPosition,
                    scale = rt.localScale,
                    angle = rt.rotation.eulerAngles.z
                });
                dirty = true;
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