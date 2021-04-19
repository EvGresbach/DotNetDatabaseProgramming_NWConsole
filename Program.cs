using System;
using NLog.Web;
using System.IO;
using System.Linq;
using NorthwindConsole.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace NorthwindConsole
{
    class Program
    {
        // create static instance of Logger
        private static NLog.Logger logger = NLogBuilder.ConfigureNLog(Directory.GetCurrentDirectory() + "\\nlog.config").GetCurrentClassLogger();
        static void Main(string[] args)
        {
            logger.Info("Program started");

             try
            {
                string choice;
                do
                {
                    Console.WriteLine("Menu\n"); 
                    //2
                    Console.WriteLine("1) Display all Categories");
                    Console.WriteLine("2) Add Category");
                    Console.WriteLine("3) Edit record from Categories");
                    Console.WriteLine("4) Delete record from Categories"); //3
                    Console.WriteLine("5) Display specified Category and all related products");
                    Console.WriteLine("6) Display specified Category and active products");
                    Console.WriteLine("7) Display all Categories and all related products");
                    Console.WriteLine("8) Display all Categories and active products");
                    //1
                    Console.WriteLine("9) Display Products");
                    Console.WriteLine("10) Display specified Product");
                    Console.WriteLine("11) Add new record to Products");
                    Console.WriteLine("12) Edit record from Products");
                    Console.WriteLine("13) Delete record from Products"); //3
                   
                    //4??
                    Console.WriteLine("\"q\" to quit");
                    choice = Console.ReadLine();
                    Console.Clear();
                    logger.Info($"Option {choice} selected");
                    
                    //1 Display all Categories
                    if (choice == "1")
                    {
                        var db = new NWConsole_96_EXGContext();
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
                    
                    //2 Add record to category
                    else if (choice == "2")
                    {
                        Categories category = new Categories();
                        Console.WriteLine("Enter Category Name:");
                        category.CategoryName = Console.ReadLine();
                        Console.WriteLine("Enter the Category Description:");
                        category.Description = Console.ReadLine();
                        
                        ValidationContext context = new ValidationContext(category, null, null);
                        List<ValidationResult> results = new List<ValidationResult>();

                        var isValid = Validator.TryValidateObject(category, context, results, true);
                        if (isValid)
                        {
                            logger.Info("Validation passed");
                            var db = new NWConsole_96_EXGContext();
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
                                // TODO: save category to db
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
                    //3) Edit record from Categories
                    //4) Delete record from Categories
                    //5 Display specified category + all rel products
                    else if (choice == "5")
                    {
                        var db = new NWConsole_96_EXGContext();
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
                        Categories category = db.Categories.Include("Products").FirstOrDefault(c => c.CategoryId == id);
                        Console.WriteLine($"{category.CategoryName} - {category.Description}");
                        foreach (Products p in category.Products)
                        {
                            Console.WriteLine(p.ProductName);
                        }
                    }
                    //6) Display specified Category and active products
                    //7) display all categories + all related products
                    else if (choice == "7")
                    {
                        var db = new NWConsole_96_EXGContext();
                        var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryName}");
                            foreach (Products p in item.Products)
                            {
                                Console.WriteLine($"\t{p.ProductName}");
                            }
                        }
                    }
                    // 8) Display all Categories and active products
                    // 9) Display Products
                    else if (choice == "9"){
                        Console.WriteLine("Display Products \n1) All Products \n2) Discontinued Products \n3) Active Products");
                        string userChoice = Console.ReadLine(); 
                        logger.Info($"Display Products - Option {userChoice} selected");

                        var db = new NWConsole_96_EXGContext(); 
                        if(userChoice == "1"){
                            //display all products
                            var query = db.Products.OrderBy(p => p.ProductId);
                            foreach(var product in query){
                                if(product.Discontinued)
                                    Console.ForegroundColor = ConsoleColor.DarkRed; 
                                else Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine($"{product.ProductName}");
                            }
                        }
                        else if(userChoice == "2"){
                            //display all discontinued
                            var query = db.Products.Where(p => p.Discontinued == true).OrderBy(p => p.ProductId); 
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            foreach(var product in query){
                                Console.WriteLine($"{product.ProductName}");
                            }
                            Console.ForegroundColor = ConsoleColor.White; 
                        }
                        else if(userChoice == "3"){
                            //display all active
                        }
                    }
                    // 10) Display specified Products
                    // 11) Add new record to Products
                    // 12) Edit record from Products
                    // 13) Delete record from Products



                    Console.WriteLine();

                } while (choice.ToLower() != "q");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

            logger.Info("Program ended");
        }
    }
}
