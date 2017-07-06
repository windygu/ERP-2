using ERP.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BLL.Workflow.Approval
{
    public class ApprovalInspectionReceipt : BaseApproval
    {
        public ApprovalInspectionReceipt(ApprovalInfo info)
             : base(info)
        {
        }

        private Inspection_InspectionReceipt _item;

        public override object ItemInfo
        {
            get
            {
                if (_item == null)
                    _item = context.Inspection_InspectionReceipt.Where(i => i.InspectionReceiptID == approvalInfo.IdentityID).FirstOrDefault();

                return _item;
            }
        }

        public override int ItemCreateUserID
        {
            get { return _item.CreateUserID; }
        }

        public override int ItemStatus
        {
            get
            {
                return _item.InspectionReceiptStatus;
            }
            set
            {
                _item.InspectionReceiptStatus = value;
            }
        }

        public override int? ItemApproverIndex
        {
            get
            {
                return _item.ApproverIndex;
            }
            set
            {
                _item.ApproverIndex = value;
            }
        }

        protected override void DoLogMethod()
        {
            var history = approvalInfo.LogMethod() as Inspection_InspectionReceiptHis;
            if (history != null)
                _item.Inspection_InspectionReceiptHis.Add(history);
        }

        protected override void DoSaveItemInfo()
        {
            var entry = context.Entry(_item);
            entry.State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();
        }
    }
}