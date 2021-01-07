namespace PlateformeConcours.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Annotations;
    using System.Data.Entity.Migrations;
    
    public partial class UniqueKey : DbMigration
    {
        public override void Up()
        {
            AlterTableAnnotations(
                "dbo.Uploads",
                c => new
                    {
                        EtudiantUploadId = c.Int(nullable: false),
                        FichierPdf = c.String(),
                        Photo = c.String(),
                        FileYear = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    },
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "DynamicFilter_Upload_IsDeleted",
                        new AnnotationValues(oldValue: "EntityFramework.DynamicFilters.DynamicFilterDefinition", newValue: null)
                    },
                });
            
            CreateIndex("dbo.Etudiants", new[] { "Cne", "Cin", "Email" }, unique: true, name: "IX_FirstAndSecond");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Etudiants", "IX_FirstAndSecond");
            AlterTableAnnotations(
                "dbo.Uploads",
                c => new
                    {
                        EtudiantUploadId = c.Int(nullable: false),
                        FichierPdf = c.String(),
                        Photo = c.String(),
                        FileYear = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    },
                annotations: new Dictionary<string, AnnotationValues>
                {
                    { 
                        "DynamicFilter_Upload_IsDeleted",
                        new AnnotationValues(oldValue: null, newValue: "EntityFramework.DynamicFilters.DynamicFilterDefinition")
                    },
                });
            
        }
    }
}
