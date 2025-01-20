using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inspector
{
    public partial class 吸嘴Teach : Form
    {
        HWindowHelper helper = null;
        HObject dRegionNozzle = null;
        HObject Image;
        public HTuple model = null;
        HTuple dRow, dCol, dAngle, dScore;
        HObject xld;

        public 吸嘴Teach()
        {
            InitializeComponent();
        }

        public void ShowDialog(HObject image, HObject RegionNozzle, ref HTuple shapemodel, ref bool 特徵為針頭)
        {
            dRegionNozzle = RegionNozzle;
            HOperatorSet.ReduceDomain(image, dRegionNozzle, out Image);
            //Image = image.ReduceDomain(dRegionNozzle);
            model = shapemodel;
            DialogResult = DialogResult.No;
            ck_特徵為針頭.Checked = 特徵為針頭;
            if (ShowDialog() == DialogResult.OK)
            {
                shapemodel = model;
                特徵為針頭 = ck_特徵為針頭.Checked;
            }
        }

        private void 吸嘴Teach_Load(object sender, EventArgs e)
        {
#if (false)
            helper = new HWindowHelper(hWin) { ShowResult = ShowResult };
            helper.Image = Image;
            helper.AdjustView();
#endif
        }
        
        void ShowResult(HWindowControl Win, HObject image)
        {
            Win.HalconWindow.DispObj(image);
            if ((model != null) && (xld != null) && (dRow != null) && (dRow.Length > 0))
            {
                Win.HalconWindow.SetColor("green");
                Win.HalconWindow.DispCross(dRow, dCol, 120, 0);
                Win.HalconWindow.DispObj(xld);
            }
        }

        private void btn_分析_Click(object sender, EventArgs e)
        {
            Find();
        }

        private void btn_Accept_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        void Find()
        {
            HOperatorSet.FindShapeModel(Image,model, -0.39, 0.79, 0.7, 1, 0.5, "least_squares", 0, 0.9, out dRow, out dCol, out dAngle, out dScore);
            //model.FindShapeModel(Image, -0.39, 0.79, 0.7, 1, 0.5, "least_squares", 0, 0.9, out dRow, out dCol, out dAngle, out dScore);
            if (dRow.Length == 1)
            {
                var Hom2D = new HHomMat2D();
                HObject mContours;
                Hom2D.VectorAngleToRigid(0, 0, 0, dRow, dCol, dAngle);
                HOperatorSet.GetShapeModelContours(out mContours, model, 1);
                HOperatorSet.AffineTransContourXld(mContours, out xld, Hom2D);
                //xld = Hom2D.AffineTransContourXld(model.GetShapeModelContours(1));
                var deg = dAngle.TupleDeg();
                ShowResult(hWin, Image);
            }
        }

        private void btn_建立特徵_Click(object sender, EventArgs e)
        {
#if (false)
            helper.inShow = true;
#endif
            hWin.Focus();
            double row1, col1, row2, col2;
            hWin.HalconWindow.DrawRectangle1(out row1, out col1, out row2, out col2);
            if ((Math.Abs(row1-row2) > 10) && (Math.Abs(col1 - col2) > 10))
            {
                HObject temp;
                HOperatorSet.CropRectangle1(Image, out temp, row1, col1, row2, col2);
                HOperatorSet.CreateShapeModel(temp, "auto", -0.39, 0.79, "auto", "auto", "use_polarity", "auto", "auto", out model);
                
                //HOperatorSet.GetShapeModelOrigin(model, out row1, out col1);
                //var temp = Image.CropRectangle1(row1, col1, row2, col2);
                //model = temp.CreateShapeModel("auto", -0.39, 0.79, "auto", "auto", "use_polarity", "auto", "auto");
                //model.GetShapeModelOrigin(out row1, out col1);
                Find();
                DisposeObj(temp);
            }
            //Task.Factory.StartNew(() => 
            //{
            //    Thread.Sleep(300);
            //    helper.inShow = false;
            //});
        }

        internal void DisposeObj(params HObject[] arg)
        {
            for (int i = 0; i < arg.Length; i++)
                if (arg[i] != null)
                    arg[i].Dispose();
        }

    }
}
