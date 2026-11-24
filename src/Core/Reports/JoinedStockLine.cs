namespace Core.Reports;

public sealed record JoinedStockLine(
    string Sku,
    string Name,
    string Unit,
    int CurrentQuantity,
    int Received,
    int Issued,
    int MovementCount);