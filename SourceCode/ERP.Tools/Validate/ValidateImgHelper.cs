using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Tools.Validate
{
    public static class ValidateImgHelper
    {
        public static string GenerateRandomCode(int length)
        {
            string code = string.Empty;
            Random r = new Random();
            for (int i = 0; i < length; i++)
            {
                code += r.Next(0, 10);
            }
            return code;
        }

        public static byte[] GenerateValidateImg(string checkCode)
        {
            Bitmap image = new Bitmap((int)Math.Ceiling((checkCode.Length * 14.5)), 22);
            Graphics g = Graphics.FromImage(image);
            Pen pen = new Pen(Color.Silver);
            //        Font font = new Font("Terminal", 14, FontStyle.Bold);
            Font font = new Font("Terminal", 14);
            MemoryStream ms = new MemoryStream();

            try
            {
                //生成随机生成器
                Random random = new Random();

                //清空图片背景色
                g.Clear(Color.White);

                //画图片的背景噪音线
                for (int i = 0; i < 25; i++)
                {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);

                    g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
                }

                LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), Color.Blue, Color.DarkRed, 1.2f, true);
                g.DrawString(checkCode, font, brush, 2, 2);

                //画图片的前景噪音点
                for (int i = 0; i < 100; i++)
                {
                    int x = random.Next(image.Width);
                    int y = random.Next(image.Height);

                    image.SetPixel(x, y, Color.FromArgb(random.Next()));
                }

                //画图片的边框线
                g.DrawRectangle(pen, 0, 0, image.Width - 1, image.Height - 1);

                image.Save(ms, ImageFormat.Gif);

                //输出图片流
                return ms.ToArray();
            }
            finally
            {
                ms.Dispose();
                pen.Dispose();
                font.Dispose();
                g.Dispose();
                image.Dispose();
            }
        }
    }
}
