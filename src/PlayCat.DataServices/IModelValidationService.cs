using System.Collections.Generic;

namespace PlayCat.DataServices
{
    public interface IModelValidationService
    {
        string AssemblyName { get; set; }
        IDictionary<string, IDictionary<string, ValidationModel>> GetModel(string typeName);
    }
}
