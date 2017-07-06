using ERP.Models.Common;
using ERP.Models.CustomEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Packs
{
    public class VMPacks : PendingApproveBasePage
    {
        /// <summary>
        /// 页面类型：1=查看；2=编辑；3=审核；4=；
        /// </summary>
        public int PageTypeID { get; set; }
        
        /// <summary>
        /// 包装资料自编号
        /// </summary>
        public int PacksID { get; set; }

        /// <summary>
        /// 创建人自编号
        /// </summary>
        public int CreateUserID { get; set; }

        /// <summary>
        /// 审批流权限值
        /// </summary>
        public int? ApproverIndex { get; set; }

        /// <summary>
        /// 当前登录用户能否审批数据
        /// </summary>
        public bool IsCanAudit { get; set; }

        /// <summary>
        /// 采购合同自编号
        /// </summary>
        public int PurchaseContractID { get; set; }

        /// <summary>
        /// 采购合同编号
        /// </summary>
        public string PurchaseNumber { get; set; }

        /// <summary>
        /// 工厂简称
        /// </summary>
        public string FactoryAbbreviation { get; set; }

        /// <summary>
        /// 客户代号
        /// </summary>
        public string CustomerCode { get; set; }

        /// <summary>
        /// 采购合同日期
        /// </summary>
        public string PurchaseDate { get; set; }

        /// <summary>
        /// 采购合同日期——开始
        /// </summary>
        public string PurchaseDateStart { get; set; }

        /// <summary>
        /// 采购合同日期——结束
        /// </summary>
        public string PurchaseDateEnd { get; set; }

        /// <summary>
        /// 开始交货日期
        /// </summary>
        public string DateStart { get; set; }

        /// <summary>
        /// 结束交货日期
        /// </summary>
        public string DateEnd { get; set; }

        /// <summary>
        /// 采购合同总金额
        /// </summary>
        public decimal AllAmount { get; set; }

        /// <summary>
        /// 包装资料——数据状态
        /// </summary>
        public int PacksStatusID { get; set; }

        /// <summary>
        /// 包装资料——数据状态
        /// </summary>
        public string PacksStatus { get; set; }

        /// <summary>
        /// 包装资料所有名称
        /// </summary>
        public string PacksName { get; set; }

        /// <summary>
        /// 是否代印
        /// </summary>
        public string IsOutsourcing { get; set; }

        /// <summary>
        /// 审批意见
        /// </summary>
        public string AuditPacksIdea { get; set; }

        /// <summary>
        /// 采购合同->包装资料审批履历列表
        /// </summary>
        public List<DTOAuditPacksHis> AuditPacksHisList { get; set; }

        /// <summary>
        /// 采购合同所含包装资料
        /// </summary>
        public List<DTOPacks> PacksList { get; set; }

        public List<DTOPHProductsUPC> PHProductsUPC { get; set; }

        public List<DTOProductPacksData> PacksProducts { get; set; }

        public string PacksUpdateDateFormatter { get; set; }
        public int CustomerID { get; set; }
        public PageTypeEnum PageType { get; set; }
    }

    public class VMFilterPacks : IndexPageBaseModel
    {
        /// <summary>
        /// 页面区分
        /// </summary>
        public PageTypeEnum PageType { get; set; }

        /// <summary>
        /// 采购合同编号
        /// </summary>
        public string PurchaseNumber { get; set; }

        /// <summary>
        /// 工厂简称
        /// </summary>
        public string FactoryAbbreviation { get; set; }

        /// <summary>
        /// 客户代号
        /// </summary>
        public string CustomerCode { get; set; }

        /// <summary>
        /// 采购合同日期
        /// </summary>
        public string PurchaseDate { get; set; }

        /// <summary>
        /// 采购合同日期——开始
        /// </summary>
        public string PurchaseDateStart { get; set; }

        /// <summary>
        /// 采购合同日期——结束
        /// </summary>
        public string PurchaseDateEnd { get; set; }

        /// <summary>
        /// 交货日期——开始
        /// </summary>
        public string DateStart { get; set; }

        /// <summary>
        /// 交货日期——结束
        /// </summary>
        public string DateEnd { get; set; }

        /// <summary>
        /// 包装资料状态
        /// </summary>
        public int PacksStatus { get; set; }

        /// <summary>
        /// 包装资料所有名称
        /// </summary>
        public string PacksName { get; set; }

        /// <summary>
        /// 是否代印
        /// </summary>
        public string IsOutsourcing { get; set; }
    }

    /// <summary>
    /// 采购合同->包装资料审批履历VM
    /// </summary>
    public class DTOAuditPacksHis
    {
        /// <summary>
        /// 审批人
        /// </summary>
        public string AuditUserName { get; set; }

        /// <summary>
        /// 审批意见
        /// </summary>
        public string AuditPacksIdea { get; set; }

        /// <summary>
        /// 审批人
        /// </summary>
        public string AuditCreateDate { get; set; }

        /// <summary>
        /// 包装资料状态
        /// </summary>
        public string PacksStatus { get; set; }
    }

    /// <summary>
    /// 采购合同所含包装资料
    /// </summary>
    public class DTOPacks
    {
        /// <summary>
        /// 包装资料自编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 采购合同自编号
        /// </summary>
        public int ContractsID { get; set; }

        /// <summary>
        ///标签种类ID
        /// </summary>
        public int TagID { get; set; }

        /// <summary>
        ///标签种类名称
        /// </summary>
        public string TagName { get; set; }

        /// <summary>
        /// 标签品名及规格
        /// </summary>
        public string TagDescribe { get; set; }

        /// <summary>
        /// 标签样张，上传
        /// </summary>
        public string TagSample { get; set; }

        /// <summary>
        /// 是否代印
        /// </summary>
        public bool IsOutsourcing { get; set; }

        /// <summary>
        /// 包装资料标签适用的产品控件的ID
        /// </summary>
        public string OrderProductID { get; set; }

        /// <summary>
        /// 包装资料标签适用的产品控件的name
        /// </summary>
        public string OrderProductName { get; set; }

        /// <summary>
        /// 包装资料标签备注
        /// </summary>
        public string PacksRemark { get; set; }

        /// <summary>
        /// 包装资料标签适用的产品
        /// </summary>
        public List<int> PackProducts { get; set; }

        /// <summary>
        /// 上传样张所在标签行索引，前台自增长
        /// </summary>
        public int TagRowUploadIndex { get; set; }

        /// <summary>
        /// 上传样张所在标签行数，前台自增长
        /// </summary>
        public int TagRowUploadCount { get; set; }

        /// <summary>
        /// 包装资料标签中的上传附件
        /// </summary>
        public List<VMUpLoadFile> UpLoadFileList { get; set; }
        public string DisplayFileName { get; set; }
        public string ServerFileName { get; set; }
    }

    /// <summary>
    /// 包装资料标签适用的产品
    /// </summary>
    public class DTOPackProducts
    {
        /// <summary>
        /// 包装资料产品自编号
        /// </summary>
        public int PackProductsID { get; set; }

        /// <summary>
        /// 包装资料自编号
        /// </summary>
        public int PacksID { get; set; }

        /// <summary>
        /// 产品信息所在产品管理主表自编号
        /// </summary>
        public int ProductID { get; set; }

        /// <summary>
        /// 产品图片
        /// </summary>
        public string ProductImage { get; set; }

        /// <summary>
        /// 销售订单中产品自编号
        /// </summary>
        public int OrderProductID { get; set; }

        /// <summary>
        /// 销售订单中客户自编号
        /// </summary>
        public int OrderCustomerID { get; set; }

        /// <summary>
        /// 销售订单中工厂自编号
        /// </summary>
        public int OrderFactoryID { get; set; }

        /// <summary>
        /// 销售订单中产品货号
        /// </summary>
        public string OrderProductNO { get; set; }

        /// <summary>
        /// 销售订单中产品SKU#
        /// </summary>
        public string SkuNumber { get; set; }

        /// <summary>
        /// 销售订单中产品名称
        /// </summary>
        public string OrderProductName { get; set; }

        /// <summary>
        /// 销售订单中产品的工厂价格
        /// </summary>
        public decimal OrderProductFPrice { get; set; }

        /// <summary>
        /// 销售订单中产品的包装方式中文名称
        /// </summary>
        public string OrderProductPackingZH { get; set; }
    }

    /// <summary>
    /// 产品包装资料清单
    /// </summary>
    public class DTOProductPacksData
    {
        /// <summary>
        /// 包装资料产品自编号
        /// </summary>
        public int PackProductsID { get; set; }

        /// <summary>
        /// 包装资料自编号
        /// </summary>
        public int PacksID { get; set; }

        /// <summary>
        /// 是否代印
        /// </summary>
        public string IsOutsourcing { get; set; }

        /// <summary>
        /// 标签标记
        /// </summary>
        public string TagFlag { get; set; }

        /// <summary>
        /// 产品信息所在产品管理主表自编号
        /// </summary>
        public int ProductID { get; set; }

        /// <summary>
        /// 产品图片
        /// </summary>
        public string ProductImage { get; set; }

        /// <summary>
        /// 销售订单中产品自编号
        /// </summary>
        public int OrderProductID { get; set; }

        /// <summary>
        /// 销售订单中客户自编号
        /// </summary>
        public int OrderCustomerID { get; set; }

        /// <summary>
        /// 销售订单中工厂自编号
        /// </summary>
        public int OrderFactoryID { get; set; }

        /// <summary>
        /// 销售订单中产品货号
        /// </summary>
        public string OrderProductNO { get; set; }

        /// <summary>
        /// 销售订单中产品SKU#
        /// </summary>
        public string SkuNumber { get; set; }

        /// <summary>
        /// 销售订单中产品名称
        /// </summary>
        public string OrderProductName { get; set; }

        /// <summary>
        /// 销售订单中产品的工厂价格
        /// </summary>
        public decimal OrderProductFPrice { get; set; }

        /// <summary>
        /// 产品标签种类
        /// </summary>
        public string TagName { get; set; }

        /// <summary>
        /// 包装资料上传样张的下载路径
        /// </summary>
        public string DownloadFilePath { get; set; }

        /// <summary>
        /// 标签品名及规格
        /// </summary>
        public string TagDescribe { get; set; }

        /// <summary>
        /// 产品描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 产品零售价
        /// </summary>
        public decimal SalePrice { get; set; }

        /// <summary>
        /// 产品UPC
        /// </summary>
        public string ProductUPC { get; set; }

        /// <summary>
        /// 内盒率
        /// </summary>
        public int InnerBoxRate { get; set; }

        /// <summary>
        /// 内盒UPC
        /// </summary>
        public string InnerUPC { get; set; }

        /// <summary>
        /// 外箱率
        /// </summary>
        public int OuterBoxRate { get; set; }

        /// <summary>
        /// 外箱UPC
        /// </summary>
        public string OuterUPC { get; set; }

        /// <summary>
        /// PDQ装率
        /// </summary>
        public int PDQPackRate { get; set; }

        public int? DataFlag { get; set; }

        /// <summary>
        /// 产品标签数量
        /// </summary>
        public int ProductTagsNumber { get; set; }

        /// <summary>
        /// 内盒标签数量=产品标签数量/内盒率
        /// </summary>
        public int InnerTagsNumber { get; set; }

        /// <summary>
        /// 外箱标签数量=产品标签数量/外箱率
        /// </summary>
        public int OutTagsNumber { get; set; }

        //private int _InnerTagsNumber = 0;

        ///// <summary>
        ///// 内盒标签数量=产品标签数量/内盒率
        ///// </summary>
        //public int InnerTagsNumber
        //{
        //    get
        //    {
        //        int iValue = 0;
        //        if (_InnerTagsNumber == 0)
        //        {
        //            if (InnerBoxRate > 0)
        //            {
        //                iValue = (int)Math.Ceiling((double)ProductTagsNumber / (double)InnerBoxRate);
        //            }
        //        }
        //        else
        //        {
        //            iValue = _InnerTagsNumber;
        //        }

        //        return iValue;
        //    }

        //    set { _InnerTagsNumber = value; }
        //}

        //private int _OutTagsNumber = 0;

        ///// <summary>
        ///// 外箱标签数量=产品标签数量/外箱率
        ///// </summary>
        //public int OutTagsNumber
        //{
        //    get
        //    {
        //        int iValue = 0;
        //        if (_OutTagsNumber == 0)
        //        {
        //            if (OuterBoxRate > 0)
        //            {
        //                iValue = (int)Math.Ceiling((double)ProductTagsNumber / (double)OuterBoxRate);
        //            }
        //        }
        //        else
        //        {
        //            iValue = _OutTagsNumber;
        //        }

        //        return iValue;
        //    }

        //    set { _OutTagsNumber = value; }
        //}
    }

    /// <summary>
    /// 采购合同对应的产品信息，从销售订单OrderProduct中取得产品UPC、内盒UPC、外箱UPC作为默认值，以便在编辑包装资料页面中可以编辑
    /// </summary>
    public class DTOPHProductsUPC
    {
        /// <summary>
        /// 包装资料产品UPC自编号
        /// </summary>
        public int PPPUPCID { get; set; }

        /// <summary>
        /// 采购合同自编号
        /// </summary>
        public int ContractsID { get; set; }

        /// <summary>
        /// 产品信息所在产品管理主表自编号
        /// </summary>
        public int ProductID { get; set; }

        /// <summary>
        /// 产品图片
        /// </summary>
        public string ProductImage { get; set; }

        /// <summary>
        /// 销售订单中产品自编号
        /// </summary>
        public int OrderProductID { get; set; }

        /// <summary>
        /// 销售订单中产品货号
        /// </summary>
        public string OrderProductNO { get; set; }

        /// <summary>
        /// 销售订单中产品品名
        /// </summary>
        public string OrderProductName { get; set; }

        /// <summary>
        /// 销售订单中产品SKU#
        /// </summary>
        public string SkuNumber { get; set; }

        /// <summary>
        /// 产品UPC
        /// </summary>
        public string ProductUPC { get; set; }

        /// <summary>
        /// 内盒UPC
        /// </summary>
        public string InnerUPC { get; set; }

        /// <summary>
        /// 外箱UPC
        /// </summary>
        public string OuterUPC { get; set; }
    }
}