DECLARE @InterID INT,
        @EntryID INT,
        @ItemID INT,
        @BatchNo NVARCHAR(500)
SELECT  @InterID=8524
       ,@EntryID=1
       ,@ItemID=4734

SELECT
    S1.FNumber AS [�ֿ����],
    ICI.FItemID,
    INV.FStockID,
    ICI.FName AS [��������],
    S1.FName AS [�ֿ�����],
    INV.FQty AS [�������],
    INV.FBatchNo AS [����],
    ISNULL(LS.FQty,0) AS [��������],
    ISNULL(OLS.FQty,0) AS [��������],
    INV.FQty-ISNULL(LS.FQty,0)-ISNULL(OLS.FQty,0) AS [��������]
FROM t_ICItem AS ICI
INNER JOIN ICInventory AS INV ON ICI.FItemID=INV.FItemID
INNER JOIN t_Stock AS S1 ON INV.FStockID=S1.FItemID
LEFT JOIN(
    SELECT
         LS.FStockID
        ,LS.FItemID
        ,LS.FBatchNo
        ,SUM(LS.FQty) AS FQty
    FROM t_LockStock AS LS
    WHERE LS.FInterID=@InterID AND LS.FEntryID=@EntryID
    GROUP BY LS.FStockID,LS.FItemID,LS.FBatchNo
    ) AS LS ON LS.FStockID=INV.FStockID AND LS.FItemID=INV.FItemID AND LS.FBatchNo=INV.FBatchNo
LEFT JOIN(
    SELECT
         LS.FStockID
        ,LS.FItemID
        ,LS.FBatchNo
        ,SUM(LS.FQty) AS FQty
    FROM t_LockStock AS LS
    WHERE LS.FInterID<>@InterID AND LS.FEntryID<>@EntryID
    GROUP BY LS.FStockID,LS.FItemID,LS.FBatchNo
    ) AS OLS ON OLS.FStockID=INV.FStockID AND OLS.FItemID=INV.FItemID  AND OLS.FBatchNo=INV.FBatchNo
WHERE ICI.FItemID=@ItemID