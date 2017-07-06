using ERP.Models.CustomEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.Models.Common;

namespace ERP.Models.OutSourcing
{
    /// <summary>
    /// 采购管理->代印合同业务视图模型
    /// </summary>
    public class DTOOutsourcing : PendingApproveBasePage
    {

        /// <summary>
        /// 页面类型：1=新建；2=查看；3=编辑；4=审核；
        /// </summary>
        public int PageTypeID { get; set; }

        /// <summary>
        /// 页面标题：1=新建；2=查看；3=编辑；4=审核；
        /// </summary>
        public string PageTitle { get; set; }

        /// <summary>
        /// 代印合同自编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 代印合同编号
        /// </summary>
        public string OutContracNo { get; set; }

        /// <summary>
        /// 审批流权限值
        /// </summary>
        public int? ApproverIndex { get; set; }

        /// <summary>
        /// 当前登录用户能否审批数据
        /// </summary>
        public bool IsCanAudit { get; set; }

        /// <summary>
        /// 联系人电话
        /// </summary>
        public string TelePhone { get; set; }

        /// <summary>
        /// 采购合同编号
        /// </summary>
        public int ContractsID { get; set; }

        /// <summary>
        /// 代印公司邮箱
        /// </summary>
        public string FactoryEmail { get; set; }

        /// <summary>
        /// 代印公司自编号
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// 代印公司ID
        /// </summary>
        public int OutCompanyID { get; set; }

        /// <summary>
        /// 代印公司
        /// </summary>
        public string OutCompany { get; set; }

        /// <summary>
        /// 交货地名称
        /// </summary>
        public string DeliveryName { get; set; }

        /// <summary>
        /// 代印公司联系人
        /// </summary>
        public string CallPeoPle { get; set; }

        /// <summary>
        /// 交货地编号
        /// </summary>
        public int DeliveryID { get; set; }

        /// <summary>
        /// 其它费用
        /// </summary>
        public decimal OthersFee { get; set; }

        /// <summary>
        /// 代印合同金额
        /// </summary>
        public decimal OutContractSum { get; set; }

        /// <summary>
        /// 代印合同金额（前台显示用）
        /// </summary>
        public decimal OutContractSumText { get; set; }

        /// <summary>
        /// 合同条款及备注
        /// </summary>
        public string Clasue { get; set; }

        /// <summary>
        /// 标签要求
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 代印合同交货日期-开始
        /// </summary>
        public string DeliveryDateStart { get; set; }

        /// <summary>
        /// 代印合同交货日期-结束
        /// </summary>
        public string DeliveryDateEnd { get; set; }

        /// <summary>
        /// 合同附件
        /// </summary>
        public string AddFileName { get; set; }

        /// <summary>
        /// 代印合同状态ID
        /// </summary>
        public int OutContractStatusID { get; set; }

        /// <summary>
        /// 代印合同审核内容
        /// </summary>
        public string AuditIdea { get; set; }

        /// <summary>
        /// 代印合同状态
        /// </summary>
        public string OutContractStatus { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateDate { get; set; }

        /// <summary>
        /// 创建人id
        /// </summary>
        public int CreateUserID { get; set; }

        /// <summary>
        /// 创建末端
        /// </summary>
        public string CreateTerminal { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public string UpdateDate { get; set; }

        /// <summary>
        /// 更新人id
        /// </summary>
        public int UpdateUserID { get; set; }

        /// <summary>
        /// 更新末端
        /// </summary>
        public string UpdateTerminal { get; set; }

        /// <summary>
        /// 客户PO#
        /// </summary>
        public string CustomerPo { get; set; }

        /// <summary>
        /// 客户自编号
        /// </summary>
        public int OCID { get; set; }

        /// <summary>
        /// 客户代号
        /// </summary>
        public string CustomerCode { get; set; }

        /// <summary>
        /// 工厂简称
        /// </summary>
        public string FactoryAbbreviation { get; set; }

        /// <summary>
        /// 采购合同金额
        /// </summary>
        public decimal PurchaseAmount { get; set; }

        /// <summary>
        /// 采购合同日期日期——开始
        /// </summary>
        public string PurchaseDateStart { get; set; }

        /// <summary>
        /// 采购合同日期日期——结束
        /// </summary>
        public string PurchaseDateEnd { get; set; }

        /// <summary>
        /// 采购合同日期
        /// </summary>
        public string PurchaseDate { get; set; }

        /// <summary>
        /// 采购合同编号
        /// </summary>
        public string PurchaseNumber { get; set; }

        /// <summary>
        /// 代印合同->标签清单
        /// </summary>
        public List<DTOOCPacksData> OCPacksData { get; set; }

        /// <summary>
        /// 代印合同->审批履历VM
        /// </summary>
        public List<DTOOutContractHis> OCOutContractHis { get; set; }

        public string DT_MODIFYDATEFormatter { get; set; }

        public string CurrentSign { get; set; }

        public string list_ToEmailAddress { get; set; }

        public string list_CallName { get; set; }
        public string EmailSign { get; set; }
        public List<VMUpLoadFile> list_PacksUpload { get; set; }

        public PageTypeEnum PageType { get; set; }
    }

    public class VMFilterOC : IndexPageBaseModel
    {
        /// <summary>
        /// 页面区分
        /// </summary>
        public PageTypeEnum PageType { get; set; }

        /// <summary>
        /// 代印合同编号
        /// </summary>
        public string OutContracNo { get; set; }

        /// <summary>
        /// 代印公司
        /// </summary>
        public string OutCompany { get; set; }

        /// <summary>
        /// 代印合同交货日期-开始
        /// </summary>
        public string DeliveryDateStart { get; set; }

        /// <summary>
        /// 代印合同交货日期-结束
        /// </summary>
        public string DeliveryDateEnd { get; set; }

        /// <summary>
        /// 代印合同状态ID
        /// </summary>
        public int OutContractStatusID { get; set; }

        /// <summary>
        /// 客户代号
        /// </summary>
        public string CustomerCode { get; set; }

        /// <summary>
        /// 工厂简称
        /// </summary>
        public string FactoryAbbreviation { get; set; }

        /// <summary>
        /// 采购合同日期——开始
        /// </summary>
        public string PurchaseDateStart { get; set; }

        /// <summary>
        /// 采购合同日期——结束
        /// </summary>
        public string PurchaseDateEnd { get; set; }

        /// <summary>
        /// 采购合同要求交货日期
        /// </summary>
        public string PurchaseDeliveryDate { get; set; }

        /// <summary>
        /// 采购合同编号
        /// </summary>
        public string PurchaseNumber { get; set; }
    }

    /// <summary>
    /// 代印合同审批履历VM
    /// </summary>
    public class DTOOutContractHis
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
        /// 代印合同状态ID
        /// </summary>
        public string OutContractStatus { get; set; }
    }

    /// <summary>
    /// 采购合同及其相关信息视图模型
    /// </summary>
    public class VMPCDataList
    {
        /// <summary>
        /// 代印合同自编号
        /// </summary>
        public int PCID { get; set; }

        /// <summary>
        /// 客户代号
        /// </summary>
        public string CustomerCode { get; set; }

        /// <summary>
        /// 工厂简称
        /// </summary>
        public string FactoryAbbreviation { get; set; }

        /// <summary>
        /// 采购合同金额
        /// </summary>
        public decimal PurchaseAmount { get; set; }

        /// <summary>
        /// 采购合同编号
        /// </summary>
        public string PurchaseNumber { get; set; }

        /// <summary>
        /// 采购合同日期
        /// </summary>
        public string PurchaseDate { get; set; }

        /// <summary>
        /// 产品标签种类
        /// </summary>
        public string TagName { get; set; }

        /// <summary>
        /// 标签品名及规格
        /// </summary>
        public string TagDescribe { get; set; }
    }

    /// <summary>
    /// 代印合同->标签清单
    /// </summary>
    public class DTOOCPacksData
    {
        /// <summary>
        /// 代印合同标签自编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 采购合同自编号
        /// </summary>
        public int ContractsID { get; set; }

        /// <summary>
        ///包装资料自编号
        /// </summary>
        public int PacksID { get; set; }

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
        /// 代印公司ID
        /// </summary>
        public int OutCompanyID { get; set; }

        /// <summary>
        /// 代印公司
        /// </summary>
        public string OutCompany { get; set; }

        /// <summary>
        /// 交货地编号
        /// </summary>
        public int DeliveryID { get; set; }

        /// <summary>
        /// 交货地名称
        /// </summary>
        public string DeliveryName { get; set; }

        /// <summary>
        /// 其它费用
        /// </summary>
        public decimal OthersFee { get; set; }

        /// <summary>
        /// 一条标签记录->所有产品的标签合计金额
        /// </summary>
        public decimal TagProductsAmount { get; set; }

        /// <summary>
        /// 代印合同标签备注
        /// </summary>
        public string PacksRemark { get; set; }

        /// <summary>
        /// 代印合同条款
        /// </summary>
        public string OCClasue { get; set; }

        /// <summary>
        /// 代印合同->标签->产品清单
        /// </summary>
        public List<DTOOCPacksProductData> OCPacksProducts { get; set; }
    }

    /// <summary>
    /// 代印合同->标签->产品清单
    /// </summary>
    public class DTOOCPacksProductData
    {
        /// <summary>
        /// 代印合同->标签->产品自编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 包装资料产品自编号
        /// </summary>
        public int PackProductsID { get; set; }

        /// <summary>
        /// 包装资料自编号
        /// </summary>
        public int PacksID { get; set; }

        /// <summary>
        /// 销售订单中产品自编号
        /// </summary>
        public int OrderProductID { get; set; }

        /// <summary>
        /// 产品信息所在产品管理主表自编号
        /// </summary>
        public int ProductID { get; set; }

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
        /// 产品图片
        /// </summary>
        public string ProductImage { get; set; }

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

        //private decimal _OrderProductFPrice = 0;
        ///// <summary>
        ///// 销售订单中产品的工厂价格
        ///// </summary>
        //public decimal OrderProductFPrice {
        //    get { return _OrderProductFPrice; }

        //    set {
        //        value = _OrderProductFPrice;
        //    }
        //}

        /// <summary>
        /// 产品标签种类
        /// </summary>
        public string TagName { get; set; }

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
        public string SalePrice { get; set; }

        /// <summary>
        /// 产品标签数量
        /// </summary>
        public int ProductTagsNumber { get; set; }

        //private int _ProductTagsNumber = 0;
        ///// <summary>
        ///// 产品标签数量
        ///// </summary>
        //public int ProductTagsNumber
        //{
        //    get
        //    {
        //        return _ProductTagsNumber;
        //    }
        //    set { value = _ProductTagsNumber; }
        //}
        /// <summary>
        /// 代印产品标签金额=单价×产品标签数量
        /// </summary>
        public decimal ProductTagsAmount { get; set; }

        //private decimal _ProductTagsAmount = 0;
        ///// <summary>
        ///// 代印产品标签金额=单价×产品标签数量
        ///// </summary>
        //public decimal ProductTagsAmount
        //{
        //    get
        //    {
        //        return _OrderProductFPrice*ProductTagsNumber;

        //    }

        //    set { value = _ProductTagsAmount; }
        //}

        public string ProductUPC { get; set; }
    }
}