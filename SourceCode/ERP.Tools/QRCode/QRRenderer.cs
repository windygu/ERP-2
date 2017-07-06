﻿using Gma.QrCodeNet.Encoding;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Tools.QRCode
{
    public class QRRenderer
    {
        private readonly int m_ModuleSize;
        private readonly Brush m_DarkBrush;
        private readonly Brush m_LightBrush;
        private int m_Padding;

        public const int QuietZoneModules = 2;

        public QRRenderer(int moduleSize)
            : this(moduleSize, Brushes.Black, Brushes.White)
        {
        }

        public QRRenderer(int moduleSize, Brush darkBrush, Brush lightBrush)
        {
            m_ModuleSize = moduleSize;
            m_DarkBrush = darkBrush;
            m_LightBrush = lightBrush;
        }

        public void Draw(Graphics graphics, BitMatrix matrix)
        {
            this.Draw(graphics, matrix, new Point(0, 0));
        }

        public void Draw(Graphics graphics, BitMatrix matrix, Point offset)
        {
            DrawQuietZone(graphics, matrix.Width, offset);
            Size paddingOffset = new Size(m_Padding, m_Padding) + new Size(offset.X, offset.Y);
            Size moduleSize = new Size(m_ModuleSize, m_ModuleSize);

            for (int j = 0; j < matrix.Width; j++)
            {
                for (int i = 0; i < matrix.Width; i++)
                {
                    Point moduleRelativePosition = new Point(i * m_ModuleSize, j * m_ModuleSize);
                    Rectangle moduleAbsoluteArea = new Rectangle(moduleRelativePosition + paddingOffset, moduleSize);
                    Brush bush = matrix[i, j] ? m_DarkBrush : m_LightBrush;
                    graphics.FillRectangle(bush, moduleAbsoluteArea);
                }
            }
        }

        private void DrawQuietZone(Graphics graphics, int matrixWidth, Point offset)
        {
            int barLength = m_ModuleSize * (matrixWidth + 2);
            graphics.FillRectangle(m_LightBrush, offset.X, offset.Y, barLength, m_Padding);
            graphics.FillRectangle(m_LightBrush, barLength + offset.X, offset.Y, m_Padding, barLength);
            graphics.FillRectangle(m_LightBrush, m_Padding + offset.X, barLength + offset.Y, barLength, m_Padding);
            graphics.FillRectangle(m_LightBrush, offset.X, m_Padding + offset.Y, m_Padding, barLength);
        }

        public void CreateImageFile(BitMatrix matrix, string fileName, ImageFormat imageFormat)
        {
            Size size = Measure(matrix.Width);
            using (Bitmap bitmap = new Bitmap(size.Width, size.Height))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                Draw(graphics, matrix);
                bitmap.Save(fileName, imageFormat);
            }
        }

        public MemoryStream CreateImageFile(BitMatrix matrix, ImageFormat imageFormat)
        {
            Size size = Measure(matrix.Width);
            using (Bitmap bitmap = new Bitmap(size.Width, size.Height))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                MemoryStream ms = new MemoryStream();
                Draw(graphics, matrix);
                bitmap.Save(ms, imageFormat);
                return ms;
            }
        }

        public Size Measure(int matrixWidth)
        {
            int areaWidth = m_ModuleSize * matrixWidth;
            m_Padding = QuietZoneModules * m_ModuleSize;
            int padding = m_Padding;
            int totalWidth = areaWidth + 2 * padding;
            return new Size(totalWidth + 1, totalWidth + 1);
        }
    }
}
