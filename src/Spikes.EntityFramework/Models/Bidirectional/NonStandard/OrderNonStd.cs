using System;
using System.Collections.Generic;

namespace Spikes.EntityFramework.Models.Bidirectional.NonStandard
{
    public class OrderNonStd
    {
        public OrderNonStd()
        {
            PlacedOn = DateTime.Now;
            Lines = new List<OrderLineNonStd>();
        }

        public int MyId { get; set; }
        public DateTime PlacedOn { get; set; }

        public ICollection<OrderLineNonStd> Lines { get; set; } 
    }
}