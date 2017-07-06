using ERP.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BLL.Workflow.Approval
{
    public class ApprovalQuot:BaseApproval
    {
        public ApprovalQuot(ApprovalInfo info)
            : base(info)
        {

        }

        private Quot_Quot _quot;
        public override object ItemInfo
        {
            get {
                if (_quot == null)
                {
                        _quot = (from q in context.Quot_Quot
                                 where q.ID == approvalInfo.IdentityID
                                  select q).FirstOrDefault();
                }
                return _quot;
            }
        }

        public override int ItemCreateUserID
        {
            get { return _quot.ST_CREATEUSER; }
        }

        public override int ItemStatus
        {
            get
            {
                return _quot.StatusID;
            }
            set
            {
                _quot.StatusID = value;
            }
        }

        public override int? ItemApproverIndex
        {
            get
            {
                return _quot.ApproverIndex;
            }
            set
            {
                _quot.ApproverIndex = value;
            }
        }

        protected override void DoLogMethod()
        {
            Quot_QuotHistory t = approvalInfo.LogMethod() as Quot_QuotHistory;
            if (t != null)
            {
                _quot.Quot_QuotHistory.Add(t);
            }
        }

        protected override void DoSaveItemInfo()
        {
                var entry = context.Entry(_quot);
                entry.State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
        }
    }
}
