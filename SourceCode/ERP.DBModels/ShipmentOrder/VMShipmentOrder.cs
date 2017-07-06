using ERP.Models.CustomEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.ShipmentOrder
{
    /// <summary>
    /// 订舱管理
    /// </summary>
    public class VMShipmentOrder : PendingApproveBasePage
    {
        #region Model

        /// <summary>
        /// 订舱编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 销售订单编号
        /// </summary>
        public int OrderID { get; set; }

        /// <summary>
        /// 状态类型
        /// </summary>
        public int StatusID { get; set; }

        /// <summary>
        /// 审核意见
        /// </summary>
        public string CheckSuggest { get; set; }

        /// <summary>
        /// 船运公司
        /// </summary>
        public int? Shipment_AgencyID { get; set; }

        /// <summary>
        /// 是否删除（0：未删除，1：已删除）
        /// </summary>
        public bool IsDelete { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime DT_CREATEDATE { get; set; }

        /// <summary>
        /// 创建用户
        /// </summary>
        public int ST_CREATEUSER { get; set; }

        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime DT_MODIFYDATE { get; set; }

        /// <summary>
        /// 修改日期
        /// </summary>
        public string DT_MODIFYDATEFormatter { get; set; }

        /// <summary>
        /// 修改用户
        /// </summary>
        public int ST_MODIFYUSER { get; set; }

        /// <summary>
        /// ip地址
        /// </summary>
        public string IPAddress { get; set; }

        #endregion Model

        public string Shipment_AgencyName { get; set; }

        public string POID { get; set; }

        public string EHIPO { get; set; }

        public int DestinationPortID { get; set; }

        public string DestinationPortName { get; set; }

        public PageTypeEnum PageType { get; set; }

        public string OrderNumber { get; set; }

        public int NotificationStatusID { get; set; }

        public int RegisterFeesStatusID { get; set; }

        public string NotificationCheckSuggest { get; set; }

        public string CustomerCode { get; set; }

        public string CustomerDate { get; set; }

        public decimal OrderAmount { get; set; }

        public string StatusName { get; set; }

        public string NotificationStatusName { get; set; }

        public string RegisterFeesStatusName { get; set; }

        /// <summary>
        /// 是否是添加页面
        /// </summary>
        public bool IsAddPage { get; set; }

        /// <summary>
        /// 箱柜列表
        /// </summary>
        public List<VMShipmentOrderCabinet> list_cabinet { get; set; }

        /// <summary>
        /// 历史记录列表
        /// </summary>
        public List<VMShipmentOrderHistory> list_history { get; set; }

        public List<VMShipmentOrderHistory> list_historyRegister { get; set; }

        public List<VMShipmentOrderHistory> list_NotificationHistory { get; set; }

        /// <summary>
        /// 判断待审核页面是否显示审批按钮
        /// </summary>
        public bool IsHasApprovalPermission { get; set; }

        public int? ApproverIndex { get; set; }

        public int? ApproverIndexNotification { get; set; }

        public DateTime OrderDateStart { get; set; }

        public DateTime OrderDateEnd { get; set; }

        public string OrderDateStartFormatter { get; set; }

        public string OrderDateEndFormatter { get; set; }

        public int CustomerID { get; set; }

        public int? PortID { get; set; }

        public string PortName { get; set; }

        public string OrderIDList { get; set; }

        public bool IsMerge { get; set; }

        public string Merge { get; set; }

        public string OrderNumberList { get; set; }
        public int? ST_CREATEUSERNotification { get; set; }
        public string Notification_DT_MODIFYDATEFormatter { get; set; }
        public string RegisterFees_DT_MODIFYDATEFormatter { get; set; }

        /// <summary>
        /// 是否分批出运
        /// </summary>
        public bool IsBacthShipped { get; set; }
        public bool IsBatchShipped { get; set; }
        public string SelectCustomer { get; set; }

        public int InspectionReceiptListID { get; set; }

        public int InspectionCustomsID { get; set; }
    }
}