USE [HGSDB]
GO
CREATE TRIGGER [dbo].[trg_EnsureSingleActiveBatch]
ON [dbo].[GradeBatches]
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Cập nhật các batch khác thành "Không Hoạt Động" nếu có batch mới được đặt là "Hoạt Động"
    UPDATE GradeBatches
    SET Status = N'Không Hoạt Động'
    WHERE BatchID NOT IN (SELECT BatchID FROM inserted)
          AND Status = N'Hoạt Động'
          AND EXISTS (
              SELECT 1
              FROM inserted
              WHERE Status = N'Hoạt Động'
          );
END;
GO
ALTER TABLE [dbo].[GradeBatches] ENABLE TRIGGER [trg_EnsureSingleActiveBatch]
GO
/****** Object:  Trigger [dbo].[trg_EnsureOnlyOneActiveTimetable]    Script Date: 4/28/2025 1:42:40 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[trg_EnsureOnlyOneActiveTimetable]
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