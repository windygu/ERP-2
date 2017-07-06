using ERP.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BLL.Workflow.Approval
{
    public class ApprovalOutsourcingContract:BaseApproval
    {
        public ApprovalOutsourcingContract(ApprovalInfo info)
            : base(info)
        {

        }

        private Purchase_OutContracts _item;
        public override object ItemInfo
        {
            get
            {
                if (_item == null)
                {
                        _item = (from q in context.Purchase_OutContracts
                                 where q.ID == approvalInfo.IdentityID
                                 select q).FirstOrDefault();
                }
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
                return _item.OutContractStatus.HasValue ? _item.OutContractStatus.Value : -1;
            }
            set
            {
                _item.OutContractStatus = value;
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
            Purchase_OutContractHis t = approvalInfo.LogMethod() as Purchase_OutContractHis;
            if (t != null)
            {
                _item.Purchase_OutContractHis.Add(t);
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
