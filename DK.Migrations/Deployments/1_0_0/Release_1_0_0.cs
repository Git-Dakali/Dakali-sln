using Dakali.Interface.Connection;
using DK.DatabaseMigrations.Deployments;

namespace ICR.DatabaseMigrations.Deployments._1_0_0
{
    public class Release_1_0_0 : Migration
    {
        public Release_1_0_0 (ISession session) 
            : base (session)
        {
            SQLs.Add(@"
                CREATE TABLE WebHookEvents (
                    Id bigint identity(1, 1),
                    EventType varchar(255),
                    JSon text,
                    IsProcessed bit,
                    Error varchar(500)
                );
            ");

            GetProductSQL();
        }
        public override void BasicRun()
        {
        }

        public void GetProductSQL()
        {
            SQLs.Add(@"
                CREATE TABLE dbo.Category (
                  Id            BIGINT IDENTITY(1,1) PRIMARY KEY,
                  Code          NVARCHAR(64)  NOT NULL UNIQUE,
                  Name          NVARCHAR(200) NOT NULL,
                  CreationDate  DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  RemoveDate    DATETIME2 NULL,
                  UpdateDate    DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  Version       BIGINT    NOT NULL DEFAULT 1,
                  Guid          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
                  IsDeleted     BIT NOT NULL DEFAULT 0
                );
            ");

            SQLs.Add(@"
                CREATE TABLE dbo.StoredFile (
                  Id            BIGINT IDENTITY(1,1) PRIMARY KEY,
                  FileName      NVARCHAR(260) NOT NULL,
                  [Content]     VARBINARY(MAX) NOT NULL,
                  [Module]      NVARCHAR(100) NOT NULL,
                  CreationDate  DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  RemoveDate    DATETIME2 NULL,
                  UpdateDate    DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  Version       BIGINT    NOT NULL DEFAULT 1,
                  Guid          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
                  IsDeleted     BIT NOT NULL DEFAULT 0
                );
            ");

            SQLs.Add(@"
                CREATE TABLE dbo.Model (
                  Id            BIGINT IDENTITY(1,1) PRIMARY KEY,
                  Code          NVARCHAR(64) NOT NULL UNIQUE,
                  CategoryId    BIGINT NOT NULL
                      CONSTRAINT FK_Model_Category
                      REFERENCES dbo.Category(Id),
                  CreationDate  DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  RemoveDate    DATETIME2 NULL,
                  UpdateDate    DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  Version       BIGINT    NOT NULL DEFAULT 1,
                  Guid          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
                  IsDeleted     BIT NOT NULL DEFAULT 0
                );
            ");

            SQLs.Add(@"
                CREATE TABLE dbo.FieldGroup (
                    Id           BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_FieldGroup PRIMARY KEY,
                    ModelId BIGINT NOT NULL,
                    [Name]       NVARCHAR(150) NOT NULL,
                    SortOrder    INT NOT NULL CONSTRAINT DF_FieldGroup_SortOrder DEFAULT (1),
                    CreationDate DATETIME2(3) NOT NULL CONSTRAINT DF_FieldGroup_CreationDate DEFAULT (SYSUTCDATETIME()),
                    UpdateDate   DATETIME2(3) NULL,
                    RemoveDate   DATETIME2(3) NULL,
                    [Version]    INT NOT NULL CONSTRAINT DF_FieldGroup_Version DEFAULT (1),
                    [Guid]       UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_FieldGroup_Guid DEFAULT (NEWID()),
                    IsDeleted    BIT NOT NULL CONSTRAINT DF_FieldGroup_IsDeleted DEFAULT (0),

                    CONSTRAINT FK_FieldGroup_Model
                        FOREIGN KEY (ModelId) REFERENCES dbo.Model(Id)
                );
            ");
            SQLs.Add(@"
                CREATE INDEX IX_FieldGroup_ModelId ON dbo.FieldGroup(ModelId);
                
                CREATE UNIQUE INDEX UX_FieldGroup_Model_Name
                ON dbo.FieldGroup(ModelId, [Name])
                WHERE IsDeleted = 0;
                
            ");

            SQLs.Add(@"
                CREATE TABLE dbo.Field (
                  Id                  BIGINT IDENTITY(1,1) PRIMARY KEY,
                  CreationDate  DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  RemoveDate    DATETIME2 NULL,
                  UpdateDate    DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  Version       BIGINT    NOT NULL DEFAULT 1,
                  Guid          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
                  IsDeleted     BIT NOT NULL DEFAULT 0,
                  FieldGroupId BIGINT NOT NULL
                      CONSTRAINT FK_Field_FieldGroup
                      REFERENCES dbo.FieldGroup(Id),
                  [Name]              NVARCHAR(150) NOT NULL,
                  SortOrder           INT NOT NULL DEFAULT 1,
                  CONSTRAINT UX_Field UNIQUE(FieldGroupId, [Name])
                );

            ");
            SQLs.Add(@"
                CREATE TABLE dbo.Size (
                  Id             BIGINT IDENTITY(1,1) PRIMARY KEY,
                  CreationDate  DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  RemoveDate    DATETIME2 NULL,
                  UpdateDate    DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  Version       BIGINT    NOT NULL DEFAULT 1,
                  Guid          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
                  IsDeleted     BIT NOT NULL DEFAULT 0,
                  ModelId BIGINT NOT NULL
                      CONSTRAINT FK_Size_Model
                      REFERENCES dbo.Model(Id),
                  [Name]         NVARCHAR(50) NOT NULL,
                  SortOrder      INT NOT NULL DEFAULT 1
                );
            ");
            SQLs.Add(@"
                CREATE TABLE dbo.Product (
                  Id            BIGINT IDENTITY(1,1) PRIMARY KEY,
                  [Name]        NVARCHAR(200) NOT NULL,
                  [Description] NVARCHAR(MAX) NULL,
                  ModelId BIGINT NOT NULL
                      CONSTRAINT FK_Product_Model
                      REFERENCES dbo.Model(Id),
                  CreationDate  DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  RemoveDate    DATETIME2 NULL,
                  UpdateDate    DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  Version       BIGINT    NOT NULL DEFAULT 1,
                  Guid          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
                  IsDeleted     BIT NOT NULL DEFAULT 0
                );

                CREATE INDEX IX_Product_ModelId ON dbo.Product(ModelId);
                CREATE INDEX IX_Product_IsDeleted     ON dbo.Product(IsDeleted);
            ");
            SQLs.Add(@"
                CREATE TABLE dbo.Variant (
                  Id            BIGINT IDENTITY(1,1) PRIMARY KEY,
                  ProductId     BIGINT NOT NULL
                      CONSTRAINT FK_Variant_Product
                      REFERENCES dbo.Product(Id),
                  [Size]        NVARCHAR(50)  NOT NULL,
                  Cost          DECIMAL(18,2) NOT NULL DEFAULT 0,
                  CreationDate  DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  RemoveDate    DATETIME2 NULL,
                  UpdateDate    DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  Version       BIGINT    NOT NULL DEFAULT 1,
                  Guid          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
                  IsDeleted     BIT NOT NULL DEFAULT 0
                );

                CREATE INDEX IX_Variant_ProductId ON dbo.Variant(ProductId);
                CREATE INDEX IX_Variant_Size      ON dbo.Variant([Size]);
            ");
            SQLs.Add(@"
                CREATE TABLE dbo.Color (
                  Id          BIGINT IDENTITY(1,1) PRIMARY KEY,
                  CreationDate  DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  RemoveDate    DATETIME2 NULL,
                  UpdateDate    DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  Version       BIGINT    NOT NULL DEFAULT 1,
                  Guid          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
                  IsDeleted     BIT NOT NULL DEFAULT 0,
                  VariantId     BIGINT NOT NULL
                      CONSTRAINT FK_Color_Variant
                      REFERENCES dbo.Variant(Id),
                  Name          NVARCHAR(500) NOT NULL,
                  Hex           NVARCHAR(16) NOT NULL,   
                  SortOrder   INT NOT NULL DEFAULT 1,
                  CONSTRAINT UX_Color UNIQUE(VariantId, Hex)
                );
            ");
            SQLs.Add(@"
                CREATE TABLE dbo.Image (
                  Id            BIGINT IDENTITY(1,1) PRIMARY KEY,
                  VariantId     BIGINT NOT NULL
                      CONSTRAINT FK_Image_Variant
                      REFERENCES dbo.Variant(Id),
                  StoredFileId  BIGINT NOT NULL
                      CONSTRAINT FK_Image_StoredFile
                      REFERENCES dbo.StoredFile(Id),
                  IsPrimary     BIT NOT NULL DEFAULT 0,
                  SortOrder     INT NOT NULL DEFAULT 1,
                  CreationDate  DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  RemoveDate    DATETIME2 NULL,
                  UpdateDate    DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  Version       BIGINT    NOT NULL DEFAULT 1,
                  Guid          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
                  IsDeleted     BIT NOT NULL DEFAULT 0
                );

                CREATE UNIQUE INDEX UX_Image_Primary
                  ON dbo.Image(VariantId, IsPrimary)
                  WHERE IsPrimary = 1;

                CREATE INDEX IX_Image_StoredFileId ON dbo.Image(StoredFileId);
                CREATE INDEX IX_Image_VariantId    ON dbo.Image(VariantId);
            ");
            SQLs.Add(@"
                CREATE TABLE dbo.Attribute (
                  Id            BIGINT IDENTITY(1,1) PRIMARY KEY,
                  VariantId     BIGINT NOT NULL
                      CONSTRAINT FK_Attribute_Variant
                      REFERENCES dbo.Variant(Id),
                  [Field]       NVARCHAR(150)  NOT NULL,
                  [Value]       NVARCHAR(4000) NOT NULL,
                  CreationDate  DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  RemoveDate    DATETIME2 NULL,
                  UpdateDate    DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  Version       BIGINT    NOT NULL DEFAULT 1,
                  Guid          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
                  IsDeleted     BIT NOT NULL DEFAULT 0
                );

                CREATE INDEX IX_Attribute_VariantId ON dbo.Attribute(VariantId);
                CREATE INDEX IX_Attribute_Field     ON dbo.Attribute([Field]);
            ");
        }
    }
}
