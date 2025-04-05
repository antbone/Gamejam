

using System;
using System.Text.RegularExpressions;
using UnityEngine;
/// <summary>
/// 大数字结构体（只支持正数）
/// </summary>
public struct BNum
{
    private float baseNum;//1000进制
    private int indexNum;//indexNum=1即代表k，=2及代表M
    private readonly int mantissa;//字符串尾数位数
    private string GetUnit(int indexNum)
    {
        if (indexNum < 0)
        {
            return "";
        }

        string[] baseUnit = new string[] {
            "", "K", "M", "B", "T"
        };

        if (indexNum < baseUnit.Length)
        {
            return baseUnit[indexNum];
        }

        // 处理超过 "T" 的情况
        indexNum -= baseUnit.Length; // 减去基础单位的数量
        string unit = "";

        do
        {
            int remainder = indexNum % 26;
            char letter = (char)('a' + remainder);
            unit = letter + unit;
            indexNum /= 26;
            if (remainder == 0)
            {
                indexNum--; // 调整进位
            }
        } while (indexNum > 0);

        return unit;
    }
    public readonly bool IsValid => indexNum >= 0 && baseNum >= 0;
    private string GetStr(float a)
    {
        return a.ToString($"f{this.mantissa}");
    }
    public BNum(float baseNum, int indexNum = 0, int mantissa = 2)
    {
        this.baseNum = baseNum;
        this.indexNum = indexNum;
        this.mantissa = mantissa;
        Arrange();
    }
    public BNum(int baseNum, int indexNum = 0, int mantissa = 2)
    {
        this.baseNum = baseNum;
        this.indexNum = indexNum;
        this.mantissa = mantissa;
        Arrange();
    }
    public BNum(string str, int mantissa = 2)
    {
        BNum res = BNum.FromStr(str);
        this.baseNum = res.baseNum;
        this.indexNum = res.indexNum;
        this.mantissa = mantissa;
    }
    /// <summary>
    /// 转换为带单位字符串
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        if (this.indexNum == 0)
            return this.baseNum.ToString($"f{this.mantissa}");
        return this.baseNum.ToString($"f{this.mantissa}") + GetUnit(indexNum);
    }
    //转换为长字符串
    public string ToLong()
    {
        float baseTmp = this.baseNum;
        string str = Mathf.Floor(baseTmp).ToString();
        int index = this.indexNum;
        baseTmp = baseTmp - Mathf.Floor(baseTmp);
        float floor = Mathf.Floor(baseTmp);
        while (index > 0)
        {
            baseTmp *= 1000;
            index--;
            floor = Mathf.Floor(baseTmp);
            str += floor.ToString("f0");
            baseTmp -= floor;
        }
        floor = Mathf.Floor(baseTmp);
        str += floor.ToString();
        baseTmp = baseTmp - floor;
        str += baseTmp.ToString()[1..];
        return str;
    }
    private void Arrange()
    {
        while (baseNum > 1000)
        {
            baseNum /= 1000;
            indexNum++;
        }
        while (baseNum < 1 && indexNum > 0)
        {
            baseNum *= 1000;
            indexNum--;
        }
        baseNum = baseNum.RoundTo(11);
    }
    public static BNum operator +(BNum a, BNum b)
    {
        if (a.indexNum < b.indexNum - 4)
            return b;
        if (b.indexNum < a.indexNum - 4)
            return a;
        if (a.indexNum < b.indexNum)
        {
            BNum tmp = new BNum(a.baseNum, a.indexNum);
            while (tmp.indexNum < b.indexNum)
            {
                tmp.baseNum /= 1000;
                tmp.indexNum++;
            }
            return new BNum(tmp.baseNum + b.baseNum, tmp.indexNum);
        }
        else if (a.indexNum > b.indexNum)
        {
            BNum tmp = new BNum(b.baseNum, b.indexNum);
            while (tmp.indexNum < a.indexNum)
            {
                tmp.baseNum /= 1000;
                tmp.indexNum++;
            }
            return new BNum(tmp.baseNum + a.baseNum, tmp.indexNum);
        }
        else
        {
            return new BNum(a.baseNum + b.baseNum, a.indexNum);
        }
    }
    public static BNum operator -(BNum a, BNum b)
    {

        if (a.indexNum < b.indexNum - 4)
            return new BNum(0, 0);
        if (b.indexNum < a.indexNum - 4)
            return a;
        if (a.indexNum < b.indexNum)
        {
            BNum tmp = new BNum(a.baseNum, a.indexNum);
            while (tmp.indexNum < b.indexNum)
            {
                tmp.baseNum /= 1000;
                tmp.indexNum++;
            }
            return new BNum(tmp.baseNum - b.baseNum, tmp.indexNum);
        }
        else if (a.indexNum > b.indexNum)
        {
            BNum tmp = new BNum(b.baseNum, b.indexNum);
            while (tmp.indexNum < a.indexNum)
            {
                tmp.baseNum /= 1000;
                tmp.indexNum++;
            }
            return new BNum(a.baseNum - tmp.baseNum, a.indexNum);
        }
        else
        {
            return new BNum(a.baseNum - b.baseNum, a.indexNum);
        }
    }

    public static BNum operator *(BNum a, float b)
    {
        return new BNum(a.baseNum * b, a.indexNum);
    }
    public static BNum operator /(BNum a, float b)
    {
        return new BNum(a.baseNum / b, a.indexNum);
    }
    public static BNum operator *(BNum a, int b)
    {
        return new BNum(a.baseNum * b, a.indexNum);
    }
    public static BNum operator /(BNum a, int b)
    {
        return new BNum(a.baseNum / b, a.indexNum);
    }
    public override readonly bool Equals(object obj)
    {
        if (obj is BNum b)
        {
            return this == b;
        }
        return false;
    }
    public override readonly int GetHashCode()
    {
        return base.GetHashCode();
    }
    public readonly bool Equals(BNum b)
    {
        return baseNum == b.baseNum && indexNum == b.indexNum;
    }
    public static bool operator >(BNum a, BNum b)
    {
        if (a.indexNum > b.indexNum)
            return true;
        else if (a.indexNum < b.indexNum)
            return false;
        else
            return a.baseNum > b.baseNum;
    }
    public static bool operator <(BNum a, BNum b)
    {
        if (a.indexNum < b.indexNum)
            return true;
        else if (a.indexNum > b.indexNum)
            return false;
        else
            return a.baseNum < b.baseNum;
    }
    public static bool operator ==(BNum a, BNum b)
    {
        return a.baseNum == b.baseNum && a.indexNum == b.indexNum;
    }
    public static bool operator !=(BNum a, BNum b)
    {
        return a.baseNum != b.baseNum || a.indexNum != b.indexNum;
    }
    public static implicit operator BNum(float d)
    {
        return new BNum(d);
    }
    public static implicit operator BNum(int d)
    {
        return new BNum(d);
    }
    public static implicit operator string(BNum d)
    {
        return d.ToString();
    }

    public static BNum FromStr(string str)
    {
        // 正则表达式匹配两种格式
        Regex longNumPattern = new Regex(@"^(\d+)$");
        Regex shortNumPattern = new Regex(@"^(\d+(\.\d+)?)\s*([KMBT]?)$");

        Match longMatch = longNumPattern.Match(str);
        if (longMatch.Success)
        {
            string numberStr = longMatch.Groups[1].Value;
            int length = numberStr.Length;
            float baseNum = float.Parse(numberStr.Substring(0, Math.Min(10, length)));
            int indexNum = (length - 1) / 3;
            if (length > 10)
            {
                baseNum += float.Parse("0." + numberStr.Substring(10, Math.Min(3, length - 10)));
            }
            return new BNum(baseNum, indexNum);
        }

        Match shortMatch = shortNumPattern.Match(str);
        if (shortMatch.Success)
        {
            float baseNum = float.Parse(shortMatch.Groups[1].Value);
            string unit = shortMatch.Groups[3].Value;
            int indexNum = Array.IndexOf(new string[] { "", "K", "M", "B", "T" }, unit);
            if (indexNum == -1)
            {
                indexNum = 0;
            }
            return new BNum(baseNum, indexNum);
        }

        // 如果都不匹配，返回默认错误值
        return new BNum(-1, -1);
    }
}