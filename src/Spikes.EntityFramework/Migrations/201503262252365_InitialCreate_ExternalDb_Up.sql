CREATE SCHEMA Docs AUTHORIZATION dbo
GO

CREATE SYNONYM Docs.ExternalFileContents 
FOR SpikesExternalDb.dbo.FileContents;
GO

CREATE VIEW	dbo.FileContentInfoes
AS  
SELECT fh.Id AS FilterHeaderId,
       fc.FileName,
       fc.FileExtension,
       fc.ContentType
  FROM
       dbo.FileHeaders fh
       INNER JOIN Docs.ExternalFileContents fc ON fc.MediaGroupId = fh.MediaGroupId