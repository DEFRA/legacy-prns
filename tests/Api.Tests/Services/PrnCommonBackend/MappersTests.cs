using AwesomeAssertions;
using Defra.LegacyPrns.Api.Data.Entities;
using Defra.LegacyPrns.Api.Services.PrnCommonBackend;

namespace Defra.LegacyPrns.Api.Tests.Services.PrnCommonBackend;

public class MappersTests
{
    [Fact]
    public void ToLegacyPrn_ShouldMapRawDataToLegacyPrn()
    {
        const int prnId = 8231;
        const string prnNumber = "EA240001234";
        const string organisationName = "Compliance Scheme Alpha Ltd";
        const string agency = "Environment Agency";
        const string acceptedStatus = "accepted";
        const string awaitingAcceptanceStatus = "awaiting-acceptance";
        const string materialName = "Paper/Board";
        const string issuerNotes = "Q1 baled mixed paper, weighbridge tickets available on request.";
        const string issuerReference = "TVR/2024/PPR/0042";
        const string prnSignatory = "Sarah Tester";
        const string prnSignatoryPosition = "Compliance Manager";
        const string issuedByOrg = "Thames Valley Reprocessing Ltd";
        const string signature = "S. Tester";
        const string processToBeUsed = "R3";
        const string accreditationNumber = "ER2024/10321";
        const string reprocessingSite = "Unit 7, Riverside Industrial Estate, Reading";
        const string issuedComment = "Issued for Q1 tonnage.";
        const string accreditationYear = "2024";
        const string obligationYear = "2024";
        const string createdBy = "stester";
        const string sourceSystemId = "NPWD";
        const int parsedYear = 2024;
        const string schemaVersion = "v1.0";
        const int acceptedStatusId = 1;
        const int awaitingAcceptanceStatusId = 4;
        const int tonnageValue = 150;

        var organisationId = Guid.Parse("10048200-0000-0000-0000-000000000001");
        var externalId = Guid.Parse("c7f3a2e1-9b4d-4c6a-8e2f-1d5b7a9c3e01");
        var createdByUser = Guid.Parse("20011400-0000-0000-0000-000000000001");
        var createdByOrganisationId = Guid.Parse("20011400-0000-0000-0000-000000000002");
        var updatedBy = Guid.Parse("10048200-0000-0000-0000-000000000002");
        var createdOn = new DateTime(2024, 4, 1, 16, 40, 12, DateTimeKind.Utc);
        var issueDate = new DateTime(2024, 4, 2, 9, 15, 0, DateTimeKind.Utc);
        var statusUpdatedOn = new DateTime(2024, 4, 18, 14, 32, 7, DateTimeKind.Utc);

        var dto = new PrnRawDataDto
        {
            Id = prnId,
            ExternalId = externalId,
            PrnNumber = prnNumber,
            OrganisationId = organisationId,
            OrganisationName = organisationName,
            ProducerAgency = agency,
            ReprocessorExporterAgency = agency,
            PrnStatusId = acceptedStatusId,
            TonnageValue = tonnageValue,
            MaterialName = materialName,
            IssuerNotes = issuerNotes,
            IssuerReference = issuerReference,
            PrnSignatory = prnSignatory,
            PrnSignatoryPosition = prnSignatoryPosition,
            Signature = signature,
            IssueDate = issueDate,
            ProcessToBeUsed = processToBeUsed,
            DecemberWaste = false,
            StatusUpdatedOn = statusUpdatedOn,
            IssuedByOrg = issuedByOrg,
            AccreditationNumber = accreditationNumber,
            ReprocessingSite = reprocessingSite,
            AccreditationYear = accreditationYear,
            ObligationYear = obligationYear,
            PackagingProducer = organisationName,
            CreatedBy = createdBy,
            CreatedOn = createdOn,
            LastUpdatedBy = updatedBy,
            LastUpdatedDate = statusUpdatedOn,
            IsExport = false,
            SourceSystemId = sourceSystemId,
            PrnStatusHistories =
            [
                new PrnStatusHistoryRawDataDto
                {
                    Id = 50102,
                    CreatedOn = issueDate,
                    CreatedByUser = createdByUser,
                    CreatedByOrganisationId = createdByOrganisationId,
                    PrnStatusIdFk = awaitingAcceptanceStatusId,
                    PrnIdFk = prnId,
                    Comment = issuedComment,
                    ObligationYear = obligationYear,
                },
                new PrnStatusHistoryRawDataDto
                {
                    Id = 50101,
                    CreatedOn = createdOn,
                    CreatedByUser = createdByUser,
                    CreatedByOrganisationId = createdByOrganisationId,
                    PrnStatusIdFk = acceptedStatusId,
                    PrnIdFk = prnId,
                    ObligationYear = obligationYear,
                },
            ],
        };

        var document = dto.ToLegacyPrn();

        document
            .Should()
            .BeEquivalentTo(
                new LegacyPrn
                {
                    SchemaVersion = schemaVersion,
                    Version = 1,
                    PrnNumber = prnNumber,
                    Organisation = new Organisation { Id = organisationId, Name = organisationName },
                    ProducerAgency = agency,
                    ReprocessorExporterAgency = agency,
                    TonnageValue = tonnageValue,
                    MaterialName = materialName,
                    Notes = issuerNotes,
                    PrnSignatory = prnSignatory,
                    PrnSignatoryPosition = prnSignatoryPosition,
                    IssuedByOrg = issuedByOrg,
                    Signature = signature,
                    IssueDate = issueDate,
                    IsDecemberWaste = false,
                    AccreditationNumber = accreditationNumber,
                    ReprocessingSite = reprocessingSite,
                    AccreditationYear = parsedYear,
                    ObligationYear = parsedYear,
                    PackagingProducer = organisationName,
                    IsExport = false,
                    Status = new Status
                    {
                        CurrentStatus = acceptedStatus,
                        CurrentStatusAt = statusUpdatedOn,
                        History =
                        [
                            new StatusHistory
                            {
                                Id = 50101,
                                CreatedOn = createdOn,
                                CreatedByUser = createdByUser,
                                PrnStatus = acceptedStatus,
                                ObligationYear = parsedYear,
                                Legacy = new StatusHistoryLegacy
                                {
                                    CreatedByOrganisationId = createdByOrganisationId,
                                    PrnStatusIdFk = acceptedStatusId,
                                },
                            },
                            new StatusHistory
                            {
                                Id = 50102,
                                CreatedOn = issueDate,
                                CreatedByUser = createdByUser,
                                PrnStatus = awaitingAcceptanceStatus,
                                Comment = issuedComment,
                                ObligationYear = parsedYear,
                                Legacy = new StatusHistoryLegacy
                                {
                                    CreatedByOrganisationId = createdByOrganisationId,
                                    PrnStatusIdFk = awaitingAcceptanceStatusId,
                                },
                            },
                        ],
                    },
                    Legacy = new Legacy
                    {
                        PrnId = prnId,
                        ExternalId = externalId,
                        SourceSystemId = sourceSystemId,
                        PrnStatusId = acceptedStatusId,
                        IssuerReference = issuerReference,
                        ProcessToBeUsed = processToBeUsed,
                    },
                    CreatedAt = createdOn,
                    CreatedBy = new User { Name = createdBy },
                    UpdatedAt = statusUpdatedOn,
                    UpdatedBy = new User { Id = updatedBy },
                },
                options => options.WithStrictOrdering()
            );
    }

    [Fact]
    public void ToLegacyPrn_WhenRawDataDoesNotContainLookupValues_ShouldLeaveDocumentFieldsNull()
    {
        const string year = "2024";

        var dto = new PrnRawDataDto
        {
            PrnNumber = "EA240001234",
            OrganisationName = "Compliance Scheme Alpha Ltd",
            ProducerAgency = "Environment Agency",
            ReprocessorExporterAgency = "Environment Agency",
            MaterialName = "Paper/Board",
            IssuerReference = "TVR/2024/PPR/0042",
            IssuedByOrg = "Thames Valley Reprocessing Ltd",
            AccreditationNumber = "ER2024/10321",
            AccreditationYear = year,
            ObligationYear = year,
            PackagingProducer = "Compliance Scheme Alpha Ltd",
            LastUpdatedBy = Guid.NewGuid(),
            PrnStatusHistories =
            [
                new PrnStatusHistoryRawDataDto
                {
                    CreatedByUser = Guid.NewGuid(),
                    CreatedByOrganisationId = Guid.NewGuid(),
                },
            ],
        };

        var document = dto.ToLegacyPrn();

        document.Id.Should().BeNull();
        document.CreatedBy.Id.Should().BeNull();
        document.UpdatedBy.Name.Should().BeNull();
        document.Status.History.Should().ContainSingle();
        document.Status.History[0].CreatedByOrganisation.Should().BeNull();
    }

    [Theory]
    [InlineData(1, "accepted")]
    [InlineData(2, "rejected")]
    [InlineData(3, "cancelled")]
    [InlineData(4, "awaiting-acceptance")]
    [InlineData(999, null)]
    public void ToLegacyPrn_ShouldMapStatusIdsToLegacyStatuses(int statusId, string? expectedStatus)
    {
        var dto = CreateRequiredPrnRawDataDto(statusId) with
        {
            PrnStatusHistories =
            [
                new PrnStatusHistoryRawDataDto { PrnStatusIdFk = statusId, CreatedByOrganisationId = Guid.NewGuid() },
            ],
        };

        var document = dto.ToLegacyPrn();

        document.Status.CurrentStatus.Should().Be(expectedStatus);
        document.Status.History.Should().ContainSingle();
        document.Status.History[0].PrnStatus.Should().Be(expectedStatus);
    }

    [Fact]
    public void ToLegacyPrn_WhenYearsCannotBeParsed_ShouldLeaveYearsNull()
    {
        var dto = CreateRequiredPrnRawDataDto(1) with
        {
            AccreditationYear = "not-a-year",
            ObligationYear = "2024.5",
            PrnStatusHistories =
            [
                new PrnStatusHistoryRawDataDto { ObligationYear = "unknown", CreatedByOrganisationId = Guid.NewGuid() },
            ],
        };

        var document = dto.ToLegacyPrn();

        document.AccreditationYear.Should().BeNull();
        document.ObligationYear.Should().BeNull();
        document.Status.History.Should().ContainSingle();
        document.Status.History[0].ObligationYear.Should().BeNull();
    }

    [Theory]
    [InlineData(0, 10, 0)]
    [InlineData(1, 10, 1)]
    [InlineData(20, 10, 2)]
    [InlineData(21, 10, 3)]
    [InlineData(21, 0, 0)]
    public void PageCount_ShouldCalculateFromTotalItemsAndPageSize(int totalItems, int pageSize, int expectedPageCount)
    {
        var response = new PaginatedResponse<PrnRawDataDto> { TotalItems = totalItems, PageSize = pageSize };

        response.PageCount.Should().Be(expectedPageCount);
    }

    private static PrnRawDataDto CreateRequiredPrnRawDataDto(int statusId)
    {
        const string year = "2024";

        return new PrnRawDataDto
        {
            PrnNumber = "EA240001234",
            OrganisationName = "Compliance Scheme Alpha Ltd",
            ProducerAgency = "Environment Agency",
            ReprocessorExporterAgency = "Environment Agency",
            PrnStatusId = statusId,
            MaterialName = "Paper/Board",
            IssuerReference = "TVR/2024/PPR/0042",
            IssuedByOrg = "Thames Valley Reprocessing Ltd",
            AccreditationNumber = "ER2024/10321",
            AccreditationYear = year,
            ObligationYear = year,
            PackagingProducer = "Compliance Scheme Alpha Ltd",
            LastUpdatedBy = Guid.NewGuid(),
        };
    }
}
