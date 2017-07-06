using ERP.Models.CustomEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.OutSourcing
{
    public class VMoutsourContract : IndexPageBaseModel
    {
        public PageTypeEnum PageType { get; set; }

        /// <summary>
        /// 采购合同自编号
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 采购合同编号
        /// </summary>
        public string PurchaseNumber { get; set; }

        /// <summary>
        /// 客户PO#
        /// </summary>
        public string CustomerPo { get; set; }

        /// <summary>
        /// 客户代号
        /// </summary>
        public string CustomerCode { get; set; }

        /// <summary>
        /// 采购合同总金额
        /// </summary>
        public decimal PurchaseAmount { get; set; }

        /// <summary>
        /// 采购合同日期
        /// </summary>
        public string PurchaseDate { get; set; }

        /// <summary>
        /// 下单日期——开始
        /// </summary>
        public string PurchaseDateStart { get; set; }

        /// <summary>
        /// 下单日期——结束
        /// </summary>
        public string PurchaseDateEnd { get; set; }

        /// <summary>
        /// 代印合同编号
        /// </summary>
        public string OutContracNo { get; set; }

        /// <summary>
        /// 代印公司
        /// </summary>
        public string OutCompany { get; set; }

        /// <summary>
        /// 代印合同金额
        /// </summary>
        public string OutContractSum { get; set; }

        /// <summary>
        /// 代印合同状态
        /// </summary>
        public string OutContractStatus { get; set; }

        /// <summary>
        /// 工厂简称
        /// </summary>
        public string FactoryAbbreviation { get; set; }

        /// <summary>
        /// 要求交货期——开始
        /// </summary>
        public string DateStart { get; set; }

        /// <summary>
        /// 要求交货期——结束
        /// </summary>
        public string DateEnd { get; set; }

        /// <summary>
        /// 审核状态（代印合同）
        /// </summary>
        public string PurchaseStatus
        { get; set; }

        /// <summary>
        /// 代印合同备注
        /// </summary>
        public string AddFlieName
        { get; set; }

        public string Author { get; set; }

        public string CreateDate { get; set; }

        /// <summary>
        /// 客户id
        /// </summary>
        public int CustomerID { get; set; }
    }
}