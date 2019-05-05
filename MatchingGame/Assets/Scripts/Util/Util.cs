using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    public static System.Security.Cryptography.MD5 md5Hash = System.Security.Cryptography.MD5.Create();
    public static string GetMd5Hash(byte[] rawdata)
    {
        byte[] data = md5Hash.ComputeHash(rawdata);
        var stringBuilder = new System.Text.StringBuilder();
        for (int i = 0; i < data.Length; i++)
            stringBuilder.Append(data[i].ToString("x2"));
        return stringBuilder.ToString();
    }

    public static bool QuickCompare(byte[] b1, byte[] b2)
    {
        if (b1 == null || b2 == null || b1.Length != b2.Length)
            return false;
        int len = b1.Length;
        for (int i = 0; i < len; i++)
        {
            if (b1[i] != b2[i])
                return false;
        }

        return true;
    }

    public static string NumberFormat(long number)
    {
        return number.ToString("N0");
    }

    public static Vector3 GetPostConvert(Vector3 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100))
            Debug.DrawLine(ray.origin, hit.point);
        return hit.point;
    }

    public static void PlayAnim(GameObject gObj, Vector2 scaleStart, float duration)
    {
        if (gObj == null)
            return;
        Vector2 vecDefault = new Vector2(0.25f, 0.25f);
        Vector2 step1 = scaleStart + vecDefault;
        Vector2 step2 = step1 + vecDefault;
        Vector2 step3 = step2 + vecDefault;
        Vector2 step4 = step3 + vecDefault;
        Sequence seq = DOTween.Sequence();
        seq.Append(gObj.transform.DOScale(step1, duration));
        seq.Append(gObj.transform.DOScale(step1, duration));
        seq.Append(gObj.transform.DOScale(step1, duration));
        seq.Append(gObj.transform.DOScale(step1, duration));
        seq.SetLoops(-1);
    }

    public static int CalculateScore(int numMatchedNode)
    {
        //todo: Calculate score by Game Design
        if (numMatchedNode <= 0)
            return 0;
        return (Mathf.RoundToInt(numMatchedNode / 3f));

    }
}
