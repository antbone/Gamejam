// Learn TypeScript:
//  - https://docs.cocos.com/creator/manual/en/scripting/typescript.html
// Learn Attribute:
//  - https://docs.cocos.com/creator/manual/en/scripting/reference/attributes.html
// Learn life-cycle callbacks:
//  - https://docs.cocos.com/creator/manual/en/scripting/life-cycle-callbacks.html



using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public enum EScrollAxis
{
    Vertical,
    Horizontal
}
public static class ListEvent
{
    public static readonly string
    SCROLL = "SCROLL",
    BEGIN_SCROLL = "BEGIN_SCROLL",
    RELAYOUT = "RE_LAYOUT",
    LOAD_ITEM = "LOAD_ITEM";//参数：item:ScrollListItem
}
public interface IListView
{
    public Type ComType { get; }
    public Type DataType { get; }
}
[RequireComponent(typeof(ScrollRect))]
public class MVList : MView<List<object>>, IPointerDownHandler, IListView
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
    public EScrollAxis scrollAxis = EScrollAxis.Vertical;
    public GameObject itemPrefab;
    private List<object> dataList = new();
    public bool clearAwake = true;
    private List<VirtualListItem> itemArray = new();
    private STField<MVList, ScrollRect> scrollRect = new(e => e.GetComponent<ScrollRect>());
    private STField<MVList, LayoutGroup> layout = new(e => e.scrollRect.Get(e).content.GetComponent<LayoutGroup>());
    private int row = 1;
    private int col = 1;
    private void Awake()
    {

        scrollRect.Get(this).onValueChanged.AddListener((v) =>
        {
            this.ScrollCheck();
        });

        if (clearAwake)
            for (int i = 0; i < layout.Get(this).transform.childCount; i++)
                Destroy(layout.Get(this).transform.GetChild(i).gameObject);
        RefreshSize();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        this.Send(ListEvent.BEGIN_SCROLL);
    }
    public override void SetData(List<object> dataList)
    {
        this.dataList = dataList;
        this.OnRender();
        RefreshSize();
        CT.DelayCmd(this.ScrollCheck, 1);
        // this.ScrollCheck();
        this.Send(ListEvent.RELAYOUT);
    }
    public void SetDataInPreview(int cnt)
    {
        int childrenCnt = layout.Get(this).transform.childCount;
        while (childrenCnt > cnt)
        {
            DestroyImmediate(layout.Get(this).transform.GetChild(0).gameObject);
            childrenCnt--;
        }
        while (cnt > childrenCnt)
        {
            var item = Instantiate(itemPrefab);
            item.transform.SetParent(layout.Get(this).transform);
            childrenCnt++;
        }
        VerticalLayoutGroup vlg = layout.Get(this) as VerticalLayoutGroup;
        if (vlg != null)
        {
            vlg.GetComponent<RectTransform>().sizeDelta =
            new Vector2(vlg.GetComponent<RectTransform>().sizeDelta.x,
            (itemPrefab.GetComponent<RectTransform>().sizeDelta.y + vlg.spacing) * vlg.transform.childCount - vlg.spacing + vlg.padding.top + vlg.padding.bottom);
        }

    }
    public void RefreshSize()
    {
        RectTransform rt = layout.Get(this).GetComponent<RectTransform>();
        VerticalLayoutGroup vlg = layout.Get(this) as VerticalLayoutGroup;
        if (vlg != null)
        {
            rt.sizeDelta = new Vector2(rt.sizeDelta.x,
            (itemPrefab.GetComponent<RectTransform>().sizeDelta.y + vlg.spacing) * itemArray.Count - vlg.spacing + vlg.padding.top + vlg.padding.bottom);
            col = 1;
            row = itemArray.Count;
        }
        HorizontalLayoutGroup hlg = layout.Get(this) as HorizontalLayoutGroup;
        if (hlg != null)
        {
            rt.sizeDelta =
            new Vector2((itemPrefab.GetComponent<RectTransform>().sizeDelta.x + hlg.spacing) * itemArray.Count - hlg.spacing + hlg.padding.left + hlg.padding.right,
            rt.sizeDelta.y);
            row = 1;
            col = itemArray.Count;
        }
        GridLayoutGroup glg = layout.Get(this) as GridLayoutGroup;
        if (glg != null)
        {
            if (this.scrollAxis == EScrollAxis.Vertical)
            {
                glg.startAxis = GridLayoutGroup.Axis.Horizontal;
                glg.constraint = GridLayoutGroup.Constraint.Flexible;
                int col = this.col = Mathf.Max(1, Mathf.FloorToInt((rt.rect.width - glg.padding.horizontal + glg.spacing.x + 0.001f) / (glg.cellSize.x + glg.spacing.x)));
                int row = this.row = Mathf.CeilToInt(itemArray.Count * 1f / col);
                rt.sizeDelta = new Vector2(rt.sizeDelta.x,
            (glg.cellSize.y + glg.spacing.y) * row - glg.spacing.y + glg.padding.top + glg.padding.bottom);

            }
            else
            {
                glg.startAxis = GridLayoutGroup.Axis.Vertical;
                glg.constraint = GridLayoutGroup.Constraint.Flexible;
                int row = this.row = Mathf.Max(1, Mathf.FloorToInt((rt.rect.height - glg.padding.vertical + glg.spacing.y + 0.001f) / (glg.cellSize.y + glg.spacing.y)));
                int col = this.col = Mathf.CeilToInt(itemArray.Count * 1f / row);
                rt.sizeDelta =
                new Vector2((glg.cellSize.x + glg.spacing.x) * col - glg.spacing.x + glg.padding.left + glg.padding.right,
                rt.sizeDelta.y);
            }
        }
    }
    public void ScrollToX(float x)
    {
        ScrollRect rect = scrollRect.Get(this);
        rect.normalizedPosition = new Vector2(x, rect.normalizedPosition.y);
    }
    public void ScrollToY(float y)
    {
        ScrollRect rect = scrollRect.Get(this);
        rect.normalizedPosition = new Vector2(rect.normalizedPosition.x, y);
    }
    public void ScrollToStart()
    {
        if (scrollAxis == EScrollAxis.Vertical)
            ScrollToY(0);
        else
            ScrollToX(0);
    }
    public void ScrollToEnd()
    {
        if (scrollAxis == EScrollAxis.Vertical)
            ScrollToY(1);
        else
            ScrollToX(1);

    }
    public void OnRender()
    {
        for (int i = 0; i < this.dataList.Count; i++)
        {
            if (i < this.itemArray.Count)
            {
                var com = this.itemArray[i].GetComponent<VirtualListItem>();
                com.idx = i;
                com.data = this.dataList[i];
                com.gameObject.name = itemPrefab.name + "_" + i;
                itemArray[i].OnData();
            }
            else
            {
                var item = Instantiate(itemPrefab);
                item.transform.SetParent(layout.Get(this).transform);
                var com = item.GetComponent<VirtualListItem>();
                this.Send(ListEvent.LOAD_ITEM, com);
                itemArray.Add(com);
                com.idx = i;
                com.data = this.dataList[i];
                com.OnLoadItem();
                com.gameObject.name = itemPrefab.name + "_" + i;
                com.OnData();
            }
        }
        while (this.itemArray.Count > this.dataList.Count)
        {
            var item = this.itemArray[^1];
            this.SetItemActive(item, false);
            item.OnDestoryItem();
            Destroy(item.gameObject);
        }
    }
    private void SetItemActive(VirtualListItem com, bool active)
    {
        if (active)
        {
            com.ShowIdle();
            if (com.itemState != EListItemState.Show)
                com.OnShow();
            com.itemState = EListItemState.Show;
        }
        else
        {
            com.HideIdle();
            if (com.itemState != EListItemState.Hide)
                com.OnHide();
            com.itemState = EListItemState.Hide;
        }
    }
    void Reset()
    {
        if (this.layout.Get(this) == null)
        {
            this.scrollRect.Get(this).content.AddComponent<VerticalLayoutGroup>();
        }
        var rt = this.layout.Get(this).GetComponent<RectTransform>();
        var prt = rt.parent as RectTransform;
        if (this.scrollAxis == EScrollAxis.Vertical)
        {
            rt.anchorMax = new Vector2(1, 1);
            rt.anchorMin = new Vector2(0, 1);
            rt.sizeDelta = new Vector2(0, prt.rect.height);
            rt.pivot = new Vector2(0.5f, 1);
            scrollRect.Get(this).horizontal = false;
            scrollRect.Get(this).vertical = true;
        }
        else if (this.scrollAxis == EScrollAxis.Horizontal)
        {
            rt.anchorMax = new Vector2(0, 1);
            rt.anchorMin = new Vector2(0, 0);
            rt.pivot = new Vector2(0, 0.5f);
            rt.sizeDelta = new Vector2(prt.rect.width, 0);
            scrollRect.Get(this).horizontal = true;
            scrollRect.Get(this).vertical = false;

        }
        var glg = this.layout.Get(this).GetComponent<GridLayoutGroup>();
        if (glg != null)
        {

            if (this.scrollAxis == EScrollAxis.Vertical)
            {
                glg.startAxis = GridLayoutGroup.Axis.Horizontal;
                glg.constraint = GridLayoutGroup.Constraint.Flexible;
            }
            else
            {
                glg.startAxis = GridLayoutGroup.Axis.Vertical;
                glg.constraint = GridLayoutGroup.Constraint.Flexible;
            }
        }
        rt.anchoredPosition = Vector2.zero;
    }
    private void ScrollCheck()
    {
        this.Send(ListEvent.SCROLL);
        float Vy = scrollRect.Get(this).viewport.rect.height;
        float Vx = scrollRect.Get(this).viewport.rect.width;
        if (scrollAxis == EScrollAxis.Vertical)
        {
            var Oy = layout.Get(this).GetComponent<RectTransform>().anchoredPosition.y;
            for (int i = 0; i < itemArray.Count; i++)
            {
                var comRt = itemArray[i].GetComponent<RectTransform>();
                var Cy = comRt.anchoredPosition.y;
                if (Cy + (1 - comRt.pivot.y) * comRt.rect.height > -Oy - Vy && Cy - comRt.pivot.y * comRt.rect.height < -Oy)
                    SetItemActive(itemArray[i], true);
                else
                    SetItemActive(itemArray[i], false);
            }
        }
        else if (scrollAxis == EScrollAxis.Horizontal)
        {
            var Ox = layout.Get(this).GetComponent<RectTransform>().anchoredPosition.x;
            for (int i = 0; i < itemArray.Count; i++)
            {
                var comRt = itemArray[i].GetComponent<RectTransform>();
                var Cx = comRt.anchoredPosition.x;
                if (Cx + (1 - comRt.pivot.x) * comRt.rect.width > -Ox && Cx - comRt.pivot.x * comRt.rect.width < -Ox + Vx)
                    SetItemActive(itemArray[i], true);
                else
                    SetItemActive(itemArray[i], false);
            }
        }

    }
#if UNITY_EDITOR
    [CustomEditor(typeof(MVList))]
    class VirtualListEditor : Editor
    {
        private SerializedProperty itemTemplate;
        private int previewNum = 0;
        MVList obj;

        private void OnEnable()
        {
            itemTemplate = serializedObject.FindProperty("itemPrefab");
            obj = (MVList)target;
        }

        public override void OnInspectorGUI()
        {
            // serializedObject.Update();
            bool dirty = false;
            var oldVal = obj.itemPrefab;
            obj.itemPrefab = EditorGUILayout.ObjectField("模板预制体", obj.itemPrefab, typeof(GameObject), true) as GameObject;
            if (oldVal != obj.itemPrefab)
                dirty = true;
            EScrollAxis oldAxis = obj.scrollAxis;
            obj.scrollAxis = (EScrollAxis)EditorGUILayout.EnumPopup("滚动模式", obj.scrollAxis);
            if (oldAxis != obj.scrollAxis)
            {
                dirty = true;
                obj.Reset();
                EditorUtility.SetDirty(obj.layout.Get(obj));
            }

            previewNum = EditorGUILayout.IntField("预览数量", previewNum);

            var virtualList = (MVList)target;
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