using ERP.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BLL.Workflow.Approval
{
    public class ApprovalThirdPeriodQC : BaseApproval
    {
        public ApprovalThirdPeriodQC(ApprovalInfo info)
            : base(info)
        {

        }

        private Purchase_ThreeTimesQC _item;
        public override object ItemInfo
        {
            get
            {
                if (_item == null)
                {
                    _item = context.Purchase_ThreeTimesQC.Where(q => q.ID == approvalInfo.IdentityID).FirstOrDefault();
                }
                return _item;
            }
        }

        public override int ItemCreateUserID
        {
            get
            {
                if (_item.ApproverIndex.HasValue && _item.ApproverIndex == 10)
                {
                    return _item.ST_CREATEUSER2 ?? 0;
                }
                return _item.ST_CREATEUSER;
            }

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
            Purchase_ThreeTimesQCHistory t = approvalInfo.LogMethod() as Purchase_ThreeTimesQCHistory;
            if (t != null)
            {
                _item.Purchase_ThreeTimesQCHistory.Add(t);
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
