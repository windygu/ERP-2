using Aspose.Pdf;
using ERP.Tools.Logs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web;

namespace ERP.Tools
{
    public class AsposeX
    {
        //public bool ExcelToJpg(string officeFile, int jpgWidth, int jpgHeight, string jpgFile, out string errMsg)
        //{
        //    //bool b = ExcelToPdf(officeFile, jpgFile + ".pdf", out errMsg);

        //    bool b = PdfToJpg(jpgFile + ".pdf", 1, jpgWidth, jpgHeight, jpgFile, out errMsg);
        //    return b;
        //}

        public bool ExcelToJpg(string officeFile, string jpgFile, out string errMsg)
        {
            //return ExcelToJpg(officeFile, -1, -1, jpgFile, out errMsg);
            bool b = PdfToJpg(officeFile, 1, -1, -1, jpgFile, out errMsg);
            return b;
        }

        public bool ExcelToJpg(string officeFile, string jpgFile, out string errMsg, int nPage)
        {
            //return ExcelToJpg(officeFile, -1, -1, jpgFile, out errMsg);
            bool b = PdfToJpg(officeFile, nPage, -1, -1, jpgFile, out errMsg);
            return b;
        }

        public bool ExcelToJpg(string officeFile, string jpgPath, string fileName, out List<string> imageFiles, out string errMsg)
        {
            //return ExcelToJpg(officeFile, -1, -1, jpgFile, out errMsg);
            bool b = PdfToJpg(officeFile, -1, -1, jpgPath, fileName, out imageFiles, out errMsg);
            return b;
        }

        //public bool ExcelToPdf2(string officeFile, string PdfFile, out string errMsg)
        //{
        //    errMsg = null;
        //    bool b = false;
        //    try
        //    {
        //        Application app = new Application();
        //        Workbook workbook = app.Workbooks.Open(officeFile);
        //        workbook.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF, PdfFile);
        //        workbook.Close();
        //        app.Quit();

        //        b = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        errMsg = ex.Message;// +officeFile + "," + PdfFile;
        //        LogHelper.WriteError(ex);
        //        b = false;
        //    }
        //    return b;
        //}


        public bool ExcelToPdf(string officeFile, string PdfFile, out string errMsg)
        {
            errMsg = null;
            bool b = false;
            try
            {
                Aspose.Cells.Workbook doc = new Aspose.Cells.Workbook(officeFile);
                doc.CalculateFormula();//计算列
                doc.Save(PdfFile, Aspose.Cells.SaveFormat.Pdf);
                b = true;
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;// +officeFile + "," + PdfFile;
                LogHelper.WriteError(ex);
                b = false;
            }
            finally
            {
                PendingFinalizers();
            }
            return b;
        }

        public bool PdfToJpg(string pdfFile, int nPage, string ImageFile, out string errMsg)
        {
            return PdfToJpg(pdfFile, nPage, -1, -1, ImageFile, out errMsg);
        }

        public bool PdfToJpg(string pdfFile, string ImageFile, out string errMsg)
        {
            return PdfToJpg(pdfFile, 1, -1, -1, ImageFile, out errMsg);
        }


        public bool PdfToJpg(string pdfFile, int nPage, int Width, int Height, string ImageFile, out string errMsg)
        {
            errMsg = null;
            bool b = false;
            if (nPage < 1) nPage = 1;
            try
            {
                Aspose.Pdf.Document doc = new Aspose.Pdf.Document(pdfFile);
                if (nPage > doc.Pages.Count) nPage = doc.Pages.Count;

                Aspose.Pdf.Devices.JpegDevice j = new Aspose.Pdf.Devices.JpegDevice(80);

                using (Stream stream = new MemoryStream())
                {
                    j.Process(doc.Pages[nPage], stream);
                    using (System.Drawing.Image image = Bitmap.FromStream(stream)) // 原始图
                    {
                        if (Width > 0 && Height > 0)
                        {
                            using (Bitmap image2 = new Bitmap(image, Width, Height))
                            {
                                image2.Save(ImageFile);
                            }
                        }
                        else
                            image.Save(ImageFile);
                    }
                }
                b = true;
            }
            catch (Exception te)
            {
                errMsg = te.Message;
                b = false;
            }
            finally
            {
                PendingFinalizers();
            }
            return b;
        }

        public static void PendingFinalizers()
        {
            PendingFinalizers(2);
        }

        public static void PendingFinalizers(int n)
        {
            for (int i = 0; i < n; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        public void MakeWordFile(List<string> fileList, List<string> physicalPathList, string tempAsolutePath, string sTargetFilePath, string sNewFileName, List<string> list_ShippingMark = null)
        {
            //生成新文件的绝对路径
            string sNewFileAsolutePath = HttpContext.Current.Server.MapPath("~" + sTargetFilePath);

            if (!Directory.Exists(sNewFileAsolutePath))
            {
                Directory.CreateDirectory(sNewFileAsolutePath);
            }
            sNewFileAsolutePath += sNewFileName;//

            tempAsolutePath = HttpContext.Current.Server.MapPath("~" + tempAsolutePath);

            Aspose.Words.Document doc = new Aspose.Words.Document(tempAsolutePath);

            try
            {
                int iIndex = 0, iCount = fileList.Count;
                double dImageProportion = 0;
                string[] arrcList = physicalPathList.ToArray();
                string sFilePhysicalPath = string.Empty;
                Aspose.Words.DocumentBuilder docBuilder = new Aspose.Words.DocumentBuilder(doc);
                Aspose.Words.Drawing.Shape shape;
                double docBuilderPageWidth = docBuilder.PageSetup.PageWidth;
                double docBuilderPageHeight = docBuilder.PageSetup.PageHeight;
                docBuilderPageWidth = 430;//减去页边距后的值
                docBuilderPageHeight = 650;//减去页边距后的值

                Aspose.Pdf.Image image = new Aspose.Pdf.Image();
                System.Drawing.Image drawingImage;
                double dImageWidth = 0, dImageHeight = 0;

                foreach (string item1 in fileList)
                {
                    sFilePhysicalPath = arrcList[iIndex];
                    drawingImage = System.Drawing.Image.FromFile(sFilePhysicalPath);

                    dImageWidth = (double)drawingImage.Width;
                    dImageHeight = (double)drawingImage.Height;
                    dImageProportion = Math.Round(dImageWidth / dImageHeight, 2);

                    //图片实际宽度/高度大于生成PDF文件宽度/高度时，按图片等比例缩小，算出插入图片的高度/宽度
                    if (dImageWidth > docBuilderPageWidth)
                    {
                        dImageWidth = docBuilderPageWidth;
                        dImageHeight = dImageWidth / dImageProportion;
                    }
                    if (dImageHeight > docBuilderPageHeight)
                    {
                        dImageHeight = docBuilderPageHeight;
                        dImageWidth = dImageHeight * dImageProportion;
                    }

                    shape = new Aspose.Words.Drawing.Shape(doc, Aspose.Words.Drawing.ShapeType.Image);
                    shape.Width = Math.Round(dImageWidth, 1);
                    shape.Height = Math.Round(dImageHeight, 1);

                    bool b = true;
                    if (list_ShippingMark != null && list_ShippingMark.Count > 0 && list_ShippingMark[iIndex] != null)
                    {
                        if (list_ShippingMark[iIndex].Contains("唛头的介绍"))
                        {
                            b = false;
                        }
                    }

                    if (!b && iIndex > 0)
                    {
                        iIndex++;
                        continue;
                    }

                    shape.ImageData.SetImage(item1);

                    if (list_ShippingMark != null && list_ShippingMark.Count > 0 && list_ShippingMark[iIndex] != null)
                    {
                        if (b)
                        {
                            docBuilder.Writeln(list_ShippingMark[iIndex]);
                        }
                    }

                    docBuilder.InsertNode(shape);

                    iIndex++;

                    if (iIndex < iCount)
                    {
                        docBuilder.InsertBreak(Aspose.Words.BreakType.PageBreak);
                    }

                    //builder.InsertParagraph();
                    //shape.HorizontalAlignment = HorizontalAlignment.Right; //靠右对齐

                }

                doc.Save(sNewFileAsolutePath, Aspose.Words.SaveFormat.Pdf);
            }
            catch (Exception e)
            {
                LogHelper.WriteError(e);
            }
            finally
            {
                doc.Clone();
            }
        }

        public static bool CombineMultiplePDFs(List<string> fileNames, string outFile)
        {
            try
            {
                using (iTextSharp.text.Document document = new iTextSharp.text.Document())
                using (iTextSharp.text.pdf.PdfCopy writer = new iTextSharp.text.pdf.PdfCopy(document, new FileStream(outFile, FileMode.Create)))
                {
                    if (writer == null)
                    {
                        return false;
                    }

                    document.Open();

                    foreach (string fileName in fileNames)
                    {
                        using (iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(fileName))
                        {
                            iTextSharp.text.pdf.PdfReader.unethicalreading = true;
                            writer.AddDocument(reader);
                        }
                    }

                    document.Close();

                    return true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return false;
        }

        //public bool DocToJpg(string officeFile, int nPage, int jpgWidth, int jpgHeight, out string errMsg)
        //{
        //    string jpgFile = CacheX.LocalJpgCacheFile(officeFile, nPage, jpgWidth, jpgWidth, "ss");
        //    return DocToJpg(officeFile, nPage, jpgWidth, jpgHeight, jpgFile, out errMsg);
        //}

        //public bool DocToJpg(string officeFile, int nPage, int jpgWidth, int jpgHeight, string jpgFile, out string errMsg)
        //{
        //    if (IsX.Word(officeFile))
        //        return WordToJpg(officeFile, nPage, jpgWidth, jpgHeight, jpgFile, out errMsg);
        //    else
        //        if (IsX.Pdf(officeFile))
        //        return PdfToJpg(officeFile, nPage, jpgWidth, jpgHeight, jpgFile, out errMsg);
        //    else
        //            if (IsX.PPT(officeFile))
        //        return PptToJpg(officeFile, nPage, jpgWidth, jpgHeight, jpgFile, out errMsg);
        //    else
        //                if (IsX.Excel(officeFile))
        //        return ExcelToJpg(officeFile, jpgWidth, jpgHeight, jpgFile, out errMsg);
        //    else
        //    {
        //        errMsg = "不支持的文件格式";
        //        return false;
        //    }
        //}

        //public bool PptToJpg(string officeFile, int nPage, int jpgWidth, int jpgHeight, string jpgFile, out string errMsg)
        //{
        //    errMsg = null;
        //    bool b = false;
        //    try
        //    {
        //        Aspose.Slides.Presentation doc = new Aspose.Slides.Presentation(officeFile);
        //        if (jpgWidth >= 1 && jpgHeight >= 1)
        //            doc.Slides[nPage].GetThumbnail(new Size(jpgWidth, jpgHeight)).Save(jpgFile, System.Drawing.Imaging.ImageFormat.Jpeg);
        //        else
        //            doc.Slides[nPage].GetThumbnail(1.0f, 1.0f).Save(jpgFile, System.Drawing.Imaging.ImageFormat.Jpeg);
        //        b = true;
        //        errMsg = null;
        //    }
        //    catch (Exception te)
        //    {
        //        errMsg = te.Message;
        //        b = false;
        //    }
        //    finally
        //    {
        //        ShellX.PendingFinalizers();

        //    }
        //    return b;
        //}

        //public bool PptToJpg(string officeFile, int nPage, string jpgFile, out string errMsg)
        //{
        //    return PptToJpg(officeFile, nPage, 0, 0, jpgFile, out errMsg);
        //}

        //public bool PptToJpg(string officeFile, string jpgFile, out string errMsg)
        //{
        //    return PptToJpg(officeFile, 1, 0, 0, jpgFile, out errMsg);
        //}

        //public bool PptToPdf(string officeFile, string PdfFile, out string errMsg)
        //{
        //    return PptToPdf(officeFile, 1, 0, PdfFile, out errMsg);
        //}

        //public bool PptToPdf(string officeFile, int nFromPage, int PageCount, string PdfFile, out string errMsg)
        //{
        //    errMsg = null;
        //    bool b = false;
        //    if (nFromPage < 1) nFromPage = 1;
        //    try
        //    {
        //        Aspose.Slides.Presentation doc = new Aspose.Slides.Presentation(officeFile);
        //        int[] slides;

        //        if (nFromPage > doc.Slides.Count) nFromPage = doc.Slides.Count;

        //        if (PageCount <= 0) PageCount = doc.Slides.Count - nFromPage + 1;

        //        if (PageCount > 1)
        //        {
        //            slides = new int[PageCount];
        //            for (int i = nFromPage; i < PageCount + nFromPage; i++)
        //                slides[i - nFromPage] = i;
        //        }
        //        else
        //        {
        //            slides = new int[1];
        //            slides[0] = nFromPage;
        //        }
        //        //Aspose.Slides.Export.SaveOptions so=new  Aspose.Slides.Export.SaveOptions();
        //        //doc.Save(PdfFile,Aspose.Slides.Export.SaveFormat.Pdf,so.
        //        // doc.SaveToPdf(PdfFile, slides);
        //        doc.Save(PdfFile, Aspose.Slides.Export.SaveFormat.Pdf);
        //        b = true;
        //    }
        //    catch (Exception te)
        //    {
        //        errMsg = te.Message;
        //        b = false;
        //    }
        //    finally
        //    {
        //        ShellX.PendingFinalizers();

        //    }
        //    return b;
        //}

        //public bool PdfToDoc(string pdfFile, string docFile, out string errMsg)
        //{
        //    errMsg = null;
        //    bool b = false;
        //    try
        //    {
        //        Aspose.Pdf.Document doc = new Aspose.Pdf.Document(pdfFile);
        //        Aspose.Pdf.SaveOptions iso = null;
        //        string fileExt = (GetX.FileExt(docFile) ?? "").ToLower();
        //        switch (fileExt)
        //        {
        //            case ".xps":
        //                iso = new Aspose.Pdf.XpsSaveOptions();

        //                break;
        //            case ".htm":
        //            case ".html":
        //                iso = new Aspose.Pdf.HtmlSaveOptions();
        //                break;
        //            case ".xml":
        //                iso = new Aspose.Pdf.XmlSaveOptions();
        //                break;
        //            case ".tex":
        //                iso = new Aspose.Pdf.LaTeXSaveOptions();
        //                break;
        //            default:
        //                iso = new Aspose.Pdf.DocSaveOptions();
        //                break;
        //        }
        //        doc.Save(docFile, iso);
        //        b = true;
        //    }
        //    catch (Exception te)
        //    {
        //        errMsg = te.Message;
        //        b = false;
        //    }
        //    finally
        //    {
        //        ShellX.PendingFinalizers();

        //    }
        //    return b;
        //}

        //public bool WordToJpg(string wordFile, int Width, int Height, string ImageFile, out string errMsg)
        //{
        //    return WordToJpg(wordFile, 1, Width, Height, ImageFile, out errMsg);
        //}

        //public bool WordToJpg(string wordFile, int nPage, string ImageFile, out string errMsg)
        //{
        //    return WordToJpg(wordFile, nPage, 0, 0, ImageFile, out errMsg);
        //}

        //public bool WordToJpg(string wordFile, string ImageFile, out string errMsg)
        //{
        //    return WordToJpg(wordFile, 1, 0, 0, ImageFile, out errMsg);
        //}

        //public bool WordToJpg(string wordFile, int nPage, int Width, int Height, string ImageFile, out string errMsg)
        //{
        //    errMsg = null;
        //    bool b = false;
        //    if (nPage < 1) nPage = 1;
        //    try
        //    {
        //        Aspose.Words.Document doc = new Aspose.Words.Document(wordFile);
        //        if (nPage > doc.PageCount) nPage = doc.PageCount;
        //        Aspose.Words.Saving.ImageSaveOptions iso = new Aspose.Words.Saving.ImageSaveOptions(Aspose.Words.SaveFormat.Jpeg);
        //        iso.JpegQuality = 80;
        //        iso.PrettyFormat = true;
        //        iso.UseAntiAliasing = true;
        //        iso.PageIndex = nPage - 1;
        //        iso.PageCount = 1;
        //        using (Stream stream = new MemoryStream())
        //        {
        //            doc.Save(stream, iso);
        //            using (System.Drawing.Image image = Bitmap.FromStream(stream)) // 原始图

        //            {
        //                if (Width > 0 && Height > 0)
        //                {
        //                    using (Bitmap image2 = new Bitmap(image, Width, Height))
        //                    {
        //                        image2.Save(ImageFile);
        //                    }
        //                }
        //                else
        //                    image.Save(ImageFile);
        //            }
        //        }
        //        b = true;
        //    }
        //    catch (Exception te)
        //    {
        //        errMsg = te.Message;
        //        b = false;
        //    }
        //    finally
        //    {
        //        ShellX.PendingFinalizers();

        //    }
        //    return b;
        //}

        public bool WordToPdf(string officeFile, string PdfFile, out string errMsg)
        {
            return WordToPdf(officeFile, 0, 0, PdfFile, out errMsg);

        }
        public bool WordToPdf(string officeFile, int nFromPage, int PageCount, string PdfFile, out string errMsg)
        {
            errMsg = null;
            bool b = false;
            Aspose.Words.Document doc = new Aspose.Words.Document(officeFile);
            try
            {
                Aspose.Words.Saving.PdfSaveOptions iso = new Aspose.Words.Saving.PdfSaveOptions();
                iso.JpegQuality = 80;
                if (nFromPage >= 0)
                    iso.PageIndex = nFromPage;
                if (PageCount >= 1)
                    iso.PageCount = PageCount;
                iso.PrettyFormat = true;
                iso.UseAntiAliasing = true;
                doc.Save(PdfFile, iso);
                b = true;
                errMsg = null;
            }
            catch (Exception te)
            {
                errMsg = te.Message;
                b = false;
            }
            finally
            {
                PendingFinalizers();
                doc.Clone();

            }
            return b;
        }

        //public bool WordToDoc(string officeFile, int nFromPage, int PageCount, string DocFile, out string errMsg)
        //{
        //    errMsg = null;
        //    bool b = false;
        //    try
        //    {
        //        Aspose.Words.Document doc = new Aspose.Words.Document(officeFile);
        //        string f = (GetX.FileExt(DocFile) ?? "").Trim();
        //        switch (f)
        //        {
        //            case ".swf":
        //                {
        //                    Aspose.Words.Saving.SwfSaveOptions
        //                       iso = new Aspose.Words.Saving.SwfSaveOptions();
        //                    iso.JpegQuality = 80;
        //                    if (nFromPage >= 0)
        //                        iso.PageIndex = nFromPage;
        //                    if (PageCount >= 1)
        //                        iso.PageCount = PageCount;
        //                    iso.PrettyFormat = true;
        //                    iso.UseAntiAliasing = true;
        //                    doc.Save(DocFile, iso);
        //                    break;
        //                }
        //            case ".svg":
        //                {
        //                    Aspose.Words.Saving.SvgSaveOptions
        //                       iso = new Aspose.Words.Saving.SvgSaveOptions();
        //                    iso.JpegQuality = 80;
        //                    if (nFromPage >= 0)
        //                        iso.PageIndex = nFromPage;
        //                    if (PageCount >= 1)
        //                        iso.PageCount = PageCount;
        //                    iso.PrettyFormat = true;
        //                    iso.UseAntiAliasing = true;
        //                    doc.Save(DocFile, iso);
        //                    break;
        //                }
        //            case ".xps":
        //                {
        //                    Aspose.Words.Saving.XpsSaveOptions
        //                       iso = new Aspose.Words.Saving.XpsSaveOptions();
        //                    if (nFromPage >= 0)
        //                        iso.PageIndex = nFromPage;
        //                    if (PageCount >= 1)
        //                        iso.PageCount = PageCount;
        //                    iso.PrettyFormat = true;
        //                    iso.UseAntiAliasing = true;
        //                    doc.Save(DocFile, iso);
        //                    break;
        //                }
        //            default:
        //                {
        //                    Aspose.Words.Saving.PdfSaveOptions iso = new Aspose.Words.Saving.PdfSaveOptions();
        //                    iso.JpegQuality = 80;
        //                    if (nFromPage >= 0)
        //                        iso.PageIndex = nFromPage;
        //                    if (PageCount >= 1)
        //                        iso.PageCount = PageCount;
        //                    iso.PrettyFormat = true;
        //                    iso.UseAntiAliasing = true;
        //                    doc.Save(DocFile, iso);
        //                }
        //                break;
        //        }
        //        b = true;
        //        errMsg = null;
        //    }
        //    catch (Exception te)
        //    {
        //        errMsg = te.Message;
        //        b = false;
        //    }

        //    finally
        //    {
        //        ShellX.PendingFinalizers();

        //    }
        //    return b;
        //}

        //public bool WordToSwf(string officeFile, string SwfFile, out string errMsg)
        //{
        //    return WordToSwf(officeFile, 0, 0, SwfFile, out errMsg);
        //}

        //public bool WordToSwf(string officeFile, int nFromPage, int PageCount, string SwfFile, out string errMsg)
        //{
        //    errMsg = null;
        //    bool b = false;
        //    try
        //    {
        //        Aspose.Words.Document doc = new Aspose.Words.Document(officeFile);
        //        Aspose.Words.Saving.SwfSaveOptions iso = new Aspose.Words.Saving.SwfSaveOptions();
        //        iso.JpegQuality = 60;
        //        iso.PrettyFormat = true;
        //        iso.EnableContextMenu = false;
        //        if (nFromPage >= 0)
        //            iso.PageIndex = nFromPage;
        //        if (PageCount >= 1)
        //            iso.PageCount = PageCount;
        //        iso.AllowReadMode = true;
        //        //iso.BookmarksOutlineLevel = 0;
        //        iso.UseAntiAliasing = true;
        //        //iso.HeadingsOutlineLevels = 3;
        //        //iso.ExpandedOutlineLevels = 2;
        //        iso.ViewerIncluded = false;
        //        doc.Save(SwfFile, iso);
        //        b = true;
        //    }
        //    catch (Exception te)
        //    {
        //        errMsg = te.Message;
        //        b = false;
        //    }

        //    finally
        //    {
        //        ShellX.PendingFinalizers();

        //    }
        //    return b;
        //}

        public bool PdfToJpg(string pdfFile, int Width, int Height, string imagePath, string fileName, out List<string> imageFiles, out string errMsg)
        {
            errMsg = null;
            imageFiles = new List<string>();
            bool b = false;
            try
            {
                Aspose.Pdf.Document doc = new Aspose.Pdf.Document(pdfFile);
                var nPage = doc.Pages.Count;

                Aspose.Pdf.Devices.JpegDevice j = new Aspose.Pdf.Devices.JpegDevice(80);

                for (var i = 1; i <= nPage; i++)
                {
                    var imageFileName = Path.Combine(imagePath, fileName + "_" + i + ".jpg");
                    using (Stream stream = new MemoryStream())
                    {
                        j.Process(doc.Pages[i], stream);

                        using (System.Drawing.Image image = Bitmap.FromStream(stream)) // 原始图
                        {
                            if (Width > 0 && Height > 0)
                            {
                                using (Bitmap image2 = new Bitmap(image, Width, Height))
                                {
                                    image2.Save(imageFileName);
                                }
                            }
                            else
                            {
                                image.Save(imageFileName);
                            }

                            imageFiles.Add(imageFileName);
                        }
                    }
                }
                b = true;
            }
            catch (Exception te)
            {
                errMsg = te.Message;
                b = false;
            }
            finally
            {
                PendingFinalizers();
            }
            return b;
        }

    }
}