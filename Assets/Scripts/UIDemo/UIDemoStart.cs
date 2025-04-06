using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
/// <summary>
/// UI Demo Start
/// 新建界面流程：
/// 1. 在Canvas/panelLayer下新建一个空物体，假设命名为APanel，设置适配模式为全适配（即点击RectTransform中左边的框，打开Anchor Presets面板后，按住Alt键点击右下角的图标选项）
/// 2. 在APanel上挂载MVUI脚本
/// 3. 布局UI（添加各种UI控件，比如Image，Text等等）
/// 4. 新建或共用数据模型（继承VMInventory）,demo中用的是UIDemoModel
/// 5. 在UIDemoModel中添加各种需要绑定的数据属性，即UI控件的数据来源，主要用用VM<>和VMList<>来定义属性，记得要赋初始值
/// 6. 在各个数据驱动的UI控件上挂载对应的View脚本，比如MVImage，MVText，MVList，MVProgress等，并挂载MVStructOp脚本，点击MVStructOp中的new struct，选择组件，数据模型和数据字段进行绑定
/// 7. 需要保证MVStructOp组件下出现绿色字条才算ok，如果数据类型不匹配（例如你定义了VM<int>但是你需要根据这个数值渲染文本，则考虑添加VMAdapater适配器，用法详见动态图的实现
/// 8. 一切布局完毕后，回到APanel对象，在MVUI脚本中点击Scan按钮，扫描所有控件，生成ScriptableObject文件即可完成数据绑定
/// 9. 完成后，当数据模型中的数据字段被更改时，其绑定的UI控件会自动更新，无需再维护UI控件，只需要关注数据模型的变化即可
/// 10. APanel界面搭建完成后，将其拖拽到Assets/Resources/Prefabs/UI下做出预制体
/// 11. 通过VController和预制体的名字来控制界面的打开和关闭
/// 
/// </summary>
public class UIDemoStart : MonoBehaviour
{
    int panelId = 0;
    void Start()
    {
        panelId = VController.Ins.ShowST("PanelDemo");//打开PanelDemo界面，并获取界面id
        TM.SetTimer(this.Hash("Timer"), 2, t =>
        {
            UIDemoModel.Ins.progressFloat.D = Mathf.Sin(t * 2f * Mathf.PI) * 0.5f + 0.5f;//动态改变进度条的值
        }, s =>
        {
            //改变字段的值，就可以驱动UI的变化，但注意赋值的是字段中的D属性，例如UIDemoModel.Ins.dynamicId.D = 1;而不是UIDemoModel.Ins.dynamicId = 1;
            UIDemoModel.Ins.dynamicId.D = UIDemoModel.Ins.backpacks.list.GetRandom();
            UIDemoModel.Ins.dynamicText.D = new List<string>(){
                "原神，启动!",
                "星穹铁道！启动！",
                "绝区零，启动！"
            }.GetRandom();
        }, -1);
    }
    void Update()
    {
        TM.OnUpdate();
        CT.OnUpdate();
        if (Input.GetKeyDown(KeyCode.Escape))//按下esc键关闭界面
        {
            VController.Ins.HideST(panelId);
            // VController.Ins.HideST("PanelDemo"); //也可以直接通过预制体名关闭界面
        }
        VPanel panel = VController.Ins.GetSTUI(panelId);//获取界面对象
        MVUI ui = panel.UI;//获取MVUI对象
        if (Input.GetKeyDown(KeyCode.Space))//按下空格键，发送消息
        {
            ui.SendVMsg("SendMessage");//发送消息
        }
    }

}
