using ERP.Models.Common;
using ERP.Models.Purchase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.ProducePlan
{
    public class DTOProducePlan : PendingApproveBasePage
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 采购合同编号
        /// </summary>
        ///
        public int PurchaseID { get; set; }

        /// <summary>
        /// 保存按钮区分
        /// </summary>
        public int ButtonNum { get; set; }

        /// <summary>
        /// 销售订单编号集合
        /// </summary>
        public string OrderList { get; set; }

        /// <summary>
        /// 采购合同号
        /// </summary>
        public string PurchaseNumber { get; set; }

        /// <summary>
        /// 采购合同日期
        /// </summary>
        public string PurchaseDate { get; set; }

        /// <summary>
        /// 销售订单编号
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 工厂简称
        /// </summary>
        public string FactoryAbbreviation { get; set; }

        /// <summary>
        /// 客户
        /// </summary>
        public string CustomerCode { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 状态文本形式
        /// </summary>
        public string StatusName { get; set; }

        /// <summary>
        /// 审核结果
        /// </summary>
        public int Result { get; set; }

        /// <summary>
        /// 审核结果文本形式
        /// </summary>
        public string ResultName { get; set; }

        /// <summary>
        /// 审核意见
        /// </summary>
        public string SomeThing { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime DT_CREATEDATE { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public int ST_CREATEUSER { get; set; }

        /// <summary>
        /// DT_MODIFYDATE
        /// </summary>
        public DateTime DT_MODIFYDATE { get; set; }

        /// <summary>
        /// DT_MODIFYDATE
        /// </summary>
        public string DT_MODIFYDATEFormatter { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public int ST_MODIFYUSER { get; set; }

        /// <summary>
        /// ip地址
        /// </summary>
        public string IPAdress { get; set; }

        /// <summary>
        /// 上传时间
        /// </summary>
        public DateTime UploadTime { get; set; }

        public VMPurchase PurchaseContract { get; set; }

        /// <summary>
        /// 前期上传的文件列表
        /// </summary>
        public List<VMUpLoadFile> list_UpLoadFile { get; set; }

        /// <summary>
        /// 判断待审核页面是否显示审批按钮
        /// </summary>
        public bool IsHasApprovalPermission { get; set; }

        public int? ApproverIndex { get; set; }

        public CustomEnums.PageTypeEnum PageTypeEnum { get; set; }

        public List<DTOProducePlanHistory> list_history { get; set; }
        public int CustomerID { get; set; }
        public int? ST_CREATEUSER2 { get; set; }
    }
}