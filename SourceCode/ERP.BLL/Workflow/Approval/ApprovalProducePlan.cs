using ERP.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BLL.Workflow.Approval
{
    public class ApprovalProducePlan : BaseApproval
    {
        public ApprovalProducePlan(ApprovalInfo info)
            : base(info)
        {

        }

        private Plan_ProducePlan _item;
        public override object ItemInfo
        {
            get
            {
                if (_item == null)
                {
                    _item = (from q in context.Plan_ProducePlan
                             where q.ID == approvalInfo.IdentityID
                             select q).FirstOrDefault();
                }

                return _item;
            }
        }

        public override int ItemCreateUserID
        {
            get { return _item.ST_CREATEUSER2 ?? 0; }
        }

        public override int ItemStatus
        {
            get
            {
                return _item.Status;
            }
            set
            {
                _item.Status = value;
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
            Plan_ProducePlanHistory t = approvalInfo.LogMethod() as Plan_ProducePlanHistory;
            if (t != null)
            {
                _item.Plan_ProducePlanHistory.Add(t);
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
