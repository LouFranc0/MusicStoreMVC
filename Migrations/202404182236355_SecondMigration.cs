namespace EcommerceChitarre.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SecondMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Articoli",
                c => new
                    {
                        Articolo_ID = c.Int(nullable: false, identity: true),
                        Nome = c.String(nullable: false, maxLength: 50),
                        Img = c.String(maxLength: 255),
                        Prezzo = c.Decimal(nullable: false, precision: 10, scale: 2),
                        Tempo_Cons = c.String(maxLength: 50),
                        Dettagli = c.String(nullable: false, unicode: false, storeType: "text"),
                    })
                .PrimaryKey(t => t.Articolo_ID);
            
            CreateTable(
                "dbo.OrdArt",
                c => new
                    {
                        Articolo_ID = c.Int(nullable: false),
                        Ordine_ID = c.Int(nullable: false),
                        Quantita = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Articolo_ID, t.Ordine_ID, t.Quantita })
                .ForeignKey("dbo.Ordini", t => t.Ordine_ID)
                .ForeignKey("dbo.Articoli", t => t.Articolo_ID)
                .Index(t => t.Articolo_ID)
                .Index(t => t.Ordine_ID);
            
            CreateTable(
                "dbo.Ordini",
                c => new
                    {
                        Ordine_ID = c.Int(nullable: false, identity: true),
                        Indirizzo = c.String(nullable: false, maxLength: 50),
                        Note = c.String(maxLength: 250),
                        Data = c.DateTime(),
                        Stato = c.String(maxLength: 50),
                        Totale = c.Decimal(precision: 18, scale: 2),
                        CostoCons = c.Decimal(precision: 10, scale: 2),
                        User_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Ordine_ID)
                .ForeignKey("dbo.Users", t => t.User_ID)
                .Index(t => t.User_ID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        User_ID = c.Int(nullable: false, identity: true),
                        Nome = c.String(nullable: false, maxLength: 50),
                        Cognome = c.String(nullable: false, maxLength: 50),
                        Email = c.String(nullable: false, maxLength: 255),
                        Password = c.String(nullable: false, maxLength: 255),
                        Ruolo = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.User_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrdArt", "Articolo_ID", "dbo.Articoli");
            DropForeignKey("dbo.Ordini", "User_ID", "dbo.Users");
            DropForeignKey("dbo.OrdArt", "Ordine_ID", "dbo.Ordini");
            DropIndex("dbo.Ordini", new[] { "User_ID" });
            DropIndex("dbo.OrdArt", new[] { "Ordine_ID" });
            DropIndex("dbo.OrdArt", new[] { "Articolo_ID" });
            DropTable("dbo.Users");
            DropTable("dbo.Ordini");
            DropTable("dbo.OrdArt");
            DropTable("dbo.Articoli");
        }
    }
}
