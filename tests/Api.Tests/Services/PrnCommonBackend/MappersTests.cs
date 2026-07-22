using AwesomeAssertions;
using Defra.LegacyPrns.Api.Data.Entities;
using Defra.LegacyPrns.Api.Services.PrnCommonBackend;
using Defra.LegacyPrns.Testing.Fixtures.PrnCommonBackend;
using MongoDB.Bson;

namespace Defra.LegacyPrns.Api.Tests.Services.PrnCommonBackend;

public class MappersTests
{
    [Fact]
    public void ToLegacyPrn_ShouldMapRawDataToLegacyPrn()
    {
        const int prnId = 8231;
        const string agency = "Environment Agency";
        const string acceptedStatus = "accepted";
        const string awaitingAcceptanceStatus = "awaiting-acceptance";
        const string issuerReference = "TVR/2024/PPR/0042";
        const string issuedComment = "Issued for Q1 tonnage.";
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

        var dto = PrnRawDataDtoFixture.Create(prnId) with
        {
            ExternalId = externalId,
            PrnNumber = "EA240001234",
            OrganisationId = organisationId,
            OrganisationName = "Compliance Scheme Alpha Ltd",
            ProducerAgency = agency,
            ReprocessorExporterAgency = agency,
            PrnStatusId = acceptedStatusId,
            TonnageValue = tonnageValue,
            IssuerNotes = "Q1 baled mixed paper, weighbridge tickets available on request.",
            IssuerReference = issuerReference,
            PrnSignatory = "Sarah Tester",
            PrnSignatoryPosition = "Compliance Manager",
            Signature = "S. Tester",
            IssueDate = issueDate,
            ProcessToBeUsed = "R3",
            DecemberWaste = false,
            StatusUpdatedOn = statusUpdatedOn,
            IssuedByOrg = "Thames Valley Reprocessing Ltd",
            AccreditationNumber = "ER2024/10321",
            ReprocessingSite = "Unit 7, Riverside Industrial Estate, Reading",
            AccreditationYear = "2024",
            ObligationYear = "2024",
            PackagingProducer = "Compliance Scheme Alpha Ltd",
            CreatedBy = "stester",
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
                    ObligationYear = "2024",
                },
                new PrnStatusHistoryRawDataDto
                {
                    Id = 50101,
                    CreatedOn = createdOn,
                    CreatedByUser = createdByUser,
                    CreatedByOrganisationId = createdByOrganisationId,
                    PrnStatusIdFk = acceptedStatusId,
                    PrnIdFk = prnId,
                    ObligationYear = "2024",
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
                    PrnNumber = dto.PrnNumber,
                    Organisation = new Organisation { Id = organisationId, Name = dto.OrganisationName },
                    ProducerAgency = agency,
                    ReprocessorExporterAgency = agency,
                    TonnageValue = tonnageValue,
                    MaterialName = dto.MaterialName,
                    Notes = dto.IssuerNotes,
                    PrnSignatory = dto.PrnSignatory,
                    PrnSignatoryPosition = dto.PrnSignatoryPosition,
                    IssuedByOrg = dto.IssuedByOrg,
                    Signature = dto.Signature,
                    IssueDate = issueDate,
                    IsDecemberWaste = false,
                    AccreditationNumber = dto.AccreditationNumber,
                    ReprocessingSite = dto.ReprocessingSite,
                    AccreditationYear = parsedYear,
                    ObligationYear = parsedYear,
                    PackagingProducer = dto.PackagingProducer,
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
                        ProcessToBeUsed = dto.ProcessToBeUsed,
                    },
                    CreatedAt = createdOn,
                    CreatedBy = new User { Name = dto.CreatedBy },
                    UpdatedAt = statusUpdatedOn,
                    UpdatedBy = new User { Id = updatedBy },
                },
                options => options.WithStrictOrdering()
            );
    }

    [Fact]
    public void ToLegacyPrn_WhenRawDataDoesNotContainSharedUserValues_ShouldLeaveUserFieldsNull()
    {
        var dto = PrnRawDataDtoFixture.Create(1) with { CreatedBy = null };

        var document = dto.ToLegacyPrn();

        document.Id.Should().Be(ObjectId.Empty);
        document.CreatedBy.Id.Should().BeNull();
        document.CreatedBy.Name.Should().BeNull();
        document.UpdatedBy.Name.Should().BeNull();
    }

    [Theory]
    [InlineData(1, "accepted")]
    [InlineData(2, "rejected")]
    [InlineData(3, "cancelled")]
    [InlineData(4, "awaiting-acceptance")]
    [InlineData(999, null)]
    public void ToLegacyPrn_ShouldMapStatusIdsToLegacyStatuses(int statusId, string? expectedStatus)
    {
        var dto = PrnRawDataDtoFixture.Create(1) with
        {
            PrnStatusId = statusId,
            PrnStatusHistories =
            [
                PrnRawDataDtoFixture.Create(1).PrnStatusHistories[0] with
                {
                    PrnStatusIdFk = statusId,
                },
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
        var dto = PrnRawDataDtoFixture.Create(1) with
        {
            AccreditationYear = "not-a-year",
            ObligationYear = "2024.5",
            PrnStatusHistories =
            [
                PrnRawDataDtoFixture.Create(1).PrnStatusHistories[0] with
                {
                    ObligationYear = "unknown",
                },
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
        var response = PaginatedPrnRawDataResponseFixture.Create(
            currentPage: 1,
            pageSize,
            Array.Empty<PrnRawDataDto>(),
            totalItems
        );

        response.PageCount.Should().Be(expectedPageCount);
    }
}
