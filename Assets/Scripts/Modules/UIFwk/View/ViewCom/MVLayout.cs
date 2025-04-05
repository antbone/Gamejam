using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif
[RequireComponent(typeof(LayoutGroup))]
public class MVLayout : MView<List<object>>, IListView
{
    public Type ComType
    {
        get
        {
            if (itemPrefab == null)
                return null;
            else if (itemPrefab.GetComponent<VirtualListItem>() != null)
                return itemPrefab.GetComponent<VirtualListItem>().GetType();
            return null;
        }
    }
    public Type DataType
    {
        get
        {
            if (itemPrefab == null)
                return null;
            else if (itemPrefab.GetComponent<VirtualListItem>() != null)
                return itemPrefab.GetComponent<VirtualListItem>().DataType;
            return null;

        }
    }
    public GameObject itemPrefab;
    public bool clearAwake = true;
    private List<object> dataList;
    private readonly List<VirtualListItem> itemArray = new();
    private void Awake()
    {
        if (clearAwake)
            for (int i = 0; i < transform.childCount; i++)
                Destroy(transform.GetChild(i).gameObject);
    }
    public override void SetData(List<object> dataList)
    {
        this.dataList = dataList;
        OnRender();
    }
    public void SetDataInPreview(int cnt)
    {
        int childrenCnt = transform.childCount;
        while (childrenCnt > cnt)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
            childrenCnt--;
        }
        while (cnt > childrenCnt)
        {
            var item = Instantiate(itemPrefab);
            item.transform.SetParent(transform);
            childrenCnt++;
        }

    }

    private void OnRender()
    {
        for (int i = 0; i < dataList.Count; i++)
        {
            if (i < itemArray.Count)
            {
                var com = itemArray[i].GetComponent<VirtualListItem>();
                com.idx = i;
                com.data = dataList[i];
                com.gameObject.name = itemPrefab.name + "_" + i;
                itemArray[i].OnData();
            }
            else
            {
                var item = Instantiate(itemPrefab);
                item.transform.SetParent(transform);
                var com = item.GetComponent<VirtualListItem>();
                this.Send(ListEvent.LOAD_ITEM, com);
                itemArray.Add(com);
                com.idx = i;
                com.data = dataList[i];
                com.OnLoadItem();
                com.gameObject.name = itemPrefab.name + "_" + i;
                com.OnData();
            }
        }
        while (itemArray.Count > dataList.Count)
        {
            var item = itemArray[^1];
            item.OnDestoryItem();
            Destroy(item.gameObject);
        }
    }
#if UNITY_EDITOR

    [CustomEditor(typeof(MVLayout))]
    public class MVLayoutEditor : Editor
    {
        private SerializedProperty itemTemplate;
        private int previewNum = 0;
        private MVLayout obj;


        private void OnEnable()
        {
            itemTemplate = serializedObject.FindProperty("itemPrefab");
            obj = (MVLayout)target;
        }

        public override void OnInspectorGUI()
        {
            // serializedObject.Update();
            bool dirty = false;
            var oldVal = obj.itemPrefab;
            obj.itemPrefab = EditorGUILayout.ObjectField("模板预制体", obj.itemPrefab, typeof(GameObject), true) as GameObject;
            if (oldVal != obj.itemPrefab)
                dirty = true;
            previewNum = EditorGUILayout.IntField("预览数量", previewNum);

            var virtualList = (MVLayout)target;
            // 测试按钮
            if (GUILayout.Button("Preview in Editor"))
            {
                virtualList.SetDataInPreview(previewNum);
                dirty = true;
            }
            if (dirty)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(virtualList.gameObject);
            }
        }
    }
#endif
}