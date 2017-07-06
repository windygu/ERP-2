using ERP.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BLL.Workflow.Approval
{
    public class ApprovalInspectionCustoms : BaseApproval
    {
        public ApprovalInspectionCustoms(ApprovalInfo info)
            : base(info)
        {
        }

        private Inspection_InspectionCustoms _item;

        public override object ItemInfo
        {
            get
            {
                if (_item == null)
                    _item = context.Inspection_InspectionCustoms.Where(i => i.InspectionCustomsID == approvalInfo.IdentityID).FirstOrDefault();

                return _item;
            }
        }

        public override int ItemCreateUserID
        {
            get { return _item.ST_CREATEUSER; }
        }

        public override int ItemStatus
        {
            get
            {
                return _item.StatusID;
            }
            set
            {
                _item.StatusID = value;
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
            Inspection_InspectionCustomsHis his = approvalInfo.LogMethod() as Inspection_InspectionCustomsHis;
            if (his != null)
                _item.Inspection_InspectionCustomsHis.Add(his);
        }

        protected override void DoSaveItemInfo()
        {
            var entry = context.Entry(_item);
            entry.State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();
        }
    }
}