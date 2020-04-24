using System.Security.Claims;
using Pims.Dal.Services;

namespace Pims.Dal
{
    /// <summary>
    /// IPimsService interface, provides a way to interface with the backend datasource.
    /// </summary>
    public interface IPimsService : IService
    {
        #region Properties
        IPropertyService Property { get; }
        IBuildingService Building { get; }
        ClaimsPrincipal Principal { get; }
        ILookupService Lookup { get; }
        IParcelService Parcel { get; }
        IUserService User { get; }
        #endregion

        #region Methods
        #endregion
    }
}
