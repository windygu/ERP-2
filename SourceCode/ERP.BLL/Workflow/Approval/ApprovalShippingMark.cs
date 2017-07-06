using ERP.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BLL.Workflow.Approval
{
    public class ApprovalShippingMark : BaseApproval
    {
        public ApprovalShippingMark(ApprovalInfo info)
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
            get { return (int)_item.ShippingMark_CreateUser; }
        }

        public override int ItemStatus
        {
            get
            {
                return _item.ShippingMark_StatusID;
            }
            set
            {
                _item.ShippingMark_StatusID = value;
            }
        }

        public override int? ItemApproverIndex
        {
            get
            {
                return _item.ApproverIndexShippingMark;
            }
            set
            {
                _item.ApproverIndexShippingMark = value;
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
