using System;
using System.Collections.Generic;
using System.Text;
using zsq.Project.Domain.SeedWork;

namespace zsq.Project.Domain.AggregatesModel
{
    public class ProjectProperty : ValueObject
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public string Text { get; set; }

        public int ProjectId { get; set; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Key;
            yield return Value;
            yield return Text;
        }
    }
}
