
namespace Core.Domain.Library;

public sealed class Reader
{
    public string Id { get; }

    private Reader(string id)
    {
        Id = id;
    }

    public static Reader Create(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException(
                "Ідентифікатор читача обов'язковий", nameof(id));

        return new Reader(id.Trim());
    }
}

public sealed class BookCopy
{
    public string Id { get; }
    public bool IsOnLoan { get; private set; }

    private BookCopy(string id)
    {
        Id = id;
    }

    public static BookCopy Create(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException(
                "Ідентифікатор примірника обов'язковий", nameof(id));

        return new BookCopy(id.Trim());
    }

    internal void MarkIssued()
    {
        if (IsOnLoan)
            throw new InvalidOperationException(
                "Примірник уже виданий");

        IsOnLoan = true;
    }
}

public sealed class Loan
{
    public string ReaderId { get; }
    public string CopyId { get; }
    public bool IsOpen { get; private set; }

    private Loan(string readerId, string copyId)
    {
        ReaderId = readerId;
        CopyId = copyId;
        IsOpen = true;
    }

    internal static Loan Open(string readerId, string copyId)
        => new(readerId, copyId);

    public void ReturnCopy()
    {
        if (!IsOpen)
            throw new InvalidOperationException(
                "Видача вже закрита");

        IsOpen = false;
    }
}

public static class LoanService
{
    private const int MaxOpenLoans = 5;

    public static Loan IssueCopy(
        Reader reader,
        BookCopy copy,
        IReadOnlyCollection<Loan> loans)
    {
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentNullException.ThrowIfNull(copy);
        ArgumentNullException.ThrowIfNull(loans);

        int openLoans = loans.Count(loan =>
            loan.ReaderId == reader.Id && loan.IsOpen);

        if (openLoans >= MaxOpenLoans)
            throw new InvalidOperationException(
                "Читач уже має 5 відкритих видач");

        copy.MarkIssued();

        return Loan.Open(reader.Id, copy.Id);
    }
}
