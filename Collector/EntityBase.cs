using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collector
{
    public abstract class EntityBase
    {

        public static int Id { get; set; }
        public int SampleId { get; set; }

        public string SampleType { get; set; }
                
        public DateTime? TimeStamp { get; set; }

        public bool IsActive{ get; set; }

        private string _indexName;
        public string IndexName {
            get
            {
                return _indexName;
            }
            set
            {
                _indexName = value.ToLower();
            }
        }

        public int Interval { get; set; }

        public virtual bool Validate()
        {
            var isValid = true;

            // Currently only WebApi tests are supported
            if (!SampleType.Equals("WebApi")) { isValid = false; }
            if (!IsActive) { isValid = false; }

            return isValid;
        }

    }
}
