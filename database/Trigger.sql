USE [HGSDB]
GO
CREATE TRIGGER trg_EnsureOnlyOneActiveTimetable
ON [dbo].[Timetables]
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Chỉ xử lý nếu có cái nào đang được set là Hoạt động
    IF EXISTS (SELECT 1 FROM inserted WHERE Status = N'Hoạt động')
    BEGIN
        UPDATE dbo.Timetables
        SET Status = N'Không hoạt động'
        WHERE Status = N'Hoạt động'
          AND TimetableId NOT IN (SELECT TimetableId FROM inserted);
    END
END;
ALTER TABLE [dbo].[Timetables]
ALTER COLUMN [Status] NVARCHAR(20) NOT NULL;

GO

CREATE TRIGGER trg_EnsureSingleActiveBatch
ON GradeBatches
INSTEAD OF INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Update all other batches to 'Không Hoạt Động' if any inserted/updated batch is 'Hoạt Động'
    UPDATE GradeBatches
    SET Status = N'Không Hoạt Động'
    WHERE BatchID NOT IN (SELECT BatchID FROM inserted)
          AND Status = N'Hoạt Động'
          AND EXISTS (
              SELECT 1 FROM inserted WHERE Status = N'Hoạt Động'
          );

    -- Reapply the INSERT or UPDATE manually
    MERGE GradeBatches AS target
    USING inserted AS source
    ON target.BatchID = source.BatchID
    WHEN MATCHED THEN
        UPDATE SET 
            BatchName = source.BatchName,
            SemesterID = source.SemesterID,
            StartDate = source.StartDate,
            EndDate = source.EndDate,
            Status = source.Status
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (BatchName, SemesterID, StartDate, EndDate, Status)
        VALUES (source.BatchName, source.SemesterID, source.StartDate, source.EndDate, source.Status);
END;
