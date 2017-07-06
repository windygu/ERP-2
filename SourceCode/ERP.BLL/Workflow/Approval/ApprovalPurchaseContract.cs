using ERP.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BLL.Workflow.Approval
{
    public class ApprovalPurchaseContract:BaseApproval
    {
        public ApprovalPurchaseContract(ApprovalInfo info)
        :base(info){ 
            
        }

        private Purchase_Contract _contract;
        public override object ItemInfo
        {
            get {
                if (_contract == null)
                {
                        _contract = (from q in context.Purchase_Contract
                                     where q.ID == approvalInfo.IdentityID
                                     select q).FirstOrDefault();
                }

                return _contract;
            }
        }

        public override int ItemCreateUserID
        {
            get { return _contract.ST_CREATEUSER; }
        }

        public override int ItemStatus
        {
            get
            {
                return _contract.PurchaseStatus;
            }
            set
            {
                _contract.PurchaseStatus = value;
            }
        }

        public override int? ItemApproverIndex
        {
            get
            {
                return _contract.ApproverIndexPurchaseContract;
            }
            set
            {
                _contract.ApproverIndexPurchaseContract = value;
            }
        }

        protected override void DoLogMethod()
        {
            Purchase_ContractHistory t = approvalInfo.LogMethod() as Purchase_ContractHistory;
            if (t != null)
            {
                _contract.Purchase_ContractHistory.Add(t);
            }
        }

        protected override void DoSaveItemInfo()
        {
                var entry = context.Entry(_contract);
                entry.State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
        }
    }
}
