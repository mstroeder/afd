using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AFDApp
{
    public class CustomerData
    {
        [Key]
        public Guid id { get; set; }
        [StringLength(30)]
        public string property { get; set; }
        [StringLength(100)]
        public string customer { get; set; }
        [StringLength(30)]
        public string action { get; set; }
        public int value { get; set; }
        [StringLength(100)]
        public string file { get; set; }
        public byte uploadstatus { get; set; }
        public string hash { get; set; }
        public string uploaderror { get; set; }
        public byte checkResult { get; set; }
        public string checkerror { get; set; }
        public override string ToString()
        {
            return "customer: " + this.customer + " property: " + this.property;
        }
    }
}