using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.Models.CustomEnums;

namespace ERP.Models.Encasement
{
    /// <summary>
    /// 出运明细信息视图模型
    /// </summary>
    public class DTOEncasement : PendingApproveBasePage
    {
        /// <summary>
        /// 页面类型：1=查看；2=编辑；3=审核；
        /// </summary>
        public int PageTypeID { get; set; }

        /// <summary>
        /// 页面标题：1=查看出运明细；2=编辑出运明细；3=审核出运明细；
        /// </summary>
        public string PageTitle { get; set; }

        /// <summary>
        /// 出运明细自编号
        /// </summary>
        public int EncasementID { get; set; }

        /// <summary>
        /// 采购合同自编号
        /// </summary>
        public int ContractID { get; set; }

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
        /// 客户PO
        /// </summary>
        public string CustomerPO { get; set; }

        /// <summary>
        /// 客户Shipping Window开始
        /// </summary>
        public string CustomerSWStart { get; set; }

        /// <summary>
        /// 客户Shipping Window结束
        /// </summary>
        public string CustomerSWEnd { get; set; }

        /// <summary>
        /// EHI PC#
        /// </summary>
        public string EHIPC { get; set; }
        
        /// <summary>
        /// 采购合同日期
        /// </summary>
        public string PurchaseDate { get; set; }

        /// <summary>
        /// 是否需要报检
        /// </summary>
        public string IsInspectionReceipt { get; set; }



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
        /// 带有货币符号的采购合同总金额
        /// </summary>
        public string ContractAmountSymbol { get; set; }

        /// <summary>
        /// 总箱数
        /// </summary>
        public int SumProductBoxNum { get; set; }

        /// <summary>
        /// 实际总体积
        /// </summary>
        public decimal ActualCUFT { get; set; }

        /// <summary>
        /// 采购合同——交货地
        /// </summary>
        public string PortName { get; set; }

        /// <summary>
        /// 出运明细数据状态
        /// </summary>
        public int EncasementStatusID { get; set; }

        /// <summary>
        /// 出运明细数据状态
        /// </summary>
        public string EncasementStatus { get; set; }

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
        /// 审批意见
        /// </summary>
        public string AuditIdea { get; set; }

        /// <summary>
        /// 采购合同->出运明细审批履历列表
        /// </summary>
        public List<DTOAuditHis> AuditHisList { get; set; }

        /// <summary>
        /// 采购合同所含产品出运信息
        /// </summary>
        public List<DTOEncasementProducts> EncasementProducts { get; set; }
        public List<DTOEncasementProducts> listProducts_Mixed { get; set; }
        public string EncasementUpdateDateFormatter { get; set; }
        public string SelectCustomer { get; set; }
    }

    /// <summary>
    /// 出运明细筛选条件
    /// </summary>
    public class VMFilterEncasement : IndexPageBaseModel
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
        /// 出运明细数据状态
        /// </summary>
        public int? EncasementStatus { get; set; }


    }

    /// <summary>
    /// 采购合同->出运明细审批履历VM
    /// </summary>
    public class DTOAuditHis
    {
        /// <summary>
        /// 审批人
        /// </summary>
        public string AuditUserName { get; set; }

        /// <summary>
        /// 审批意见
        /// </summary>
        public string AuditIdea { get; set; }

        /// <summary>
        /// 出运明细数据状态
        /// </summary>
        public string EncasementStatus { get; set; }

        /// <summary>
        /// 审批时间
        /// </summary>
        public string AuditCreateDate { get; set; }
    }

    /// <summary>
    /// 采购合同所含产品出运信息
    /// </summary>
    public class DTOEncasementProducts
    {
        /// <summary>
        /// 出运明细自编号
        /// </summary>
        public int EncasementID { get; set; }

        /// <summary>
        /// 出运明细产品自编号
        /// </summary>
        public int EncasementProductID { get; set; }

        /// <summary>
        /// 销售订单中产品自编号
        /// </summary>
        public int OrderProductID { get; set; }

        /// <summary>
        /// 产品信息所在产品管理主表自编号
        /// </summary>
        public int ProductID { get; set; }

        /// <summary>
        /// 产品图片
        /// </summary>
        public string ProductImage { get; set; }

        /// <summary>
        /// 销售订单中产品货号
        /// </summary>
        public string No { get; set; }

        /// <summary>
        /// 销售订单中产品SKU#
        /// </summary>
        public string SkuNumber { get; set; }

        /// <summary>
        /// 销售订单中产品名称
        /// </summary>
        public string OrderProductName { get; set; }

        /// <summary>
        /// 产品描述
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 销售订单中产品的工厂简称
        /// </summary>
        public string FactoryName { get; set; }

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

        /// <summary>
        /// 内盒率
        /// </summary>
        public int InnerBoxRate{ get; set; }

        /// <summary>
        /// 外箱率
        /// </summary>
        public int OuterBoxRate { get; set; }

        /// <summary>
        /// 产品数量
        /// </summary>
        public int Qty { get; set; }

        public int Qty2 { get; set; }


        private int _BoxQty = 0;
        /// <summary>
        /// 产品箱数=产品数量/外箱率
        /// </summary>
        public int BoxQty
        {
            get
            {
                double d = 0;
                if (OuterBoxRate > 0)
                {
                    d = ((double)Qty / (double)OuterBoxRate);

                }

                return (int)Math.Ceiling(d);

            }
            set { value = _BoxQty; }
        }

        /// <summary>
        /// 原外箱长(cm)
        /// </summary>
        public decimal BeforeProductOuterLength{ get; set; }

        /// <summary>
        /// 原外箱宽(cm)
        /// </summary>
        public decimal BeforeProductOuterWidth { get; set; }

        /// <summary>
        /// 原外箱高(cm)
        /// </summary>
        public decimal BeforeProductOuterHeight { get; set; }

        /// <summary>
        /// 外箱长(cm)
        /// </summary>
        public decimal OuterLength { get; set; }

        /// <summary>
        /// 外箱宽(cm)
        /// </summary>
        public decimal OuterWidth { get; set; }

        /// <summary>
        /// 外箱高(cm)
        /// </summary>
        public decimal OuterHeight { get; set; }
        
        public decimal _OuterVolume { get; set; }

        /// <summary>
        /// 原单箱材积(销售订单中产品的CU'FT，只读)
        /// </summary>
        public decimal OuterVolume
        {
            get { return _OuterVolume; }
            set { _OuterVolume = Math.Round(value, 2); }
        }


        /// <summary>
        /// 实际单箱材积=((外箱长×宽×高)/1000000) * 35.315，四舍五入，
        /// </summary>
        public decimal ActualVolume
        {
            get
            {
                decimal molecular = CalculateMolecular() * (decimal)35.315;

                return Math.Round(molecular, 2);
            }

        }

        /// <summary>
        /// 原总体积（m³）=((外箱长×宽×高)/1000000)×产品箱数，四舍五入
        /// </summary>
        public decimal CUFT { get; set; }
        //{
        //    get
        //    {
        //        decimal molecular = CalculateMolecular() * ProductBoxNum;

        //        return Math.Round(molecular, 2);
        //    }

        //}


        /// <summary>
        /// 实际总体积（m³）=((外箱长×宽×高)/1000000) × 产品箱数，四舍五入
        /// </summary>
        public decimal ActualCUFT
        {
            get
            {
                decimal molecular = CalculateMolecular() * BoxQty;

                return Math.Round(molecular, 2);
            }

        }


        private decimal CalculateMolecular(){
            decimal molecular = OuterLength * OuterWidth * OuterHeight;
            if (molecular > 0)
            {
                molecular = (molecular / 1000000);

            }
            return molecular;   
        }

        /// <summary>
        /// 单箱毛重(kg)
        /// </summary>
        public decimal OuterWeightGross{ get; set; }

        private decimal _WeightGrossSum=0;
        /// <summary>
        /// 总毛重=单箱毛重×产品箱数
        /// </summary>
        public decimal WeightGrossSum
        {
            get
            {
                return Math.Round(OuterWeightGross * BoxQty,2);
            }
            set { value = _WeightGrossSum; }
        }

        /// <summary>
        /// 单箱净重(kg)
        /// </summary>
        public decimal OuterWeightNet{ get; set; }

        private decimal _WeightNetSum=0;
        /// <summary>
        /// 总净重=单箱净重×产品箱数
        /// </summary>
        public decimal WeightNetSum
        {
            get
            {
                return Math.Round(OuterWeightNet * BoxQty,2);
            }
            set { value = _WeightNetSum; }
        }

        public bool IsProductMixed { get; set; }
        public int? ParentProductMixedID { get; set; }
    }


}
