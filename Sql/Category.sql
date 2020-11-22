--Install-Package EntityFramework -Version 6.3.0
--Install-Package Microsoft.SqlServer.Types -Version 14.0.1016.290
/*
[Table("Category", Schema = "Hierarchy")]
    public partial class HierarchyCategory
    {
        public int Id { get; set; }

        [Required]
        [StringLength(256)]
        public string Name { get; set; }

        public HierarchyId Level { get; set; }
    }
	var topLevelQuery = Context.HierarchyCategory.Where(x => x.Level.GetLevel() == 1);
*/
CREATE TABLE [dbo].[Category](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[Level] [hierarchyid] NULL,
 CONSTRAINT [PK_ProductCategoryHId] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[Category] ADD  CONSTRAINT [LevelUnique] UNIQUE NONCLUSTERED 
(
	[Level] ASC
)

ALTER TABLE [Category]
ADD ParentLevel as [Level].GetAncestor(1) PERSISTED    
      REFERENCES [Category]([Level])

INSERT dbo.[Category] ([Name], [Level]) VALUES (N'First ', N'/1/')
INSERT dbo.[Category] ([Name], [Level]) VALUES (N'Second ', N'/2/')
INSERT dbo.[Category] ([Name], [Level]) VALUES (N'Third ', N'/3/')

INSERT dbo.[Category] ([Name], [Level]) VALUES (N'First Subcategory', N'/1/1/')
INSERT dbo.[Category] ([Name], [Level]) VALUES (N'Second Subcategory', N'/1/2/')
INSERT dbo.[Category] ([Name], [Level]) VALUES (N'Third Subcategory', N'/1/3/')

INSERT dbo.[Category] ([Name], [Level]) VALUES (N'First Subcategory', N'/3/1/')
INSERT dbo.[Category] ([Name], [Level]) VALUES (N'Subcategory', N'/3/1/1/')
INSERT dbo.[Category] ([Name], [Level]) VALUES (N'Second Subcategory', N'/3/2/')

SELECT 
       [Name]
      ,[Level].ToString()
  FROM [dbo].[Category]