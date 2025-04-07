using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using TMPro;

public class CardController : MonoBehaviour
{
    public CardData cardData;
    public int nowScore = 0;
    private CardPhysicsController physicsController;

    private void Awake()
    {
        physicsController = GetComponent<CardPhysicsController>();
    }

    // 计算当前卡牌的分数
    public int CalculateScore()
    {
       

        

       
        // 根据卡牌类型计算基础分数
        switch (cardData.cardType)
        {
            // 临接类卡牌 (1xx)
            case (CardType)101: // 临接收益
                nowScore  = CalculateAdjacentScore();
                return nowScore;
                break;
            case (CardType)103: // 临接销毁
                return nowScore;
                break;
            case (CardType)104: // 临接销毁成长
                return nowScore;
                break;
            case (CardType)105: // 临接销毁生成
                return nowScore;
                break;

            case (CardType)201: // 周期收益
                return nowScore;
                break;
            case (CardType)202: // 周期提升
                return nowScore;
                break;
            case (CardType)203: // 周期生成
                return nowScore;
                break;
            case (CardType)204: // 定时销毁
                return nowScore;
                break;

            // 销毁类卡牌 (3xx)
            case (CardType)301: // 销毁收益
                return nowScore;
                break;
            case (CardType)302: // 销毁生成
                return nowScore;
                break;

            // 范围类卡牌 (4xx)
            case (CardType)401: // 范围随机
                return nowScore;
                break;

            // 全场类卡牌 (9xx)
            case (CardType)901: // 全场重复增益
                return nowScore;
                break;

            default:
                return nowScore;
                break;
        }

        // 应用状态倍率
    }

    // 计算临接收益的分数
    public int CalculateAdjacentScore()
    {
        nowScore = 0;

        switch ((int)cardData.cardType) // 转换为int更直观
        {
            case 101: // ID匹配型卡牌
                      // 基础分累加
                nowScore += cardData.score;

                // 获取相邻卡牌（已添加空安全校验）
                if (physicsController == null) return nowScore;

                var adjacentCards101 = physicsController.GetAdjacentCards();
                if (adjacentCards101 == null) return nowScore;
                // 遍历相邻卡牌
                foreach (var card in adjacentCards101)
                {
                    var controller = card?.GetComponent<CardController>();
                    if (controller == null || controller.cardData == null) continue;
                    // 类型匹配时应用效果（带类型安全检查）
                    if ((int)controller.cardData.cardType == cardData.效果[1])
                    {
                        nowScore += cardData.效果[0];
                    }
                }
            break; 
            case 103: // 临接销毁
                // 基础分累加
                nowScore += cardData.score;

                // 获取相邻卡牌（已添加空安全校验）
                if (physicsController == null) return nowScore;

                var adjacentCards103 = physicsController.GetAdjacentCards();
                if (adjacentCards103 == null) return nowScore;
                
                // 遍历相邻卡牌
                foreach (var card in adjacentCards103)
                {
                    var controller = card?.GetComponent<CardController>();
                    if (controller == null || controller.cardData == null) continue;
                    
                    // 类型匹配时应用效果（带类型安全检查）
                    if ((int)controller.cardData.cardType == cardData.效果[1])
                    {
                        nowScore += cardData.效果[0];
                        // 从CardInventory中移除匹配的卡牌
                        //CardInventory.cards.Remove(card);这里有问题，我找不到管理player cards引用的地方
                        // 销毁卡牌对象
                        Destroy(card.gameObject);
                    }
                }
                break;
            case 104: // 临接销毁成长
                // 基础分累加
                nowScore += cardData.score;

                // 获取相邻卡牌（已添加空安全校验）
                if (physicsController == null) return nowScore;

                var adjacentCards104 = physicsController.GetAdjacentCards();
                if (adjacentCards104 == null) return nowScore;
                
                // 遍历相邻卡牌
                foreach (var card in adjacentCards104)
                {
                    var controller = card?.GetComponent<CardController>();
                    if (controller == null || controller.cardData == null) continue;
                    
                    // 类型匹配时应用效果（带类型安全检查）
                    if ((int)controller.cardData.cardType == cardData.效果[1])
                    {
                        // 增加分数（基础分 + 成长值）
                        nowScore += cardData.效果[0] + controller.cardData.score;
                        // 销毁卡牌对象
                        Destroy(card.gameObject);
                    }
                }
                break;
            case 105: // 临接销毁生成
                // 基础分累加
                nowScore += cardData.score;

                // 获取相邻卡牌（已添加空安全校验）
                if (physicsController == null) return nowScore;

                var adjacentCards105 = physicsController.GetAdjacentCards();
                if (adjacentCards105 == null) return nowScore;
                
                // 遍历相邻卡牌
                foreach (var card in adjacentCards105)
                {
                    var controller = card?.GetComponent<CardController>();
                    if (controller == null || controller.cardData == null) continue;
                    
                    // 类型匹配时应用效果（带类型安全检查）
                    if ((int)controller.cardData.cardType == cardData.效果[1])
                    {
                        // 增加基础分数
                        nowScore += cardData.效果[0];
                        
                        // 生成新卡牌
                        if (cardData.效果.Count >= 3)
                        {
                            // 在卡牌位置生成新卡牌
                            Vector3 spawnPosition = card.transform.position;
                            Quaternion spawnRotation = card.transform.rotation;
                            
                            // 销毁原卡牌
                            Destroy(card.gameObject);
                            
                            // 生成新卡牌（这里需要实现生成新卡牌的逻辑）
                            // TODO: 实现生成新卡牌的具体逻辑
                            // 可能需要调用Table或其他管理类的方法来生成新卡牌
                        }
                    }
                }
                break;
        }
        return nowScore;
        // 计算特殊卡牌的分数

    }
    //正常启动效果
    public void OnNormalEffect()
    {
       
    }
    //被覆盖效果
    public void OnCoveredEffect()
    {
       
    }
    //激活联动效果
    public void OnActiveEffect()
    {
        
    }
    
}
