using Defra.LegacyPrns.Api.Services.PrnCommonBackend;

namespace Defra.LegacyPrns.Testing.Fixtures.PrnCommonBackend;

public static class PrnRawDataDtoFixture
{
    public static PrnRawDataDto Create(int id)
    {
        const string agency = "Environment Agency";
        const string year = "2024";

        var timestamp = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddDays(id);

        return new PrnRawDataDto
        {
            Id = id,
            ExternalId = Guid.Parse($"00000000-0000-0000-0000-{id:000000000000}"),
            PrnNumber = $"EA24{id:0000000}",
            OrganisationId = Guid.Parse($"10000000-0000-0000-0000-{id:000000000000}"),
            OrganisationName = $"Organisation {id}",
            ProducerAgency = agency,
            ReprocessorExporterAgency = agency,
            PrnStatusId = 1,
            TonnageValue = id,
            MaterialName = "Paper/Board",
            IssuerNotes = $"Issuer notes {id}",
            IssuerReference = $"ISSUER-{id:0000}",
            PrnSignatory = $"Signatory {id}",
            PrnSignatoryPosition = "Compliance Manager",
            Signature = $"Signature {id}",
            IssueDate = timestamp,
            ProcessToBeUsed = "R3",
            DecemberWaste = false,
            StatusUpdatedOn = timestamp.AddHours(1),
            IssuedByOrg = $"Issuer {id}",
            AccreditationNumber = $"ACC-{id:0000}",
            ReprocessingSite = $"Site {id}",
            AccreditationYear = year,
            ObligationYear = year,
            PackagingProducer = $"Producer {id}",
            CreatedBy = $"creator-{id}",
            CreatedOn = timestamp,
            LastUpdatedBy = Guid.Parse($"20000000-0000-0000-0000-{id:000000000000}"),
            LastUpdatedDate = timestamp.AddHours(1),
            IsExport = false,
            SourceSystemId = null,
            PrnStatusHistories =
            [
                new PrnStatusHistoryRawDataDto
                {
                    Id = id * 10,
                    CreatedOn = timestamp,
                    CreatedByUser = Guid.Parse($"30000000-0000-0000-0000-{id:000000000000}"),
                    CreatedByOrganisationId = Guid.Parse($"40000000-0000-0000-0000-{id:000000000000}"),
                    PrnStatusIdFk = 1,
                    PrnIdFk = id,
                    Comment = $"Accepted {id}",
                    ObligationYear = year,
                },
            ],
        };
    }
}
