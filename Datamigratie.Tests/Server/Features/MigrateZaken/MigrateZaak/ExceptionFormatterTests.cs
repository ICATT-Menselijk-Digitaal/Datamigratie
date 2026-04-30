using System.Net;
using Datamigratie.Server.Features.MigrateZaken.MigrateZaak;

namespace Datamigratie.Tests.Server.Features.MigrateZaken.MigrateZaak;

public class ExceptionFormatterTests
{
    private static readonly Guid DocumentId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    #region OrphanedDocumentException

    [Fact]
    public void FormatHttpStatusInfo_OrphanedDocument_ShowsDocumentIdAndCleanupMessage()
    {
        var orphaned = new OrphanedDocumentException(DocumentId, new HttpRequestException("Delete failed"));
        var agg = new AggregateException(new HttpRequestException("Linking failed"), orphaned);

        var result = ExceptionFormatter.FormatHttpStatusInfo(agg);

        Assert.Contains(DocumentId.ToString(), result);
        Assert.Contains("handmatig worden verwijderd", result);
    }

    [Fact]
    public void FormatHttpStatusInfo_OrphanedDocumentNestedInWrapper_StillDetected()
    {
        var orphaned = new OrphanedDocumentException(DocumentId, new HttpRequestException("Delete failed"));
        var agg = new AggregateException(new HttpRequestException("Linking failed"), orphaned);
        var wrapper = new Exception("outer", agg);

        var result = ExceptionFormatter.FormatHttpStatusInfo(wrapper);

        Assert.Contains(DocumentId.ToString(), result);
        Assert.Contains("handmatig worden verwijderd", result);
    }

    [Fact]
    public void FormatHttpStatusInfo_OrphanedDocumentDirect_ShowsMessage()
    {
        var orphaned = new OrphanedDocumentException(DocumentId, new HttpRequestException("Delete failed"));

        var result = ExceptionFormatter.FormatHttpStatusInfo(orphaned);

        Assert.Contains(DocumentId.ToString(), result);
    }

    #endregion

    #region HttpRequestException formatting

    [Fact]
    public void FormatHttpStatusInfo_SingleHttpException_ShowsStatusCodeAndMessage()
    {
        var ex = new HttpRequestException("Not Found", null, HttpStatusCode.NotFound);

        var result = ExceptionFormatter.FormatHttpStatusInfo(ex);

        Assert.Equal(" | HTTP 404: Not Found", result);
    }

    [Fact]
    public void FormatHttpStatusInfo_HttpExceptionWithoutStatusCode_ShowsMessage()
    {
        var ex = new HttpRequestException("Connection refused");

        var result = ExceptionFormatter.FormatHttpStatusInfo(ex);

        Assert.Equal(" | Connection refused", result);
    }

    [Fact]
    public void FormatHttpStatusInfo_MultipleHttpExceptionsInAggregate_ShowsAll()
    {
        var agg = new AggregateException(
            new HttpRequestException("Not Found", null, HttpStatusCode.NotFound),
            new HttpRequestException("Server Error", null, HttpStatusCode.InternalServerError));

        var result = ExceptionFormatter.FormatHttpStatusInfo(agg);

        Assert.Equal(" | HTTP 404: Not Found; HTTP 500: Server Error", result);
    }

    [Fact]
    public void FormatHttpStatusInfo_HttpExceptionNestedInInnerException_Found()
    {
        var inner = new HttpRequestException("Bad Gateway", null, HttpStatusCode.BadGateway);
        var wrapper = new InvalidOperationException("Something went wrong", inner);

        var result = ExceptionFormatter.FormatHttpStatusInfo(wrapper);

        Assert.Equal(" | HTTP 502: Bad Gateway", result);
    }

    #endregion

    #region AggregateException with non-HTTP exceptions

    [Fact]
    public void FormatHttpStatusInfo_AggregateWithNonHttpExceptions_ShowsTypeAndMessage()
    {
        var agg = new AggregateException(
            new InvalidOperationException("Op A failed"),
            new TimeoutException("Timed out"));

        var result = ExceptionFormatter.FormatHttpStatusInfo(agg);

        Assert.Equal(" | InvalidOperationException: Op A failed; TimeoutException: Timed out", result);
    }

    #endregion

    #region Generic exceptions

    [Fact]
    public void FormatHttpStatusInfo_GenericException_ShowsTypeAndMessage()
    {
        var ex = new InvalidOperationException("Something broke");

        var result = ExceptionFormatter.FormatHttpStatusInfo(ex);

        Assert.Equal(" | InvalidOperationException: Something broke", result);
    }

    #endregion

    #region OrphanedDocumentException takes priority

    [Fact]
    public void FormatHttpStatusInfo_AggregateWithOrphanedAndHttp_PrioritizesOrphanedMessage()
    {
        var orphaned = new OrphanedDocumentException(DocumentId, new HttpRequestException("Delete failed"));
        var agg = new AggregateException(
            new HttpRequestException("Not Found", null, HttpStatusCode.NotFound),
            orphaned);

        var result = ExceptionFormatter.FormatHttpStatusInfo(agg);

        Assert.Contains("handmatig worden verwijderd", result);
        Assert.DoesNotContain("HTTP 404", result);
    }

    #endregion
}
