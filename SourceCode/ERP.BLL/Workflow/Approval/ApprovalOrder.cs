using ERP.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BLL.Workflow.Approval
{
    public class ApprovalOrder : BaseApproval
    {
        private Order _order = null;
        public override object ItemInfo
        {
            get
            {
                if (_order == null)
                {
                    _order = (from q in context.Orders
                                where q.OrderID == approvalInfo.IdentityID
                                select q).FirstOrDefault();
                }
                return _order;
            }
        }

        public override int ItemStatus
        {
            get
            {
                return _order.OrderStatusID;
            }
            set {
                _order.OrderStatusID = value;
            }
        }

        public override int ItemCreateUserID
        {
            get { return _order.ST_CREATEUSER; }
        }

        public override int? ItemApproverIndex
        {
            get { return _order.ApproverIndex; }
            set { _order.ApproverIndex = value; }
        }

        public ApprovalOrder(ApprovalInfo info)
            : base(info)
        {

        }

        protected override void DoLogMethod()
        {
            OrderHistory t = approvalInfo.LogMethod() as OrderHistory;
            if (t != null)
            {
                _order.OrderHistories.Add(t);
            }
        }

        protected override void DoSaveItemInfo()
        {
                var entry = context.Entry(_order);
                entry.State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
        }
    }
}
