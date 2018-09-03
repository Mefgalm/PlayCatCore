using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PlayCat.DataServices;

namespace PlayCat.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ValidationController : BaseController
    {
        private readonly IModelValidationService _modelValidationService;

        public ValidationController(IModelValidationService modelValidationService)
        {
            _modelValidationService = modelValidationService;
        }

        [HttpGet("validateRules/{typeName}")]
        public IDictionary<string, IDictionary<string, ValidationModel>> ValidateRules(string typeName)
        {
            return _modelValidationService.GetModel(typeName);
        }
    }
}
