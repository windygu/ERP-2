using ERP.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BLL.Workflow.Approval
{
    public class ApprovalPacking : BaseApproval
    {
        public ApprovalPacking(ApprovalInfo info)
            : base(info)
        {

        }

        private Purchase_Contract _item;
        public override object ItemInfo
        {
            get
            {
                if (_item == null)
                {
                    _item = (from q in context.Purchase_Contract
                             where q.ID == approvalInfo.IdentityID
                             select q).FirstOrDefault();
                }
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
                return _item.PacksStatus;
            }
            set
            {
                _item.PacksStatus = value;
            }
        }

        public override int? ItemApproverIndex
        {
            get
            {
                return _item.ApproverIndexPacks;
            }
            set
            {
                _item.ApproverIndexPacks = value;
            }
        }

        protected override void DoLogMethod()
        {
            //Purchase_ContractHistory t = approvalInfo.LogMethod() as Purchase_ContractHistory;
            //if (t != null)
            //{
            //    _item.Purchase_ContractHistory.Add(t);
            //}
        }

        protected override void DoSaveItemInfo()
        {
            var entry = context.Entry(_item);
            entry.State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();
        }
    }
}
