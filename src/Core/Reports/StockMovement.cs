namespace Core.Reports;

public sealed record StockMovement(
    string ProductId,
    string Operation,
    int Quantity);