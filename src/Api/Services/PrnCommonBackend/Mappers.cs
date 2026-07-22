using System.Globalization;
using Defra.LegacyPrns.Api.Data.Entities;

namespace Defra.LegacyPrns.Api.Services.PrnCommonBackend;

public static class Mappers
{
    public static LegacyPrn ToLegacyPrn(this PrnRawDataDto prn)
    {
        const int version = 1;

        return new LegacyPrn
        {
            Version = version,
            PrnNumber = prn.PrnNumber,
            Organisation = new Organisation { Id = prn.OrganisationId, Name = prn.OrganisationName },
            ProducerAgency = prn.ProducerAgency,
            ReprocessorExporterAgency = prn.ReprocessorExporterAgency,
            TonnageValue = prn.TonnageValue,
            MaterialName = prn.MaterialName,
            Notes = prn.IssuerNotes,
            PrnSignatory = prn.PrnSignatory,
            PrnSignatoryPosition = prn.PrnSignatoryPosition,
            IssuedByOrg = prn.IssuedByOrg,
            Signature = prn.Signature,
            IssueDate = prn.IssueDate,
            IsDecemberWaste = prn.DecemberWaste,
            AccreditationNumber = prn.AccreditationNumber,
            ReprocessingSite = prn.ReprocessingSite,
            AccreditationYear = ToYear(prn.AccreditationYear),
            ObligationYear = ToYear(prn.ObligationYear),
            PackagingProducer = prn.PackagingProducer,
            IsExport = prn.IsExport,
            Status = new Status
            {
                CurrentStatus = ToLegacyStatus(prn.PrnStatusId),
                CurrentStatusAt = prn.StatusUpdatedOn,
                History = prn
                    .PrnStatusHistories.OrderBy(x => x.CreatedOn)
                    .ThenBy(x => x.Id)
                    .Select(ToStatusHistory)
                    .ToArray(),
            },
            Legacy = new Legacy
            {
                PrnId = prn.Id,
                ExternalId = prn.ExternalId,
                SourceSystemId = prn.SourceSystemId,
                PrnStatusId = prn.PrnStatusId,
                IssuerReference = prn.IssuerReference,
                ProcessToBeUsed = prn.ProcessToBeUsed,
            },
            CreatedAt = prn.CreatedOn,
            CreatedBy = new User { Name = prn.CreatedBy },
            UpdatedAt = prn.LastUpdatedDate,
            UpdatedBy = new User { Id = prn.LastUpdatedBy },
        };
    }

    private static StatusHistory ToStatusHistory(PrnStatusHistoryRawDataDto history) =>
        new()
        {
            Id = history.Id,
            CreatedOn = history.CreatedOn,
            CreatedByUser = history.CreatedByUser,
            PrnStatus = ToLegacyStatus(history.PrnStatusIdFk),
            Comment = history.Comment,
            ObligationYear = ToYear(history.ObligationYear),
            Legacy = new StatusHistoryLegacy
            {
                CreatedByOrganisationId = history.CreatedByOrganisationId,
                PrnStatusIdFk = history.PrnStatusIdFk,
            },
        };

    private static int? ToYear(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var year) ? year : null;
    }

    private static string? ToLegacyStatus(int statusId) =>
        statusId switch
        {
            1 => "accepted",
            2 => "rejected",
            3 => "cancelled",
            4 => "awaiting-acceptance",
            _ => null,
        };
}
