using HardwareStoreLibrary.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;

namespace HardwareStoreLibrary.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<HardwareStoreLibrary.Storage.HardwareStoreContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(HardwareStoreLibrary.Storage.HardwareStoreContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.

            Tool t1 = new Tool("Hammer", "Bruges til at slå søm i f.eks. træbrædder.", 50, 20);
            Tool t2 = new Tool("Skruetrækker", "Bruges til at skrue skruer i f.eks. vægge.", 100, 50);
            Tool t3 = new Tool("Vaterpas", "Bruges til at måle om en ting, f.eks. et billede er i vater, så det hænger lige.", 50, 30);
            Tool t4 = new Tool("Kompostkværn", "Bruges til at omdanne kviste, blade, grene og ukrudt til kompost og træflis.", 1000, 100);
            Tool t5 = new Tool("Motorsav", "Bruges til at skære i store ting.", 1000, 80);

            Customer c1 = new Customer("Alice", "Hovedgaden 1", "alice@byggemarkedet.dk");
            Customer c2 = new Customer("Bob", "Abevej 64", "bob@byggemarkedet.dk");
            Customer c3 = new Customer("Cille", "Solvej 66", "cille@byggemarkedet.dk");

            context.Tools.AddOrUpdate(t => t.Name, new Tool { Name = "Kultivator", Description = "Manuel plov til at pløje øverste lag jord.", SecurityDeposit = 150, DailyPrice = 40 });
            context.Customers.AddOrUpdate(c => c.Name, new Customer("Benny", "Hovedvej 10", "benny@byggemarkedet.dk", "admin", "admin1234"));

            List<Tool> tools = new List<Tool>
            {
                t1,
                t2,
                t3
            };

            List<Tool> toolsList = new List<Tool>
            {
                t1,
                t4,
                t5
            };

            context.Bookings.AddOrUpdate(b => b.Status, new Booking(DateTime.Now.AddDays(2), DateTime.Now.AddDays(10), tools, c1, "Tilbageleveret", 1000));
            context.Bookings.AddOrUpdate(b => b.Status, new Booking(DateTime.Now.AddDays(10), DateTime.Now.AddDays(14), tools, c2, "Tilbageleveret", 600));
            context.Bookings.AddOrUpdate(b => b.Status, new Booking(DateTime.Now.AddDays(5), DateTime.Now.AddDays(15), toolsList, c3, "Tilbageleveret", 4050));
            context.Bookings.AddOrUpdate(b => b.Status, new Booking(DateTime.Now.AddDays(8), DateTime.Now.AddDays(14), tools, c3, "Tilbageleveret", 800));
            context.Bookings.AddOrUpdate(b => b.Status, new Booking(DateTime.Now.AddDays(2), DateTime.Now.AddDays(4), tools, c3, "Tilbageleveret", 400));
        }
    }
}