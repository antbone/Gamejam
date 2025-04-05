using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public static class EM
{

    private static Vector2 FlyCurve(Vector2 startPos, Vector2 endPos, Vector2 vec, float t)
    {
        Vector2 a = 2 * (endPos - startPos) - 2 * vec;
        Vector2 pos = startPos + vec * t + 0.5f * a * t * t;
        return pos;
    }
    public static void RandomFly(this Transform transform, Vector2 target, Action cb = null, Action<Vector2, float> flyFunc = null, float speed = 3f, float time = 1, float offset = -0.3f, float horScale = 2f)
    {
        Vector2 startPos = transform.position;
        Vector2 dir = (target - new Vector2(transform.position.x, transform.position.y)).normalized;
        Vector2 horDir = new Vector2(-dir.y, dir.x).normalized;
        Vector2 unit = UnityEngine.Random.insideUnitCircle * speed + offset * (target - startPos);
        float horLen = Vector2.Dot(unit, horDir) * horScale;
        float verLen = Vector2.Dot(unit, dir);
        unit = horDir * horLen + dir * verLen;

        Vector2 lastPos = transform.position;
        TM.SetTimer(transform.GetHashCode().ToString() + "RandomFly", time, t =>
        {
            transform.position = FlyCurve(startPos, target, unit, t);
            Vector2 delta = new Vector2(transform.position.x, transform.position.y) - lastPos;
            lastPos = transform.position;
        }, s =>
        {
            transform.position = target;
            cb?.Invoke();
        });
    }
    public static void RandomFlyUI(this RectTransform transform, Vector2 target, Action cb = null, Action<Vector2, float> flyFunc = null, float speed = 3f, float time = 1, float offset = -0.3f, float horScale = 2f)
    {
        Vector2 startPos = transform.anchoredPosition;
        Vector2 dir = (target - new Vector2(transform.anchoredPosition.x, transform.anchoredPosition.y)).normalized;
        Vector2 horDir = new Vector2(-dir.y, dir.x).normalized;
        Vector2 unit = UnityEngine.Random.insideUnitCircle * speed + offset * (target - startPos);
        float horLen = Vector2.Dot(unit, horDir) * horScale;
        float verLen = Vector2.Dot(unit, dir);
        unit = horDir * horLen + dir * verLen;
        Vector2 lastPos = transform.anchoredPosition;
        TM.SetTimer(transform.GetHashCode().ToString() + "RandomFly", time, t =>
        {
            transform.anchoredPosition = FlyCurve(startPos, target, unit, t);
            Vector2 delta = transform.anchoredPosition - lastPos;
            lastPos = transform.anchoredPosition;
            flyFunc?.Invoke(delta, t);
        }, s =>
        {
            transform.anchoredPosition = target;
            cb?.Invoke();
        });
    }
    public static void JellyJump(this Transform transform, float time = 0.7f, float maxDeltaScale = 0.25f, float cnt = 2f)
    {
        // 获取初始缩放比例
        TM.SetEnd(transform.GetHashCode().ToString() + "JellyJump", true);
        Vector3 originalScale = transform.localScale;
        TM.SetTimer(transform.GetHashCode().ToString() + "JellyJump", time, t =>
        {
            float scaleX = -Mathf.Sin(t * 2 * Mathf.PI * cnt / time) * maxDeltaScale * (1 - t) + originalScale.x;
            float scaleY = Mathf.Sin(t * 2 * Mathf.PI * cnt / time) * maxDeltaScale * (1 - t) + originalScale.y;
            transform.localScale = new Vector3(scaleX, scaleY, originalScale.z);
        }, s =>
        {
            transform.localScale = originalScale;
        });
    }
    public static void Emit(this SpriteRenderer sprr, Sprite sprite, Transform parent, Vector2 lifeTime, Vector2 distance, Vector2 scale, Vector2 angle, Vector2 cnt, Color[] colors)
    {
        int num = Mathf.FloorToInt(UnityEngine.Random.Range(cnt.x, cnt.y));
        for (int i = 0; i < num; i++)
        {
            float life = UnityEngine.Random.Range(lifeTime.x, lifeTime.y);
            GameObject go = new GameObject();
            SpriteRenderer sprr2 = go.AddComponent<SpriteRenderer>();
            sprr2.sprite = sprite;
            sprr2.color = colors[UnityEngine.Random.Range(0, colors.Length)];
            go.transform.SetParent(parent);
            go.transform.localScale = Vector3.one * UnityEngine.Random.Range(scale.x, scale.y);
            go.transform.position = sprr.transform.position;
            go.transform.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(angle.x, angle.y));
            float dist = UnityEngine.Random.Range(distance.x, distance.y);
            Vector3 delta = dist * UnityEngine.Random.insideUnitCircle;
            Vector3 pos = go.transform.position + delta;
            go.transform.DOMove(pos, life / 2).SetEase(Ease.OutQuad).onComplete = () =>
            {
                go.GetComponent<SpriteRenderer>().DOFade(0, life / 2).onComplete = () =>
                {
                    GameObject.Destroy(go);
                };
            };
        }
    }
    public static void EmitUI(Vector2 targetSp, Sprite sprite, Transform parent, Vector2 lifeTime, Vector2 distance, Vector2 scale, Vector2 angle, Vector2 cnt, Color[] colors)
    {

        int num = Mathf.FloorToInt(UnityEngine.Random.Range(cnt.x, cnt.y));
        for (int i = 0; i < num; i++)
        {
            float life = UnityEngine.Random.Range(lifeTime.x, lifeTime.y);
            GameObject go = new GameObject();
            go.transform.SetParent(parent);
            RectTransform rt = go.AddComponent<RectTransform>();
            rt.anchoredPosition = targetSp;
            Image img = go.AddComponent<Image>();
            img.sprite = sprite;
            img.color = colors[UnityEngine.Random.Range(0, colors.Length)];

            rt.sizeDelta = Vector2.one * UnityEngine.Random.Range(scale.x, scale.y);
            float dist = UnityEngine.Random.Range(distance.x, distance.y);
            Vector2 delta = dist * UnityEngine.Random.insideUnitCircle.normalized;
            Vector2 pos = rt.anchoredPosition + delta;
            rt.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(angle.x, angle.y));
            rt.DOAnchorPos(pos, life / 2).SetEase(Ease.OutQuad).onComplete = () =>
            {
                go.GetComponent<Image>().DOFade(0, life / 2).onComplete = () =>
                {
                    GameObject.Destroy(go);
                };
            };
        }
    }
}
