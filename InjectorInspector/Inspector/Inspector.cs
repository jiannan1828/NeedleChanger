using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inspector
{
    public partial class Inspector : UserControl
    {
        public Inspector()
        {
            InitializeComponent();
        }

        public enum CCDName : int { ALL, 入料, 震動盤, 吸嘴, Socket, CCD5, CCD6 }

        public struct Vector3
        {
            public double X;
            public double Y;
            public double θ;
        }

        /// <summary>初始化，執行一次</summary>
        public void xInit()
        {

        }
        /// <summary>設定持續取像</summary>
        public void xFreeRun(params CCDName[] items)
        {

        }
        /// <summary>設定停止取像</summary>
        public void xStop(params CCDName[] items)
        {

        }
        /// <summary>分析入料盤，回覆有無料</summary>
        public bool xInsp入料()
        {
            return false;
        }
        /// <summary>分析震動盤，回覆針位置，無料時回覆 false</summary>
        public bool xInsp震動盤(out List<Vector3> target)
        {
            target = new List<Vector3>();
            return target.Count > 0;
        }
        /// <summary>分析吸嘴，傳入目前吸附位置(X / Y / θ)，輸出針位置，無料 / 重疊時回覆 false</summary>
        public bool xInsp吸嘴(Vector3 Loc, out Vector3 target)
        {
            bool success = true;
            target = new Vector3();
            return success;
        }
        /// <summary>吸嘴校正初始化，會將分析資料輸出為像素位置</summary>
        public void xCarbInit吸嘴()
        {

        }
        /// <summary>吸嘴校正，依序傳入 0度X1Y1 / 90度X1Y1 / 90度X2Y1 / 90度X2Y2</summary>
        public void xCarb吸嘴(List<Vector3> ImageLoc, List<Vector3> axisLoc)
        {

        }
        /// <summary>分析Socket盤，，傳入目前Socket軸位置(X / Y)，回覆針孔位置</summary>
        public List<Vector3> xInspSocket(Vector3 Loc)
        {
            return new List<Vector3>();
        }
        /// <summary>Socket盤校正初始化，會將分析資料輸出為像素位置</summary>
        public void xCarbInitSocket()
        {

        }
        /// <summary>吸嘴校正，依序傳入Socket分析後第一筆資料 X1Y1 / X2Y1 / X2Y2</summary>
        public void xCarbSocket(List<Vector3> ImageLoc, List<Vector3> axisLoc)
        {

        }
        /// <summary>未知作用</summary>
        public void xInspCCD5()
        {
        }
        /// <summary>未知作用</summary>
        public void xInspCCD6()
        {
        }

    }
}
