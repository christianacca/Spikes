using System;
using System.Collections.Generic;

namespace Spikes.EntityFramework.Models.Bidirectional.Conventional
{
    public class Order : EntityBase
    {
        public Order()
        {
            PlacedOn = DateTime.Now;
            Lines = new List<OrderLine>();
        }

        public DateTime PlacedOn { get; set; }

        /// <summary>
        /// Note: in respect to EF mapping conventions, it doesn't matter what 
        /// the collection nav.property is named
        /// </summary>
        public ICollection<OrderLine> Lines { get; set; }

        public ICollection<FileHeader> Files { get; set; }
    }
}