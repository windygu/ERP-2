using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.Purchase;
using ERP.Models.ShippingMark;
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
    /// <summary>
    /// 生成唛头。所有客户
    /// </summary>
    public class MakerExcel_ShippingMark
    {
        public static VMAjaxProcessResult CreateShippingMark(ShippingMarkEnum shippingMarkEnum, VMPurchaseProduct vm_product)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();

            try
            {
                switch (shippingMarkEnum)
                {
                    case ShippingMarkEnum.S188:
                        result = Bind_S188(vm_product);
                        break;

                    case ShippingMarkEnum.S60:
                        result = Bind_S60(vm_product);
                        break;

                    case ShippingMarkEnum.S05:
                        result = Bind_S05(vm_product);
                        break;

                    case ShippingMarkEnum.S13:
                        result = Bind_S13(vm_product);
                        break;

                    case ShippingMarkEnum.DG:
                        result = Bind_DG(vm_product);
                        break;

                    case ShippingMarkEnum.S235:
                        result = Bind_S235(vm_product);
                        break;

                    case ShippingMarkEnum.S220:
                        result = Bind_S220(vm_product);
                        break;

                    case ShippingMarkEnum.F20:
                        result = Bind_F20(vm_product);
                        break;

                    case ShippingMarkEnum.S135:
                        result = Bind_S135(vm_product);
                        break;

                    case ShippingMarkEnum.S164:
                        result = Bind_S164(vm_product);
                        break;

                    case ShippingMarkEnum.S10:
                        result = Bind_S10(vm_product);
                        break;

                    case ShippingMarkEnum.S56:
                        result = Bind_S56(vm_product);
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Msg = ex.Message;

                LogHelper.WriteError(ex);
            }
            return result;
        }

        /// <summary>
        /// 生成ShippingMark文件
        /// </summary>
        /// <param name="bmp"></param>
        public static string CreateShippingMark(Bitmap bmp, ShippingMarkEnum shippingMarkEnum)
        {
            string dirPath = "/data/ShippingMark/out/" + shippingMarkEnum.ToString();
            string filePath = dirPath + "/" + Guid.NewGuid() + ".jpg";
            string filePath2 = Utils.GetMapPath(filePath);
            if (!Directory.Exists(Utils.GetMapPath(dirPath)))
            {
                Directory.CreateDirectory(Utils.GetMapPath(dirPath));
            }
            bmp.Save(filePath2, ImageFormat.Jpeg);
            return filePath;
        }

        /// <summary>
        /// S188唛头填写内容
        /// </summary>
        /// <param name="g"></param>
        /// <param name="sb"></param>
        /// <param name="shippingMark_S188_Image">色带</param>
        private static void S188_Draw(Graphics g, SolidBrush sb_ribbon, ShippingMark_S188_Image shippingMark_S188_Image, ShippingMark_S188_Icon shippingMark_S188_Icon, ShippingMark_S188_SpaceCode shippingMark_S188_SpaceCode)
        {
            SolidBrush sb = new SolidBrush(Color.Black);

            //色带
            Image image1 = Image.FromFile(Utils.GetMapPath("~/data/ShippingMark/S188/ribbon/" + shippingMark_S188_Image.ToString() + ".png"));
            g.DrawImage(image1, 83, 140, 790, 145);

            //图标
            Image image2 = Image.FromFile(Utils.GetMapPath("~/data/ShippingMark/S188/icon/" + shippingMark_S188_Icon.ToString() + ".png"));
            g.DrawImage(image2, 650, 290, 150, 150);

            //SPACE CODE
            g.DrawString(shippingMark_S188_SpaceCode.ToString(), new Font("Helvetica Neue", 26, FontStyle.Bold), sb, 190, 335);
        }

        private static VMAjaxProcessResult Bind_S188(VMPurchaseProduct vm_product)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            result.IsSuccess = true;

            List<VMShippingMark> list_ShippingMark = new List<VMShippingMark>();//箱唛图片列表

            string templatePath = Utils.GetMapPath("/data/ShippingMark/Template/" + ShippingMarkEnum.S188.ToString() + ".jpg");
            Bitmap bmp = new Bitmap(templatePath);
            Graphics g = Graphics.FromImage(bmp);

            SolidBrush sb = new SolidBrush(Color.Black);

            Font font = new Font("Helvetica Neue", 22, FontStyle.Regular);
            if (!string.IsNullOrEmpty(vm_product.SkuNumber) && vm_product.SkuNumber.Length >= 6)
            {
                string SkuNumber_Start = vm_product.SkuNumber.Substring(0, 4);
                string SkuNumber_End = vm_product.SkuNumber.Substring(4, 2);

                g.DrawString(SkuNumber_Start, new Font("Helvetica Neue", 30, FontStyle.Bold), sb, 370, 338);
                g.DrawString(SkuNumber_End, new Font("Helvetica Neue", 30, FontStyle.Bold), sb, 445, 338);
            }

            g.DrawString(vm_product.No, new Font("Helvetica Neue", 24, FontStyle.Bold), sb, 380, 25);
            g.DrawString(vm_product.OuterBoxRate + " PCS", font, sb, 200, 495);
            g.DrawString(vm_product.WeightLBS + " LBS", font, sb, 160, 538);
            g.DrawString(vm_product.OuterLengthIN + " X " + vm_product.OuterWidthIN + " X " + vm_product.OuterHeightIN + " INCH", font, sb, 150, 580);

            ShippingMark_S188_SpaceCode shippingMark_S188_SpaceCode = ShippingMark_S188_SpaceCode.RR;

            if (!string.IsNullOrEmpty(vm_product.SeasonName))
            {
                var SeasonNameEnum = EnumHelper.GetCustomEnumByDesc(typeof(ShippingMark_S188_Icon), vm_product.SeasonName);
                if (SeasonNameEnum == null)
                {
                    result.IsSuccess = false;
                    result.Msg = "不存在季节：" + vm_product.SeasonName;
                    return result;
                }
                ShippingMark_S188_Icon shippingMark_S188_Icon = (ShippingMark_S188_Icon)SeasonNameEnum;

                SolidBrush sb_ribbon = new SolidBrush(Color.White);

                SolidBrush sb_year = new SolidBrush(Color.FromArgb(230, 0, 62));

                bool isPDQPackRate = false;
                int temp1 = 200;
                int temp2 = 215;
                int temp3 = 190;
                int temp4 = 230;
                if (vm_product.PDQPackRate.HasValue && vm_product.PDQPackRate.Value > 0)
                {
                    isPDQPackRate = true;
                    temp1 = 190;
                    temp2 = 200;
                    temp3 = 180;
                }

                switch (shippingMark_S188_Icon)
                {
                    case ShippingMark_S188_Icon.Valentines_Candles_Gifts_Novelties:
                        shippingMark_S188_SpaceCode = ShippingMark_S188_SpaceCode.RR;
                        sb_year = new SolidBrush(Color.FromArgb(230, 0, 62));
                        S188_Draw(g, sb_ribbon, ShippingMark_S188_Image.Valentines, shippingMark_S188_Icon, shippingMark_S188_SpaceCode);

                        g.DrawString("Valentines Candles/Gifts", new Font("Helvetica Neue", 20, FontStyle.Bold), sb_ribbon, 300, temp3);//色带上的分类品名
                        g.DrawString("Novelties", new Font("Helvetica Neue", 20, FontStyle.Bold), sb_ribbon, 370, temp2);

                        break;

                    case ShippingMark_S188_Icon.Easter_Baskets_Pails:
                        shippingMark_S188_SpaceCode = ShippingMark_S188_SpaceCode.Q;
                        sb_year = new SolidBrush(Color.FromArgb(195, 181, 216));
                        S188_Draw(g, sb_ribbon, ShippingMark_S188_Image.Easter, shippingMark_S188_Icon, shippingMark_S188_SpaceCode);

                        g.DrawString("Easter Baskets Pails", new Font("Helvetica Neue", 20, FontStyle.Bold), sb_ribbon, 320, temp1);//色带上的分类品名
                        break;

                    case ShippingMark_S188_Icon.Easter_Décor:
                        shippingMark_S188_SpaceCode = ShippingMark_S188_SpaceCode.E;
                        sb_year = new SolidBrush(Color.FromArgb(195, 181, 216));
                        S188_Draw(g, sb_ribbon, ShippingMark_S188_Image.Easter, shippingMark_S188_Icon, shippingMark_S188_SpaceCode);

                        g.DrawString("Easter Décor", new Font("Helvetica Neue", 20, FontStyle.Bold), sb_ribbon, 350, temp1);//色带上的分类品名
                        temp4 = 220;
                        break;

                    case ShippingMark_S188_Icon.Harvest:
                        shippingMark_S188_SpaceCode = ShippingMark_S188_SpaceCode.RR;
                        sb_year = new SolidBrush(Color.FromArgb(232, 106, 47));
                        S188_Draw(g, sb_ribbon, ShippingMark_S188_Image.Halloween, shippingMark_S188_Icon, shippingMark_S188_SpaceCode);

                        g.DrawString("Harvest", new Font("Helvetica Neue", 20, FontStyle.Bold), sb_ribbon, 370, temp1);//色带上的分类品名
                        break;

                    case ShippingMark_S188_Icon.Halloween_Candles_Gifts:
                        shippingMark_S188_SpaceCode = ShippingMark_S188_SpaceCode.E;
                        sb_year = new SolidBrush(Color.FromArgb(232, 106, 47));
                        S188_Draw(g, sb_ribbon, ShippingMark_S188_Image.Halloween, shippingMark_S188_Icon, shippingMark_S188_SpaceCode);

                        g.DrawString("Halloween Candles/Gifts", new Font("Helvetica Neue", 20, FontStyle.Bold), sb_ribbon, 300, temp1);//色带上的分类品名
                        temp4 = 220;

                        break;

                    case ShippingMark_S188_Icon.Halloween_Costume_Accessories:
                        shippingMark_S188_SpaceCode = ShippingMark_S188_SpaceCode.U;
                        sb_year = new SolidBrush(Color.FromArgb(232, 106, 47));
                        S188_Draw(g, sb_ribbon, ShippingMark_S188_Image.Halloween, shippingMark_S188_Icon, shippingMark_S188_SpaceCode);

                        g.DrawString("Halloween Costume", new Font("Helvetica Neue", 20, FontStyle.Bold), sb_ribbon, 310, temp3);//色带上的分类品名
                        g.DrawString("Accessories", new Font("Helvetica Neue", 20, FontStyle.Bold), sb_ribbon, 350, temp2);
                        break;

                    case ShippingMark_S188_Icon.Halloween_Home_Décor:
                        shippingMark_S188_SpaceCode = ShippingMark_S188_SpaceCode.E;
                        sb_year = new SolidBrush(Color.FromArgb(232, 106, 47));
                        S188_Draw(g, sb_ribbon, ShippingMark_S188_Image.Halloween, shippingMark_S188_Icon, shippingMark_S188_SpaceCode);

                        g.DrawString("Halloween Home Décor", new Font("Helvetica Neue", 20, FontStyle.Bold), sb_ribbon, 310, temp1);//色带上的分类品名
                        temp4 = 220;

                        break;

                    case ShippingMark_S188_Icon.Easter_Novelties:
                        shippingMark_S188_SpaceCode = ShippingMark_S188_SpaceCode.RR;
                        sb_year = new SolidBrush(Color.FromArgb(232, 106, 47));
                        S188_Draw(g, sb_ribbon, ShippingMark_S188_Image.Halloween, shippingMark_S188_Icon, shippingMark_S188_SpaceCode);

                        g.DrawString("Easter Novelties", new Font("Helvetica Neue", 20, FontStyle.Bold), sb_ribbon, 340, temp1);//色带上的分类品名
                        temp4 = 220;
                        break;

                    case ShippingMark_S188_Icon.Halloween_Novelties:
                        shippingMark_S188_SpaceCode = ShippingMark_S188_SpaceCode.RR;
                        sb_year = new SolidBrush(Color.FromArgb(232, 106, 47));
                        S188_Draw(g, sb_ribbon, ShippingMark_S188_Image.Halloween, shippingMark_S188_Icon, shippingMark_S188_SpaceCode);

                        g.DrawString("Halloween Novelties", new Font("Helvetica Neue", 20, FontStyle.Bold), sb_ribbon, 320, temp1);//色带上的分类品名
                        break;

                    case ShippingMark_S188_Icon.Christmas_Ornaments_Garland:
                        shippingMark_S188_SpaceCode = ShippingMark_S188_SpaceCode.C;
                        sb_year = new SolidBrush(Color.FromArgb(0, 148, 116));
                        S188_Draw(g, sb_ribbon, ShippingMark_S188_Image.Christmas, shippingMark_S188_Icon, shippingMark_S188_SpaceCode);

                        g.DrawString("Christmas Ornaments &", new Font("Helvetica Neue", 20, FontStyle.Bold), sb_ribbon, 296, temp3);//色带上的分类品名
                        g.DrawString("Garland", new Font("Helvetica Neue", 20, FontStyle.Bold), sb_ribbon, 380, temp2);
                        break;

                    case ShippingMark_S188_Icon.Christmas_Décor_Candles_Gifts:
                        shippingMark_S188_SpaceCode = ShippingMark_S188_SpaceCode.E;
                        sb_year = new SolidBrush(Color.FromArgb(0, 148, 116));
                        S188_Draw(g, sb_ribbon, ShippingMark_S188_Image.Christmas, shippingMark_S188_Icon, shippingMark_S188_SpaceCode);

                        g.DrawString("Christmas Décor/Candles", new Font("Helvetica Neue", 20, FontStyle.Bold), sb_ribbon, 296, temp3);//色带上的分类品名
                        g.DrawString("/Gifts", new Font("Helvetica Neue", 20, FontStyle.Bold), sb_ribbon, 400, temp2);

                        break;

                    case ShippingMark_S188_Icon.Christmas_Housewares:
                        shippingMark_S188_SpaceCode = ShippingMark_S188_SpaceCode.D;
                        sb_year = new SolidBrush(Color.FromArgb(0, 148, 116));
                        S188_Draw(g, sb_ribbon, ShippingMark_S188_Image.Christmas, shippingMark_S188_Icon, shippingMark_S188_SpaceCode);

                        g.DrawString("Christmas Housewares", new Font("Helvetica Neue", 20, FontStyle.Bold), sb_ribbon, 310, temp1);//色带上的分类品名
                        break;

                    case ShippingMark_S188_Icon.Christmas_Santa_Hats_Stockings:
                        shippingMark_S188_SpaceCode = ShippingMark_S188_SpaceCode.F;
                        sb_year = new SolidBrush(Color.FromArgb(0, 148, 116));
                        S188_Draw(g, sb_ribbon, ShippingMark_S188_Image.Christmas, shippingMark_S188_Icon, shippingMark_S188_SpaceCode);

                        g.DrawString("Christmas Santa Hats &", new Font("Helvetica Neue", 20, FontStyle.Bold), sb_ribbon, 300, temp3);//色带上的分类品名
                        g.DrawString("Stockings", new Font("Helvetica Neue", 20, FontStyle.Bold), sb_ribbon, 380, temp2);
                        break;

                    case ShippingMark_S188_Icon.Lawn_Garden_Sundries:
                        shippingMark_S188_SpaceCode = ShippingMark_S188_SpaceCode.FF;
                        sb_year = new SolidBrush(Color.FromArgb(35, 24, 22));
                        S188_Draw(g, sb_ribbon, ShippingMark_S188_Image.Spring_Garden, shippingMark_S188_Icon, shippingMark_S188_SpaceCode);
                        temp4 = 220;

                        g.DrawString("Lawn & Garden Sundries", new Font("Helvetica Neue", 20, FontStyle.Bold), sb_ribbon, 307, temp1);//色带上的分类品名
                        break;

                    case ShippingMark_S188_Icon.Spring_Summer_Home_Décor:
                        shippingMark_S188_SpaceCode = ShippingMark_S188_SpaceCode.JJ;
                        sb_year = new SolidBrush(Color.FromArgb(35, 24, 22));
                        S188_Draw(g, sb_ribbon, ShippingMark_S188_Image.Spring_Garden, shippingMark_S188_Icon, shippingMark_S188_SpaceCode);
                        temp4 = 220;

                        g.DrawString("Spring & Summer Home", new Font("Helvetica Neue", 20, FontStyle.Bold), sb_ribbon, 310, temp3);//色带上的分类品名
                        g.DrawString("Décor", new Font("Helvetica Neue", 20, FontStyle.Bold), sb_ribbon, 390, temp2);
                        break;

                    default:
                        break;
                }
                if (isPDQPackRate)
                {
                    g.DrawString("PDQ Display Is Inside", new Font("Helvetica Neue", 18, FontStyle.Bold), sb_ribbon, 330, temp4);
                }

                //年份
                g.DrawString(DateTime.Now.Year.ToString().Substring(2, 2), new Font("Helvetica Neue", 100, FontStyle.Bold), sb_year, 140, 160);

                if (!string.IsNullOrEmpty(vm_product.CartonBarcodeLabel_Image))
                {
                    var stream = Thumbnail.GetRemoteImage(vm_product.CartonBarcodeLabel_Image);

                    //外箱条码标
                    Image image1 = Image.FromStream(stream);
                    g.DrawImage(image1, 600, 520, 220, 140);
                }
            }

            //Desc = "(" + shippingMark_S188_SpaceCode.ToString() + ")" + Desc;

            string Desc_Start = vm_product.Desc;
            if (!string.IsNullOrEmpty(vm_product.Desc) && vm_product.Desc.Length > 28)
            {
                Desc_Start = CommonCode.GetSumbstring_NotWord(vm_product.Desc, 28);
                g.DrawString(vm_product.Desc.Substring(Desc_Start.Length, vm_product.Desc.Length - Desc_Start.Length), font, sb, 205, 472);
            }

            g.DrawString(Desc_Start, font, sb, 205, 452);

            if (vm_product.IsFragile2.HasValue && vm_product.IsFragile2.Value)
            {
                //易碎标
                Image image3 = Image.FromFile(Utils.GetMapPath("~/data/ShippingMark/S188/FragileMark.jpg"));
                g.DrawImage(image3, 580, 445, 280, 62);
            }

            g.Save();

            string img = CreateShippingMark(bmp, ShippingMarkEnum.S188);
            list_ShippingMark.Add(new VMShippingMark()
            {
                ImagePath = img,
            });

            result.IsSuccess = true;
            result.Data = list_ShippingMark;
            return result;
        }

        private static VMAjaxProcessResult Bind_S60(VMPurchaseProduct vm_product)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            result.IsSuccess = true;
            List<VMShippingMark> list_ShippingMark = new List<VMShippingMark>();//箱唛图片列表

            Font font = new Font("Arial", 20, FontStyle.Regular);//字体
            SolidBrush sb = new SolidBrush(Color.Black);
            Image image1 = null;
            string Desc_Start = vm_product.Desc;

            int x = 16;//初始X坐标的值
            int y = 25;//初始Y坐标的值
            int i = 33;//Y坐标每次增加的值，行间距
            int y2 = y + 25;//字符换行后，更新后的y2的坐标的值

            #region S60唛头的介绍

            string templatePath = Utils.GetMapPath("/data/ShippingMark/S60/S60唛头的介绍.png");
            Bitmap bmp = new Bitmap(templatePath);
            Graphics g = Graphics.FromImage(bmp);

            string img = CreateShippingMark(bmp, ShippingMarkEnum.S60);
            list_ShippingMark.Add(new VMShippingMark()
            {
                ImagePath = img,
                Description = "唛头的介绍",
            });

            #endregion S60唛头的介绍

            #region 外箱正唛

            templatePath = Utils.GetMapPath("/data/ShippingMark/S60/S60外箱正侧唛模板.jpg");
            bmp = new Bitmap(templatePath);
            g = Graphics.FromImage(bmp);
            //更换背景图

            string filePath = Utils.GetMapPath("~/data/ShippingMark/S60/" + vm_product.SeasonPrefix + ".png");
            if (!File.Exists(filePath))
            {
                throw new Exception("季节色带不存在！" + vm_product.SeasonPrefix);
            }
            image1 = Image.FromFile(filePath);//图标路径

            g.DrawImageUnscaledAndClipped(image1, new Rectangle(2, 2, 343, 33));//写季节图标

            Pen p = new Pen(sb, 33);
            if (vm_product.SeasonPrefix == "Christmas")//只有圣诞的季节，才会有黑色季节标
            {
                g.DrawLine(p, 2, 51, 345, 51);//写黑色图标
            }

            sb = new SolidBrush(Color.White);
            font = new Font("Arial", 16, FontStyle.Regular);//字体
            g.DrawString(vm_product.DepartmentName, font, sb, (155 - 5 * vm_product.DepartmentName.Length), 38);//往黑色图标里写白色文字,最多只能放35个字符,每个字符10个像素

            sb = new SolidBrush(Color.Black);
            font = new Font("Arial", 14, FontStyle.Regular);//字体

            g.DrawString("BIGLOTS", font, sb, 110, 85);//写BIGLOTS固定值
            g.DrawString("STORES", font, sb, 110, 110);//写STORES固定值

            x = 5;
            y = 160;
            i = 30;
            g.DrawString("PO#: " + vm_product.POID, font, sb, x, y);//写customer po值
            g.DrawString("SKU#: " + vm_product.SkuNumber, font, sb, x, y + i);//写product sku值
            g.DrawString("DEPT#: " + vm_product.SeasonDepartmentNumber, font, sb, x, y + 2 * i);//写dept number值
            g.DrawString("Country of Origin:  China", font, sb, x, y + 3 * i);//写固定值

            g.Save();

            string img1 = CreateShippingMark(bmp, ShippingMarkEnum.S60);
            list_ShippingMark.Add(new VMShippingMark()
            {
                ImagePath = img1,
                Description = vm_product.No + " ——S60外箱正唛",
            });

            #endregion 外箱正唛

            #region 外箱侧唛

            templatePath = Utils.GetMapPath("/data/ShippingMark/S60/S60外箱正侧唛模板.jpg");
            bmp = new Bitmap(templatePath);
            g = Graphics.FromImage(bmp);
            //更换背景图

            g.DrawImageUnscaledAndClipped(image1, new Rectangle(2, 2, 343, 33));//写季节图标

            p = new Pen(sb, 33);
            if (vm_product.SeasonPrefix == "Christmas")//只有圣诞的季节，才会有黑色季节标
            {
                g.DrawLine(p, 2, 51, 345, 51);//写黑色图标
            }

            sb = new SolidBrush(Color.White);
            font = new Font("Arial", 16, FontStyle.Regular);//字体
            g.DrawString(vm_product.DepartmentName, font, sb, (155 - 5 * vm_product.DepartmentName.Length), 38);//往黑色图标里写白色文字,最多只能放35个字符,每个字符10个像素

            sb = new SolidBrush(Color.Black);
            font = new Font("Arial", 14, FontStyle.Regular);//字体

            x = 5;
            y = 90;
            i = 30;
            y2 = y + 25;
            if (!string.IsNullOrEmpty(vm_product.Desc) && vm_product.Desc.Length > 22)
            {
                Desc_Start = CommonCode.GetSumbstring_NotWord(vm_product.Desc, 22);
                i = 27;
            }

            g.DrawString("SKU#: " + vm_product.SkuNumber, font, sb, x, y + i);//写product sku值
            if (!string.IsNullOrEmpty(vm_product.Desc) && vm_product.Desc.Length > 22)
            {
                g.DrawString("Description: " + Desc_Start, font, sb, x, y + 2 * i);//写第一行item description的值
                g.DrawString(vm_product.Desc.Substring(Desc_Start.Length, vm_product.Desc.Length - Desc_Start.Length), font, sb, x + 103, y2 + 2 * i);//写第二行item description的值
                y = y2;
            }
            else
            {
                g.DrawString("Description: " + Desc_Start, font, sb, x, y + 2 * i);//写item description的值
            }

            g.DrawString("Master Pack: " + vm_product.OuterBoxRate + "  PCS", font, sb, x, y + 3 * i);//写外箱率的值
            g.DrawString("Inner Pack: " + vm_product.InnerBoxRate + "  PCS", font, sb, x, y + 4 * i);//写内核率的值
            g.DrawString("GW/NW (kgs):", font, sb, x, y + 5 * i);//写固定值

            g.Save();

            string img2 = CreateShippingMark(bmp, ShippingMarkEnum.S60);//创建jpg的文件

            list_ShippingMark.Add(new VMShippingMark()
            {
                ImagePath = img2,
                Description = vm_product.No + " ——S60外箱侧唛",
            });

            #endregion 外箱侧唛

            if (vm_product.InnerBoxRate.HasValue && vm_product.InnerBoxRate.Value > 0)
            {
                #region 内盒唛

                if (vm_product.OuterBoxRate != vm_product.InnerBoxRate)
                {
                    templatePath = Utils.GetMapPath("/data/ShippingMark/S60/S60内核唛模板.jpg");
                    bmp = new Bitmap(templatePath);
                    g = Graphics.FromImage(bmp);
                    //更换背景图

                    font = new Font("Arial", 14, FontStyle.Regular);//字体
                    g.DrawString("SKU#: " + vm_product.SkuNumber, font, sb, 5, 10);//写product sku值
                    if (!string.IsNullOrEmpty(vm_product.Desc) && vm_product.Desc.Length > 22)
                    {
                        g.DrawString("Description: " + Desc_Start, font, sb, 5, 43);//写第一行item description的值
                        g.DrawString(vm_product.Desc.Substring(Desc_Start.Length, vm_product.Desc.Length - Desc_Start.Length), font, sb, 105, 70);//写第二行item description的值
                    }
                    else
                    {
                        g.DrawString("Description: " + Desc_Start, font, sb, 5, 43);//写item description的值
                    }
                    g.Save();
                    string img3 = CreateShippingMark(bmp, ShippingMarkEnum.S60);

                    list_ShippingMark.Add(new VMShippingMark()
                    {
                        ImagePath = img3,
                        Description = vm_product.No + " ——S60内盒唛",
                    });
                }

                #endregion 内盒唛
            }

            result.IsSuccess = true;
            result.Data = list_ShippingMark;
            return result;
        }

        private static VMAjaxProcessResult Bind_S05(VMPurchaseProduct vm_product)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            result.IsSuccess = true;
            List<VMShippingMark> list_ShippingMark = new List<VMShippingMark>();//箱唛图片列表

            string templatePath = Utils.GetMapPath("/data/ShippingMark/S05/S05外箱正唛侧唛参考.jpg");
            Bitmap bmp = new Bitmap(templatePath);
            Graphics g = Graphics.FromImage(bmp);
            Font font = new Font("Arial", 20, FontStyle.Regular);//字体
            SolidBrush sb = new SolidBrush(Color.Black);
            Image image1 = null;
            Image image2 = null;
            Image image3 = null;
            string Desc_Start = vm_product.Desc;

            int x = 16;//初始X坐标的值
            int y = 25;//初始Y坐标的值
            int i = 33;//Y坐标每次增加的值，行间距
            int y2 = y + 25;//字符换行后，更新后的y2的坐标的值

            #region 外箱正侧唛

            templatePath = Utils.GetMapPath("/data/ShippingMark/S05/S05外箱正唛侧唛参考.jpg");
            bmp = new Bitmap(templatePath);
            g = Graphics.FromImage(bmp);
            //更换背景图

            x = 30;
            y = 150;
            i = 33;
            y2 = y + 25;
            if (!string.IsNullOrEmpty(vm_product.Desc) && vm_product.Desc.Length > 35)
            {
                Desc_Start = CommonCode.GetSumbstring_NotWord(vm_product.Desc, 35);
                i = 30;
            }
            if (!File.Exists(Utils.GetMapPath("~/data/ShippingMark/S05/" + vm_product.SeasonPrefix + ".jpg")))
            {
                throw new Exception("季节标不存在！" + vm_product.SeasonPrefix);
            }
            image1 = Image.FromFile(Utils.GetMapPath("~/data/ShippingMark/S05/" + vm_product.SeasonPrefix + ".jpg"));//图标路径

            g.DrawImage(image1, 509, 109, 130, 130);//写正唛图标
            g.DrawImage(image1, 720, 109, 130, 130);//写侧唛图标
            font = new Font("Arial Black", 20, FontStyle.Bold);//字体
            g.DrawString(vm_product.DestinationPortName, font, sb, 710, 240);//写港口
            //往背景图里填充大字体的内容

            font = new Font("Arial Black", 14, FontStyle.Regular);//字体
            g.DrawString("Fred's Inc", font, sb, x, y);//写固定值
            g.DrawString("UPC Code:  " + vm_product.OuterPKUPC, font, sb, x, y + i);//写外箱UPC的值
            g.DrawString("P.O.#:  " + vm_product.POID, font, sb, x, y + 2 * i);//写PO.#的值
            g.DrawString("Vender Style#:  " + vm_product.No, font, sb, x, y + 3 * i);//写JK_NO的值
            g.DrawString("Fred's Sku#:  " + vm_product.SkuNumber, font, sb, x, y + 4 * i);//写customer sku的值
            if (!string.IsNullOrEmpty(vm_product.Desc) && vm_product.Desc.Length > 35)
            {
                g.DrawString("Item Description:  " + Desc_Start, font, sb, x, y + 5 * i);//写第一行item description的值
                g.DrawString(vm_product.Desc.Substring(Desc_Start.Length, vm_product.Desc.Length - Desc_Start.Length), font, sb, x + 200, y2 + 5 * i);//写第二行item description的值
                y = y2;
            }
            else
            {
                g.DrawString("Item Description:  " + Desc_Start, font, sb, x, y + 5 * i);//写item description的值
            }

            g.DrawString("Case Qty.:  " + vm_product.OuterBoxRate.ToString() + "  PCS", font, sb, x, y + 6 * i);//写外箱率的值
            string PackQty = vm_product.OuterBoxRate.ToString();
            if (!string.IsNullOrEmpty(vm_product.InnerBoxRate.ToString()))
            {
                PackQty = vm_product.InnerBoxRate.ToString();
            }//设置Pack Qty的值
            g.DrawString("Pack Qty.:  " + PackQty + "  PCS", font, sb, x, y + 7 * i);//写Pack Qty的值
            g.DrawString("Carton#               of " + vm_product.CaseOrderQty, font, sb, x, y + 8 * i);//写固定值
            g.DrawString("Country of Origin:  China", font, sb, x, y + 9 * i);//写固定值
            //写左侧内容
            x = 710;
            y = 280;
            i = 33;
            g.DrawString("Gross Weight:  " + vm_product.OuterWeightGrossLBS.ToString() + "  lbs", font, sb, x, y);//写gross weight的值
            g.DrawString("Net Weight:  " + vm_product.OuterWeightNetLBS.ToString() + "  lbs", font, sb, x, y + i);//写net weight的值
            g.DrawString("Carton Measurements:" + vm_product.OuterLengthIN.ToString() + "X" + vm_product.OuterWidthIN.ToString() + "X" + vm_product.OuterHeightIN.ToString() + " INCH", font, sb, x, y + 2 * i);//写长宽高的值
            g.DrawString("     L   -   W   -   H", font, sb, x + 230, y + 3 * i);//写固定值
            //写右侧内容
            //往背景图里填充小字体的内容

            g.Save();

            string img1 = MakerExcel_ShippingMark.CreateShippingMark(bmp, ShippingMarkEnum.S05);
            list_ShippingMark.Add(new VMShippingMark()
            {
                ImagePath = img1,
                Description = vm_product.No + " ——S05外箱正侧唛",
            });

            #endregion 外箱正侧唛

            if (vm_product.InnerBoxRate.HasValue && vm_product.InnerBoxRate.Value > 0)
            {
                #region 内盒-正唛

                x = 16;
                y = 25;
                i = 33;
                y2 = y + 25;//字符换行后，更新后的y2的坐标的值
                Desc_Start = vm_product.Desc;
                if (!string.IsNullOrEmpty(vm_product.Desc) && vm_product.Desc.Length > 40)
                {
                    Desc_Start = CommonCode.GetSumbstring_NotWord(vm_product.Desc, 40);
                    i = 30;
                }
                font = new Font("Arial", 20, FontStyle.Regular);//字体
                templatePath = Utils.GetMapPath("/data/ShippingMark/S05/DUBLIN内盒 正唛.jpg");
                bmp = new Bitmap(templatePath);
                g = Graphics.FromImage(bmp);
                //更换背景图

                g.DrawString("Fred's Inc", font, sb, x, y);//写固定值
                g.DrawString("UPC Code:  " + vm_product.InnerPKUPC, font, sb, x, y + i);//写内核UPC的值
                g.DrawString("P.O.#:  " + vm_product.POID, font, sb, x, y + 2 * i);//写PO.#的值
                g.DrawString("Vender Style#:  " + vm_product.No, font, sb, x, y + 3 * i);//写JK_NO的值
                g.DrawString("Fred's Sku#:  " + vm_product.SkuNumber, font, sb, x, y + 4 * i);//写customer sku的值

                if (!string.IsNullOrEmpty(vm_product.Desc) && vm_product.Desc.Length > 40)
                {
                    g.DrawString("Item Description:  " + Desc_Start, font, sb, x, y + 5 * i);//写第一行item description的值
                    g.DrawString(vm_product.Desc.Substring(Desc_Start.Length, vm_product.Desc.Length - Desc_Start.Length), font, sb, x + 160, y2 + 5 * i);//写第二行item description的值
                    y = y2;
                }
                else
                {
                    g.DrawString("Item Description:  " + Desc_Start, font, sb, x, y + 5 * i);//写item description的值
                }

                g.DrawString("Pack Qty.:  " + vm_product.InnerBoxRate.ToString() + "  PCS", font, sb, x, y + 6 * i);//写内核率的值
                g.DrawString("Country of Origin:  China", font, sb, x, y + 7 * i);//写固定值
                                                                                  //往背景图里填充内容

                if (!File.Exists(Utils.GetMapPath("~/data/ShippingMark/S05/" + vm_product.SeasonPrefix + ".jpg")))
                {
                    throw new Exception("季节色带不存在！" + vm_product.SeasonPrefix);
                }
                image2 = Image.FromFile(Utils.GetMapPath("~/data/ShippingMark/S05/" + vm_product.SeasonPrefix + ".jpg"));//图标路径
                g.DrawImage(image2, 480, 25, 130, 130);//写图标

                g.Save();

                string img2 = MakerExcel_ShippingMark.CreateShippingMark(bmp, ShippingMarkEnum.S05);//创建jpg的文件
                list_ShippingMark.Add(new VMShippingMark()
                {
                    ImagePath = img2,
                    Description = vm_product.No + " ——S05内盒正唛",
                });

                #endregion 内盒-正唛

                #region 内盒-侧唛

                templatePath = Utils.GetMapPath("/data/ShippingMark/S05/DUBLIN 内盒侧唛.jpg");
                bmp = new Bitmap(templatePath);
                g = Graphics.FromImage(bmp);
                //更换背景图

                x = 20;
                y = 160;
                i = 35;
                if (!File.Exists(Utils.GetMapPath("~/data/ShippingMark/S05/" + vm_product.SeasonPrefix + ".jpg")))
                {
                    throw new Exception("季节色带不存在！" + vm_product.SeasonPrefix);
                }
                image3 = Image.FromFile(Utils.GetMapPath("~/data/ShippingMark/S05/" + vm_product.SeasonPrefix + ".jpg"));//图标路径

                g.DrawImage(image3, 50, 20, 130, 130);//写图标

                font = new Font("Arial", 20, FontStyle.Bold);//字体
                g.DrawString(vm_product.DestinationPortName, font, sb, x, y);//写港口
                g.DrawString("Gross Weight:  ", font, sb, x, y + i);//写固定值
                g.DrawString("Net Weight:  ", font, sb, x, y + 2 * i);//写固定值
                g.DrawString("Carton Measurements:  ", font, sb, x, y + 3 * i);//写固定值
                                                                               //往背景图里填充大字体的内容

                font = new Font("Arial", 18, FontStyle.Regular);//字体
                g.DrawString(vm_product.InnerGrossWeightLBS.ToString() + "  lbs", font, sb, 235, 195);//写gross weight的值
                g.DrawString(vm_product.InnerNetWeightLBS.ToString() + "  lbs", font, sb, 235, 232);//写net weight的值
                g.DrawString(vm_product.InnerLengthIN.ToString() + "L X " + vm_product.InnerWidthIN.ToString() + "W X " + vm_product.InnerHeightIN.ToString() + "H INCH", font, sb, x, 300);//写长宽高的值
                g.DrawString("     L      -     W     -     H", font, sb, x, 330);//写固定值
                                                                                  //往背景图里填充小字体的内容

                g.Save();

                string img3 = MakerExcel_ShippingMark.CreateShippingMark(bmp, ShippingMarkEnum.S05);
                list_ShippingMark.Add(new VMShippingMark()
                {
                    ImagePath = img3,
                    Description = vm_product.No + " ——S05内盒侧唛",
                });

                #endregion 内盒-侧唛
            }
            result.IsSuccess = true;
            result.Data = list_ShippingMark;
            return result;
        }

        private static VMAjaxProcessResult Bind_DG(VMPurchaseProduct vm_product)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            result.IsSuccess = true;
            List<VMShippingMark> list_ShippingMark = new List<VMShippingMark>();//箱唛图片列表

            string templatePath = Utils.GetMapPath("/data/ShippingMark/DG/色带/Easter.jpg");
            Bitmap bmp = new Bitmap(templatePath);
            Graphics g = Graphics.FromImage(bmp);
            Font font = new Font("Arial", 14, FontStyle.Bold);//字体
            Font font_1 = new Font("Arial", 14, FontStyle.Bold);//字体
            SolidBrush sb = new SolidBrush(Color.Black);
            Image image1 = null;
            Image image2 = null;
            Image image3 = null;
            string Desc_Start = vm_product.Desc;

            int x = 16;//初始X坐标的值
            int y = 25;//初始Y坐标的值
            int i = 33;//Y坐标每次增加的值，行间距
            int y2 = y + 25;//字符换行后，更新后的y2的坐标的值
            string SeasonFullName = "";

            if (!string.IsNullOrEmpty(vm_product.SeasonSuffix))
            {
                SeasonFullName = vm_product.SeasonPrefix + " - " + vm_product.SeasonSuffix;
            }
            else
            {
                SeasonFullName = vm_product.SeasonPrefix;
            }

            string SeasonTemp = vm_product.SeasonPrefix;
            if (SeasonTemp != null)
            {
                if (SeasonFullName.Contains("Christmas") || SeasonFullName.Contains("Lawn & Garden"))//圣诞和春天花园用全称 其他的用季节前缀
                {
                    SeasonTemp = SeasonFullName;
                }
            }

            #region 外箱正侧唛

            if (vm_product.SeasonPrefix != "Lawn & Garden")
            {
                templatePath = Utils.GetMapPath("/data/ShippingMark/DG/背景图.jpg");

                y = 0;
                if (vm_product.OuterHeightIN > 4)
                {
                    templatePath = Utils.GetMapPath("/data/ShippingMark/DG/背景图_H.jpg");
                    y = 130;
                }
                bmp = new Bitmap(templatePath);
                g = Graphics.FromImage(bmp);
                //更换背景图

                if (SeasonTemp != "Lawn Décor" && SeasonTemp != "Citronella")
                {
                    if (!File.Exists(Utils.GetMapPath("~/data/ShippingMark/DG/色带/" + SeasonTemp + ".jpg")))
                    {
                        throw new Exception("季节色带不存在！" + SeasonTemp);
                    }
                    image1 = Image.FromFile(Utils.GetMapPath("~/data/ShippingMark/DG/色带/" + SeasonTemp + ".jpg"));//色带路径

                    g.DrawImageUnscaledAndClipped(image1, new Rectangle(1, 40, 998, 80));//写色带
                }

                g.DrawLine(new Pen(sb, 1), 750, 0, 750, 160);//写竖线
                g.DrawString("Do not place carton on top", font, sb, 350, 15);//写固定值

                if (!string.IsNullOrEmpty(vm_product.PDQPackRate.ToString()) && vm_product.PDQPackRate > 0)
                {
                    string pdqIconPath = Utils.GetMapPath("~/data/ShippingMark/DG/有PDQ箱唛分类图标/" + SeasonTemp + ".jpg");
                    if (!File.Exists(pdqIconPath))
                    {
                        throw new Exception("有PDQ箱唛分类图标不存在！" + pdqIconPath);
                    }
                    image3 = Image.FromFile(pdqIconPath);
                }
                else
                {
                    string pdqIconPath = Utils.GetMapPath("~/data/ShippingMark/DG/无PDQ箱唛分类图标/" + SeasonTemp + ".jpg");
                    if (!File.Exists(pdqIconPath))
                    {
                        throw new Exception("无PDQ箱唛分类图标不存在！" + pdqIconPath);
                    }
                    image3 = Image.FromFile(pdqIconPath);
                }
                g.DrawImage(image3, 600, 40, 80, 80);//写正唛图标
                g.DrawImage(image3, 875, 40, 80, 80);//写侧唛图标

                if (vm_product.IsFragile2.HasValue && vm_product.IsFragile2.Value)
                {
                    image3 = Image.FromFile(Utils.GetMapPath("~/data/ShippingMark/DG/FRAGILE MARK.jpg"));
                    g.DrawImage(image3, 580, 120, 170, 40);//写正唛易碎标
                    g.DrawImage(image3, 828, 120, 170, 40);//写侧唛易碎标
                }

                Image image1_1 = Image.FromFile(Utils.GetMapPath("~/data/ShippingMark/DG/空白填充图.jpg"));
                g.DrawImage(image1_1, 100, y + 20);//写空白区域
                g.DrawLine(new Pen(sb, 1), 105, y + 63, 155, y + 40);
                g.DrawLine(new Pen(sb, 1), 155, y + 40, 205, y + 63);
                g.DrawLine(new Pen(sb, 1), 205, y + 63, 155, y + 86);
                g.DrawLine(new Pen(sb, 1), 155, y + 86, 105, y + 63);
                //画菱形
                font = new Font("Arial", 8, FontStyle.Regular);
                font_1 = new Font("Arial", 8, FontStyle.Regular);
                int a = 78;
                int DescLenth = 22;
                g.DrawString("P.O.#", font, sb, 137, y + 50);//写固定值
                g.DrawString(vm_product.POID, font, sb, (155 - 4 * vm_product.POID.Length), y + 60);//写customer po,最多只能放8个字符,每个字符8个像素

                x = 105;
                i = 14;
                if (!string.IsNullOrEmpty(vm_product.Desc) && vm_product.Desc.Length > 22)
                {
                    Desc_Start = CommonCode.GetSumbstring_NotWord(vm_product.Desc, 22);
                    i = 11;
                }
                g.DrawString("CASE PACK: " + vm_product.OuterBoxRate, font, sb, 215, y + 63);//写外箱率值
                g.DrawString("CU.FT.: " + vm_product.OuterVolume.Round(2) + "\'", font, sb, 215, y + 90);//写外箱材积值

                g.DrawString("S.K.U.#: " + vm_product.SkuNumber, font, sb, x, y + 25);//写Product sku值
                g.DrawString("F" + DateTime.Now.Year.ToString().Substring(2, 2), font, sb, x + 180, y + 25);//写年份值

                y = y + 90;
                g.DrawString("GR.WT.:  " + vm_product.OuterWeightGrossLBS + "  LBS", font, sb, x, y);//写外箱毛重值
                g.DrawString("MADE IN CHINA", font, sb, x, y + 1 * i);//写固定值\
                if (vm_product.Desc.Length > 40)
                {
                    DescLenth = 28;
                    font_1 = new Font("Arial", 7, FontStyle.Regular);
                    a = 68;
                }
                if (!string.IsNullOrEmpty(vm_product.Desc) && vm_product.Desc.Length > DescLenth)
                {
                    g.DrawString("DESCRIPTION:" + Desc_Start, font_1, sb, x, y + 2 * i);//写第一行item description的值
                    g.DrawString(vm_product.Desc.Substring(Desc_Start.Length, vm_product.Desc.Length - Desc_Start.Length), font_1, sb, x + a, y + 3 * i);//写第二行item description的值
                }
                else
                {
                    g.DrawString("DESCRIPTION:" + vm_product.Desc, font, sb, x, y + 2 * i);//写item description的值
                }
            }
            else
            {
                templatePath = Utils.GetMapPath("/data/ShippingMark/DG/春天花园背景图.jpg");
                bmp = new Bitmap(templatePath);
                g = Graphics.FromImage(bmp);
                //更换背景图

                if (!string.IsNullOrEmpty(vm_product.PDQPackRate.ToString()) && vm_product.PDQPackRate > 0)
                {
                    string pdqIconPath = Utils.GetMapPath("~/data/ShippingMark/DG/有PDQ箱唛分类图标/" + SeasonTemp + ".jpg");
                    if (!File.Exists(pdqIconPath))
                    {
                        throw new Exception("有PDQ箱唛分类图标不存在！" + pdqIconPath);
                    }
                    image3 = Image.FromFile(pdqIconPath);
                }
                else
                {
                    string pdqIconPath = Utils.GetMapPath("~/data/ShippingMark/DG/无PDQ箱唛分类图标/" + SeasonTemp + ".jpg");
                    if (!File.Exists(pdqIconPath))
                    {
                        throw new Exception("无PDQ箱唛分类图标不存在！" + pdqIconPath);
                    }
                    image3 = Image.FromFile(pdqIconPath);
                }
                g.DrawImage(image3, 390, 5, 120, 120);//写正唛图标
                g.DrawImage(image3, 790, 5, 80, 80);//写侧唛图标

                if (vm_product.IsFragile2.HasValue && vm_product.IsFragile2.Value)
                {
                    image3 = Image.FromFile(Utils.GetMapPath("~/data/ShippingMark/DG/FRAGILE MARK.jpg"));
                    g.DrawImage(image3, 75, 90, 255, 60);//写正唛易碎标
                    g.DrawImage(image3, 665, 105, 170, 40);//写侧唛易碎标
                }

                font = new Font("Arial", 14, FontStyle.Bold);
                g.DrawString("P.O.#", font, sb, 95, 190);//写固定值
                g.DrawString(vm_product.POID, font, sb, (130 - 8 * vm_product.POID.Length), 210);//写customer po,最多只能放8个字符,每个字符16个像素

                x = 40;
                y = 155;
                i = 25;
                g.DrawString("CASE PACK: " + vm_product.OuterBoxRate, font, sb, 215, 210);//写外箱率值

                g.DrawString("S.K.U.#: " + vm_product.SkuNumber + "       S" + DateTime.Now.Year.ToString().Substring(2, 2), font, sb, x, y);//写Product sku + 年份值

                y = 250;
                g.DrawString("GR.WT.:  " + vm_product.OuterWeightGrossLBS + "  LBS" + "     CU.FT.: " + vm_product.OuterVolume + "\'", font, sb, x, y);//写外箱毛重和材积的值
                g.DrawString("MADE IN CHINA", font, sb, x, y + 1 * i);//写固定值

                if (!string.IsNullOrEmpty(vm_product.Desc) && vm_product.Desc.Length > 50)
                {
                    Desc_Start = CommonCode.GetSumbstring_NotWord(vm_product.Desc, 50);
                    g.DrawString("DESCRIPTION:" + Desc_Start, font, sb, x, y + 2 * i);//写第一行item description的值
                    g.DrawString(vm_product.Desc.Substring(Desc_Start.Length, vm_product.Desc.Length - Desc_Start.Length), font, sb, x + 140, y + 3 * i);//写第二行item description的值
                }
                else
                {
                    g.DrawString("DESCRIPTION:" + vm_product.Desc, font, sb, x, y + 2 * i);//写item description的值
                }
            }

            g.Save();

            string img1 = MakerExcel_ShippingMark.CreateShippingMark(bmp, ShippingMarkEnum.DG);

            list_ShippingMark.Add(new VMShippingMark()
            {
                ImagePath = img1,
                Description = vm_product.No + " ——DG外箱正侧唛",
            });

            #endregion 外箱正侧唛

            result.IsSuccess = true;
            result.Data = list_ShippingMark;
            return result;
        }

        private static VMAjaxProcessResult Bind_S13(VMPurchaseProduct vm_product)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            result.IsSuccess = true;
            List<VMShippingMark> list_ShippingMark = new List<VMShippingMark>();//箱唛图片列表

            string templatePath = Utils.GetMapPath("/data/ShippingMark/S13/标准箱外箱正唛.jpg");//存放正唛路径
            string templatePath_1 = Utils.GetMapPath("/data/ShippingMark/S13/标准箱外箱侧唛.jpg");//存放侧唛路径
            Bitmap bmp = new Bitmap(templatePath);
            Bitmap bmp_2 = new Bitmap(templatePath);
            Graphics g = Graphics.FromImage(bmp);
            Font font = new Font("Arial", 9, FontStyle.Regular);//字体
            SolidBrush sb = new SolidBrush(Color.Black);
            string Desc_Start = vm_product.Desc;
            string img1 = "";
            string img2 = "";
            string img3 = "";

            int x = 16;//初始正唛X坐标的值
            int y = 25;//初始正唛Y坐标的值
            int i = 33;//Y坐标每次增加的值，行间距
            int x0 = 16;//初始侧唛X坐标的值
            int y0 = 25;//初始侧唛Y坐标的值
            int DescLength = 30;

            int y2 = y + 25;//字符换行后，更新后的y2的坐标的值
                            // String SeasonFullName = "";

            #region S13唛头的介绍

            string templatePath_Introduction = Utils.GetMapPath("/data/ShippingMark/S13/S13唛头的介绍.png");
            Bitmap bmp_Introduction = new Bitmap(templatePath_Introduction);

            string img_Introduction = CreateShippingMark(bmp_Introduction, ShippingMarkEnum.S13);
            list_ShippingMark.Add(new VMShippingMark()
            {
                ImagePath = img_Introduction,
                Description = "唛头的介绍",
            });

            #endregion S13唛头的介绍

            #region 外箱正侧唛

            if (vm_product.CarbinetType == (int)PurchaseProduct_CarbinetTypeEnum.StandardContainer)//是标准箱
            {
                templatePath = Utils.GetMapPath("/data/ShippingMark/S13/标准箱外箱正唛.jpg");//获取标准箱的正唛图片
                templatePath_1 = Utils.GetMapPath("/data/ShippingMark/S13/标准箱外箱侧唛.jpg");//获取标准箱的侧唛图片
                x = 122;
                y = 164;
                i = 20;
                x0 = 31;
                y0 = 122;
                DescLength = 25;
                y2 = y0 + 16;
            }
            else if (vm_product.CarbinetType == (int)PurchaseProduct_CarbinetTypeEnum.NonStandardContainer)//是非标准箱
            {
                templatePath = Utils.GetMapPath("/data/ShippingMark/S13/非标准箱外箱正侧唛.jpg");//获取非标准箱的正唛图片
                x = 20;
                y = 188;
                i = 20;
                x0 = 194;
                y0 = 46;
                DescLength = 20;
                y2 = y0 + 16;
            }
            else //是小箱
            {
                templatePath = Utils.GetMapPath("/data/ShippingMark/S13/小箱外箱正唛.jpg");//获取非标准箱的正唛图片
                x = 200;
                y = 124;
                i = 20;
                x0 = 17;
                y0 = 205;
                DescLength = 15;
                y2 = y0 + 16;
            }

            bmp = new Bitmap(templatePath);
            g = Graphics.FromImage(bmp);
            //写正唛的值
            g.DrawString("PO #: " + vm_product.POID, font, sb, x, y);//写PO#值
            g.DrawString("SKU #: " + vm_product.SkuNumber, font, sb, x, y + i);//写sku#值
            g.DrawString("CTN #:           OF  " + vm_product.CaseOrderQty, font, sb, x, y + 2 * i);//写订单总箱数的值
            g.DrawString("MADE IN CHINA", font, sb, x, y + 3 * i);//写固定值

            if (vm_product.CarbinetType == (int)PurchaseProduct_CarbinetTypeEnum.StandardContainer)
            {
                var BarcodeImage = CommonCode.GetBarcodeImage(vm_product.UPC, 300, 150);//条形码的图片
                if (BarcodeImage != null)
                {
                    g.DrawImage(BarcodeImage, 94, 265, 150, 60);
                }
                g.DrawRectangle(new Pen(sb, 1), 94, 260, 150, 70);//条形码的边框

                g.Save();
                img1 = MakerExcel_ShippingMark.CreateShippingMark(bmp, ShippingMarkEnum.S13);
                list_ShippingMark.Add(new VMShippingMark()
                {
                    ImagePath = img1,
                    Description = vm_product.No + " ——S13标准箱 外箱正嘜 (須印前後兩面)",
                });

                bmp_2 = new Bitmap(templatePath_1);
                g = Graphics.FromImage(bmp_2);

                g.DrawString("DEPT:" + vm_product.SeasonDepartmentNumber, font, sb, x0, y0);//写固定值
                g.DrawString("PO #: " + vm_product.POID, font, sb, x0, y0 + i);//写PO#值
                g.DrawString("SKU #: " + vm_product.SkuNumber, font, sb, x0, y0 + 2 * i);//写sku#值

                if (!string.IsNullOrEmpty(vm_product.Desc) && vm_product.Desc.Length > DescLength)
                {
                    Desc_Start = CommonCode.GetSumbstring_NotWord(vm_product.Desc, DescLength);
                    g.DrawString("DESC:  " + Desc_Start, font, sb, x0, y0 + 3 * i);//写第一行item description的值
                    g.DrawString(vm_product.Desc.Substring(Desc_Start.Length, vm_product.Desc.Length - Desc_Start.Length), font, sb, x0 + 40, y2 + 3 * i);//写第二行item description的值
                    y0 = y2;
                }
                else
                {
                    g.DrawString("DESC:  " + Desc_Start, font, sb, x0, y0 + 3 * i);//写item description的值
                }

                g.DrawString("COLOR: " + vm_product.ColorName, font, sb, x0, y0 + 4 * i);//写产品颜色的值
                g.DrawString("QTY: " + vm_product.OuterBoxRate + " PCS", font, sb, x0, y0 + 5 * i);//写箱数的值
                g.DrawString("N.W.: " + vm_product.OuterWeightNet + "  KGS", font, sb, x0, y0 + 6 * i);//写外箱净重的值
                g.DrawString("G.W.: " + vm_product.OuterWeightGross + "  KGS", font, sb, x0, y0 + 7 * i);//写外箱毛重的值
                g.DrawString("MEAS: " + vm_product.OuterLengthIN + "X" + vm_product.OuterWidthIN + "X" + vm_product.OuterHeightIN + "IN.", font, sb, x0, y0 + 8 * i);//写外箱净重的值

                g.Save();
                img2 = MakerExcel_ShippingMark.CreateShippingMark(bmp_2, ShippingMarkEnum.S13);
                list_ShippingMark.Add(new VMShippingMark()
                {
                    ImagePath = img2,
                    Description = vm_product.No + " ——S13标准箱 外箱側嘜(須印左右兩面)",
                });
            }
            else if (vm_product.CarbinetType == (int)PurchaseProduct_CarbinetTypeEnum.NonStandardContainer)//写侧唛的值
            {
                g.DrawString("DEPT:" + vm_product.SeasonDepartmentNumber, font, sb, x0, y0);//写固定值
                g.DrawString("PO #: " + vm_product.POID, font, sb, x0, y0 + i);//写PO#值
                g.DrawString("SKU #: " + vm_product.SkuNumber, font, sb, x0, y0 + 2 * i);//写sku#值

                if (!string.IsNullOrEmpty(vm_product.Desc) && vm_product.Desc.Length > DescLength)
                {
                    Desc_Start = CommonCode.GetSumbstring_NotWord(vm_product.Desc, DescLength);
                    g.DrawString("DESC:  " + Desc_Start, font, sb, x0, y0 + 3 * i);//写第一行item description的值
                    g.DrawString(vm_product.Desc.Substring(Desc_Start.Length, vm_product.Desc.Length - Desc_Start.Length), font, sb, x0 + 40, y2 + 3 * i);//写第二行item description的值
                    y0 = y2;
                }
                else
                {
                    g.DrawString("DESC:  " + Desc_Start, font, sb, x0, y0 + 3 * i);//写item description的值
                }

                g.DrawString("COLOR: " + vm_product.ColorName, font, sb, x0, y0 + 4 * i);//写产品颜色的值
                g.DrawString("QTY: " + vm_product.OuterBoxRate + " PCS", font, sb, x0, y0 + 5 * i);//写箱数的值
                g.DrawString("N.W.: " + vm_product.OuterWeightNet + "  KGS", font, sb, x0, y0 + 6 * i);//写外箱净重的值
                g.DrawString("G.W.: " + vm_product.OuterWeightGross + "  KGS", font, sb, x0, y0 + 7 * i);//写外箱毛重的值
                g.DrawString("MEAS: " + vm_product.OuterLengthIN + "X" + vm_product.OuterWidthIN + "X" + vm_product.OuterHeightIN + "IN.", font, sb, x0, y0 + 8 * i);//写外箱净重的值

                var BarcodeImage = CommonCode.GetBarcodeImage(vm_product.UPC, 300, 150);//条形码的图片
                if (BarcodeImage != null)
                {
                    g.DrawImage(BarcodeImage, 210, 280, 150, 60);
                }
                g.DrawRectangle(new Pen(sb, 1), 210, 275, 150, 70);//条形码的边框

                g.Save();
                img1 = MakerExcel_ShippingMark.CreateShippingMark(bmp, ShippingMarkEnum.S13);
                list_ShippingMark.Add(new VMShippingMark()
                {
                    ImagePath = img1,
                    Description = vm_product.No + " ——S13非标准箱 外箱正嘜 (須印在箱面左邊, 上下兩面)、外箱側嘜 (須印在箱面右邊, 上下兩面)",
                });
            }
            else//写小箱侧唛的值
            {
                g.DrawString("DEPT:" + vm_product.SeasonDepartmentNumber, font, sb, x0, y0);//写固定值
                g.DrawString("PO #: " + vm_product.POID, font, sb, x0, y0 + i);//写PO#值
                g.DrawString("SKU #: " + vm_product.SkuNumber, font, sb, x0, y0 + 2 * i);//写sku#值

                if (!string.IsNullOrEmpty(vm_product.Desc) && vm_product.Desc.Length > DescLength)
                {
                    Desc_Start = CommonCode.GetSumbstring_NotWord(vm_product.Desc, DescLength);
                    g.DrawString("DESC:  " + Desc_Start, font, sb, x0, y0 + 3 * i);//写第一行item description的值
                    g.DrawString(vm_product.Desc.Substring(Desc_Start.Length, vm_product.Desc.Length - Desc_Start.Length), font, sb, x0 + 40, y2 + 3 * i);//写第二行item description的值
                    y0 = y2;
                }
                else
                {
                    g.DrawString("DESC:  " + Desc_Start, font, sb, x0, y0 + 3 * i);//写item description的值
                }

                g.DrawString("COLOR: " + vm_product.ColorName, font, sb, x0, y0 + 4 * i);//写产品颜色的值
                x0 = 200;
                y0 = 205;
                g.DrawString("QTY: " + vm_product.OuterBoxRate + " PCS", font, sb, x0, y0);//写箱数的值
                g.DrawString("N.W.: " + vm_product.OuterWeightNet + "  KGS", font, sb, x0, y0 + i);//写外箱净重的值
                g.DrawString("G.W.: " + vm_product.OuterWeightGross + "  KGS", font, sb, x0, y0 + 2 * i);//写外箱毛重的值
                g.DrawString("MEAS: " + vm_product.OuterLengthIN + "X" + vm_product.OuterWidthIN + "X" + vm_product.OuterHeightIN + "IN.", font, sb, x0, y0 + 3 * i);//写外箱净重的值

                g.Save();
                img1 = MakerExcel_ShippingMark.CreateShippingMark(bmp, ShippingMarkEnum.S13);
                list_ShippingMark.Add(new VMShippingMark()
                {
                    ImagePath = img1,
                    Description = vm_product.No + " ——S13小箱 外箱正嘜 (須印前後兩面)",
                });

                templatePath = Utils.GetMapPath("/data/ShippingMark/S13/小箱外箱侧唛.jpg");//获取小箱的外箱侧唛图片

                bmp = new Bitmap(templatePath);
                g = Graphics.FromImage(bmp);

                var BarcodeImage = CommonCode.GetBarcodeImage(vm_product.UPC, 300, 150);//条形码的图片
                if (BarcodeImage != null)
                {
                    g.DrawImage(BarcodeImage, 40, 205, 150, 60);
                }
                g.DrawRectangle(new Pen(sb, 1), 40, 200, 150, 70);//条形码的边框

                g.Save();
                img2 = MakerExcel_ShippingMark.CreateShippingMark(bmp, ShippingMarkEnum.S13);
                list_ShippingMark.Add(new VMShippingMark()
                {
                    ImagePath = img2,
                    Description = vm_product.No + " ——S13小箱 外箱側嘜(須印左右兩面)",
                });
            }

            #endregion 外箱正侧唛

            if (vm_product.InnerBoxRate.HasValue && vm_product.InnerBoxRate.Value > 0)
            {
                #region 内盒唛

                font = new Font("Arial", 12, FontStyle.Regular);//字体
                templatePath = Utils.GetMapPath("/data/ShippingMark/S13/内核正唛.jpg");//获取内盒正唛图片
                x = 14;
                y = 125;
                i = 25;
                DescLength = 15;
                y2 = y + 18;

                bmp = new Bitmap(templatePath);
                g = Graphics.FromImage(bmp);

                g.DrawString("DEPT:" + vm_product.SeasonDepartmentNumber, font, sb, x, y);//写固定值
                g.DrawString("PO #: " + vm_product.POID, font, sb, x, y + i);//写PO#值
                g.DrawString("SKU #: " + vm_product.SkuNumber, font, sb, x, y + 2 * i);//写sku#值

                if (!string.IsNullOrEmpty(vm_product.Desc) && vm_product.Desc.Length > DescLength)
                {
                    Desc_Start = CommonCode.GetSumbstring_NotWord(vm_product.Desc, DescLength);
                    g.DrawString("DESC:  " + Desc_Start, font, sb, x, y + 3 * i);//写第一行item description的值
                    g.DrawString(vm_product.Desc.Substring(Desc_Start.Length, vm_product.Desc.Length - Desc_Start.Length), font, sb, x + 55, y2 + 3 * i);//写第二行item description的值
                    y = y2;
                }
                else
                {
                    g.DrawString("DESC:  " + Desc_Start, font, sb, x, y + 3 * i);//写item description的值
                }

                g.DrawString("COLOR: " + vm_product.ColorName, font, sb, x, y + 4 * i);//写产品颜色的值
                g.DrawString("QTY: " + vm_product.InnerBoxRate + " PCS", font, sb, x, y + 5 * i);//写内核箱数的值

                g.Save();
                img3 = MakerExcel_ShippingMark.CreateShippingMark(bmp, ShippingMarkEnum.S13);
                list_ShippingMark.Add(new VMShippingMark()
                {
                    ImagePath = img3,
                    Description = vm_product.No + " ——S13內盒正嘜(須印前後兩面)",
                });

                #endregion 內盒唛
            }

            result.IsSuccess = true;
            result.Data = list_ShippingMark;
            return result;
        }

        private static VMAjaxProcessResult Bind_S235(VMPurchaseProduct vm_product)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            result.IsSuccess = true;
            List<VMShippingMark> list_ShippingMark = new List<VMShippingMark>();//箱唛图片列表

            string templatePath = Utils.GetMapPath("/data/ShippingMark/S235/Christmas唛头模板.jpg");
            Bitmap bmp = new Bitmap(templatePath);
            Graphics g = Graphics.FromImage(bmp);
            string img1 = "";
            Font font = new Font("Arial", 20, FontStyle.Bold);//正唛字体
            Font font_1 = new Font("Arial", 20, FontStyle.Bold);//正唛字体
            SolidBrush sb = new SolidBrush(Color.Black);
            string Desc_Start = vm_product.Desc;

            int x = 16;//初始X坐标的值
            int y = 25;//初始Y坐标的值
            int i = 30;//Y坐标每次增加的值，行间距
            int y2 = y + 25;//字符换行后，更新后的y2的坐标的值

            int x0 = 16;//初始侧唛X坐标的值
            int y0 = 25;//初始侧唛Y坐标的值
            int i0 = 33;//侧唛Y坐标每次增加的值，行间距
            int a = 130;//换行品名的品名对齐
            int Desc_Lenth = 25;

            #region 外箱正侧唛

            if (!string.IsNullOrEmpty(vm_product.SeasonPrefix) && vm_product.SeasonPrefix == "Christmas")
            //季节是圣诞节
            {
                templatePath = Utils.GetMapPath("/data/ShippingMark/S235/Christmas唛头模板.jpg");
                font = new Font("Arial", 16, FontStyle.Bold);//正唛字体
                font_1 = new Font("Arial", 22, FontStyle.Bold);//正唛字体
                x = 25;
                y = 220;
                i = 22;
                y2 = y + 20;
                x0 = 450;
                y0 = 220;
                i0 = 30;
                a = 130;
                Desc_Lenth = 25;
                if (!string.IsNullOrEmpty(vm_product.Desc) && vm_product.Desc.Length > Desc_Lenth)
                {
                    Desc_Start = CommonCode.GetSumbstring_NotWord(vm_product.Desc, Desc_Lenth);
                    i = 22;
                }
            }
            else if (!string.IsNullOrEmpty(vm_product.SeasonPrefix) && vm_product.SeasonPrefix == "Easter")
            //季节是复活节
            {
                templatePath = Utils.GetMapPath("/data/ShippingMark/S235/Easter唛头模板.jpg");
                font = new Font("Arial", 9, FontStyle.Bold);//正唛字体
                font_1 = new Font("Arial", 9, FontStyle.Bold);//正唛字体
                x = 35;
                y = 150;
                i = 16;
                y2 = y + 16;
                x0 = 350;
                y0 = 150;
                i0 = 20;
                a = 102;
                Desc_Lenth = 25;
                if (!string.IsNullOrEmpty(vm_product.Desc) && vm_product.Desc.Length > Desc_Lenth)
                {
                    Desc_Start = CommonCode.GetSumbstring_NotWord(vm_product.Desc, Desc_Lenth);
                    i = 16;
                }
            }
            else if (!string.IsNullOrEmpty(vm_product.SeasonPrefix) && vm_product.SeasonPrefix == "Halloween")
            //季节是鬼节
            {
                templatePath = Utils.GetMapPath("/data/ShippingMark/S235/Halloween唛头模板.jpg");
                font = new Font("Arial", 5, FontStyle.Bold);//正唛字体
                font_1 = new Font("Arial", 5, FontStyle.Bold);//正唛字体
                x = 5;
                y = 55;
                i = 28;
                y2 = y + 18;
                x0 = 570;
                y0 = 25;
                i0 = 28;
                a = 180;
                Desc_Lenth = 30;
                if (!string.IsNullOrEmpty(vm_product.Desc) && vm_product.Desc.Length > Desc_Lenth)
                {
                    Desc_Start = CommonCode.GetSumbstring_NotWord(vm_product.Desc, Desc_Lenth);
                    i = 25;
                }
            }
            else if (!string.IsNullOrEmpty(vm_product.SeasonPrefix) && vm_product.SeasonPrefix == "Spring & Summer")
            //季节是春天花园
            {
                templatePath = Utils.GetMapPath("/data/ShippingMark/S235/Spring & Summer唛头模板.jpg");
                font = new Font("Arial", 12, FontStyle.Bold);//正唛字体
                font_1 = new Font("Arial", 12, FontStyle.Bold);//正唛字体
                x = 30;
                y = 127;
                i = 20;
                y2 = y + 14;
                x0 = 442;
                y0 = 129;
                i0 = 20;
                a = 102;
                Desc_Lenth = 40;
                if (!string.IsNullOrEmpty(vm_product.Desc) && vm_product.Desc.Length > Desc_Lenth)
                {
                    Desc_Start = CommonCode.GetSumbstring_NotWord(vm_product.Desc, Desc_Lenth);
                    i = 20;
                }
            }
            else
            {
                font = new Font("Arial", 16, FontStyle.Bold);//正唛字体
                font_1 = new Font("Arial", 22, FontStyle.Bold);//正唛字体
                x = 25;
                y = 220;
                i = 22;
                y2 = y + 20;
                x0 = 450;
                y0 = 220;
                i0 = 30;
                a = 130;
                Desc_Lenth = 25;
                if (!string.IsNullOrEmpty(vm_product.Desc) && vm_product.Desc.Length > Desc_Lenth)
                {
                    Desc_Start = CommonCode.GetSumbstring_NotWord(vm_product.Desc, Desc_Lenth);
                    i = 22;
                }

            }
            bmp = new Bitmap(templatePath);
            g = Graphics.FromImage(bmp);

            //更换背景图
            g.DrawString("PO#  " + vm_product.POID, font, sb, x, y);//写po值
            g.DrawString("UPC#  " + vm_product.UPC, font, sb, x, y + i);//写UPC的值
            g.DrawString("Vender Name: Shanghai Jet Crafts Inc", font, sb, x, y + 2 * i);//写固定值
            g.DrawString("Item#  " + vm_product.No, font, sb, x, y + 3 * i);//写JK_NO的值
            if (!string.IsNullOrEmpty(vm_product.Desc) && vm_product.Desc.Length > Desc_Lenth)
            {
                g.DrawString("Item Description: " + Desc_Start, font, sb, x, y + 4 * i);//写第一行item description的值
                g.DrawString(vm_product.Desc.Substring(Desc_Start.Length, vm_product.Desc.Length - Desc_Start.Length), font, sb, x + a, y + 5 * i);//写第二行item description的值
                y = y2;
            }
            else
            {
                g.DrawString("Item Description: " + Desc_Start, font, sb, x, y + 4 * i);//写item description的值
            }

            g.DrawString("QTY: " + vm_product.OuterBoxRate.ToString() + "  PCS", font, sb, x, y + 5 * i);//写外箱率的值
            g.DrawString("Ctn No:               of   " + vm_product.CaseOrderQty, font, sb, x, y + 6 * i);//写订单箱数值
            g.DrawString("MADE IN CHINA", font, sb, x, y + 7 * i);//写固定值
            //写正唛内容

            g.DrawString("Item#  " + vm_product.No, font_1, sb, x0, y0);//写JK_NO的值
            g.DrawString("QTY: " + vm_product.OuterBoxRate.ToString() + "  PCS", font_1, sb, x0, y0 + i0);//写外箱率的值
            g.DrawString("N.W.: " + vm_product.OuterWeightNet + " KGS", font_1, sb, x0, y0 + 2 * i0);//写外箱净重的值
            g.DrawString("G.W.: " + vm_product.OuterWeightGross + " KGS", font_1, sb, x0, y0 + 3 * i0);//写外箱毛重的值
            g.DrawString("CU\'FT: " + vm_product.OuterVolume + " \'", font_1, sb, x0, y0 + 4 * i0);//写外箱材积（cuft）的值
                                                                                                   //写侧唛内容

            g.Save();

            img1 = MakerExcel_ShippingMark.CreateShippingMark(bmp, ShippingMarkEnum.S235);

            string Description = vm_product.No + " ——S235外箱正侧唛";
            if (string.IsNullOrEmpty(vm_product.SeasonPrefix))
            {
                Description += "（该产品没有季节）";
            }

            list_ShippingMark.Add(new VMShippingMark()
            {
                ImagePath = img1,
                Description = Description,
            });

            #endregion 外箱正侧唛

            result.IsSuccess = true;
            result.Data = list_ShippingMark;

            if (string.IsNullOrEmpty(vm_product.SeasonPrefix))
            {
                result.identity = "该产品没有季节！";
            }
            return result;
        }

        private static VMAjaxProcessResult Bind_S220(VMPurchaseProduct vm_product)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            result.IsSuccess = true;
            List<VMShippingMark> list_ShippingMark = new List<VMShippingMark>();//箱唛图片列表

            string templatePath = Utils.GetMapPath("/data/ShippingMark/S220/外箱唛头模板.jpg");
            Bitmap bmp = new Bitmap(templatePath);
            Graphics g = Graphics.FromImage(bmp);
            string img1 = "";
            string img2 = "";
            Font font = new Font("Arial", 3, FontStyle.Bold);//正唛字体
            Font font_1 = new Font("Arial", 3, FontStyle.Bold);//侧唛字体
            SolidBrush sb = new SolidBrush(Color.Black);
            string Desc_Start = vm_product.Desc;

            int x = 10;//初始X坐标的值
            int y = 150;//初始Y坐标的值
            int i = 20;//Y坐标每次增加的值，行间距
            int y2 = y + 15;//字符换行后，更新后的y2的坐标的值

            int x0 = 360;//初始侧唛X坐标的值
            int y0 = 180;//初始侧唛Y坐标的值
            int i0 = 18;//侧唛Y坐标每次增加的值，行间距
            int y02 = y0 + 15;
            int Desc_Lenth = 30;

            #region 外箱正侧唛

            //正唛
            g.DrawString("Season:" + vm_product.SeasonPrefix + " " + DateTime.Now.Year, font, sb, x, y);
            g.DrawString("Shoppers Drug Mart Inc/ Pharmaprix", font, sb, x, y + i);//写固定值
            if (!string.IsNullOrEmpty(vm_product.Desc) && vm_product.Desc.Length > Desc_Lenth)
            {
                Desc_Start = CommonCode.GetSumbstring_NotWord(vm_product.Desc, Desc_Lenth);
                g.DrawString("Product Description: " + Desc_Start, font, sb, x, y + 2 * i);//写第一行item description的值
                g.DrawString(vm_product.Desc.Substring(Desc_Start.Length, vm_product.Desc.Length - Desc_Start.Length), font, sb, x + 125, y + 3 * i);//写第二行item description的值
                y = y2;
            }
            else
            {
                g.DrawString("Product Description: " + Desc_Start, font, sb, x, y + 2 * i);//写item description的值
            }
            g.DrawString("UPC: " + vm_product.UPC, font, sb, x, y + 3 * i);//写产品UPC值
            g.DrawString("PO#  " + vm_product.POID, font, sb, x, y + 4 * i);//写PO值
            g.DrawString("Country of Origin: China", font, sb, x, y + 5 * i);//写固定值
            g.DrawString("Case Pack Quantity: " + vm_product.CaseOrderQty, font, sb, x, y + 6 * i);//写订单箱数值

            if (vm_product.InnerBoxRate.HasValue && vm_product.InnerBoxRate.Value > 0)
            {
                g.DrawString("Inner: " + vm_product.InnerBoxRate + " PCS/Master: " + vm_product.OuterBoxRate + " PCS", font, sb, x, y + 7 * i);//写内核率外箱率值
            }
            else
            {
                g.DrawString("Master: " + vm_product.OuterBoxRate + " PCS", font, sb, x, y + 7 * i);//写内核率外箱率值
            }

            g.DrawString("Carton Dimension in CM:", font, sb, x, y + 8 * i);//写固定值
            g.DrawString(vm_product.OuterLength.ToString() + "cm(L)x" + vm_product.OuterWidth.ToString() + "cm(W)x" + vm_product.OuterHeight.ToString() + "cm(H)", font, sb, x, y + 9 * i);//写外箱长宽高值
            g.DrawString("N.W.: " + vm_product.OuterWeightNet + " KG/G.W.: " + vm_product.OuterWeightGross + " KG", font, sb, x, y + 10 * i);//写外箱净重毛重的值
            g.DrawString("Carton#:                 of  " + vm_product.CaseOrderQty, font, sb, x, y + 11 * i);//写订单箱数值

            //侧唛
            Desc_Start = vm_product.Desc;
            Desc_Lenth = 25;
            g.DrawString("Season:" + vm_product.SeasonPrefix + " " + DateTime.Now.Year, font_1, sb, x0, y0);
            g.DrawString("Shoppers Drug Mart Inc/ Pharmaprix", font_1, sb, x0, y0 + i0);//写固定值
            if (!string.IsNullOrEmpty(vm_product.Desc) && vm_product.Desc.Length > Desc_Lenth)
            {
                Desc_Start = CommonCode.GetSumbstring_NotWord(vm_product.Desc, Desc_Lenth);
                g.DrawString("Product Description: " + Desc_Start, font_1, sb, x0, y0 + 2 * i0);//写第一行item description的值
                g.DrawString(vm_product.Desc.Substring(Desc_Start.Length, vm_product.Desc.Length - Desc_Start.Length), font_1, sb, x + 475, y0 + 3 * i0);//写第二行item description的值
                y0 = y02;
            }
            else
            {
                g.DrawString("Product Description: " + Desc_Start, font_1, sb, x0, y0 + 2 * i0);//写item description的值
            }
            g.DrawString("UPC: " + vm_product.UPC, font_1, sb, x0, y0 + 3 * i0);//写产品UPC值
            g.DrawString("PO#  " + vm_product.POID, font_1, sb, x0, y0 + 4 * i0);//写PO值
            g.DrawString("Country of Origin: China", font_1, sb, x0, y0 + 5 * i0);//写固定值
            g.DrawString("Case Pack Quantity: " + vm_product.CaseOrderQty, font_1, sb, x0, y0 + 6 * i0);//写订单箱数值

            if (vm_product.InnerBoxRate.HasValue && vm_product.InnerBoxRate.Value > 0)
            {
                g.DrawString("Inner: " + vm_product.InnerBoxRate + " PCS/Master: " + vm_product.OuterBoxRate + " PCS", font_1, sb, x0, y0 + 7 * i0);//写内核率外箱率值
            }
            else
            {
                g.DrawString("Master: " + vm_product.OuterBoxRate + " PCS", font_1, sb, x0, y0 + 7 * i0);//写内核率外箱率值
            }

            g.DrawString("Carton Dimension in CM:", font_1, sb, x0, y0 + 8 * i0);//写固定值
            g.DrawString(vm_product.OuterLength.ToString() + "cm(L)x" + vm_product.OuterWidth.ToString() + "cm(W)x" + vm_product.OuterHeight.ToString() + "cm(H)", font_1, sb, x0, y0 + 9 * i0);//写外箱长宽高值
            g.DrawString("N.W.: " + vm_product.OuterWeightNet + " KG/G.W.: " + vm_product.OuterWeightGross + " KG", font_1, sb, x0, y0 + 10 * i0);//写外箱净重毛重的值
            g.DrawString("Carton#:                 of  " + vm_product.CaseOrderQty, font_1, sb, x0, y0 + 11 * i0);//写订单箱数值

            if (!string.IsNullOrEmpty(vm_product.CartonBarcodeLabel_Image))
            {
                var stream = Thumbnail.GetRemoteImage(vm_product.CartonBarcodeLabel_Image);

                //产品设计图
                Image image1 = Image.FromStream(stream);
                g.DrawImage(image1, 230, 284, 80, 120);
                g.DrawImage(image1, 564, 250, 80, 120);
            }

            g.Save();

            img1 = MakerExcel_ShippingMark.CreateShippingMark(bmp, ShippingMarkEnum.S220);
            list_ShippingMark.Add(new VMShippingMark()
            {
                ImagePath = img1,
                Description = vm_product.No + " ——S220外箱正侧唛",
            });

            #endregion 外箱正侧唛

            if (vm_product.InnerBoxRate.HasValue && vm_product.InnerBoxRate.Value > 0)
            {
                #region 内核正侧唛

                templatePath = Utils.GetMapPath("/data/ShippingMark/S220/内核唛头模板.jpg");
                bmp = new Bitmap(templatePath);
                g = Graphics.FromImage(bmp);

                Desc_Start = vm_product.Desc;

                x = 10;//初始X坐标的值
                y = 50;//初始Y坐标的值
                i = 20;//Y坐标每次增加的值，行间距
                y2 = y + 15;//字符换行后，更新后的y2的坐标的值

                x0 = 380;//初始侧唛X坐标的值
                y0 = 70;//初始侧唛Y坐标的值
                i0 = 18;//侧唛Y坐标每次增加的值，行间距
                y02 = y0 + 15;
                Desc_Lenth = 50;

                //正唛
                g.DrawString("Season: Harvest 2016", font, sb, x, y);//写固定值
                g.DrawString("Shoppers Drug Mart Inc/ Pharmaprix", font, sb, x, y + i);//写固定值
                if (!string.IsNullOrEmpty(vm_product.Desc) && vm_product.Desc.Length > Desc_Lenth)
                {
                    Desc_Start = CommonCode.GetSumbstring_NotWord(vm_product.Desc, Desc_Lenth);
                    g.DrawString(Desc_Start, font, sb, x, y + 2 * i);//写第一行item description的值
                    g.DrawString(vm_product.Desc.Substring(Desc_Start.Length, vm_product.Desc.Length - Desc_Start.Length), font, sb, x + 125, y + 3 * i);//写第二行item description的值
                    y = y2;
                }
                else
                {
                    g.DrawString(Desc_Start, font, sb, x, y + 2 * i);//写item description的值
                }
                g.DrawString("UPC: " + vm_product.UPC, font, sb, x, y + 3 * i);//写产品UPC值
                g.DrawString("PO#  " + vm_product.POID, font, sb, x, y + 4 * i);//写PO值
                g.DrawString("Country of Origin: China", font, sb, x, y + 5 * i);//写固定值
                g.DrawString("Inner Case Quantity: " + vm_product.OuterBoxRate / vm_product.InnerBoxRate + " PCS", font, sb, x, y + 6 * i);//写内核数量值
                g.DrawString("Carton Dimension in CM:", font, sb, x, y + 7 * i);//写固定值
                g.DrawString(vm_product.InnerLength.ToString() + "cm(L)x" + vm_product.InnerWidth.ToString() + "cm(W)x" + vm_product.InnerHeight.ToString() + "cm(H)", font, sb, x, y + 8 * i);//写外箱长宽高值
                g.DrawString("N.W.: " + vm_product.InnerWeightNet + " KG/G.W.: " + vm_product.InnerWeightGross + " KG", font, sb, x, y + 9 * i);//写外箱净重毛重的值

                //侧唛
                Desc_Start = vm_product.Desc;
                Desc_Lenth = 50;
                g.DrawString("Season: Harvest 2016", font_1, sb, x0, y0);//写固定值
                g.DrawString("Shoppers Drug Mart Inc/ Pharmaprix", font_1, sb, x0, y0 + i0);//写固定值
                if (!string.IsNullOrEmpty(vm_product.Desc) && vm_product.Desc.Length > Desc_Lenth)
                {
                    Desc_Start = CommonCode.GetSumbstring_NotWord(vm_product.Desc, Desc_Lenth);
                    g.DrawString(Desc_Start, font_1, sb, x0, y0 + 2 * i0);//写第一行item description的值
                    g.DrawString(vm_product.Desc.Substring(Desc_Start.Length, vm_product.Desc.Length - Desc_Start.Length), font_1, sb, x + 515, y0 + 3 * i0);//写第二行item description的值
                    y0 = y02;
                }
                else
                {
                    g.DrawString(Desc_Start, font_1, sb, x0, y0 + 2 * i0);//写item description的值
                }
                g.DrawString("UPC: " + vm_product.UPC, font_1, sb, x0, y0 + 3 * i0);//写产品UPC值
                g.DrawString("PO#  " + vm_product.POID, font_1, sb, x0, y0 + 4 * i0);//写PO值
                g.DrawString("Country of Origin: China", font_1, sb, x0, y0 + 5 * i0);//写固定值
                g.DrawString("Inner Case Quantity: " + vm_product.OuterBoxRate / vm_product.InnerBoxRate + " PCS", font_1, sb, x0, y0 + 6 * i0);//写内核数量值
                g.DrawString("Carton Dimension in CM:", font_1, sb, x0, y0 + 7 * i0);//写固定值
                g.DrawString(vm_product.InnerLength.ToString() + "cm(L)x" + vm_product.InnerWidth.ToString() + "cm(W)x" + vm_product.InnerHeight.ToString() + "cm(H)", font_1, sb, x0, y0 + 8 * i0);//写外箱长宽高值
                g.DrawString("N.W.: " + vm_product.InnerWeightNet + " KG/G.W.: " + vm_product.InnerWeightGross + " KG", font_1, sb, x0, y0 + 9 * i0);//写外箱净重毛重的值

                g.Save();

                img2 = MakerExcel_ShippingMark.CreateShippingMark(bmp, ShippingMarkEnum.S220);
                list_ShippingMark.Add(new VMShippingMark()
                {
                    ImagePath = img2,
                    Description = vm_product.No + " ——S220内核正侧唛",
                });

                #endregion 内核正侧唛
            }
            result.IsSuccess = true;

            result.Data = list_ShippingMark;
            return result;
        }

        private static VMAjaxProcessResult Bind_F20(VMPurchaseProduct vm_product)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            result.IsSuccess = true;
            List<VMShippingMark> list_ShippingMark = new List<VMShippingMark>();//箱唛图片列表

            string templatePath = Utils.GetMapPath("/data/ShippingMark/F20/外箱正唛.jpg");
            Bitmap bmp = new Bitmap(templatePath);
            Graphics g = Graphics.FromImage(bmp);
            Font font = new Font("Arial", 9, FontStyle.Bold);//字体
            SolidBrush sb = new SolidBrush(Color.Black);
            Image image1 = null;
            Image image2 = null;
            Image image3 = null;
            string Desc_Start = vm_product.Desc;

            //int x = 16;//初始X坐标的值
            //int y = 25;//初始Y坐标的值
            //int i = 33;//Y坐标每次增加的值，行间距
            //int y2 = y + 25;//字符换行后，更新后的y2的坐标的值

            #region 外箱正唛

            templatePath = Utils.GetMapPath("/data/ShippingMark/F20/外箱正唛.jpg");
            bmp = new Bitmap(templatePath);
            g = Graphics.FromImage(bmp);

            image2 = Image.FromFile(Utils.GetMapPath("~/data/ShippingMark/F20/易碎标.jpg"));//图标路径
            if (vm_product.IsFragile2 == true)
            {
                g.DrawImageUnscaled(image2, 10, 10, 134, 126);//写易碎标
                g.DrawImageUnscaled(image2, 414, 10, 134, 126);//写易碎标
            }

            if (!File.Exists(Utils.GetMapPath("~/data/ShippingMark/F20/" + vm_product.SeasonPrefix + ".jpg")))
            {
                throw new Exception("季节图标不存在！" + vm_product.SeasonPrefix);
            }

            image1 = Image.FromFile(Utils.GetMapPath("~/data/ShippingMark/F20/" + vm_product.SeasonPrefix + ".jpg"));//图标路径
            g.DrawImage(image1, 230, 48, 99, 62);//写季节图标

            g.DrawString(vm_product.No, font, sb, 252, 232);//写JK货号值
            g.DrawString(vm_product.Department.ToString(), font, sb, 252, 252);//写Dept的值
            g.DrawString(vm_product.CaseOrderQty.ToString(), font, sb, 340, 288);//写总箱数的值

            g.Save();
            string img1 = MakerExcel_ShippingMark.CreateShippingMark(bmp, ShippingMarkEnum.F20);
            list_ShippingMark.Add(new VMShippingMark()
            {
                ImagePath = img1,
                Description = vm_product.No + " ——F20外箱正唛",
            });

            #endregion 外箱正唛

            #region 外箱侧唛

            templatePath = Utils.GetMapPath("/data/ShippingMark/F20/外箱侧唛.jpg");
            bmp = new Bitmap(templatePath);
            g = Graphics.FromImage(bmp);
            font = new Font("Arial", 12, FontStyle.Bold);//字体

            g.DrawString(vm_product.UPC, font, sb, 192, 4);//写产品条码的值
            g.DrawString(vm_product.POID, font, sb, 138, 41);//写customer po的值
            g.DrawString(vm_product.SkuNumber, font, sb, 115, 78);//写SKU的值
            if (!string.IsNullOrEmpty(vm_product.Desc) && vm_product.Desc.Length > 25)
            {
                Desc_Start = CommonCode.GetSumbstring_NotWord(vm_product.Desc, 25);
                g.DrawString(Desc_Start, font, sb, 122, 98);//写第一行item description的值
                g.DrawString(vm_product.Desc.Substring(Desc_Start.Length, vm_product.Desc.Length - Desc_Start.Length), font, sb, 122, 114);//写第二行item description的值
            }
            else
            {
                g.DrawString(Desc_Start, font, sb, 122, 114);//写item description的值
            }
            //g.DrawString(vm_product.Desc, font, sb, 122, 114);//写item description的值
            g.DrawString(vm_product.OuterBoxRate.ToString(), font, sb, 249, 151);//写外箱率值

            if (!string.IsNullOrEmpty(vm_product.InnerBoxRate.ToString()) && vm_product.InnerBoxRate != 0)
            {
                g.DrawString((vm_product.OuterBoxRate / vm_product.InnerBoxRate).ToString(), font, sb, 150, 188);//写外箱率除以内核率的值
            }

            g.DrawString(vm_product.InnerBoxRate.ToString(), font, sb, 182, 224);//写内核率值
            g.DrawString(vm_product.OuterWeightNetLBS.ToString(), font, sb, 90, 262);//写外箱净重值
            g.DrawString(vm_product.OuterWeightGrossLBS.ToString(), font, sb, 108, 299);//写外箱毛重值
            g.DrawString(vm_product.OuterLengthIN.ToString(), font, sb, 73, 334);//写外箱尺寸的值
            g.DrawString(vm_product.OuterWidthIN.ToString(), font, sb, 157, 334);//写外箱尺寸的值
            g.DrawString(vm_product.OuterHeightIN.ToString(), font, sb, 240, 334);//写外箱尺寸的值
            g.DrawString(vm_product.OuterVolume.ToString(), font, sb, 63, 372);//写外箱材积值
            g.DrawString("ASSORTED", font, sb, 70, 408);//写color值

            g.Save();
            string img2 = MakerExcel_ShippingMark.CreateShippingMark(bmp, ShippingMarkEnum.F20);

            list_ShippingMark.Add(new VMShippingMark()
            {
                ImagePath = img2,
                Description = vm_product.No + " ——F20外箱侧唛",
            });

            #endregion 外箱侧唛

            if (vm_product.InnerBoxRate.HasValue && vm_product.InnerBoxRate.Value > 0)
            {
                #region 内核正唛

                templatePath = Utils.GetMapPath("/data/ShippingMark/F20/内核正唛.jpg");
                bmp = new Bitmap(templatePath);
                g = Graphics.FromImage(bmp);
                image1 = Image.FromFile(Utils.GetMapPath("~/data/ShippingMark/F20/" + vm_product.SeasonPrefix + ".jpg"));//图标路径
                g.DrawImage(image1, 194, 60, 99, 62);//写季节图标
                if (vm_product.IsFragile2 == true)
                {
                    g.DrawImageUnscaled(image2, 10, 10, 134, 126);//写易碎标
                    g.DrawImageUnscaled(image2, 360, 10, 134, 126);//写易碎标
                }
                font = new Font("Arial", 9, FontStyle.Bold);//字体
                g.DrawString(vm_product.No, font, sb, 242, 268);//写JK货号值
                g.DrawString(vm_product.Department.ToString(), font, sb, 242, 288);//写Dept的值

                g.Save();
                string img3 = MakerExcel_ShippingMark.CreateShippingMark(bmp, ShippingMarkEnum.F20);

                list_ShippingMark.Add(new VMShippingMark()
                {
                    ImagePath = img3,
                    Description = vm_product.No + " ——F20内核正唛",
                });

                #endregion 内核正唛

                #region 内核侧唛

                templatePath = Utils.GetMapPath("/data/ShippingMark/F20/内核侧唛.jpg");
                bmp = new Bitmap(templatePath);
                g = Graphics.FromImage(bmp);
                font = new Font("Arial", 12, FontStyle.Bold);//字体

                g.DrawString(vm_product.UPC, font, sb, 188, 20);//写产品条码的值
                g.DrawString(vm_product.SkuNumber, font, sb, 116, 57);//写SKU的值
                if (!string.IsNullOrEmpty(vm_product.Desc) && vm_product.Desc.Length > 20)
                {
                    Desc_Start = CommonCode.GetSumbstring_NotWord(vm_product.Desc, 20);
                    g.DrawString(Desc_Start, font, sb, 122, 78);//写第一行item description的值
                    g.DrawString(vm_product.Desc.Substring(Desc_Start.Length, vm_product.Desc.Length - Desc_Start.Length), font, sb, 122, 94);//写第二行item description的值
                }
                else
                {
                    g.DrawString(Desc_Start, font, sb, 122, 94);//写item description的值
                }
                g.DrawString(vm_product.InnerBoxRate.ToString(), font, sb, 157, 130);//写内核率值
                g.DrawString("ASSORTED", font, sb, 68, 167);//写color值

                g.Save();
                string img4 = MakerExcel_ShippingMark.CreateShippingMark(bmp, ShippingMarkEnum.F20);

                list_ShippingMark.Add(new VMShippingMark()
                {
                    ImagePath = img4,
                    Description = vm_product.No + " ——F20内核侧唛",
                });

                #endregion 内核侧唛
            }

            result.IsSuccess = true;

            result.Data = list_ShippingMark;
            return result;
        }

        private static VMAjaxProcessResult Bind_S135(VMPurchaseProduct vm_product)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            result.IsSuccess = true;
            List<VMShippingMark> list_ShippingMark = new List<VMShippingMark>();//箱唛图片列表

            string templatePath = Utils.GetMapPath("/data/ShippingMark/S135/外箱唛头模板.jpg");//存放正唛路径
            Bitmap bmp = new Bitmap(templatePath);
            Graphics g = Graphics.FromImage(bmp);
            Font font = new Font("Arial", 12, FontStyle.Bold);//字体
            SolidBrush sb = new SolidBrush(Color.Black);
            string Desc_Start = vm_product.Desc;
            string img1 = "";
            string img2 = "";

            int x = 18;//初始正唛X坐标的值
            int y = 240;//初始正唛Y坐标的值
            int i = 25;//Y坐标每次增加的值，行间距
            int DescLength = 25;

            int y2 = y + 18;//字符换行后，更新后的y2的坐标的值

            #region 外箱唛头

            Image image1 = Image.FromFile(Utils.GetMapPath("~/data/ShippingMark/S135/易碎标.jpg"));//图标路径
            if (vm_product.IsFragile2 == true)
            {
                g.DrawImageUnscaled(image1, 227, 5, 134, 108);//写易碎标
            }
            g.DrawString("PO #:  " + vm_product.POID, font, sb, x, y);//写PO#值
            g.DrawString("CODE: " + vm_product.SkuCode, font, sb, x, y + i);//写核算单中的SKU CODE
            g.DrawString(vm_product.SkuNumber, font, sb, x + 56, y + 2 * i);//写核算单中的SKU
            //g.DrawString("MADE IN CHINA", font, sb, x, y + 3 * i);//写固定值
            if (!string.IsNullOrEmpty(vm_product.Desc) && vm_product.Desc.Length > DescLength)
            {
                Desc_Start = CommonCode.GetSumbstring_NotWord(vm_product.Desc, DescLength);
                g.DrawString("DESCRIPTION:  " + Desc_Start, font, sb, x, y + 3 * i);//写第一行item description的值
                g.DrawString(vm_product.Desc.Substring(Desc_Start.Length, vm_product.Desc.Length - Desc_Start.Length), font, sb, x + 125, y2 + 3 * i);//写第二行item description的值
                y = y2;
            }
            else
            {
                g.DrawString("DESCRIPTION:  " + Desc_Start, font, sb, x, y + 3 * i);//写item description的值
            }

            g.DrawString("QTY: " + vm_product.OuterBoxRate.ToString() + vm_product.UnitEngName, font, sb, x, y + 4 * i);//写外箱率值
            g.DrawString("G.W.: " + vm_product.OuterWeightGross + "  Pound", font, sb, x, y + 5 * i);//写外箱毛重的值
            g.DrawString("Measure: " + vm_product.OuterLengthIN + " L x  " + vm_product.OuterWidthIN + " W x  " + vm_product.OuterHeightIN + " H   Inch", font, sb, x, y + 6 * i);//写外箱净重的值
            g.DrawString("Case#:              OF   " + vm_product.CaseOrderQty, font, sb, x, y + 7 * i);//写箱数的值
            g.DrawString("IIIinois, USA", font, sb, x, y + 8 * i);//写固定值
            g.DrawString("MADE IN CHINA", font, sb, x, y + 9 * i);//写固定值

            g.Save();
            img1 = MakerExcel_ShippingMark.CreateShippingMark(bmp, ShippingMarkEnum.S135);

            list_ShippingMark.Add(new VMShippingMark()
            {
                ImagePath = img1,
                Description = vm_product.No + " ——S135外箱唛头",
            });

            #endregion 外箱唛头

            //邮购内核，暂时不管

            //#region 内盒唛头

            //x = 18;//初始正唛X坐标的值
            //y = 240;//初始正唛Y坐标的值
            //i = 25;//Y坐标每次增加的值，行间距
            //y2 = y + 18;//字符换行后，更新后的y2的坐标的值
            //if (vm_product.IsFragile2 == true)
            //{
            //    g.DrawImageUnscaled(image1, 227, 5, 134, 108);//写易碎标
            //}
            //g.DrawString("PO #:  " + vm_product.POID, font, sb, x, y);//写PO#值
            //g.DrawString("CODE: " + vm_product.SkuCode, font, sb, x, y + i);//写sku#值
            //g.DrawString(vm_product.SkuNumber, font, sb, x + 60, y + 2 * i);//写我司货号的值
            ////g.DrawString("MADE IN CHINA", font, sb, x, y + 3 * i);//写固定值
            //if (!string.IsNullOrEmpty(vm_product.Desc) && vm_product.Desc.Length > DescLength)
            //{
            //    Desc_Start = CommonCode.GetSumbstring_NotWord(vm_product.Desc, DescLength);
            //    g.DrawString("DESCRIPTION:  " + Desc_Start, font, sb, x, y + 3 * i);//写第一行item description的值
            //    g.DrawString(vm_product.Desc.Substring(Desc_Start.Length, vm_product.Desc.Length - Desc_Start.Length), font, sb, x + 125, y2 + 3 * i);//写第二行item description的值
            //    y = y2;
            //}
            //else
            //{
            //    g.DrawString("DESCRIPTION:  " + Desc_Start, font, sb, x, y + 3 * i);//写item description的值
            //}

            //g.DrawString("QTY: " + vm_product.OuterBoxRate.ToString(), font, sb, x, y + 4 * i);//写外箱率值
            //g.DrawString("G.W.: " + vm_product.OuterWeightGross + "  Pound", font, sb, x, y + 5 * i);//写外箱毛重的值
            //g.DrawString("Measure: " + vm_product.OuterLengthIN + " L x  " + vm_product.OuterWidthIN + " W x  " + vm_product.OuterHeightIN + " H   Inch", font, sb, x, y + 6 * i);//写外箱净重的值
            //g.DrawString("Case#:              OF   " + vm_product.CaseOrderQty, font, sb, x, y + 7 * i);//写箱数的值
            //g.DrawString("IIIinois, USA", font, sb, x, y + 8 * i);//写固定值
            //g.DrawString("MADE IN CHINA", font, sb, x, y + 9 * i);//写固定值

            //g.Save();
            //img2 = MakerExcel_ShippingMark.CreateShippingMark(bmp, ShippingMarkEnum.S135);

            //#endregion 内盒唛头

            result.IsSuccess = true;

            result.Data = list_ShippingMark;
            return result;
        }

        private static VMAjaxProcessResult Bind_S164(VMPurchaseProduct vm_product)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            result.IsSuccess = true;
            List<VMShippingMark> list_ShippingMark = new List<VMShippingMark>();//箱唛图片列表

            string templatePath = Utils.GetMapPath("/data/ShippingMark/S164/外箱正唛.jpg");
            Bitmap bmp = new Bitmap(templatePath);
            Graphics g = Graphics.FromImage(bmp);
            Font font = new Font("Arial", 20, FontStyle.Regular);//字体
            SolidBrush sb = new SolidBrush(Color.Black);
            Image image1 = null;
            Image image2 = null;
            Image image3 = null;
            string Desc_Start = vm_product.Desc;

            int x = 16;//初始X坐标的值
            int y = 25;//初始Y坐标的值
            int i = 33;//Y坐标每次增加的值，行间距
            int y2 = y + 25;//字符换行后，更新后的y2的坐标的值

            #region 外箱正唛

            templatePath = Utils.GetMapPath("/data/ShippingMark/S164/外箱正唛.jpg");
            bmp = new Bitmap(templatePath);
            g = Graphics.FromImage(bmp);
            //更换背景图

            x = 20;
            y = 118;
            i = 33;
            y2 = y + 25;

            font = new Font("Arial Regular", 14, FontStyle.Regular);//字体
            g.DrawString(vm_product.DestinationPortName, font, sb, x, y);//写港口
            g.DrawString("P.O.#: " + vm_product.POID, font, sb, x, y + i);//写po
            g.DrawString("Made In:  China", font, sb, x, y + 2 * i);//写固定值

            g.Save();

            string img1 = MakerExcel_ShippingMark.CreateShippingMark(bmp, ShippingMarkEnum.S164);

            list_ShippingMark.Add(new VMShippingMark()
            {
                ImagePath = img1,
                Description = vm_product.No + " ——S164外箱正唛",
            });

            #endregion 外箱正唛

            #region 外箱侧唛

            templatePath = Utils.GetMapPath("/data/ShippingMark/S164/外箱侧唛.jpg");
            bmp = new Bitmap(templatePath);
            g = Graphics.FromImage(bmp);
            //更换背景图

            x = 5;
            y = 7;
            i = 30;
            y2 = y + 20;

            font = new Font("Arial Regular", 12, FontStyle.Regular);//字体
            if (!string.IsNullOrEmpty(vm_product.Desc) && vm_product.Desc.Length > 40)
            {
                Desc_Start = CommonCode.GetSumbstring_NotWord(vm_product.Desc, 40);
                g.DrawString("Description of Product: " + Desc_Start, font, sb, x, y);//写第一行item description的值
                g.DrawString(vm_product.Desc.Substring(Desc_Start.Length, vm_product.Desc.Length - Desc_Start.Length), font, sb, x + 162, y2);//写第二行item description的值
                y = y2;
            }
            else
            {
                g.DrawString("Description of Product: " + Desc_Start, font, sb, x, y);//写item description的值
            }

            g.DrawString("Menard SKU #: " + vm_product.SkuNumber, font, sb, x, y + i);//写sku
            g.DrawString("Quantity: " + vm_product.OuterBoxRate, font, sb, x, y + 2 * i);//写外箱率
            g.DrawString("Net Weight: " + vm_product.OuterWeightNetLBS + "  LBS", font, sb, x, y + 3 * i);//写外箱净重
            g.DrawString("Gross Weight: " + vm_product.OuterWeightGrossLBS + "  LBS", font, sb, x, y + 4 * i);//写外箱毛重
            g.DrawString("Carton _____ of: " + vm_product.CaseOrderQty, font, sb, x, y + 5 * i);//写总箱数

            g.Save();

            string img2 = MakerExcel_ShippingMark.CreateShippingMark(bmp, ShippingMarkEnum.S164);

            list_ShippingMark.Add(new VMShippingMark()
            {
                ImagePath = img2,
                Description = vm_product.No + " ——S164外箱侧唛",
            });

            #endregion 外箱侧唛

            if (vm_product.InnerBoxRate.HasValue && vm_product.InnerBoxRate.Value > 0)
            {
                #region 内核正唛

                templatePath = Utils.GetMapPath("/data/ShippingMark/S164/内核正唛.jpg");
                bmp = new Bitmap(templatePath);
                g = Graphics.FromImage(bmp);
                //更换背景图

                x = 10;
                y = 118;
                i = 33;
                y2 = y + 25;

                font = new Font("Arial Regular", 12, FontStyle.Regular);//字体
                g.DrawString(vm_product.DestinationPortName, font, sb, x, y);//写港口
                g.DrawString("P.O.#: " + vm_product.POID, font, sb, x, y + i);//写po
                g.DrawString("Made In:  China", font, sb, x, y + 2 * i);//写固定值

                g.Save();

                string img3 = MakerExcel_ShippingMark.CreateShippingMark(bmp, ShippingMarkEnum.S164);

                list_ShippingMark.Add(new VMShippingMark()
                {
                    ImagePath = img3,
                    Description = vm_product.No + " ——S164内核正唛",
                });

                #endregion 内核正唛

                #region 内核侧唛

                templatePath = Utils.GetMapPath("/data/ShippingMark/S164/内核侧唛.jpg");
                bmp = new Bitmap(templatePath);
                g = Graphics.FromImage(bmp);
                //更换背景图

                x = 5;
                y = 7;
                i = 20;
                y2 = y + 15;

                font = new Font("Arial Regular", 10, FontStyle.Regular);//字体
                if (!string.IsNullOrEmpty(vm_product.Desc) && vm_product.Desc.Length > 40)
                {
                    Desc_Start = CommonCode.GetSumbstring_NotWord(vm_product.Desc, 40);
                    g.DrawString("Description of Product: " + Desc_Start, font, sb, x, y);//写第一行item description的值
                    g.DrawString(vm_product.Desc.Substring(Desc_Start.Length, vm_product.Desc.Length - Desc_Start.Length), font, sb, x + 140, y2);//写第二行item description的值
                    y = y2;
                }
                else
                {
                    g.DrawString("Description of Product: " + Desc_Start, font, sb, x, y);//写item description的值
                }

                g.DrawString("Menard SKU #: " + vm_product.SkuNumber, font, sb, x, y + i);//写sku
                g.DrawString("Quantity: " + vm_product.InnerBoxRate, font, sb, x, y + 2 * i);//写外箱率
                g.DrawString("Net Weight: " + vm_product.InnerNetWeightLBS + "  LBS", font, sb, x, y + 3 * i);//写外箱净重
                g.DrawString("Gross Weight: " + vm_product.InnerGrossWeightLBS + "  LBS", font, sb, x, y + 4 * i);//写外箱毛重
                g.DrawString("Carton _____ of: " + vm_product.CaseOrderQty, font, sb, x, y + 5 * i);//写总箱数

                g.Save();

                string img4 = MakerExcel_ShippingMark.CreateShippingMark(bmp, ShippingMarkEnum.S164);

                list_ShippingMark.Add(new VMShippingMark()
                {
                    ImagePath = img4,
                    Description = vm_product.No + " ——S164内核侧唛",
                });

                #endregion 内核侧唛
            }
            result.IsSuccess = true;

            result.Data = list_ShippingMark;
            return result;
        }

        private static VMAjaxProcessResult Bind_S10(VMPurchaseProduct vm_product)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            result.IsSuccess = true;
            List<VMShippingMark> list_ShippingMark = new List<VMShippingMark>();//箱唛图片列表

            string templatePath = Utils.GetMapPath("/data/ShippingMark/S10/外箱唛头.jpg");
            Bitmap bmp = new Bitmap(templatePath);
            Graphics g = Graphics.FromImage(bmp);
            Font font = new Font("Arial", 20, FontStyle.Regular);//字体
            SolidBrush sb = new SolidBrush(Color.Black);
            Image image1 = null;
            Image image2 = null;
            Image image3 = null;
            string Desc_Start = vm_product.Desc;

            int x = 16;//初始X坐标的值
            int y = 25;//初始Y坐标的值
            int i = 33;//Y坐标每次增加的值，行间距
            int y2 = y + 25;//字符换行后，更新后的y2的坐标的值

            #region 外箱唛头

            templatePath = Utils.GetMapPath("/data/ShippingMark/S10/外箱唛头.jpg");
            bmp = new Bitmap(templatePath);
            g = Graphics.FromImage(bmp);
            //更换背景图

            x = 5;
            y = 3;
            i = 20;

            font = new Font("Arial Regular", 10, FontStyle.Regular);//字体

            g.DrawString("To: " + vm_product.AcceptInformation_CompanyName, font, sb, x + 435, y);//写公司名称
            g.DrawString(vm_product.AcceptInformation_CompanyName, font, sb, x + 435, y + i);//写街道地址1
            g.DrawString(vm_product.AcceptInformation_CustomerReg, font, sb, x + 435, y + 2 * i);//写街道地址2
            //g.DrawString(vm_product.Address3, font, sb, x + 435, y + 3 * i);//写街道地址3
            g.DrawString("Vender:                                                         Glitzhome,LLC.", font, sb, x, y + i);//写固定值
            g.DrawString("MarshallsPO#                                                " + vm_product.POID, font, sb, x, y + 4 * i);//写customer po
            g.DrawString("Vendor Style:                                                " + vm_product.No, font, sb, x, y + 5 * i);//写jk no
            g.DrawString("Department #                                                 " + vm_product.DepartmentName, font, sb, x, y + 6 * i);//写部门编号
            g.DrawString("Description/Color:                                        " + vm_product.Desc, font, sb, x, y + 7 * i);//写品名描述
            g.DrawString("Size:                                                                             L" + "  x" + "               W" + "  x" + "               H" + " (cm)", font, sb, x, y + 8 * i);//写外箱尺寸
            g.DrawString("Total Units:                                                     " + vm_product.OuterBoxRate, font, sb, x, y + 9 * i);//写外箱装箱率
            g.DrawString("# of Store Ready inners enclosed:              " + vm_product.OfStoreReadyInnersEnclosed, font, sb, x, y + 10 * i);//写# of Store Ready inners enclosed
            g.DrawString("Pre-ticketed:                                                  " + vm_product.PreTicketed, font, sb, x, y + 11 * i);//写Pre-ticketed
            g.DrawString("Carton:                                                                            " + "of    " + vm_product.CaseOrderQty, font, sb, x, y + 12 * i);//写箱数
            g.DrawString("Country of Origin:                                          China", font, sb, x, y + 13 * i);//写固定值

            g.Save();

            string img1 = MakerExcel_ShippingMark.CreateShippingMark(bmp, ShippingMarkEnum.S10);

            list_ShippingMark.Add(new VMShippingMark()
            {
                ImagePath = img1,
                Description = vm_product.No + " ——S10外箱唛头（相邻2面印刷） " + vm_product.AcceptInformation_Comment,
            });

            #endregion 外箱唛头

            if (vm_product.InnerBoxRate.HasValue && vm_product.InnerBoxRate.Value > 0)
            {
                #region 内盒唛头

                templatePath = Utils.GetMapPath("/data/ShippingMark/S10/内核唛头.jpg");
                bmp = new Bitmap(templatePath);
                g = Graphics.FromImage(bmp);
                //更换背景图

                x = 5;
                y = 3;
                i = 20;

                font = new Font("Arial Regular", 10, FontStyle.Regular);//字体

                g.DrawString("MarshallsPO#                                                " + vm_product.POID, font, sb, x, y);//写customer po
                g.DrawString("Department #                                                 " + vm_product.DepartmentName, font, sb, x, y + 1 * i);//写部门编号
                g.DrawString("                Marshalls Style #                                                                           Unit", font, sb, x, y + 2 * i);//写固定值
                g.DrawString("                     " + vm_product.SkuNumber + "                                                                                        " + vm_product.InnerBoxRate, font, sb, x, y + 3 * i);//写内核率
                g.DrawString("Total:                                                              " + vm_product.InnerBoxRate, font, sb, x, y + 4 * i);//写内核率

                g.Save();

                string img2 = MakerExcel_ShippingMark.CreateShippingMark(bmp, ShippingMarkEnum.S10);

                list_ShippingMark.Add(new VMShippingMark()
                {
                    ImagePath = img2,
                    Description = vm_product.No + " ——S10内盒唛头（1面印刷） " + vm_product.AcceptInformation_Comment,
                });

                #endregion 内盒唛头
            }
            result.IsSuccess = true;

            result.Data = list_ShippingMark;
            return result;
        }

        private static VMAjaxProcessResult Bind_S56(VMPurchaseProduct vm_product)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            result.IsSuccess = true;
            List<VMShippingMark> list_ShippingMark = new List<VMShippingMark>();//箱唛图片列表

            vm_product.DestinationPortName = "BERLIN, NJ";//目的港是固定的

            string templatePath = Utils.GetMapPath("/data/ShippingMark/S56/外箱唛头.jpg");//存放唛头路径
            Bitmap bmp = new Bitmap(templatePath);
            Graphics g = Graphics.FromImage(bmp);
            Font font = new Font("Arial", 10, FontStyle.Regular);//字体
            SolidBrush sb = new SolidBrush(Color.Black);
            string Desc_Start = vm_product.Desc;
            string img1 = "";
            string img2 = "";

            int x = 5;//初始正唛X坐标的值
            int y = 35;//初始正唛Y坐标的值
            int i = 20;//Y坐标每次增加的值，行间距

            #region 外箱唛头

            if (!File.Exists(Utils.GetMapPath("~/data/ShippingMark/S56/" + vm_product.SeasonPrefix + ".jpg")))
            {
                throw new Exception("季节图标不存在！" + vm_product.SeasonPrefix);
            }

            Image image1 = Image.FromFile(Utils.GetMapPath("~/data/ShippingMark/S56/" + vm_product.SeasonPrefix + ".jpg"));//图标路径
            g.DrawImage(image1, 530, 50);//在正唛上写季节图标
            g.DrawImage(image1, 1070, 72);//在侧唛上写季节图标

            g.DrawString(vm_product.DestinationPortName, font, sb, 290, 144);//正唛中的目的港
            g.DrawString(vm_product.DestinationPortName, font, sb, 945, 163);//侧唛中的目的港
            g.DrawString("MADE IN CHINA", font, sb, 285, 164);//正唛中的固定值
            g.DrawString("MADE IN CHINA", font, sb, 940, 183);//侧唛中的固定值


            g.DrawString("PURCHASE ORDER NO.:  " + vm_product.POID, font, sb, x, y);//写Customer PO#值
            g.DrawString("TO: " + vm_product.CompanyName, font, sb, x, y + i);//写客户资料中的公司名，非收货地址的公司名
            g.DrawString(vm_product.Address1, font, sb, x + 25, y + 2 * i);//写客户资料中的street，非收货地址的street
            g.DrawString(vm_product.Address2, font, sb, x + 25, y + 3 * i);//写客户资料中的State,City,Zipcode，非收货地址的...
            g.DrawString(vm_product.Address3, font, sb, x + 25, y + 4 * i);//写客户资料中的Country，非收货地址的...
            g.DrawString("AC MOORE ITEM#: " + vm_product.SkuNumber, font, sb, x, y + 5 * i);//customer sku
            g.DrawString("VPN#: " + vm_product.No, font, sb, x, y + 6 * i);//jk no
            g.DrawString("UPC#: " + vm_product.UPC, font, sb, x, y + 7 * i);//核算单Upc
            g.DrawString("QUANTITY: " + vm_product.OuterBoxRate, font, sb, x, y + 8 * i);//外箱率
            g.DrawString("MADE IN CHINA", font, sb, x, y + 9 * i);//固定值
            g.DrawString("CARTON NO.: " + vm_product.CaseOrderQty, font, sb, x, y + 10 * i);//订单总箱数

            x = 722;
            y = 70;
            g.DrawString("AC MOORE ITEM#: " + vm_product.SkuNumber, font, sb, x, y);//customer sku
            g.DrawString("VPN#: " + vm_product.No, font, sb, x, y + 1 * i);//jk no
            g.DrawString("UPC#: " + vm_product.UPC, font, sb, x, y + 2 * i);//核算单Upc
            g.DrawString("MEAS.:       \"L X       \"W X       \"H", font, sb, x, y + 3 * i);//产品外箱尺寸
            g.DrawString("GROSS WEIGHT: " + vm_product.OuterWeightGrossLBS + "   LBS", font, sb, x, y + 4 * i);//产品外箱毛重
            g.DrawString("NET WEIGHT: " + vm_product.OuterWeightNetLBS + "   LBS", font, sb, x, y + 5 * i);//产品外箱净重            

            g.Save();
            img1 = MakerExcel_ShippingMark.CreateShippingMark(bmp, ShippingMarkEnum.S56);

            list_ShippingMark.Add(new VMShippingMark()
            {
                ImagePath = img1,
                Description = vm_product.No + " ——S56外箱唛头",
            });

            #endregion 外箱唛头

            if (vm_product.InnerBoxRate.HasValue && vm_product.InnerBoxRate.Value > 0)
            {
                #region 内盒唛头
                x = 5;
                y = 85;
                templatePath = Utils.GetMapPath("/data/ShippingMark/S56/内核唛头.jpg");//存放正唛路径
                bmp = new Bitmap(templatePath);
                g = Graphics.FromImage(bmp);
                font = new Font("Arial", 10, FontStyle.Regular);//字体
                sb = new SolidBrush(Color.Black);


                Image image2 = Image.FromFile(Utils.GetMapPath("~/data/ShippingMark/S56/" + vm_product.SeasonPrefix + ".jpg"));//图标路径
                g.DrawImage(image1, 188, 97);//在内核上写季节图标

                g.DrawString("AC MOORE ITEM#: " + vm_product.SkuNumber, font, sb, x, y);//customer sku
                g.DrawString("VPN#: " + vm_product.No, font, sb, x, y + 1 * i);//jk no
                g.DrawString("UPC#: " + vm_product.UPC, font, sb, x, y + 2 * i);//核算单Upc
                g.DrawString("QUANTITY: " + vm_product.InnerBoxRate, font, sb, x, y + 3 * i);//内核率
                g.DrawString("MADE IN CHINA", font, sb, x, y + 4 * i);//固定值

                g.Save();
                img2 = MakerExcel_ShippingMark.CreateShippingMark(bmp, ShippingMarkEnum.S56);

                list_ShippingMark.Add(new VMShippingMark()
                {
                    ImagePath = img2,
                    Description = vm_product.No + " ——S56内盒唛头",
                });
                #endregion 内盒唛头
            }
            result.IsSuccess = true;

            result.Data = list_ShippingMark;
            return result;
        }
    }
}