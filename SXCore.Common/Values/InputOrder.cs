using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Common.Values
{
    public class InputOrder
    {
        public long id { get; set; }
        public List<InputOrder> children { get; set; }

        public InputOrder()
        {
            this.id = 0;
            this.children = null;
        }

        public InputOrder(long id, params InputOrder[] orders)
        {
            this.id = id;
            this.children = orders == null || orders.Count() <= 0 ? null : orders.ToList();
        }

        public void Add(InputOrder order)
        {
            if (order == null)
                return;

            if (this.children == null)
                this.children = new List<InputOrder>();

            this.children.Add(order);
        }

        static public implicit operator long (InputOrder order)
        { return order.id; }

        static public implicit operator InputOrder(long id)
        { return new InputOrder(id); }
    }
}
