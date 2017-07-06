using ERP.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BLL.Workflow.Approval
{
   public class ApprovalShipmentNotification : BaseApproval
   {
       public ApprovalShipmentNotification(ApprovalInfo info)
            : base(info)
        {

        }

       private Delivery_ShipmentOrder _item;
        public override object ItemInfo
        {
            get
            {
                if (_item == null)
                {
                        _item = context.Delivery_ShipmentOrder.Where(q => q.ID == approvalInfo.IdentityID).FirstOrDefault();
                }
                return _item;
            }
        }

        public override int ItemCreateUserID
        {
            get {return  (int)_item.ST_CREATEUSERNotification; }
        }

        public override int ItemStatus
        {
            get
            {
                return _item.NotificationStatusID;
            }
            set
            {
                _item.NotificationStatusID = value;
            }
        }

        public override int? ItemApproverIndex
        {
            get
            {
                return _item.ApproverIndexNotification;
            }
            set
            {
                _item.ApproverIndexNotification = value;
            }
        }

        protected override void DoLogMethod()
        {
            Delivery_ShipmentOrderHistory t = approvalInfo.LogMethod() as Delivery_ShipmentOrderHistory;
            if (t != null)
            {
                _item.Delivery_ShipmentOrderHistory.Add(t);
            }
        }

        protected override void DoSaveItemInfo()
        {
                var entry = context.Entry(_item);
                entry.State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
        }
    }
}
