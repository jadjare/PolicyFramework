using System.Collections.Generic;
using System.Linq;

namespace Jadjare.PolicyFramework
{
    public abstract class PolicyContext
    {
        public IEnumerable<KeyValuePair<string, PolicySet>> GetPolicySets()
        {
            var policySets = new Dictionary<string, PolicySet>();
            foreach (var property in this.GetType().GetProperties().Where(prop => prop.PropertyType == typeof(PolicySet)))
            {
                var policySet = property.GetValue(this) as PolicySet;
                policySets.Add(property.Name, policySet);
            }

            return policySets;
        }
    }
}
