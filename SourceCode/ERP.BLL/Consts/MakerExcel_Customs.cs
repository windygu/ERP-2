using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.InspectionCustoms;
using ERP.Models.Purchase;
using ERP.Tools;
using ERP.Tools.EnumHelper;
using ERP.Tools.Logs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ERP.BLL.Consts
{
    public class MakerExcel_Customs
    {
        /// <summary>
        /// 生成报关单
        /// </summary>
        /// <param name="shippingMarkEnum"></param>
        /// <param name="vm_product"></param>
        /// <returns></returns>
        public static VMAjaxProcessResult CreateCustoms(VMInspectionCustoms vm, string SelectCustomer, List<string> list_PdfFile)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();

            try
            {
                result = Bind_S288(vm, list_PdfFile);

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Msg = ex.Message;

                LogHelper.WriteError(ex);
            }
            return result;
        }

        private static VMAjaxProcessResult Bind_S288(VMInspectionCustoms vm, List<string> list_PdfFile)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            result.IsSuccess = true;

            string templatePath = Utils.GetMapPath("/data/Documents/报关单模板.jpg");//报关单模板
            Bitmap bmp = new Bitmap(templatePath);
            Graphics g = Graphics.FromImage(bmp);
            Font font = new Font("Times New Roman", 12, FontStyle.Regular);//字体
            SolidBrush sb = new SolidBrush(Color.Black);

            if (!File.Exists(templatePath))
            {
                throw new Exception("报关单模板文件不存在！" + templatePath);
            }

            decimal? SumOuterWeightGross = 0;
            decimal? SumOuterWeightNet = 0;
            int BoxQty = 0;

            foreach (var item in vm.list_ShipmentOrderProduct)
            {
                SumOuterWeightGross += item.SumOuterWeightGross;
                SumOuterWeightNet += item.SumOuterWeightNet;
                BoxQty += item.SelectBoxQty ?? 0;
            }

            g.DrawString(vm.PortName, font, sb, 91, 171);
            g.DrawString(vm.TransportTypeName, font, sb, 332, 202);
            g.DrawString("G/T", font, sb, 332, 235);
            g.DrawString(vm.ExchangeTypeName, font, sb, 655, 235);
            g.DrawString("USA", font, sb, 326, 267);
            g.DrawString(vm.DestinationPortName, font, sb, 486, 267);
            g.DrawString(vm.SourceAreaWithin, font, sb, 646, 267);
            g.DrawString(vm.TradeTypeName, font, sb, 243, 300);
            g.DrawString(BoxQty.ToString(), font, sb, 243, 333);
            g.DrawString("CARTON", font, sb, 369, 333);
            g.DrawString(SumOuterWeightGross.ToString(), font, sb, 494, 333);
            g.DrawString(SumOuterWeightNet.ToString(), font, sb, 637, 333);

            int i = 0;
            int a = 48;
            foreach (var item in vm.list_OrdersProducts)
            {
                g.DrawString(item.HsCode, font, sb, 31, 466 + i * a);
                g.DrawString(item.HsEngName, font, sb, 147, 466 + i * a);
                g.DrawString(item.Qty.ToString() + "PCS", font, sb, 330, 466 + i * a);
                g.DrawString("USA", font, sb, 434, 466 + i * a);
                g.DrawString(item.ProductPrice.ToString() + "/PC", font, sb, 524, 466 + i * a);
                g.DrawString(item.Amount.ToString(), font, sb, 604, 466 + i * a);
                g.DrawString("USD", font, sb, 691, 466 + i * a);

                g.DrawString("(" + item.HsName + ")", font, sb, 147, 484 + i * a);
                g.DrawString(item.SumOuterWeightGross.ToString() + "kgs", font, sb, 330, 484 + i * a);
                i++;
            }

            g.DrawString("SHANGHAI Jet CRAFTS, INC.", font, sb, 90, 945);
            g.DrawLine(new Pen(sb), 26, 960, 760, 960);
            g.DrawString("BUILDING 22, SHANGHAI HEADQUARTERS BAY, NO. 2500, XIUPU ROAD,", font, sb, 90, 966);
            g.DrawString("PUDONG, SHANGHAI, CHINA", font, sb, 90, 981);
            g.DrawString("税务登记号：310115748082157", font, sb, 90, 996);

            g.Save();

            string img1 = CreateShippingMark(bmp);

            ConstsMethod.GetUploadToPdfList(list_PdfFile, img1);

            return result;
        }

        /// <summary>
        /// 生成ShippingMark文件
        /// </summary>
        /// <param name="bmp"></param>
        public static string CreateShippingMark(Bitmap bmp)
        {
            string dirPath = "/data/Template/Out/Temp";
            string filePath = dirPath + "/" + Guid.NewGuid() + ".jpg";
            string filePath2 = Utils.GetMapPath(filePath);
            if (!Directory.Exists(Utils.GetMapPath(dirPath)))
            {
                Directory.CreateDirectory(Utils.GetMapPath(dirPath));
            }
            bmp.Save(filePath2, ImageFormat.Jpeg);
            return filePath;
        }

    }
}