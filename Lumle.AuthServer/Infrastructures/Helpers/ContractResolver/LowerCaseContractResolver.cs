using Newtonsoft.Json.Serialization;

namespace Lumle.AuthServer.Infrastructures.Helpers.ContractResolver
{
    public class LowerCaseContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToLower();
        }
    }
}
