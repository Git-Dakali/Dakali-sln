using Dakali.Interface.Connection;
using DK.DatabaseMigrations.Deployments;
using DK.Domain.GeographicLocation;
using DK.Domain.Locations;
using DK.Domain.Products;
using DK.Domain.RoadMaps;
using DK.Domain.Sales;
using DK.Process.GeographicLocation;
using DK.Process.Locations;
using DK.Process.Product;
using DK.Process.RoadMaps;
using DK.Process.Sales;
using DK.Repositories.GeographicLocation;
using DK.Repositories.Locations;
using DK.Repositories.Products;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;


namespace ICR.DatabaseMigrations.Deployments._1_0_0
{
    public class Release_1_0_0 : Migration
    {
        private readonly IServiceProvider _serviceProvider;

        public Release_1_0_0 (ISession session, IServiceProvider serviceProvider) 
            : base (session)
        {
            _serviceProvider = serviceProvider;

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
        public async override Task BasicRun()
        {
            await CreateOriginSale();
            await CreateCategory();
            await CreateCity();
            await CreateModel();
            await CreateLocationState();
            await CreateHallway();
            await CreateColumn();
            await CreateLevel();
            await CreateLocation();
            await CreateProduct();
            await CreateTaxStatus();
            await CreateDriver();
        }

        public async Task CreateCategory()
        {
            var process = _serviceProvider.GetService<CategoryProcess>();
            var categoryZapato = new Category();
            categoryZapato.Code = "ZAP1";
            categoryZapato.Name = "ZAPATO";

            var categoryZapatilla = new Category();
            categoryZapatilla.Code = "ZAP2";
            categoryZapatilla.Name = "ZAPATILLA";

            await process.Create(categoryZapato);
            await process.Create(categoryZapatilla);

        }

        public async Task CreateModel()
        {
            var process = _serviceProvider.GetService<ModelProcess>();
            var categoryRepository = _serviceProvider.GetService<CategoryRepository>();

            var medidas = new FieldGroup()
            {
                Name = "Medidas",
                Fields = new List<Field>() { new Field() { Name="Alto"}, new Field() { Name = "Ancho" }, new Field() { Name = "Largo" } }
            };

            var model1800 = new Model()
            {
                Code = "1800",
                Category = await categoryRepository.Get("ZAP1"),
                VariantNames = new List<string>() { "45", "44", "43", "42", "41", "40", "39", "38" },
                FieldGroups = new List<FieldGroup>() { medidas }
            };

            var model4000 = new Model()
            {
                Code = "4000",
                Category = await categoryRepository.Get("ZAP1"),
                VariantNames = new List<string>() { "45", "44", "43", "42", "41", "40", "39", "38" },
                FieldGroups = new List<FieldGroup>() { medidas }
            };

            await process.Create(model1800);
            await process.Create(model4000);
        }

        public async Task CreateLocationState()
        {
            var process = _serviceProvider.GetService<LocationStateProcess>();
            var estadoDisponible = new LocationState();
            estadoDisponible.Code = "DIS";
            estadoDisponible.Name = "DISPONIBLE";

            await process.Create(estadoDisponible);
        }

        public async Task CreateHallway()
        {
            var process = _serviceProvider.GetService<HallwayProcess>();

            var pasillo1 = new Hallway();
            pasillo1.Code = "PAS1";
            pasillo1.Name = "PASILLO 1";

            var pasillo2 = new Hallway();
            pasillo2.Code = "PAS2";
            pasillo2.Name = "PASILLO 2";

            var pasillo3 = new Hallway();
            pasillo3.Code = "PAS3";
            pasillo3.Name = "PASILLO 3";

            var pasillo4 = new Hallway();
            pasillo4.Code = "PAS4";
            pasillo4.Name = "PASILLO 4";

            var pasillo5 = new Hallway();
            pasillo5.Code = "PAS5";
            pasillo5.Name = "PASILLO 5";

            
            await process.Create(pasillo1);
            await process.Create(pasillo2);
            await process.Create(pasillo3);
            await process.Create(pasillo4);
            await process.Create(pasillo5);
        }

        public async Task CreateColumn()
        {
            var process = _serviceProvider.GetService<ColumnProcess>();

            var column1 = new Column();
            column1.Code = "COL1";
            column1.Name = "COLUMNA 1";

            var column2 = new Column();
            column2.Code = "COL2";
            column2.Name = "COLUMNA 2";

            var column3 = new Column();
            column3.Code = "COL3";
            column3.Name = "COLUMNA 3";

            var column4 = new Column();
            column4.Code = "COL4";
            column4.Name = "COLUMNA 4";

            var column5 = new Column();
            column5.Code = "COL5";
            column5.Name = "COLUMNA 5";


            await process.Create(column1);
            await process.Create(column2);
            await process.Create(column3);
            await process.Create(column4);
            await process.Create(column5);
        }

        public async Task CreateLevel()
        {
            var process = _serviceProvider.GetService<LevelProcess>();

            var level1 = new Level();
            level1.Code = "NIV1";
            level1.Name = "NIVEL 1";

            var level2 = new Level();
            level2.Code = "NIV2";
            level2.Name = "NIVEL 2";

            var level3 = new Level();
            level3.Code = "NIV3";
            level3.Name = "NIVEL 3";

            var level4 = new Level();
            level4.Code = "NIV4";
            level4.Name = "NIVEL 4";

            var level5 = new Level();
            level5.Code = "NIV5";
            level5.Name = "NIVEL 5";


            await process.Create(level1);
            await process.Create(level2);
            await process.Create(level3);
            await process.Create(level4);
            await process.Create(level5);
        }

        public async Task CreateLocation()
        {
            var process = _serviceProvider.GetService<LocationProcess>();
            var hallwayRepository = _serviceProvider.GetService<HallwayRepository>();
            var columnRepository = _serviceProvider.GetService<ColumnRepository>();
            var levelRepository = _serviceProvider.GetService<LevelRepository>();
            var locationStateRepository = _serviceProvider.GetService<LocationStateRepository>();
            var disponible = await locationStateRepository.Get("DIS");

            var location1 = new Location();
            location1.Hallway = await hallwayRepository.Get("PAS1");
            location1.Column = await columnRepository.Get("COL1");
            location1.Level = await levelRepository.Get("NIV1");
            location1.State = disponible;

            var location2 = new Location();
            location2.Hallway = await hallwayRepository.Get("PAS1");
            location2.Column = await columnRepository.Get("COL1");
            location2.Level = await levelRepository.Get("NIV2");
            location2.State = disponible;
            
            var location3 = new Location();
            location3.Hallway = await hallwayRepository.Get("PAS1");
            location3.Column = await columnRepository.Get("COL1");
            location3.Level = await levelRepository.Get("NIV3");
            location3.State = disponible;

            var location4 = new Location();
            location4.Hallway = await hallwayRepository.Get("PAS1");
            location4.Column = await columnRepository.Get("COL1");
            location4.Level = await levelRepository.Get("NIV4");
            location4.State = disponible;

            var location5 = new Location();
            location5.Hallway = await hallwayRepository.Get("PAS1");
            location5.Column = await columnRepository.Get("COL1");
            location5.Level = await levelRepository.Get("NIV5");
            location5.State = disponible;

            var location6 = new Location();
            location6.Hallway = await hallwayRepository.Get("PAS1");
            location6.Column = await columnRepository.Get("COL2");
            location6.Level = await levelRepository.Get("NIV1");
            location6.State = disponible;

            var location7 = new Location();
            location7.Hallway = await hallwayRepository.Get("PAS1");
            location7.Column = await columnRepository.Get("COL3");
            location7.Level = await levelRepository.Get("NIV1");
            location7.State = disponible;

            var location8 = new Location();
            location8.Hallway = await hallwayRepository.Get("PAS1");
            location8.Column = await columnRepository.Get("COL4");
            location8.Level = await levelRepository.Get("NIV1");
            location8.State = disponible;

            var location9 = new Location();
            location9.Hallway = await hallwayRepository.Get("PAS1");
            location9.Column = await columnRepository.Get("COL5");
            location9.Level = await levelRepository.Get("NIV1");
            location9.State = disponible;

            var location10 = new Location();
            location10.Hallway = await hallwayRepository.Get("PAS2");
            location10.Column = await columnRepository.Get("COL1");
            location10.Level = await levelRepository.Get("NIV1");
            location10.State = disponible;

            var location11 = new Location();
            location11.Hallway = await hallwayRepository.Get("PAS3");
            location11.Column = await columnRepository.Get("COL1");
            location11.Level = await levelRepository.Get("NIV1");
            location11.State = disponible;

            var location12 = new Location();
            location12.Hallway = await hallwayRepository.Get("PAS4");
            location12.Column = await columnRepository.Get("COL1");
            location12.Level = await levelRepository.Get("NIV1");
            location12.State = disponible;

            var location13 = new Location();
            location13.Hallway = await hallwayRepository.Get("PAS5");
            location13.Column = await columnRepository.Get("COL1");
            location13.Level = await levelRepository.Get("NIV1");
            location13.State = disponible;


            await process.Create(location1);
            await process.Create(location2);
            await process.Create(location3);
            await process.Create(location4);
            await process.Create(location5);
            await process.Create(location6);
            await process.Create(location7);
            await process.Create(location8);
            await process.Create(location9);
            await process.Create(location10);
            await process.Create(location11);
            await process.Create(location12);
            await process.Create(location13);
        }

        public async Task CreateProduct()
        {
            var process = _serviceProvider.GetService<ProductProcess>();
            var modelRepository = _serviceProvider.GetService<ModelRepository>();

            var product1800 = new Product();
            product1800.Name = "Zapatos de Vestir";
            product1800.Description = "Descripcion Zapatos de Vestir";
            product1800.Model = await modelRepository.Get("1800");

            ConfigureProduct(product1800);
            var newProduct1800 = await process.Create(product1800);
            await CreateStock(newProduct1800);

            var product4000 = new Product();
            product4000.Name = "Mocasines Cuero";
            product4000.Description = "Descripcion Mocasines Cuero";
            product4000.Model = await modelRepository.Get("4000");

            ConfigureProduct(product4000);
            var newProduct4000 = await process.Create(product4000);
            await CreateStock(newProduct4000);

            await CreateSale(newProduct4000);
        }

        public async Task CreateDriver()
        {
            var process = _serviceProvider.GetService<DriverProcess>();
            
            var david = new Driver();
            david.Dni = "37188173";
            david.FirstName = "David";
            david.LastName = "Calizaya LLanos";

            var jhoasil = new Driver();
            jhoasil.Dni = "11222333";
            jhoasil.FirstName = "Jhoasil";
            jhoasil.LastName = "Garcia";

            var armando = new Driver();
            armando.Dni = "11222333";
            armando.FirstName = "Armando";
            armando.LastName = "Manzaneda";

            await process.Create(david);
            await process.Create(jhoasil);
            await process.Create(armando);
        }

        public async Task CreateCity()
        {
            var countryProcess = _serviceProvider.GetService<CountryProcess>();
            var provinceProcess = _serviceProvider.GetService<ProvinceProcess>();
            var cityProcess = _serviceProvider.GetService<CityProcess>();
            var pais = await countryProcess.Create(new Country() { Code = "ARG", Name = "Argentina" });

            await CreateCity(provinceProcess, cityProcess, pais, "GBA", "Buenos Aires");
            await CreateCity(provinceProcess, cityProcess, pais, "CABA", "Ciudad Autónoma de Buenos Aires");
            await CreateCity(provinceProcess, cityProcess, pais, "CAT", "Catamarca");
            await CreateCity(provinceProcess, cityProcess, pais, "CHA", "Chaco");
            await CreateCity(provinceProcess, cityProcess, pais, "CHU", "Chubut");
            await CreateCity(provinceProcess, cityProcess, pais, "CBA", "Córdoba");
            await CreateCity(provinceProcess, cityProcess, pais, "COR", "Corrientes");
            await CreateCity(provinceProcess, cityProcess, pais, "ER", "Entre Ríos");
            await CreateCity(provinceProcess, cityProcess, pais, "FOR", "Formosa");
            await CreateCity(provinceProcess, cityProcess, pais, "JUJ", "Jujuy");
            await CreateCity(provinceProcess, cityProcess, pais, "LP", "La Pampa");
            await CreateCity(provinceProcess, cityProcess, pais, "LR", "La Rioja");
            await CreateCity(provinceProcess, cityProcess, pais, "MZA", "Mendoza");
            await CreateCity(provinceProcess, cityProcess, pais, "MIS", "Misiones");
            await CreateCity(provinceProcess, cityProcess, pais, "NQN", "Neuquén");
            await CreateCity(provinceProcess, cityProcess, pais, "RN", "Río Negro");
            await CreateCity(provinceProcess, cityProcess, pais, "SAL", "Salta");
            await CreateCity(provinceProcess, cityProcess, pais, "SJ", "San Juan");
            await CreateCity(provinceProcess, cityProcess, pais, "SL", "San Luis");
            await CreateCity(provinceProcess, cityProcess, pais, "SC", "Santa Cruz");
            await CreateCity(provinceProcess, cityProcess, pais, "SF", "Santa Fe");
            await CreateCity(provinceProcess, cityProcess, pais, "SE", "Santiago del Estero");
            await CreateCity(provinceProcess, cityProcess, pais, "TDF", "Tierra del Fuego");
            await CreateCity(provinceProcess, cityProcess, pais, "TUC", "Tucumán");
        }

        public async Task CreateCity(ProvinceProcess provinceProcess, CityProcess cityProcess, Country country, string code, string name)
        {
            var provincia = await provinceProcess.Create(new Province() { Code = code, Name = name, Country = country });
            var citiesJson = System.IO.File.ReadAllText($"C:\\Dakali\\Git\\Dakali-sln\\DK.Migrations\\bin\\Debug\\net9.0\\Deployments\\1_0_0\\File\\{code}.js");
            var results = JsonConvert.DeserializeObject<List<dynamic>>(citiesJson);

            foreach (var item in results.Where(x => x.cp != null))
                await cityProcess.Create(new City() { ZipCode = item.cp, Name = item.nombre, Province = provincia });

            Console.WriteLine($"Provincia: {name} - Localidades: {results.Count}");
        }

        public async Task CreateOriginSale()
        {
            var process = _serviceProvider.GetService<OriginSaleProcess>();

            var originSale1 = new OriginSale();
            originSale1.Code = "MPH";
            originSale1.Name = "Mercado Libre Hugo";

            var originSale2 = new OriginSale();
            originSale2.Code = "MPD";
            originSale2.Name = "Mercado Libre David";

            var originSale3 = new OriginSale();
            originSale3.Code = "MPL";
            originSale3.Name = "Mercado Libre Lourdes";

            var originSale4 = new OriginSale();
            originSale4.Code = "DKL";
            originSale4.Name = "Dakali";

            await process.Create(originSale1);
            await process.Create(originSale2);
            await process.Create(originSale3);
            await process.Create(originSale4);
        }

        public async Task CreateTaxStatus()
        {
            var process = _serviceProvider.GetService<TaxStatusProcess>();

            var originSale1 = new TaxStatus();
            originSale1.Code = "MON";
            originSale1.Name = "Monotributo";

            var originSale2 = new TaxStatus();
            originSale2.Code = "CF";
            originSale2.Name = "Consumidor Final";

            var originSale3 = new TaxStatus();
            originSale3.Code = "RI";
            originSale3.Name = "Responsable Inscripto";

            await process.Create(originSale1);
            await process.Create(originSale2);
            await process.Create(originSale3);
        }

        public async Task CreateStock(Product product)
        {
            var locationProcess = _serviceProvider.GetService<LocationProcess>();
            var stockProcess = _serviceProvider.GetService<StockProcess>();
            
            var location = await locationProcess.Get("PAS1", "COL1", "NIV1");

            foreach (var variant in product.Variants)
            {
                foreach (var color in variant.ColorsHex)
                {
                    var stock = new Stock();
                    stock.Location = location;
                    stock.Product = product;
                    stock.Variant = variant;
                    stock.Color = color;

                    var newStock = await stockProcess.Create(stock);
                    await stockProcess.StockEntry(newStock, 100);
                }
            }
        }

        public async Task CreateSale(Product product)
        {
            var saleProcess = _serviceProvider.GetService<SaleProcess>();
            var cityProcess = _serviceProvider.GetService<CityProcess>();
            var originSaleProcess = _serviceProvider.GetService<OriginSaleProcess>();

            var city = await cityProcess.Get("1842");
            var originSale = await originSaleProcess.Get("DKL");
            var variant = product.Variants.First();
            var color = variant.ColorsHex.First();
            var total = 30;
            var count = 1;


            await CreateSale(saleProcess, 1, "Urdininea 4783", originSale, await cityProcess.Get("1757"), product, variant, color);
            await CreateSale(saleProcess, 2, "Echeverria 7086", originSale, await cityProcess.Get("1757"), product, variant, color);
            await CreateSale(saleProcess, 3, "Luis Galvani 184", originSale, await cityProcess.Get("1755"), product, variant, color);
            await CreateSale(saleProcess, 4, "Egipto 895", originSale, await cityProcess.Get("1722"), product, variant, color);
            await CreateSale(saleProcess, 5, "Ayacucho 1714", originSale, await cityProcess.Get("1722"), product, variant, color);
            await CreateSale(saleProcess, 6, "Emilio Caraffa 1427", originSale, await cityProcess.Get("1722"), product, variant, color);
            await CreateSale(saleProcess, 7, "Ricardo Guiraldes 222", originSale, await cityProcess.Get("1722"), product, variant, color);
            await CreateSale(saleProcess, 8, "Anchoris 2895", originSale, await cityProcess.Get("1722"), product, variant, color);
            await CreateSale(saleProcess, 9, "Glew 2237", originSale, await cityProcess.Get("1744"), product, variant, color);
            await CreateSale(saleProcess, 10, "Del Prado 2836", originSale, await cityProcess.Get("1661"), product, variant, color);
            await CreateSale(saleProcess, 11, "Florencio Sanchez 2978", originSale, await cityProcess.Get("1663"), product, variant, color);
            await CreateSale(saleProcess, 12, "Gelly Obes 4740", originSale, await cityProcess.Get("1665"), product, variant, color);
            await CreateSale(saleProcess, 13, "Florencio Ballesteros 1238", originSale, await cityProcess.Get("1665"), product, variant, color);
            await CreateSale(saleProcess, 14, "Matheu 1095", originSale, await cityProcess.Get("1665"), product, variant, color);
            await CreateSale(saleProcess, 15, "Santiago de Chile 1771", originSale, await cityProcess.Get("1613"), product, variant, color);
            await CreateSale(saleProcess, 16, "Echeverria 823", originSale, await cityProcess.Get("1623"), product, variant, color);
            await CreateSale(saleProcess, 17, "Charrua 3629", originSale, await cityProcess.Get("1754"), product, variant, color);
        }

        public async Task CreateSale(SaleProcess saleProcess, int index, string address, OriginSale originSale, City city, Product product, Variant variant, ProductColor color)
        {
            var sale = new Sale();
            sale.OriginSale = originSale;
            sale.Date = DateTime.Now.Date;
            sale.DeliveryDate = DateTime.Now.Date;
            sale.DeliveryStartTime = new TimeSpan(9, 0, 0);
            sale.DeliveryEndTime = new TimeSpan(21, 0, 0);
            sale.BusinessName = "Nombre " + index;
            sale.City = city;
            sale.Address = address;
            sale.Phone = "+5491127195268";
            sale.GrossPrice = variant.Price;
            sale.ShippingPrice = 6000;
            sale.TotalPrice = variant.Price;
            sale.SaleDetails = new List<SaleDetail>() { new SaleDetail { Id = 0, Product = product, Variant = variant, Color = color, Count = 1, Price = variant.SalePrice, IsExtra = false } };

            await saleProcess.Create(sale);
        }

        public void ConfigureProduct(Product product)
        {
            var variants = new List<Variant>();
            
            foreach (var name in product.Model.VariantNames)
            {
                var variant = new Variant();
                variant.Name = name;
                variant.SalePrice = 38000;
                variant.Price = 15000;
                variant.Active = true;

                var colors = new List<ProductColor>() {
                    new ProductColor() { Name = "Suela", Hex = "#ffffff", Sku = $"{product.Model.Code}{name.Trim()}Suela" },
                    new ProductColor() { Name = "Azul", Hex = "#ffffff", Sku = $"{product.Model.Code}{name.Trim()}Azul"},
                    new ProductColor() { Name = "Negro", Hex = "#ffffff", Sku = $"{product.Model.Code}{name.Trim()}Negro"}
                };

                variant.ColorsHex = colors;

                var propertyGroups = new List<PropertyGroup>();
                foreach (var fieldGroup in product.Model.FieldGroups)
                {
                    var propertyGroup = new PropertyGroup();
                    var properties = new List<Property>();

                    foreach (var field in fieldGroup.Fields)
                        properties.Add(new Property() { Field = field.Name, Value = string.Empty });

                    propertyGroup.Properties = properties;
                    propertyGroup.Name = fieldGroup.Name;
                    propertyGroups.Add(propertyGroup);
                }

                variant.PropertyGroups = propertyGroups;
                variants.Add(variant);
            }

            product.Variants = variants;
        }

        public void GetProductSQL()
        {
            SQLs.Add(@"
                CREATE TABLE dbo.Category (
                  Id            BIGINT IDENTITY(1,1) CONSTRAINT PK_Category PRIMARY KEY,
                  SearchString  NVARCHAR(MAX) NOT NULL,
                  Code          NVARCHAR(64)  NOT NULL,
                  CreationDate  DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  RemoveDate    DATETIME2 NULL,
                  UpdateDate    DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  Version       BIGINT    NOT NULL DEFAULT 1,
                  Guid          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
                  IsDeleted     BIT NOT NULL DEFAULT 0,
                  Name          NVARCHAR(200) NOT NULL
                );
            ");

            SQLs.Add(@"
                CREATE TABLE dbo.StoredFile (
                  Id                BIGINT IDENTITY(1,1) CONSTRAINT PK_StoredFile PRIMARY KEY,
                  SearchString      NVARCHAR(MAX) NOT NULL,
                  CreationDate      DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  RemoveDate        DATETIME2 NULL,
                  UpdateDate        DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  Version           BIGINT    NOT NULL DEFAULT 1,
                  Guid              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
                  IsDeleted         BIT NOT NULL DEFAULT 0,
                  FileName          NVARCHAR(260) NOT NULL,
                  [ContentBase64]   TEXT NOT NULL,
                  [Module]          NVARCHAR(100) NOT NULL
                );
            ");

            SQLs.Add(@"
                CREATE TABLE dbo.Model (
                  Id            BIGINT IDENTITY(1,1) CONSTRAINT PK_Model PRIMARY KEY,
                  SearchString  NVARCHAR(MAX) NOT NULL,
                  Code          NVARCHAR(64) NOT NULL,
                  CreationDate  DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  RemoveDate    DATETIME2 NULL,
                  UpdateDate    DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  Version       BIGINT    NOT NULL DEFAULT 1,
                  Guid          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
                  IsDeleted     BIT NOT NULL DEFAULT 0,
                  CategoryId    BIGINT NOT NULL CONSTRAINT FK_Model_Category REFERENCES dbo.Category(Id)
                );
            ");

            SQLs.Add(@"
                CREATE TABLE dbo.FieldGroup (
                    Id           BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_FieldGroup PRIMARY KEY,
                    SearchString  NVARCHAR(MAX) NOT NULL,
                    CreationDate DATETIME2(3) NOT NULL CONSTRAINT DF_FieldGroup_CreationDate DEFAULT (SYSUTCDATETIME()),
                    UpdateDate   DATETIME2(3) NULL,
                    RemoveDate   DATETIME2(3) NULL,
                    [Version]    INT NOT NULL CONSTRAINT DF_FieldGroup_Version DEFAULT (1),
                    [Guid]       UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_FieldGroup_Guid DEFAULT (NEWID()),
                    IsDeleted    BIT NOT NULL CONSTRAINT DF_FieldGroup_IsDeleted DEFAULT (0),
                    ModelId BIGINT NOT NULL,
                    [Name]       NVARCHAR(150) NOT NULL,
                    SortOrder    INT NOT NULL,

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
                  Id                  BIGINT IDENTITY(1,1) CONSTRAINT PK_Field PRIMARY KEY,
                  SearchString  NVARCHAR(MAX) NOT NULL,
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
                  SortOrder           INT NOT NULL,
                  CONSTRAINT UX_Field UNIQUE(FieldGroupId, [Name])
                );

            ");
            SQLs.Add(@"
                CREATE TABLE dbo.Model_VariantName (
                  Id             BIGINT IDENTITY(1,1) CONSTRAINT PK_Model_VariantName PRIMARY KEY,
                  ModelId BIGINT NOT NULL CONSTRAINT FK_VariantName_Model REFERENCES dbo.Model(Id),
                  [Name]         NVARCHAR(100) NOT NULL
                );
            ");
            SQLs.Add(@"
                CREATE TABLE dbo.Product (
                  Id            BIGINT IDENTITY(1,1) CONSTRAINT PK_Product PRIMARY KEY,
                  SearchString  NVARCHAR(MAX) NOT NULL,
                  CreationDate  DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  RemoveDate    DATETIME2 NULL,
                  UpdateDate    DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  Version       BIGINT    NOT NULL DEFAULT 1,
                  Guid          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
                  ModelId BIGINT NOT NULL
                      CONSTRAINT FK_Product_Model
                      REFERENCES dbo.Model(Id),
                  IsDeleted     BIT NOT NULL DEFAULT 0,
                  [Name]        NVARCHAR(200) NOT NULL,
                  [Description] NVARCHAR(MAX) NULL
                );

                CREATE INDEX IX_Product_ModelId ON dbo.Product(ModelId);
                CREATE INDEX IX_Product_IsDeleted     ON dbo.Product(IsDeleted);
            ");
            SQLs.Add(@"
                CREATE TABLE dbo.Variant (
                  Id            BIGINT IDENTITY(1,1) CONSTRAINT PK_Variant PRIMARY KEY,
                  SearchString  NVARCHAR(MAX) NOT NULL,
                  CreationDate  DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  RemoveDate    DATETIME2 NULL,
                  UpdateDate    DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  Version       BIGINT    NOT NULL DEFAULT 1,
                  Guid          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
                  IsDeleted     BIT NOT NULL DEFAULT 0,
                  ProductId     BIGINT NOT NULL
                      CONSTRAINT FK_Variant_Product
                      REFERENCES dbo.Product(Id),
                  [Name]        NVARCHAR(50)  NOT NULL,
                  SalePrice     DECIMAL(18,2) NOT NULL DEFAULT 0,
                  Price         DECIMAL(18,2) NOT NULL DEFAULT 0,
                  Active        BIT NOT NULL DEFAULT 1,
                  SortOrder     INT NOT NULL,
                );

                CREATE INDEX IX_Variant_ProductId ON dbo.Variant(ProductId);
                CREATE INDEX IX_Variant_Name      ON dbo.Variant([Name]);
            ");
            SQLs.Add(@"
                CREATE TABLE dbo.ProductColor (
                  Id          BIGINT IDENTITY(1,1) CONSTRAINT PK_ProductColor PRIMARY KEY,
                  SearchString  NVARCHAR(MAX) NOT NULL,
                  CreationDate  DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  RemoveDate    DATETIME2 NULL,
                  UpdateDate    DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  Version       BIGINT    NOT NULL DEFAULT 1,
                  Guid          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
                  IsDeleted     BIT NOT NULL DEFAULT 0,
                  VariantId     BIGINT NOT NULL CONSTRAINT FK_ProductColor_Variant REFERENCES dbo.Variant(Id),
                  Name          NVARCHAR(500) NOT NULL,
                  Hex           NVARCHAR(16) NOT NULL, 
                  Sku           NVARCHAR(500) NOT NULL,
                  SortOrder   INT NOT NULL
                );
            ");
            SQLs.Add(@"
                CREATE TABLE dbo.Image (
                  Id            BIGINT IDENTITY(1,1) CONSTRAINT PK_Image PRIMARY KEY,
                  SearchString  NVARCHAR(MAX) NOT NULL,
                  CreationDate  DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  RemoveDate    DATETIME2 NULL,
                  UpdateDate    DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  Version       BIGINT    NOT NULL DEFAULT 1,
                  Guid          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
                  IsDeleted     BIT NOT NULL DEFAULT 0,
                  ProductColorId     BIGINT NULL
                      CONSTRAINT FK_Image_ProductColor
                      REFERENCES dbo.ProductColor(Id),
                  StoredFileId  BIGINT NOT NULL
                      CONSTRAINT FK_Image_StoredFile
                      REFERENCES dbo.StoredFile(Id),
                  IsPrimary     BIT NOT NULL DEFAULT 0,
                  SortOrder     INT NOT NULL
                );

                CREATE INDEX IX_Image_StoredFileId ON dbo.Image(StoredFileId);
                CREATE INDEX IX_Image_ProductColorId    ON dbo.Image(ProductColorId);
            ");

            SQLs.Add(@"
                CREATE TABLE dbo.PropertyGroup (
                    Id           BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_PropertyGroup PRIMARY KEY,
                    SearchString  NVARCHAR(MAX) NOT NULL,
                    CreationDate DATETIME2(3) NOT NULL CONSTRAINT DF_PropertyGroup_CreationDate DEFAULT (SYSUTCDATETIME()),
                    UpdateDate   DATETIME2(3) NULL,
                    RemoveDate   DATETIME2(3) NULL,
                    [Version]    INT NOT NULL CONSTRAINT DF_PropertyGroup_Version DEFAULT (1),
                    [Guid]       UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_PropertyGroup_Guid DEFAULT (NEWID()),
                    IsDeleted    BIT NOT NULL CONSTRAINT DF_PropertyGroup_IsDeleted DEFAULT (0),
                    VariantId BIGINT NOT NULL
                        CONSTRAINT FK_PropertyGroup_Variant
                        REFERENCES dbo.Variant(Id),
                    [Name]       NVARCHAR(150) NOT NULL,
                    SortOrder    INT NOT NULL
                );

                CREATE INDEX IX_Property_VariantId ON dbo.PropertyGroup(VariantId);
            ");

            SQLs.Add(@"
                CREATE TABLE dbo.Property (
                  Id            BIGINT IDENTITY(1,1) CONSTRAINT PK_Property PRIMARY KEY,
                  SearchString  NVARCHAR(MAX) NOT NULL,
                  CreationDate  DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  RemoveDate    DATETIME2 NULL,
                  UpdateDate    DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  Version       BIGINT    NOT NULL DEFAULT 1,
                  Guid          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
                  IsDeleted     BIT NOT NULL DEFAULT 0,
                  PropertyGroupId     BIGINT NOT NULL
                      CONSTRAINT FK_Property_PropertyGroup
                      REFERENCES dbo.PropertyGroup(Id),
                  [Field]       NVARCHAR(150)  NOT NULL,
                  [Value]       NVARCHAR(4000) NOT NULL
                );

                CREATE INDEX IX_Property_PropertyGroupId ON dbo.Property(PropertyGroupId);
                CREATE INDEX IX_Property_Field     ON dbo.Property([Field]);
            ");

            SQLs.Add(@"
                CREATE TABLE dbo.LocationColumn (
                  Id            BIGINT IDENTITY(1,1) CONSTRAINT PK_LocationColumn PRIMARY KEY,
                  SearchString  NVARCHAR(MAX) NOT NULL,
                  Code          NVARCHAR(64)  NOT NULL,
                  CreationDate  DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  RemoveDate    DATETIME2 NULL,
                  UpdateDate    DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  Version       BIGINT    NOT NULL DEFAULT 1,
                  Guid          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
                  IsDeleted     BIT NOT NULL DEFAULT 0,
                  Name          NVARCHAR(200) NOT NULL
                );
            ");

            SQLs.Add(@"
                CREATE TABLE dbo.Hallway (
                  Id            BIGINT IDENTITY(1,1) CONSTRAINT PK_Hallway PRIMARY KEY,
                  SearchString  NVARCHAR(MAX) NOT NULL,
                  Code          NVARCHAR(64)  NOT NULL,
                  CreationDate  DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  RemoveDate    DATETIME2 NULL,
                  UpdateDate    DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  Version       BIGINT    NOT NULL DEFAULT 1,
                  Guid          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
                  IsDeleted     BIT NOT NULL DEFAULT 0,
                  Name          NVARCHAR(200) NOT NULL
                );
            ");

            SQLs.Add(@"
                CREATE TABLE dbo.Level (
                  Id            BIGINT IDENTITY(1,1) CONSTRAINT PK_Level PRIMARY KEY,
                  SearchString  NVARCHAR(MAX) NOT NULL,
                  Code          NVARCHAR(64)  NOT NULL,
                  CreationDate  DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  RemoveDate    DATETIME2 NULL,
                  UpdateDate    DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  Version       BIGINT    NOT NULL DEFAULT 1,
                  Guid          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
                  IsDeleted     BIT NOT NULL DEFAULT 0,
                  Name          NVARCHAR(200) NOT NULL
                );
            ");

            SQLs.Add(@"
                CREATE TABLE dbo.LocationState (
                  Id            BIGINT IDENTITY(1,1) CONSTRAINT PK_LocationState PRIMARY KEY,
                  SearchString  NVARCHAR(MAX) NOT NULL,
                  Code          NVARCHAR(64)  NOT NULL,
                  CreationDate  DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  RemoveDate    DATETIME2 NULL,
                  UpdateDate    DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  Version       BIGINT    NOT NULL DEFAULT 1,
                  Guid          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
                  IsDeleted     BIT NOT NULL DEFAULT 0,
                  Name          NVARCHAR(200) NOT NULL
                );
            ");

            SQLs.Add(@"
                CREATE TABLE dbo.Location (
                    Id            BIGINT IDENTITY(1,1) CONSTRAINT PK_Location PRIMARY KEY,
                    SearchString  NVARCHAR(MAX) NOT NULL,
                    CreationDate  DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                    RemoveDate    DATETIME2 NULL,
                    UpdateDate    DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                    Version       BIGINT    NOT NULL DEFAULT 1,
                    Guid          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
                    IsDeleted     BIT NOT NULL DEFAULT 0,
                    HallwayId     BIGINT NOT NULL CONSTRAINT FK_Location_Hallway REFERENCES dbo.Hallway(Id),
                    ColumnId      BIGINT NOT NULL CONSTRAINT FK_Location_Column REFERENCES dbo.LocationColumn(Id),
                    LevelId       BIGINT NOT NULL CONSTRAINT FK_Location_Level REFERENCES dbo.Level(Id),
                    LocationStateId  BIGINT NOT NULL CONSTRAINT FK_Location_State REFERENCES dbo.LocationState(Id)
                );

                CREATE INDEX IX_Location_HallwayId          ON dbo.Location(HallwayId);
                CREATE INDEX IX_Location_ColumnId           ON dbo.Location(ColumnId);
                CREATE INDEX IX_Location_LevelId            ON dbo.Location(LevelId);
                CREATE INDEX IX_Location_LocationStateId    ON dbo.Location(LocationStateId);
            ");

            SQLs.Add(@"
                CREATE TABLE dbo.Stock (
                    Id            BIGINT IDENTITY(1,1) CONSTRAINT PK_Stock PRIMARY KEY,
                    SearchString  NVARCHAR(MAX) NOT NULL,
                    CreationDate  DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                    RemoveDate    DATETIME2 NULL,
                    UpdateDate    DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                    Version       BIGINT    NOT NULL DEFAULT 1,
                    Guid          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
                    IsDeleted     BIT NOT NULL DEFAULT 0,
                    ProductId     BIGINT NOT NULL CONSTRAINT FK_Stock_Product REFERENCES dbo.Product(Id),
                    VariantId     BIGINT NOT NULL CONSTRAINT FK_Stock_Variant REFERENCES dbo.Variant(Id),
                    ProductColorId       BIGINT NOT NULL CONSTRAINT FK_Stock_ProductColor REFERENCES dbo.ProductColor(Id),
                    LocationId    BIGINT NOT NULL CONSTRAINT FK_Stock_Location REFERENCES dbo.Location(Id),
                    Physical      BIGINT   NOT NULL,
                    Reserved      BIGINT   NOT NULL,
                    Transit       BIGINT   NOT NULL,
                    Free          BIGINT   NOT NULL,
                    Minimum       BIGINT   NOT NULL,
                    Maximum       BIGINT   NOT NULL
                );

                CREATE INDEX IX_Stock_ProductId     ON dbo.Stock(ProductId);
                CREATE INDEX IX_Stock_VariantId     ON dbo.Stock(VariantId);
                CREATE INDEX IX_Stock_ProductColorId       ON dbo.Stock(ProductColorId);
                CREATE INDEX IX_Stock_LocationId    ON dbo.Stock(LocationId);
            ");

            SQLs.Add(@"
                CREATE TABLE dbo.Country (
                  Id            BIGINT IDENTITY(1,1) CONSTRAINT PK_Country PRIMARY KEY,
                  SearchString  NVARCHAR(MAX) NOT NULL,
                  Code          NVARCHAR(64)  NOT NULL,
                  CreationDate  DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  RemoveDate    DATETIME2 NULL,
                  UpdateDate    DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  Version       BIGINT    NOT NULL DEFAULT 1,
                  Guid          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
                  IsDeleted     BIT NOT NULL DEFAULT 0,
                  Name          NVARCHAR(200) NOT NULL
                );
            ");

            SQLs.Add(@"
                CREATE TABLE dbo.Province (
                  Id            BIGINT IDENTITY(1,1) CONSTRAINT PK_Province PRIMARY KEY,
                  SearchString  NVARCHAR(MAX) NOT NULL,
                  Code          NVARCHAR(64)  NOT NULL,
                  CreationDate  DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  RemoveDate    DATETIME2 NULL,
                  UpdateDate    DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  Version       BIGINT    NOT NULL DEFAULT 1,
                  Guid          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
                  IsDeleted     BIT NOT NULL DEFAULT 0,
                  Name          NVARCHAR(200) NOT NULL,
                  CountryId    BIGINT NOT NULL CONSTRAINT FK_Province_Country REFERENCES dbo.Country(Id)
                );
            ");

            SQLs.Add(@"
                CREATE TABLE dbo.City (
                  Id            BIGINT IDENTITY(1,1) CONSTRAINT PK_City PRIMARY KEY,
                  SearchString  NVARCHAR(MAX) NOT NULL,
                  CreationDate  DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  RemoveDate    DATETIME2 NULL,
                  UpdateDate    DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  Version       BIGINT    NOT NULL DEFAULT 1,
                  Guid          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
                  IsDeleted     BIT NOT NULL DEFAULT 0,
                  ZipCode       NVARCHAR(64)  NOT NULL,
                  Name          NVARCHAR(200) NOT NULL,
                  ProvinceId    BIGINT NOT NULL CONSTRAINT FK_City_Province REFERENCES dbo.Province(Id)
                );
            ");

            SQLs.Add(@"
                CREATE TABLE dbo.TaxStatus (
                  Id            BIGINT IDENTITY(1,1) CONSTRAINT PK_TaxStatus PRIMARY KEY,
                  SearchString  NVARCHAR(MAX) NOT NULL,
                  Code          NVARCHAR(64)  NOT NULL,
                  CreationDate  DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  RemoveDate    DATETIME2 NULL,
                  UpdateDate    DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  Version       BIGINT    NOT NULL DEFAULT 1,
                  Guid          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
                  IsDeleted     BIT NOT NULL DEFAULT 0,
                  Name          NVARCHAR(200) NOT NULL
                );
            ");

            SQLs.Add(@"
                CREATE TABLE dbo.OriginSale (
                  Id            BIGINT IDENTITY(1,1) CONSTRAINT PK_OriginSale PRIMARY KEY,
                  SearchString  NVARCHAR(MAX) NOT NULL,
                  Code          NVARCHAR(64)  NOT NULL,
                  CreationDate  DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  RemoveDate    DATETIME2 NULL,
                  UpdateDate    DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  Version       BIGINT    NOT NULL DEFAULT 1,
                  Guid          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
                  IsDeleted     BIT NOT NULL DEFAULT 0,
                  Name          NVARCHAR(200) NOT NULL
                );
            ");

            SQLs.Add(@"
                CREATE TABLE dbo.Driver (
                  Id            BIGINT IDENTITY(1,1) CONSTRAINT PK_Driver PRIMARY KEY,
                  SearchString  NVARCHAR(MAX) NOT NULL,
                  CreationDate  DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  RemoveDate    DATETIME2 NULL,
                  UpdateDate    DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  Version       BIGINT    NOT NULL DEFAULT 1,
                  Guid          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
                  IsDeleted     BIT NOT NULL DEFAULT 0,
                  FirstName     NVARCHAR(200) NOT NULL,
                  LastName      NVARCHAR(200) NOT NULL,
                  Dni           NVARCHAR(200) NOT NULL
                );
            ");

            SQLs.Add(@"
                CREATE SEQUENCE RoadMapNumberSeq START WITH 1 INCREMENT BY 1;

                CREATE TABLE dbo.RoadMap (
                  Id                BIGINT IDENTITY(1,1) CONSTRAINT PK_RoadMap PRIMARY KEY,
                  SearchString      NVARCHAR(MAX) NOT NULL,
                  CreationDate      DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  RemoveDate        DATETIME2 NULL,
                  UpdateDate        DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  Version           BIGINT    NOT NULL DEFAULT 1,
                  Guid              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
                  IsDeleted         BIT NOT NULL DEFAULT 0,
                  Number            BIGINT DEFAULT NEXT VALUE FOR RoadMapNumberSeq,
                  State             NVARCHAR(100) NOT NULL,
                  DriverId          BIGINT NOT NULL CONSTRAINT FK_Sale_Driver REFERENCES dbo.Driver(Id),
                  Date              DATETIME2 NOT NULL,
                  TravelDate        DATETIME2 NULL DEFAULT SYSUTCDATETIME(),
                  CompletionDate    DATETIME2 NULL DEFAULT SYSUTCDATETIME()
                );
            ");

            SQLs.Add(@"
                CREATE SEQUENCE SaleNumberSeq START WITH 1000 INCREMENT BY 1;


                CREATE TABLE dbo.Sale (
                  Id                    BIGINT IDENTITY(1,1) CONSTRAINT PK_Sale PRIMARY KEY,
                  SearchString          NVARCHAR(MAX) NOT NULL,
                  CreationDate          DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  RemoveDate            DATETIME2 NULL,
                  UpdateDate            DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  Version               BIGINT    NOT NULL DEFAULT 1,
                  Guid                  UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
                  IsDeleted             BIT NOT NULL DEFAULT 0,
                  Identifier            NVARCHAR(MAX) NOT NULL,
                  Number                BIGINT DEFAULT NEXT VALUE FOR SaleNumberSeq,
                  ArcaNumber            NVARCHAR(30) NULL,
                  Date                  DATETIME2 NOT NULL,
                  DeliveryDate          DATETIME2 NOT NULL,
                  DeliveryStartTime     Time NOT NULL,
                  DeliveryEndTime       Time NOT NULL,
                  BusinessName          NVARCHAR(MAX) NOT NULL,
                  Dni                   NVARCHAR(8) NOT NULL,
                  Cuit                  NVARCHAR(11) NOT NULL,
                  Address               NVARCHAR(MAX) NOT NULL,
                  Floor                 NVARCHAR(100) NOT NULL,
                  Apartment             NVARCHAR(100) NOT NULL,
                  Phone                 NVARCHAR(100) NOT NULL,
                  Observation           NVARCHAR(MAX) NOT NULL,
                  GrossPrice            DECIMAL(18,2) NOT NULL,
                  ShippingPrice         DECIMAL(18,2) NOT NULL,
                  Discounts             DECIMAL(18,2) NOT NULL,
                  TotalPrice            DECIMAL(18,2) NOT NULL,
                  TaxStatusId           BIGINT NULL CONSTRAINT FK_Sale_TaxStatus REFERENCES dbo.TaxStatus(Id),
                  OriginSaleId          BIGINT NOT NULL CONSTRAINT FK_Sale_OriginSale REFERENCES dbo.OriginSale(Id),
                  PdfInvoiceId          BIGINT NULL CONSTRAINT FK_Sale_StoredFile REFERENCES dbo.StoredFile(Id),
                  CityId                BIGINT NOT NULL CONSTRAINT FK_Sale_City REFERENCES dbo.City(Id),
                  State                 NVARCHAR(100) NOT NULL,
                  Latitude              DECIMAL(10,7) NULL,
                  Longitude             DECIMAL(10,7) NULL
                );
            ");

            SQLs.Add(@"
                CREATE TABLE dbo.SaleDetail (
                  Id                BIGINT IDENTITY(1,1) CONSTRAINT PK_SaleDetail PRIMARY KEY,
                  SearchString      NVARCHAR(MAX) NOT NULL,
                  CreationDate      DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  RemoveDate        DATETIME2 NULL,
                  UpdateDate        DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
                  Version           BIGINT    NOT NULL DEFAULT 1,
                  Guid              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
                  IsDeleted         BIT NOT NULL DEFAULT 0,
                  SaleId            BIGINT NOT NULL CONSTRAINT FK_SaleDetail_Sale REFERENCES dbo.Sale(Id),
                  ProductId         BIGINT NOT NULL CONSTRAINT FK_SaleDetail_Product REFERENCES dbo.Product(Id),
                  VariantId         BIGINT NOT NULL CONSTRAINT FK_SaleDetail_Variant REFERENCES dbo.Variant(Id),
                  ProductColorId    BIGINT NOT NULL CONSTRAINT FK_SaleDetail_ProductColor REFERENCES dbo.ProductColor(Id),
                  StockId           BIGINT NULL CONSTRAINT FK_SaleDetail_Stock REFERENCES dbo.Stock(Id),
                  Count             INT NOT NULL,
                  Price             DECIMAL(18,2) NOT NULL,
                  IsExtra           BIT NOT NULL
                );
            ");

            SQLs.Add(@"
                CREATE TABLE dbo.RoadMapSale (
                  Id                BIGINT IDENTITY(1,1) CONSTRAINT PK_RoadMapSale PRIMARY KEY,
                  RoadMapId         BIGINT NOT NULL CONSTRAINT FK_RoadMapSale_RoadMap REFERENCES dbo.RoadMap(Id),
                  SaleId            BIGINT NOT NULL CONSTRAINT FK_RoadMapSale_Sale REFERENCES dbo.Sale(Id),
                  SortOrder         INT NOT NULL
                );
            ");
        }

        public void CreateCatalogSQlServer()
        {
            var pepe = @"
                CREATE FULLTEXT CATALOG StockCatalog;
                CREATE FULLTEXT INDEX ON dbo.Stock ( SearchString LANGUAGE 3082 ) KEY INDEX PK_Stock ON StockCatalog;

                CREATE FULLTEXT CATALOG SaleCatalog;
                CREATE FULLTEXT INDEX ON dbo.Sale (SearchString LANGUAGE 3082) KEY INDEX PK_Sale ON SaleCatalog;
            ";
        }
    }
}
