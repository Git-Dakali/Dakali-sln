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
                CREATE TABLE dbo.ProductCategory (
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
                CREATE TABLE dbo.ProductModel (
                  Id            BIGINT IDENTITY(1,1) PRIMARY KEY,
                  Code          NVARCHAR(64) NOT NULL UNIQUE,
                  CategoryId    BIGINT NOT NULL
                      CONSTRAINT FK_ProductModel_ProductCategory
                      REFERENCES dbo.ProductCategory(Id),
                  CreationDate  DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  RemoveDate    DATETIME2 NULL,
                  UpdateDate    DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  Version       BIGINT    NOT NULL DEFAULT 1,
                  Guid          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
                  IsDeleted     BIT NOT NULL DEFAULT 0
                );
            ");

            SQLs.Add(@"
                CREATE TABLE dbo.ProductFieldGroup (
                  Id            BIGINT IDENTITY(1,1) PRIMARY KEY,
                  [Name]        NVARCHAR(200) NOT NULL,
                  CreationDate  DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  RemoveDate    DATETIME2 NULL,
                  UpdateDate    DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  Version       BIGINT    NOT NULL DEFAULT 1,
                  Guid          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
                  IsDeleted     BIT NOT NULL DEFAULT 0
                );
            ");

            SQLs.Add(@"
                CREATE TABLE dbo.ProductFieldGroupField (
                  Id                  BIGINT IDENTITY(1,1) PRIMARY KEY,
                  ProductFieldGroupId BIGINT NOT NULL
                      CONSTRAINT FK_ProductFieldGroupField_ProductFieldGroup
                      REFERENCES dbo.ProductFieldGroup(Id),
                  [Name]              NVARCHAR(150) NOT NULL,
                  SortOrder           INT NOT NULL DEFAULT 1,
                  CONSTRAINT UX_ProductFieldGroupField UNIQUE(ProductFieldGroupId, [Name])
                );

            ");
            SQLs.Add(@"
                CREATE TABLE dbo.ProductModelFieldGroup (
                  ProductModelId      BIGINT NOT NULL
                      CONSTRAINT FK_ProductModelFieldGroup_ProductModel
                      REFERENCES dbo.ProductModel(Id),
                  ProductFieldGroupId BIGINT NOT NULL
                      CONSTRAINT FK_ProductModelFieldGroup_ProductFieldGroup
                      REFERENCES dbo.ProductFieldGroup(Id),
                  SortOrder           INT NOT NULL DEFAULT 1,
                  CONSTRAINT PK_ProductModelFieldGroup PRIMARY KEY (ProductModelId, ProductFieldGroupId)
                );
            ");
            SQLs.Add(@"
                CREATE TABLE dbo.ProductModelSize (
                  Id             BIGINT IDENTITY(1,1) PRIMARY KEY,
                  ProductModelId BIGINT NOT NULL
                      CONSTRAINT FK_ProductModelSize_ProductModel
                      REFERENCES dbo.ProductModel(Id),
                  [Name]         NVARCHAR(50) NOT NULL,
                  SortOrder      INT NOT NULL DEFAULT 1,
                  CONSTRAINT UX_ProductModelSize UNIQUE(ProductModelId, [Name])
                );
            ");
            SQLs.Add(@"
                CREATE TABLE dbo.Product (
                  Id            BIGINT IDENTITY(1,1) PRIMARY KEY,
                  [Name]        NVARCHAR(200) NOT NULL,
                  [Description] NVARCHAR(MAX) NULL,
                  ProductModelId BIGINT NOT NULL
                      CONSTRAINT FK_Product_ProductModel
                      REFERENCES dbo.ProductModel(Id),
                  CreationDate  DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  RemoveDate    DATETIME2 NULL,
                  UpdateDate    DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  Version       BIGINT    NOT NULL DEFAULT 1,
                  Guid          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
                  IsDeleted     BIT NOT NULL DEFAULT 0
                );

                CREATE INDEX IX_Product_ProductModelId ON dbo.Product(ProductModelId);
                CREATE INDEX IX_Product_IsDeleted     ON dbo.Product(IsDeleted);
            ");
            SQLs.Add(@"
                CREATE TABLE dbo.ProductVariant (
                  Id            BIGINT IDENTITY(1,1) PRIMARY KEY,
                  ProductId     BIGINT NOT NULL
                      CONSTRAINT FK_ProductVariant_Product
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

                CREATE INDEX IX_ProductVariant_ProductId ON dbo.ProductVariant(ProductId);
                CREATE INDEX IX_ProductVariant_Size      ON dbo.ProductVariant([Size]);
            ");
            SQLs.Add(@"
                CREATE TABLE dbo.ProductVariantColor (
                  Id          BIGINT IDENTITY(1,1) PRIMARY KEY,
                  VariantId   BIGINT NOT NULL
                      CONSTRAINT FK_ProductVariantColor_ProductVariant
                      REFERENCES dbo.ProductVariant(Id),
                  Hex         NVARCHAR(16) NOT NULL,   -- ej: ""#000000""
                  SortOrder   INT NOT NULL DEFAULT 1,
                  CONSTRAINT UX_ProductVariantColor UNIQUE(VariantId, Hex)
                );
            ");
            SQLs.Add(@"
                CREATE TABLE dbo.ProductImage (
                  Id            BIGINT IDENTITY(1,1) PRIMARY KEY,
                  VariantId     BIGINT NOT NULL
                      CONSTRAINT FK_ProductImage_ProductVariant
                      REFERENCES dbo.ProductVariant(Id),
                  StoredFileId  BIGINT NOT NULL
                      CONSTRAINT FK_ProductImage_StoredFile
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

                CREATE UNIQUE INDEX UX_ProductImage_Primary
                  ON dbo.ProductImage(VariantId, IsPrimary)
                  WHERE IsPrimary = 1;

                CREATE INDEX IX_ProductImage_StoredFileId ON dbo.ProductImage(StoredFileId);
                CREATE INDEX IX_ProductImage_VariantId    ON dbo.ProductImage(VariantId);
            ");
            SQLs.Add(@"
                CREATE TABLE dbo.ProductAttribute (
                  Id            BIGINT IDENTITY(1,1) PRIMARY KEY,
                  VariantId     BIGINT NOT NULL
                      CONSTRAINT FK_ProductAttribute_ProductVariant
                      REFERENCES dbo.ProductVariant(Id),
                  [Field]       NVARCHAR(150)  NOT NULL,
                  [Value]       NVARCHAR(4000) NOT NULL,
                  CreationDate  DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  RemoveDate    DATETIME2 NULL,
                  UpdateDate    DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  Version       BIGINT    NOT NULL DEFAULT 1,
                  Guid          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
                  IsDeleted     BIT NOT NULL DEFAULT 0
                );

                CREATE INDEX IX_ProductAttribute_VariantId ON dbo.ProductAttribute(VariantId);
                CREATE INDEX IX_ProductAttribute_Field     ON dbo.ProductAttribute([Field]);
            ");
        }
    }
}
