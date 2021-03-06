using Microsoft.Extensions.Logging;
using Pims.Core.Extensions;
using Pims.Dal.Entities.Models;
using Pims.Dal.Entities.Views;
using Pims.Dal.Helpers.Extensions;
using Pims.Dal.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Pims.Dal.Services
{
    /// <summary>
    /// PropertyService class, provides a service layer to interact with properties within the datasource.
    /// </summary>
    public class PropertyService : BaseService, IPropertyService
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of a PropertyService, and initializes it with the specified arguments.
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="user"></param>
        /// <param name="service"></param>
        /// <param name="logger"></param>
        public PropertyService(PimsContext dbContext, ClaimsPrincipal user, IPimsService service, ILogger<PropertyService> logger) : base(dbContext, user, service, logger) { }
        #endregion

        #region Methods
        /// <summary>
        /// Returns the total number of properties in the database.
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return this.Context.Properties.Count();
        }

        /// <summary>
        /// Get an array of properties within the specified filters.
        /// Will not return sensitive properties unless the user has the `sensitive-view` claim and belongs to the owning agency.
        /// Note that the 'parcelFilter' will control the 'page' and 'quantity'.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IEnumerable<ProjectProperty> Get(AllPropertyFilter filter)
        {
            this.User.ThrowIfNotAuthorized(Permissions.PropertyView);
            filter.ThrowIfNull(nameof(filter));
            if (!filter.IsValid()) throw new ArgumentException("Argument must have a valid filter", nameof(filter));

            var parcelFilter = (ParcelFilter)filter;
            var buildingFilter = (BuildingFilter)filter;

            if (parcelFilter.IsValid() && !buildingFilter.IsValid())
            {
                filter.PropertyType = Entities.PropertyTypes.Land;
            }
            else if (!parcelFilter.IsValid())
            {
                filter.PropertyType = Entities.PropertyTypes.Building;
            }

            var query = this.Context.GenerateQuery(this.User, filter);
            var properties = query.Select(x => new ProjectProperty(x)).ToArray();

            var projectNumbers = properties.Select(p => p.ProjectNumber).Distinct().ToArray();
            var statuses = from p in this.Context.ProjectProperties
                           where projectNumbers.Contains(p.Project.ProjectNumber)
                           select new { p.Project.ProjectNumber, p.Project.Status };

            foreach (var status in statuses)
            {
                foreach (var projectProperty in properties.Where(property => property.ProjectNumber == status.ProjectNumber))
                {
                    projectProperty.ProjectStatus = status.Status.Code;
                }
            }

            return properties;
        }

        /// <summary>
        /// Get a page with an array of properties within the specified filters.
        /// Will not return sensitive properties unless the user has the `sensitive-view` claim and belongs to the owning agency.
        /// Note that the 'parcelFilter' will control the 'page' and 'quantity'.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Paged<Property> GetPage(AllPropertyFilter filter)
        {
            this.User.ThrowIfNotAuthorized(Permissions.PropertyView);
            filter.ThrowIfNull(nameof(filter));
            if (!filter.IsValid()) throw new ArgumentException("Argument must have a valid filter", nameof(filter));

            var parcelFilter = (ParcelFilter)filter;
            var buildingFilter = (BuildingFilter)filter;

            if (parcelFilter.IsValid() && !buildingFilter.IsValid())
            {
                filter.PropertyType = Entities.PropertyTypes.Land;
            }
            else if (!parcelFilter.IsValid() && buildingFilter.IsValid())
            {
                filter.PropertyType = Entities.PropertyTypes.Building;
            }

            var skip = (filter.Page - 1) * filter.Quantity;
            var query = this.Context.GenerateQuery(this.User, filter);
            var items = query
                .Skip(skip)
                .Take(filter.Quantity)
                .ToArray();

            return new Paged<Property>(items, filter.Page, filter.Quantity, query.Count());
        }
        #endregion
    }
}
