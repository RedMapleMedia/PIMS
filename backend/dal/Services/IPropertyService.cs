using Pims.Dal.Entities.Models;
using Pims.Dal.Entities.Views;
using System.Collections.Generic;

namespace Pims.Dal.Services
{
    /// <summary>
    /// IPropertyService interface, provides functions to interact with properties within the datasource.
    /// </summary>
    public interface IPropertyService : IService
    {
        int Count();
        IEnumerable<ProjectProperty> Get(AllPropertyFilter filter);
        Paged<Property> GetPage(AllPropertyFilter filter);
    }
}
