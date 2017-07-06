using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.OutSourcing
{
    public class DTOoutsourContract
    {
        /// <summary>
        /// 代印自编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 包装资料编号
        /// </summary>
        public int PacksID { get; set; }

        /// <summary>
        /// 采购订单编号
        /// </summary>
        public int ContractsID { get; set; }

        /// <summary>
        /// 代印工厂编号
        /// </summary>
        public int FactoryID { get; set; }

        /// <summary>
        /// 交货地编号
        /// </summary>
        public int DeliveryID { get; set; }

        /// <summary>
        /// 代印合同编号
        /// </summary>
        public string OutContracNo { get; set; }

        /// <summary>
        /// 代印合同金额
        /// </summary>
        public string OutContractSum { get; set; }

        /// <summary>
        /// 代印合同条款
        /// </summary>
        public string Clasue { get; set; }

        /// <summary>
        /// 代印合同备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 代印合同状态
        /// </summary>
        public int OutContractStatus { get; set; }

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
        /// 合同附件
        /// </summary>
        public string AddFileName { get; set; }

        /// <summary>
        /// 是否代印
        /// </summary>
        public string IsOutsourcing { get; set; }

        /// <summary>
        /// 客户PO#
        /// </summary>
        public string CustomerPo { get; set; }

        /// <summary>
        /// 客户代号
        /// </summary>
        public string CustomerCode { get; set; }

        /// <summary>
        /// 下单日期——开始
        /// </summary>
        public string PurchaseDateStart { get; set; }

        /// <summary>
        /// 下单日期——结束
        /// </summary>
        public string PurchaseDateEnd { get; set; }

        /// <summary>
        /// 代印公司
        /// </summary>
        public string OutCompany { get; set; }

        /// <summary>
        /// 采购合同总金额
        /// </summary>
        public decimal PurchaseAmount { get; set; }

        /// <summary>
        /// 采购合同日期
        /// </summary>
        public string PurchaseDate { get; set; }

        /// <summary>
        /// 采购合同编号
        /// </summary>
        public string PurchaseNumber { get; set; }

        /// <summary>
        /// 代印合同状态
        /// </summary>
        public string OutStatus { get; set; }

        /// <summary>
        /// 工厂简称
        /// </summary>
        public string FactoryAbbreviation { get; set; }
    }
}