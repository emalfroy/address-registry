namespace AddressRegistry.Api.Oslo.Address
{
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.Api.Search.Filtering;
    using Be.Vlaanderen.Basisregisters.Api.Search.Pagination;
    using Be.Vlaanderen.Basisregisters.Api.Search.Sorting;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy.Adres;
    using Infrastructure.Options;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Projections.Legacy;
    using Projections.Syndication;
    using Query;
    using Responses;
    using Swashbuckle.AspNetCore.Filters;
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Search;
    using Projections.Syndication.Municipality;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    [ApiVersion("2.0")]
    [AdvertiseApiVersions("2.0")]
    [ApiRoute("adressen")]
    [ApiExplorerSettings(GroupName = "Adressen")]
    public class AddressController : ApiController
    {
        /// <summary>
        /// Vraag een adres op.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="syndicationContext"></param>
        /// <param name="responseOptions"></param>
        /// <param name="persistentLocalId">Identificator van het adres.</param>
        /// <param name="taal">De taal in dewelke het adres wordt teruggegeven.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als het adres gevonden is.</response>
        /// <response code="404">Als het adres niet gevonden kan worden.</response>
        /// <response code="410">Als het adres verwijderd is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("{persistentLocalId}")]
        [Produces(AcceptTypes.JsonLd)]
        [ProducesResponseType(typeof(AddressOsloResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status410Gone)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AddressOsloResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(AddressNotFoundResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status410Gone, typeof(AddressGoneResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> Get(
            [FromServices] LegacyContext context,
            [FromServices] SyndicationContext syndicationContext,
            [FromServices] IOptions<ResponseOptions> responseOptions,
            [FromRoute] int persistentLocalId,
            [FromRoute] Taal? taal,
            CancellationToken cancellationToken = default)
        {
            var address = await context
                .AddressDetail
                .AsNoTracking()
                .SingleOrDefaultAsync(item => item.PersistentLocalId == persistentLocalId, cancellationToken);

            if (address != null && address.Removed)
                throw new ApiException("Adres werd verwijderd.", StatusCodes.Status410Gone);

            if (address == null || !address.Complete)
                throw new ApiException("Onbestaand adres.", StatusCodes.Status404NotFound);

            var streetName = await syndicationContext.StreetNameLatestItems.FindAsync(new object[] { address.StreetNameId }, cancellationToken);
            var municipality = await syndicationContext.MunicipalityLatestItems.FirstAsync(m => m.NisCode == streetName.NisCode, cancellationToken);
            var defaultMunicipalityName = AddressMapper.GetDefaultMunicipalityName(municipality);
            var defaultStreetName = AddressMapper.GetDefaultStreetNameName(streetName, municipality.PrimaryLanguage);
            var defaultHomonymAddition = AddressMapper.GetDefaultHomonymAddition(streetName, municipality.PrimaryLanguage);

            var gemeente = new AdresDetailGemeente(
                municipality.NisCode,
                string.Format(responseOptions.Value.GemeenteDetailUrl, municipality.NisCode),
                new GeografischeNaam(defaultMunicipalityName.Value, defaultMunicipalityName.Key));

            var straat = new AdresDetailStraatnaam(
                streetName.PersistentLocalId,
                string.Format(responseOptions.Value.StraatnaamDetailUrl, streetName.PersistentLocalId),
                new GeografischeNaam(defaultStreetName.Value, defaultStreetName.Key));

            var postInfo = string.IsNullOrEmpty(address.PostalCode)
                ? null
                : new AdresDetailPostinfo(
                    address.PostalCode,
                    string.Format(responseOptions.Value.PostInfoDetailUrl, address.PostalCode));

            var homoniemToevoeging = defaultHomonymAddition == null
                ? null
                : new HomoniemToevoeging(new GeografischeNaam(defaultHomonymAddition.Value.Value, defaultHomonymAddition.Value.Key));

            return Ok(
                new AddressOsloResponse(
                    responseOptions.Value.Naamruimte,
                    address.PersistentLocalId.ToString(),
                    address.HouseNumber,
                    address.BoxNumber,
                    gemeente,
                    straat,
                    homoniemToevoeging,
                    postInfo,
                    AddressMapper.GetAddressPoint(address.Position, address.PositionMethod, address.PositionSpecification),
                    AddressMapper.ConvertFromAddressStatus(address.Status),
                    defaultStreetName.Key,
                    address.OfficiallyAssigned,
                    address.VersionTimestamp.ToBelgianDateTimeOffset()));
        }

        /// <summary>
        /// Vraag een lijst met adressen op.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="syndicationContext"></param>
        /// <param name="taal">Gewenste taal van de respons.</param>
        /// <param name="responseOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de adresmatch gelukt is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet]
        [Produces(AcceptTypes.JsonLd)]
        [ProducesResponseType(typeof(AddressListOsloResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AddressListOsloResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> List(
            [FromServices] LegacyContext context,
            [FromServices] AddressQueryContext queryContext,
            [FromServices] IOptions<ResponseOptions> responseOptions,
            Taal? taal,
            CancellationToken cancellationToken = default)
        {
            var filtering = Request.ExtractFilteringRequest<AddressFilter>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var pagedAddresses = new AddressListOsloQuery(queryContext)
                .Fetch(filtering, sorting, pagination);

            Response.AddPagedQueryResultHeaders(pagedAddresses);

            var addresses = await pagedAddresses.Items
                .Select(a =>
                new
                {
                    a.PersistentLocalId,
                    a.StreetNameId,
                    a.HouseNumber,
                    a.BoxNumber,
                    a.PostalCode,
                    a.Status,
                    a.VersionTimestamp
                })
                .ToListAsync(cancellationToken);

            var streetNameIds = addresses
                .Select(x => x.StreetNameId)
                .Distinct()
                .ToList();

            var streetNames = await queryContext
                .StreetNameLatestItems
                .Where(x => streetNameIds.Contains(x.StreetNameId))
                .ToListAsync(cancellationToken);

            var nisCodes = streetNames
                .Select(x => x.NisCode)
                .Distinct()
                .ToList();

            var municipalities = await queryContext
                .MunicipalityLatestItems
                .Where(x => nisCodes.Contains(x.NisCode))
                .ToListAsync(cancellationToken);

            var addressListItemResponses = addresses
                .Select(a =>
                {
                    var streetName = streetNames.SingleOrDefault(x => x.StreetNameId == a.StreetNameId);
                    MunicipalityLatestItem municipality = null;
                    if (streetName != null)
                        municipality = municipalities.SingleOrDefault(x => x.NisCode == streetName.NisCode);
                    return new AddressListItemOsloResponse(
                        a.PersistentLocalId,
                        responseOptions.Value.Naamruimte,
                        responseOptions.Value.DetailUrl,
                        a.HouseNumber,
                        a.BoxNumber,
                        AddressMapper.GetVolledigAdres(a.HouseNumber, a.BoxNumber, a.PostalCode, streetName, municipality),
                        AddressMapper.ConvertFromAddressStatus(a.Status),
                        a.VersionTimestamp.ToBelgianDateTimeOffset());
                })
                .ToList();

            return Ok(new AddressListOsloResponse
            {
                Adressen = addressListItemResponses,
                Volgende = pagedAddresses.PaginationInfo.BuildNextUri(responseOptions.Value.VolgendeUrl)
            });
        }

        /// <summary>
        /// Vraag het totaal aantal adressen op.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="queryContext"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van het totaal aantal gelukt is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("totaal-aantal")]
        [Produces(AcceptTypes.JsonLd)]
        [ProducesResponseType(typeof(TotaalAantalResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(TotalCountOsloResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> Count(
            [FromServices] LegacyContext context,
            [FromServices] AddressQueryContext queryContext,
            CancellationToken cancellationToken = default)
        {
            var filtering = Request.ExtractFilteringRequest<AddressFilter>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = new NoPaginationRequest();

            return Ok(
                new TotaalAantalResponse
                {
                    Aantal = filtering.ShouldFilter
                        ? await new AddressListOsloQuery(queryContext)
                            .Fetch(filtering, sorting, pagination)
                            .Items
                            .CountAsync(cancellationToken)
                        : Convert.ToInt32(context
                            .AddressListViewCount
                            .First()
                            .Count)
                });
        }

    }
}