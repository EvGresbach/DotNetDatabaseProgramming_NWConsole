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

                    Console.WriteLine("Main Menu\n"); 
                    Console.WriteLine("1)Categories\n2)Products\n\"q\" to quit");
                    choice = Console.ReadLine();
                    Console.Clear();
                    logger.Info($"Main Menu - Option {choice} selected");
                    
                    //Categories
                    if(choice == "1"){
                        Console.WriteLine("Category Menu\n1) Display\n2) Add Record\n3) Edit Record\n4) Delete Record");
                        choice = Console.ReadLine(); 
                        Console.Clear(); 
                        logger.Info($"Category Menu - Option {choice} selected"); 
                        
                        if(choice == "1"){
                            Console.WriteLine("Display Menu\n1) All Categories");
                            Console.WriteLine("2) Specific Category - All Related PRoducts\n3) Specific Category - Active Products");
                            Console.WriteLine("4) All Cateories - All Related Products\n5) All Categories - Active Products");
                            choice = Console.ReadLine();
                            Console.Clear();
                            logger.Info($"Category Display Menu - Option {choice} selected");
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

                            //2 Display specified category + all rel products
                            else if (choice == "2")
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
                            //3) Display specified Category and active products
                            else if (choice == "3"){
                                // get category
                                // find all active product
                                // display category and active products
                                // log
                            }
                            //4) display all categories + all related products
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
                            // 5) Display all Categories and active products
                            else if(choice == "5"){
                                //display category
                                //find and display active products
                            }
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
                        else if (choice == "3"){
                            //display all categories 
                            //find category chosen 
                            //get updated info 
                            //validate
                            //save to db 
                        }
                        //4) Delete record from Categories
                        else if (choice == "4"){
                            //display all categories 
                            //find category chosen 
                            //deal with orphans -- ??
                            //delete category 
                        }
                    }

                    //Products
                    else if(choice == "2"){
                        Console.WriteLine("Product Menu\n1) Display Products \n2) Display Sepcific Product\n3) Edit Record\n4) Delete Record");
                        // 1) Display Products
                        if (choice == "1"){
                            Console.WriteLine("\nDisplay Products \n1) All Products \n2) Discontinued Products \n3) Active Products");
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
                                var query = db.Products.Where(p => p.Discontinued == false).OrderBy(p => p.ProductId); 
                                foreach(var product in query){
                                    Console.WriteLine($"{product.ProductName}");
                                } 
                            }
                        }
                        // 2) Display specified Products
                        else if(choice == "2"){
                            //Ask for Name
                            Console.Write("Enter product name: ");
                            string productName = Console.ReadLine();
                            //display all fields
                            var db = new NWConsole_96_EXGContext();
                            Products product = db.Products.FirstOrDefault(p => p.ProductName == productName);

                            Console.WriteLine("{0}:\nProduct ID: {1} \nSupplier ID: {2} \nCategory ID: {3}\nQuantity per Unit: {4}\nUnit Price: {5:n2}\nUnits in Stock: {6}\nUnits on Order: {7}\nReorder Level: {8}\nDiscontinued: {9}\n", 
                            product.ProductName, product.ProductId, product.SupplierId, product.CategoryId, product.QuantityPerUnit, product.UnitPrice, product.UnitsInStock,  
                            product.UnitsOnOrder, product.ReorderLevel, product.Discontinued);
                        }
                        // 3) Add new record to Products
                        else if(choice == "3"){
                            //create product
                            var db = new NWConsole_96_EXGContext();
                            Products product = new Products();
                            //get info
                            //add info to product
                            Console.WriteLine("Enter Product Name: "); 
                            product.ProductName = Console.ReadLine();
                            Console.WriteLine("Enter Supplier ID: "); 
                            product.SupplierId = Int32.Parse(Console.ReadLine());
                            Console.WriteLine("Enter Category ID: "); 
                            product.CategoryId = Int32.Parse(Console.ReadLine());
                            Console.WriteLine("Enter Quantity per Unit: "); 
                            product.QuantityPerUnit = Console.ReadLine();
                            Console.WriteLine("Enter Unit Price: "); 
                            product.UnitPrice = Decimal.Parse(Console.ReadLine());
                            Console.WriteLine("Enter Units in Stock: "); 
                            product.UnitsInStock = short.Parse(Console.ReadLine());
                            Console.WriteLine("Enter Units on Order: "); 
                            product.UnitsOnOrder = short.Parse(Console.ReadLine());
                            Console.WriteLine("Enter Reorder Level: "); 
                            product.ReorderLevel = short.Parse(Console.ReadLine());
                            Console.WriteLine("Enter Discontinued: "); 
                            product.Discontinued = bool.Parse(Console.ReadLine());
                            //validate product
                            ValidationContext context = new ValidationContext(product, null, null); 
                            List<ValidationResult> results = new List<ValidationResult>(); 

                            var isValid = Validator.TryValidateObject(product, context, results);
                            
                            if(isValid){
                                //ensure unqie name
                                if(db.Products.Any(p => p.ProductName.Equals(product.ProductName))){
                                    isValid = false;
                                    results.Add(new ValidationResult("Product name exists", new string[] {"ProductName"})); 
                                }
                                //add product to database  
                            }
                            else{
                                //print/log error message
                                foreach(var result in results){
                                    logger.Error($"{result.MemberNames.FirstOrDefault()} : {result.ErrorMessage}");
                                }
                            }
                            
                        }
                        // 4) Edit record from Products
                        else if(choice == "4"){
                            //display all products
                            //find chosen product
                            // get updated info
                            // validate
                            // save to db
                        }
                        // 5) Delete record from Products
                        else if(choice == "5"){
                            // display all products
                            // find product chosen
                            // deal with orpahns -- ??
                            // delete product

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
