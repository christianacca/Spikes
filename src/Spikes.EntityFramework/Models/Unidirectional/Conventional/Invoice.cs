using System;
using System.Collections.Generic;

namespace Spikes.EntityFramework.Models.Unidirectional.Conventional
{
    public class Invoice
    {
        public Invoice()
        {
            this.PlacedOn = DateTime.Now;
            this.Lines = new List<InvoiceLine>();
        }

        public int Id { get; set; }
        public DateTime PlacedOn { get; set; }

        /// <summary>
        /// Note: in respect to EF mapping conventions, it doesn't matter what 
        /// the collection nav.property is named
        /// </summary>
        public ICollection<InvoiceLine> Lines { get; set; }
    }
}