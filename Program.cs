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
                    //2
                    Console.WriteLine("1) Display all Categories");
                    Console.WriteLine("2) Add Category");
                    Console.WriteLine("3) Edit record from Categories");
                    Console.WriteLine("4) Delete record from Categories"); //3
                    Console.WriteLine("5) Display specified Category and all related products");
                    Console.WriteLine("6) Display specified Category and active products");
                    Console.WriteLine("7) Display all Categories and their related products");
                    Console.WriteLine("8) Display all Categories and active products");
                    //1
                    Console.WriteLine("9) Display all Products");
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
                    
                    //Add record to category
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

                    //Display specified category + all rel products
                    else if (choice == "3")
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

                    //display all categories + all related products
                    else if (choice == "4")
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
