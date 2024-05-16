﻿using NLog;
using System.Linq;
using Northwind_Console.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

// See https://aka.ms/new-console-template for more information
string path = Directory.GetCurrentDirectory() + "\\nlog.config";

// create instance of Logger
var logger = LogManager.LoadConfiguration(path).GetCurrentClassLogger();
logger.Info("Program started");

try
{
    var db = new NWContext();
    string choice;
    do
    {
        Console.WriteLine("1) Display Categories");
        Console.WriteLine("2) Add Category");
        Console.WriteLine("3) Display Category and related products");
        Console.WriteLine("4) Display all Categories and their related products");
        Console.WriteLine("5) Add New Record to Products Table");
        Console.WriteLine("6) Edit A Specified Record from the Products Table");
        Console.WriteLine("7) Display A Specific Product");


        Console.WriteLine("\"q\" to quit");
        choice = Console.ReadLine();
        Console.Clear();
        logger.Info($"Option {choice} selected");
        if (choice == "1")
        {
            var query = db.Categories.OrderBy(p => p.CategoryName);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{query.Count()} records returned");
            Console.ForegroundColor = ConsoleColor.Magenta;
            foreach (var item in query)
            {
                Console.WriteLine($"{item.CategoryName} - {item.Description}");
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        else if (choice == "2")
        {
            Category category = new Category();
            Console.WriteLine("Enter Category Name:");
            category.CategoryName = Console.ReadLine();
            Console.WriteLine("Enter the Category Description:");
            category.Description = Console.ReadLine();
            ValidationContext context = new ValidationContext(category, null, null);
            List<ValidationResult> results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(category, context, results, true);
            if (isValid)
            {
                // check for unique name
                if (db.Categories.Any(c => c.CategoryName == category.CategoryName))
                {
                    // generate validation error
                    isValid = false;
                    results.Add(new ValidationResult("Name exists", new string[] { "CategoryName" }));
                }
                else
                {
                    logger.Info("Validation passed");
                    db.Categories.Add(category);
                    db.SaveChanges();
                }
            }
            if (!isValid)
            {
                foreach (var result in results)
                {
                    logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                }
            }
        }
        else if (choice == "3")
        {
            var query = db.Categories.OrderBy(p => p.CategoryId);

            Console.WriteLine("Select the category whose products you want to display:");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            foreach (var item in query)
            {
                Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
            }
            Console.ForegroundColor = ConsoleColor.White;
            int id = int.Parse(Console.ReadLine());
            Console.Clear();
            logger.Info($"CategoryId {id} selected");
            Category category = db.Categories.Include("Products").FirstOrDefault(c => c.CategoryId == id);
            Console.WriteLine($"{category.CategoryName} - {category.Description}");
            foreach (Product p in category.Products)
            {
                Console.WriteLine($"\t{p.ProductName}");
            }
        }

        else if (choice == "4")
        {
            var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
            foreach (var item in query)
            {
                Console.WriteLine($"{item.CategoryName}");
                foreach (Product p in item.Products)
                {
                    Console.WriteLine($"\t{p.ProductName}");
                }
            }
        }

        else if (choice == "5")
        {
            Product product = new Product();

            Console.WriteLine("What is the Product Name?");
            product.ProductName = Console.ReadLine();

            Console.WriteLine("What is the Supplier ID?");
            product.SupplierId = int.Parse(Console.ReadLine());

            Console.WriteLine("What is the Category ID?");
            product.CategoryId = int.Parse(Console.ReadLine());

            Console.WriteLine("What is the Quantity Per Unit?");
            product.QuantityPerUnit = Console.ReadLine();

            Console.WriteLine("What is the Unit Price?");
            product.UnitPrice = decimal.Parse(Console.ReadLine());

            Console.WriteLine("How many Units In Stock?");
            product.UnitsInStock = short.Parse(Console.ReadLine());

            Console.WriteLine("How many Units On Order?");
            product.UnitsOnOrder = short.Parse(Console.ReadLine());

            Console.WriteLine("What is the Reorder Level?");
            product.ReorderLevel = short.Parse(Console.ReadLine());

            Console.WriteLine("Discontinued? (True/False)");
            product.Discontinued = bool.Parse(Console.ReadLine());

            db.Products.Add(product);
            db.SaveChanges();
        }

        else if (choice == "6")
        {
            var query = db.Products.OrderBy(p => p.ProductId);
            foreach (var item in query)
            {
                Console.WriteLine($"{item.ProductId} - {item.ProductName}");
            }

            Console.WriteLine("Enter the product ID of the item you would like to edit");
            int pId = int.Parse(Console.ReadLine());

            var product = db.Products.FirstOrDefault(p => p.ProductId == pId);

            Console.WriteLine("Enter new Product ID: ");
            product.ProductId = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter new Product Name: ");
            product.ProductName = Console.ReadLine();

            Console.WriteLine("Enter new Supplier ID: ");
            product.SupplierId = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter new Category ID: ");
            product.CategoryId = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter new Quantity Per Unit: ");
            product.QuantityPerUnit = Console.ReadLine();

            Console.WriteLine("Enter new Unit Price: ");
            product.UnitPrice = decimal.Parse(Console.ReadLine());

            Console.WriteLine("Enter new Units in Stock: ");
            product.UnitsInStock = short.Parse(Console.ReadLine());

            Console.WriteLine("Enter new Units on Order: ");
            product.UnitsOnOrder = short.Parse(Console.ReadLine());

            Console.WriteLine("Enter new Reorder Level: ");
            product.ReorderLevel = short.Parse(Console.ReadLine());

            Console.WriteLine("Enter new Discontinued (True/False): ");
            product.Discontinued = Boolean.Parse(Console.ReadLine());

            db.SaveChanges();



        }

        else if (choice == "7")
        {
            var query = db.Products.OrderBy(p => p.ProductId);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{query.Count()} records returned");

            Console.ForegroundColor = ConsoleColor.Magenta;
            foreach (var item in query)
            {
                Console.WriteLine($"{item.ProductId} - {item.ProductName}");
            }
            Console.WriteLine("Select the number of the product you want more information on:");
            Console.ForegroundColor = ConsoleColor.White;

        }
        if (int.TryParse(Console.ReadLine(), out int productId))
        {
            var product = db.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product != null)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;

                Console.WriteLine($"Product ID: {product.ProductId}");
                Console.WriteLine($"Product Name: {product.ProductName}");
                Console.WriteLine($"Supplier ID: {product.SupplierId}");
                Console.WriteLine($"Category ID: {product.CategoryId}");
                Console.WriteLine($"Quantity Per Unit: {product.QuantityPerUnit}");
                Console.WriteLine($"Unit Price: {product.UnitPrice:C}");
                Console.WriteLine($"Units In Stock: {product.UnitsInStock}");
                Console.WriteLine($"Units On Order: {product.UnitsOnOrder}");
                Console.WriteLine($"Reorder Level: {product.ReorderLevel}");
                Console.WriteLine($"Discontinued: {product.Discontinued}");

                Console.ForegroundColor = ConsoleColor.White;

            }
        }

        Console.WriteLine();

    } while (choice.ToLower() != "q");
}
catch (Exception ex)
{
    logger.Error(ex.Message);
    Console.WriteLine(ex.InnerException.Message);

}
logger.Info("Program ended");