using MapsterMapper;
using Entity = Pims.Dal.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model = Pims.Api.Models.Parcel;
using Pims.Api.Helpers.Extensions;
using Pims.Api.Policies;
using Pims.Dal;
using Pims.Dal.Entities.Models;
using Pims.Dal.Security;
using System;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.Annotations;
using Pims.Api.Helpers.Exceptions;

namespace Pims.Api.Controllers
{
    /// <summary>
    /// ParcelController class, provides endpoints for managing my parcels.
    /// </summary>
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/parcels")]
    [Route("parcels")]
    public class ParcelController : ControllerBase
    {
        #region Variables
        private readonly ILogger<ParcelController> _logger;
        private readonly IPimsService _pimsService;
        private readonly IMapper _mapper;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a ParcelController class.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="pimsService"></param>
        /// <param name="mapper"></param>
        public ParcelController(ILogger<ParcelController> logger, IPimsService pimsService, IMapper mapper)
        {
            _logger = logger;
            _pimsService = pimsService;
            _mapper = mapper;
        }
        #endregion

        #region Endpoints
        /// <summary>
        /// Get all the parcels that satisfy the filter parameters.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HasPermission(Permissions.PropertyView)]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<Model.PartialParcelModel>), 200)]
        [SwaggerOperation(Tags = new[] { "parcel" })]
        public IActionResult GetParcels()
        {
            var uri = new Uri(this.Request.GetDisplayUrl());
            var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);
            return GetParcels(new ParcelFilter(query));
        }

        /// <summary>
        /// Get all the parcels that satisfy the filter parameters.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost("filter")]
        [HasPermission(Permissions.PropertyView)]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<Model.PartialParcelModel>), 200)]
        [SwaggerOperation(Tags = new[] { "parcel" })]
        public IActionResult GetParcels([FromBody]ParcelFilter filter)
        {
            filter.ThrowBadRequestIfNull($"The request must include a filter.");
            if (!filter.IsValid()) throw new BadRequestException("Property filter must contain valid values.");

            var parcels = _pimsService.Parcel.Get(filter);
            var result = _mapper.Map<Model.PartialParcelModel[]>(parcels);
            return new JsonResult(result);
        }

        /// <summary>
        /// Get the parcel from the datasource if the user is allowed.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [HasPermission(Permissions.PropertyView)]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Model.ParcelModel), 200)]
        [SwaggerOperation(Tags = new[] { "parcel" })]
        public IActionResult GetParcel(int id)
        {
            var entity = _pimsService.Parcel.Get(id);
            var parcel = _mapper.Map<Model.ParcelModel>(entity);

            return new JsonResult(parcel);
        }

        /// <summary>
        /// Add a new parcel to the datasource for the current user.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [HasPermission(Permissions.PropertyAdd)]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Model.ParcelModel), 200)]
        [SwaggerOperation(Tags = new[] { "parcel" })]
        public IActionResult AddParcel([FromBody] Model.ParcelModel model)
        {
            var entity = _mapper.Map<Entity.Parcel>(model);

            _pimsService.Parcel.Add(entity);
            var parcel = _mapper.Map<Model.ParcelModel>(entity);

            return CreatedAtAction(nameof(GetParcel), new { id = parcel.Id }, parcel);
        }

        /// <summary>
        /// Update the specified parcel in the datasource if the user is allowed.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [HasPermission(Permissions.PropertyEdit)]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Model.ParcelModel), 200)]
        [SwaggerOperation(Tags = new[] { "parcel" })]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "To support standardized routes (/update/{id})")]
        public IActionResult UpdateParcel(int id, [FromBody] Model.ParcelModel model)
        {
            var entity = _mapper.Map<Entity.Parcel>(model);

            _pimsService.Parcel.Update(entity); // TODO: Update related properties (i.e. Address).
            var parcel = _mapper.Map<Model.ParcelModel>(entity);

            return new JsonResult(parcel);
        }

        /// <summary>
        /// Delete the specified parcel from the datasource if the user is allowed.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [HasPermission(Permissions.PropertyAdd)]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Model.ParcelModel), 200)]
        [SwaggerOperation(Tags = new[] { "parcel" })]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "To support standardized routes (/delete/{id})")]
        public IActionResult DeleteParcel(int id, [FromBody] Model.ParcelModel model)
        {
            var entity = _mapper.Map<Entity.Parcel>(model);

            _pimsService.Parcel.Remove(entity);

            return new JsonResult(model);
        }

        #region Page Endpoints

        /// <summary>
        /// Get a page of parcels that satisfy the filter parameters.
        /// </summary>
        /// <returns></returns>
        [HttpGet("page")]
        [HasPermission(Permissions.PropertyView)]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Api.Models.PageModel<Model.PartialParcelModel>), 200)]
        [SwaggerOperation(Tags = new[] { "parcel" })]
        public IActionResult GetParcelsPage()
        {
            var uri = new Uri(this.Request.GetDisplayUrl());
            var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);
            return GetParcelsPage(new ParcelFilter(query));
        }

        /// <summary>
        /// Get a page of parcels that satisfy the filter parameters.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost("page/filter")]
        [HasPermission(Permissions.PropertyView)]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Api.Models.PageModel<Model.PartialParcelModel>), 200)]
        [SwaggerOperation(Tags = new[] { "parcel" })]
        public IActionResult GetParcelsPage([FromBody]ParcelFilter filter)
        {
            filter.ThrowBadRequestIfNull($"The request must include a filter.");
            if (!filter.IsValid()) throw new BadRequestException("Property filter must contain valid values.");

            var page = _pimsService.Parcel.GetPage(filter);
            var result = _mapper.Map<Models.PageModel<Model.PartialParcelModel>>(page);
            return new JsonResult(result);
        }
        #endregion
        #endregion
    }
}
