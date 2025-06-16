using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace NzFaceLessManMod
{

    public class XenoSelectDialog : Window
    {

        public static Window CreateNew(string message, Action confirmAction)
        {
            return new XenoSelectDialog(message, confirmAction);
        }

        private string message;
        private Action confirmAction;

        public XenoSelectDialog(string message, Action confirmAction)
        {
            this.message = message;
            this.confirmAction = confirmAction;

            // 设置窗口属性
            this.forcePause = true;
            this.doCloseButton = false;
            this.doCloseX = true;
            this.closeOnClickedOutside = false;
            this.absorbInputAroundWindow = true;
        }

        public override void DoWindowContents(Rect inRect)
        {
            // 绘制消息文本
            Widgets.Label(new Rect(0, 0, inRect.width, 100), message);

            // 绘制确认按钮
            if (Widgets.ButtonText(new Rect(inRect.width / 2 - 50, inRect.height - 35, 100, 30), "Confirm"))
            {
                confirmAction?.Invoke();
                Close();
            }
        }
    }
}