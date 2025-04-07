using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
    临接收益 = 101,
    临接销毁 = 103,
    临接销毁成长 = 104,
    临接销毁生成 = 105,
    周期收益 = 201,
    周期提升 = 202,
    周期生成 = 203,
    定时销毁 = 204,
    销毁收益 = 301,
    销毁生成 = 302,
    范围随机 = 401,
    全场重复增益 = 901,
}

public enum CardRarity
{
    N,
    R,
    SR,
}