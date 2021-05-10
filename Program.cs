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
                    Console.WriteLine("1) Categories\n2) Products\n\"q\" to quit");
                    choice = Console.ReadLine();
                    Console.Clear();
                    logger.Info($"Main Menu - Option {choice} selected");
                    
                    var db = new NWConsole_96_EXGContext();

                    //Categories
                    if(choice == "1"){
                        Console.WriteLine("Category Menu\n1) Display\n2) Add Record\n3) Edit Record\n4) Delete Record");
                        choice = Console.ReadLine(); 
                        Console.Clear(); 
                        logger.Info($"Category Menu - Option {choice} selected"); 
                        
                        if(choice == "1"){
                            Console.WriteLine("Display Menu\n1) All Categories");
                            Console.WriteLine("2) Specific Category - Active Products");
                            Console.WriteLine("3) All Categories - Active Products");
                            choice = Console.ReadLine();
                            Console.Clear();
                            logger.Info($"Category Display Menu - Option {choice} selected");
                            //1 Display all Categories
                            if (choice == "1")
                            {
                                var query = db.Categories.OrderBy(p => p.CategoryName);

                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"{query.Count()} records returned");
                                Console.ForegroundColor = ConsoleColor.Magenta;
                                foreach (var item in query)
                                {
                                    if(item.CategoryId != 11)
                                        Console.WriteLine($"{item.CategoryName} - {item.Description}");
                                }
                                Console.ForegroundColor = ConsoleColor.White;
                            }

                            //2) Display specified Category and active products
                            else if (choice == "2"){
                                // get category
                                var query = db.Categories.OrderBy(c => c.CategoryId);
                                foreach (var item in query)
                                {
                                    if(item.CategoryId != 11)
                                        Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
                                }
                                Console.Write("Enter Category Id: "); 
                                string categorySearch = Console.ReadLine(); 
                                int catId; 
                                //if valid id 
                                if(Int32.TryParse(categorySearch, out catId)){
                                    try{
                                        Categories category = db.Categories.FirstOrDefault(c => c.CategoryId == catId);
                                        // find all active product
                                        var active = db.Products.Where(p => p.Discontinued == false && p.CategoryId == category.CategoryId).OrderBy(p => p.ProductName);
                                        // display category and active products
                                        Console.WriteLine($"{active.Count()} records returned");
                                        foreach(var products in active){
                                            Console.WriteLine($"{products.ProductName}");
                                        }
                                    }catch(Exception e){
                                        logger.Error(e.Message); 
                                    }
                                    
                                }
                                else logger.Info("Category ID - Not a valid int");
                                    
                            }
                            //3) Display all Categories and active products
                            else if(choice == "3"){
                                try{
                                    //display category
                                    foreach(Categories category in db.Categories){
                                        if(category.CategoryId != 11){
                                            //find and display active products
                                        var active = db.Products.Where(p => p.Discontinued == false && p.CategoryId == category.CategoryId).OrderBy(p => p.ProductName);
                                            Console.WriteLine($"{active.Count()} records returned");
                                                foreach(Products products in active){
                                                    Console.WriteLine($"{products.ProductName}");
                                                }
                                        }
                                    }
                                }catch(Exception e){logger.Error(e.Message);}
                            }
                        }
                        //2 Add record to category
                        else if (choice == "2")
                        {
                            Categories category = new Categories();
                            Console.Write("Enter Category Name:");
                            category.CategoryName = Console.ReadLine();
                            Console.Write("Enter the Category Description:");
                            category.Description = Console.ReadLine();
                            
                            //validate 
                            if(isValidCategory(category)){
                                //save to db
                                db.Categories.Add(category);
                                db.SaveChanges();
                                logger.Info($"Category \"{category.CategoryName}\" added");
                            }
                            
                        }
                        //3) Edit record from Categories
                        else if (choice == "3"){
                            //display all categories 
                            var query = db.Categories.OrderBy(p => p.CategoryId);
                            
                            foreach (var item in query)
                            {
                                if(item.CategoryId != 11)
                                    Console.WriteLine($"{item.CategoryId} - {item.CategoryName}");
                            }
                            //find category chosen 
                            Console.Write("Enter Categry id to edit: ");
                            string categorySearch = Console.ReadLine(); 
                            int catId; 
                            if(Int32.TryParse(categorySearch, out catId)){
                                try{
                                    Categories category = db.Categories.FirstOrDefault(c => c.CategoryId == catId);
                                    string userChoice;
                                    do{
                                        Console.WriteLine("1) Category Name\n2) Category Description\n\"x\" to quit");
                                        userChoice = Console.ReadLine();
                                        logger.Info($"Edit Category Record: Category {category.CategoryId} - {category.CategoryName} - Option {userChoice} selected"); 

                                        if(userChoice == "1"){
                                            Console.Write("Enter category name: ");
                                            string name = Console.ReadLine();
                                            if (String.IsNullOrEmpty(name)){
                                                logger.Error("Category must have a name");
                                                continue; 
                                            }
                                            else if(!db.Categories.Any(c => c.CategoryName == name))
                                                category.CategoryName = name;
                                            else {
                                                logger.Info($"\"{name}\" already exists");
                                                continue;
                                            }
                                        }
                                        else if(userChoice == "2"){
                                            Console.Write("Enter description: ");
                                            category.Description = Console.ReadLine(); 
                                        }
                                        //get updated info 
                                    }while(userChoice != "x");
                                    
                                    try{
                                    //save to db   
                                    db.SaveChanges(); 
                                    logger.Info($"Category {category.CategoryId} edited");
                                    }catch(Exception e){
                                        logger.Error(e.Message); 
                                            
                                    }
                                    
                                } catch(Exception e){
                                    logger.Error(e.Message);
                                     
                                }
                                    
                            }
                            
                        }
                        //4) Delete record from Categories
                        else if (choice == "4"){
                            //display all categories 
                            var query = db.Categories.OrderBy(p => p.CategoryId);
                            foreach (var item in query)
                            {
                                if(item.CategoryId != 11)
                                 Console.WriteLine($"{item.CategoryId} - {item.CategoryName}");
                            }
                            // find category chosen
                            Console.Write("Enter category id to delete: ");
                            string categorySearch = Console.ReadLine(); 
                            int categoryDelete; 
                            if(Int32.TryParse(categorySearch, out categoryDelete)){ 
                                try{
                                // deal with orphans
                                //if products w/ category then find and replace w/ unknowncategory category
                                    Categories category = db.Categories.FirstOrDefault(c => c.CategoryId == categoryDelete);
                                    if(db.Products.Any(p => p.CategoryId == category.CategoryId)){
                                        var productsToChange = db.Products.Where(p => p.CategoryId == category.CategoryId);
                                        foreach(Products product in productsToChange){
                                            product.CategoryId = 11; 
                                        }
                                        db.SaveChanges(); 
                                    }
                                // then delete category
                                    db.Remove(category); 
                                    db.SaveChanges(); 
                                    logger.Info($"Category {category.CategoryId} deleted");
                                }catch(Exception e){
                                    logger.Error(e.Message);
                                }
                            }
                            else 
                                logger.Info("Category ID - invalid int");
                        }
                    }

                    //Products
                    else if(choice == "2"){
                        Console.WriteLine("Product Menu\n1) Display Products \n2) Display Sepcific Product\n3) Add Record\n4) Edit Record\n5) Delete Record\n6) Filter Records");
                        choice = Console.ReadLine();
                        logger.Info($"Products Menu - Option {choice} selected"); 
                        // 1) Display Products
                        if (choice == "1"){
                            Console.WriteLine("\nDisplay Products \n1) All Products \n2) Discontinued Products \n3) Active Products");
                            string userChoice = Console.ReadLine(); 
                            logger.Info($"Display Products - Option {userChoice} selected");

                            if(userChoice == "1"){
                                //display all products
                                var query = db.Products.Where(p => p.ProductName != "Error Missing Product - Contact Supplier").OrderBy(p => p.ProductId);
                                Console.WriteLine($"{query.Count()} records returned");
                                foreach(var product in query){
                                    if(product.Discontinued)
                                        Console.ForegroundColor = ConsoleColor.DarkRed; 
                                    else Console.ForegroundColor = ConsoleColor.White;
                                    Console.WriteLine($"{product.ProductName}");
                                }
                                Console.ForegroundColor = ConsoleColor.White; 
                            }
                            else if(userChoice == "2"){
                                //display all discontinued
                                var query = db.Products.Where(p => p.Discontinued == true && p.ProductName != "Error Missing Product - Contact Supplier").OrderBy(p => p.ProductId); 
                                Console.WriteLine($"{query.Count()} records returned");
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                foreach(var product in query){
                                    Console.WriteLine($"{product.ProductName}");
                                }
                                Console.ForegroundColor = ConsoleColor.White; 
                            }
                            else if(userChoice == "3"){
                                //display all active
                                var query = db.Products.Where(p => p.Discontinued == false && p.ProductName != "Error Missing Product - Contact Supplier").OrderBy(p => p.ProductId); 
                                Console.WriteLine($"{query.Count()} records returned");
                                foreach(var product in query){
                                    Console.WriteLine($"{product.ProductName}");
                                } 
                            }
                        }
                        // 2) Display specified Products
                        else if(choice == "2"){
                            //display all products
                            var query = db.Products.Where(p => p.ProductName != "Error Missing Product - Contact Supplier").OrderBy(p => p.ProductId);
                            Console.WriteLine($"{query.Count()} records returned");
                            foreach(var product in query){
                                if(product.Discontinued)
                                    Console.ForegroundColor = ConsoleColor.DarkRed; 
                                else Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine($"{product.ProductId}: {product.ProductName}");
                            }
                            Console.ForegroundColor = ConsoleColor.White; 
                            //Ask for Name
                            Console.Write("Enter product ID: ");
                            string productID = Console.ReadLine();
                            int id; 
                            //display all fields
                            if(Int32.TryParse(productID, out id)){
                                try{
                                    Products product = db.Products.FirstOrDefault(p => p.ProductId == id);
                                    Console.WriteLine("{0}:\nProduct ID: {1} \nSupplier ID: {2} \nCategory ID: {3}\nQuantity per Unit: {4}\nUnit Price: {5:n2}\nUnits in Stock: {6}\nUnits on Order: {7}\nReorder Level: {8}\nDiscontinued: {9}\n", 
                                    product.ProductName, product.ProductId, product.SupplierId, product.CategoryId, product.QuantityPerUnit, product.UnitPrice, product.UnitsInStock,  
                                    product.UnitsOnOrder, product.ReorderLevel, product.Discontinued);
                                } catch(Exception e){
                                    logger.Info(e.Message); 
                                }
                            } else logger.Error("Product ID - Not a valid int");
                        }
                        // 3) Add new record to Products
                        else if(choice == "3"){
                            //create product
                            Products product = new Products();
                            //get info
                            //add info to product
                            int tempInt;  
                            short tempShort; 
                            decimal tempDecimal; 
                            bool tempBool; 
                            string productInfo;  
                            Console.Write("Enter Product Name: "); 
                            productInfo = Console.ReadLine();
                            if(!db.Products.Any(p => p.ProductName == productInfo))
                                product.ProductName = productInfo;
                            else {
                                logger.Info($"\"{productInfo}\" already exists");
                                continue;
                            }
                            Console.Write("Enter Supplier ID: "); 
                            productInfo = Console.ReadLine();
                            //allow nulls, stop if invalid int is entered
                            if(!String.IsNullOrEmpty(productInfo)){
                                if(!Int32.TryParse(productInfo, out tempInt)){
                                    logger.Info("Supplier ID - Not a valid int");
                                    continue;
                                }
                                else if(!db.Products.Any(p => p.SupplierId == tempInt)){
                                    logger.Info("Supplier ID - Invalid id");
                                    continue;
                                }
                                else
                                    product.SupplierId = tempInt;
                            } 

                            Console.Write("Enter Category ID: "); 
                            productInfo = Console.ReadLine();
                            if(!String.IsNullOrEmpty(productInfo)){
                                if(!Int32.TryParse(productInfo, out tempInt)){
                                    logger.Info("Category ID - Not a valid int");
                                    continue;
                                }
                                else if(!db.Products.Any(p => p.CategoryId == tempInt)){
                                    logger.Info("Category ID - Invalid id");
                                    continue;
                                }
                                else
                                    product.CategoryId = tempInt;
                            }
                            
                            Console.Write("Enter Quantity per Unit: "); 
                            product.QuantityPerUnit = Console.ReadLine();
                            
                            Console.Write("Enter Unit Price: "); 
                            productInfo = Console.ReadLine();
                            if(!String.IsNullOrEmpty(productInfo)){
                                if(!Decimal.TryParse(productInfo, out tempDecimal)){
                                    logger.Info("Category ID - Not a valid decimal");
                                    continue;
                                }
                                else
                                    product.UnitPrice = Math.Round(tempDecimal, 2);
                            }
                            Console.Write("Enter Units in Stock: "); 
                            productInfo = Console.ReadLine();
                            if(!String.IsNullOrEmpty(productInfo)){
                                if(!short.TryParse(productInfo, out tempShort)){
                                    logger.Info("Units in Stock - Not a valid short");
                                    continue;
                                }
                                else
                                    product.UnitsInStock = tempShort;
                            }
                            
                            Console.Write("Enter Units on Order: "); 
                            productInfo = Console.ReadLine();
                            if(!String.IsNullOrEmpty(productInfo)){
                                if(!short.TryParse(productInfo, out tempShort)){
                                    logger.Info("Units on Order - Not a valid short");
                                    continue;
                                }
                                else
                                    product.UnitsOnOrder = tempShort;
                            }
                            
                            Console.Write("Enter Reorder Level: "); 
                            productInfo = Console.ReadLine();
                            if(!String.IsNullOrEmpty(productInfo)){
                                if(!short.TryParse(productInfo, out tempShort)){
                                    logger.Info("Reorder Level - Not a valid short");
                                    continue;
                                }
                                else
                                    product.ReorderLevel = tempShort;
                            }
                            
                            Console.Write("Enter Discontinued: "); 
                            productInfo = Console.ReadLine();
                            if(!bool.TryParse(productInfo, out tempBool)){
                                    logger.Info("Discontinued - Not a valid boolean");
                                    continue;
                            }
                            else
                                product.Discontinued = tempBool;

                            //validate product
                            if(isValidProduct(product)){
                                //Add to db
                                try{
                                    db.Products.Add(product);
                                    db.SaveChanges(); 
                                    logger.Info($"Product \"{product.ProductName}\" added");
                                }catch(Exception e){
                                    logger.Error(e.Message);
                                }
                            }
                        }
                        // 4) Edit record from Products
                        else if(choice == "4"){
                            //display all products
                            var allProducts = db.Products.Where(p => p.ProductName != "Error Missing Product - Contact Supplier").OrderBy(p => p.ProductId);
                            foreach(var product in allProducts){
                                Console.WriteLine($"{product.ProductId}: {product.ProductName}"); 
                            }
                            //find chosen product
                            int productSearch; 
                            Console.Write("Enter ID of product to edit: ");
                            if(Int32.TryParse(Console.ReadLine(), out productSearch)){
                                if(db.Products.Any(p => p.ProductId == productSearch)){
                                    Products product = db.Products.FirstOrDefault(p => p.ProductId == productSearch);
                                    // get updated info
                                    string change = "";
                                    do{
                                        Console.WriteLine("\n1) Product Name\n2) Supplier ID\n3) Category ID\n4) Quantity Per Unit\n5) Unit Price\n6) Units in Stock\n7) Units on Order\n8) Reorder Level\n9) Discontinuted\n\"x\" to quit");
                                        change = Console.ReadLine();
                                        logger.Info($"Edit Product Record: Product {product.ProductId} - {product.ProductName} - Option {change} selected"); 

                                        string productInfo;
                                        int tempInt;
                                        decimal tempDecimal;
                                        short tempShort; 
                                        bool tempBool; 

                                        
                                        switch(change){
                                            case "1":
                                                Console.Write("Enter Product Name: "); 
                                                productInfo = Console.ReadLine();
                                                if(String.IsNullOrEmpty(productInfo)){
                                                    logger.Error("Product must have a name"); 
                                                    continue;
                                                }
                                                else if(!db.Products.Any(p => p.ProductName == productInfo))
                                                    product.ProductName = productInfo;
                                                else {
                                                    logger.Info($"\"{productInfo}\" already exists");
                                                    continue;
                                                }
                                                break;
                                            case "2":
                                                Console.Write("Enter Supplier ID: "); 
                                                productInfo = Console.ReadLine();
                                                //allow nulls, stop if invalid int is entered
                                                if(!Int32.TryParse(productInfo, out tempInt)){
                                                    logger.Info("Supplier ID - Not a valid int");
                                                    continue;
                                                }
                                                else if(!db.Products.Any(p => p.SupplierId == tempInt)){
                                                    logger.Info("Supplier ID - Invalid id");
                                                    continue;
                                                }
                                                else
                                                    product.SupplierId = tempInt;
                                                break;
                                            case "3":
                                                Console.Write("Enter Category ID: "); 
                                                productInfo = Console.ReadLine();
                                                if(!Int32.TryParse(productInfo, out tempInt)){
                                                    logger.Info("Category ID - Not a valid int");
                                                    continue;
                                                }
                                                else if(!db.Categories.Any(p => p.CategoryId == tempInt)){
                                                    logger.Info("Category ID - Invalid id");
                                                    continue;
                                                }
                                                else
                                                    product.CategoryId = tempInt;
                                                break;
                                            case "4":
                                                Console.Write("Enter Quantity per Unit: "); 
                                                product.QuantityPerUnit = Console.ReadLine();
                                                break;
                                            case "5":
                                                Console.Write("Enter Unit Price: "); 
                                                productInfo = Console.ReadLine();
                                                if(!Decimal.TryParse(productInfo, out tempDecimal)){
                                                    logger.Info("Category ID - Not a valid decimal");
                                                    continue;
                                                }
                                                else
                                                    product.UnitPrice = Math.Round(tempDecimal, 2);
                                                break;
                                            case "6":
                                                Console.Write("Enter Units in Stock: "); 
                                                productInfo = Console.ReadLine();
                                                if(!short.TryParse(productInfo, out tempShort)){
                                                    logger.Info("Units in Stock - Not a valid short");
                                                    continue;
                                                }
                                                else
                                                    product.UnitsInStock = tempShort;
                                                break;
                                            case "7":
                                                Console.Write("Enter Units on Order: "); 
                                                productInfo = Console.ReadLine();
                                                if(!short.TryParse(productInfo, out tempShort)){
                                                    logger.Info("Units on Order - Not a valid short");
                                                    continue;
                                                }
                                                else
                                                    product.UnitsOnOrder = tempShort;
                                                break;
                                            case "8":
                                                Console.Write("Enter Reorder Level: "); 
                                                productInfo = Console.ReadLine();
                                                if(!short.TryParse(productInfo, out tempShort)){
                                                    logger.Info("Reorder Level - Not a valid short");
                                                    continue;
                                                }
                                                else
                                                    product.ReorderLevel = tempShort;
                                                break;
                                            case "9":
                                                Console.Write("Enter Discontinued: "); 
                                                productInfo = Console.ReadLine();
                                                if(!bool.TryParse(productInfo, out tempBool)){
                                                        logger.Info("Discontinued - Not a valid boolean");
                                                        continue;
                                                }
                                                else
                                                    product.Discontinued = tempBool;
                                                break; 
                                        }
                                    } while(change != "x");
                                    // save to db
                                    try{
                                        db.SaveChanges(); 
                                        logger.Info($"Product {product.ProductId} edited");
                                    }catch (Exception e){
                                        logger.Error(e.Message); 
                                    }     
                                }
                                else
                                    logger.Info($"Product ID {productSearch} does not exit");
                            }
                            else
                                logger.Info("Not a valid int"); 
                        }
                        // 5) Delete record from Products
                        else if(choice == "5"){
                            // display all products
                            var allProducts = db.Products.Where(p => p.ProductName != "Error Missing Product - Contact Supplier").OrderBy(p => p.ProductId);
                            foreach(var product in allProducts){
                                Console.WriteLine($"{product.ProductId}: {product.ProductName}"); 
                            }
                            // find product chosen
                            Console.Write("Enter product id to delete: ");
                            string productSearch = Console.ReadLine(); 
                            int productDelete; 
                            if(Int32.TryParse(productSearch, out productDelete)){ 

                                try{
                                // deal with orphans
                                //if orders w/ product then 
                                    Products product = db.Products.FirstOrDefault(p => p.ProductId == productDelete);
                                    if(db.OrderDetails.Any(od => od.ProductId == product.ProductId)){
                                    // - change name to 'Error Missing Product - Contanct Supplier' and discontinued to true
                                        product.ProductName = "Error Missing Product - Contact Supplier"; 
                                        product.Discontinued = true; 
                                    // - set all other values to null
                                        product.CategoryId = null; 
                                        product.QuantityPerUnit = null; 
                                        product.UnitPrice = null; 
                                        product.UnitsInStock = null; 
                                        product.UnitsOnOrder = null; 
                                        product.ReorderLevel = null; 
                                    // - hide from my lists so they can't edit it 
                                    }
                                
                                // else just delete 
                                    else{
                                        db.Remove(product);
                                    }
                                    try{
                                        db.SaveChanges(); 
                                        logger.Info($"Product {product.ProductId} deleted");
                                    }catch(Exception e){
                                        logger.Error(e.Message); 
                                    }
                                }catch(Exception e){
                                    logger.Error(e.Message);
                                }
                            }

                        }
                        else if(choice == "6"){
                            // create list 1
                            List<Products> query1 = getFilteredProducts(); 
                            Console.WriteLine($"{query1.Count()} items returned"); 
                            foreach(Products product in query1){
                                Console.WriteLine(product.ProductName); 
                            }
                            // create list 2
                            Console.Write("Do you wish to create a second list (Y/N): "); 
                            string second = Console.ReadLine(); 
                            if(second.Equals("y", StringComparison.OrdinalIgnoreCase)){
                                List<Products> query2 = getFilteredProducts(); 
                                Console.WriteLine($"{query2.Count()} items returned"); 
                                foreach(Products product in query2){
                                    Console.WriteLine(product.ProductName); 
                                }
                                //ask for union, intersection, or except
                                List<Products> comparison = new List<Products>(); 
                                Console.WriteLine("1) Union\n2) Interction\n3) Except (List 1 except list 2)"); 
                                string compare = Console.ReadLine(); 
                                logger.Info($"Filter Products - Option {compare} selected"); 
                                if(compare == "1")
                                    comparison = query1.Union(query2).Distinct().ToList();
                                else if (compare == "2")
                                    comparison = query1.Intersect(query2).Distinct().ToList(); 
                                else if (compare == "3")
                                    comparison = query1.Except(query2).Distinct().ToList(); 

                                Console.WriteLine($"{comparison.Count()} items returned"); 
                                foreach(Products product in comparison){
                                    Console.WriteLine(product.ProductName); 
                                }
                                
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

        private static List<Products> getFilteredProducts(){
            var db = new NWConsole_96_EXGContext();
            Console.WriteLine("Filter by: \n1)ID \n2)Name \n3) Supplier ID\n4) Category ID\n5) Quantity per Unit\n6) Unit Price\n7) Units in Stock\n8) Units on Order\n9) Reorder Level\n10) Discontinued");
            //ask what to filter by
            string filterBy = Console.ReadLine(); 

            List<Products> products = new List<Products>(); 
            //ask for min and max || contains
            int intMin; 
            int intMax;
            short shortMin; 
            short shortMax; 
            string text; 
            switch(filterBy){
                case "1": 
                    Console.Write("Enter min (0 or above): "); 
                    if(!Int32.TryParse(Console.ReadLine(), out intMin))
                        logger.Error("Invalid Int"); 
                    else{
                        Console.Write("Enter max (0 or above): ");
                        if(!Int32.TryParse(Console.ReadLine(), out intMax))
                            logger.Error("Invalid int") ;
                        else
                            products = db.Products.Where(p => p.ProductId >= intMin && p.ProductId <= intMax).ToList();
                    }
                    break; 
                case "2": 
                    Console.Write("Enter text to search for: "); 
                    text = Console.ReadLine();
                    products = db.Products.Where(p => p.ProductName.Contains(text)).ToList();
                    break; 
                case "3": 
                    Console.Write("Enter min (0 or above): "); 
                    if(!Int32.TryParse(Console.ReadLine(), out intMin))
                        logger.Error("Invalid Int"); 
                    else{
                        Console.Write("Enter max (0 or above): ");
                        if(!Int32.TryParse(Console.ReadLine(), out intMax))
                            logger.Error("Invalid int") ;
                        else
                            products = db.Products.Where(p => p.SupplierId >= intMin && p.SupplierId <= intMax).ToList();
                    }
                    break; 
                case "4": 
                    Console.Write("Enter min (0 or above): "); 
                    if(!Int32.TryParse(Console.ReadLine(), out intMin))
                        logger.Error("Invalid Int"); 
                    else{
                        Console.Write("Enter max (0 or above): ");
                        if(!Int32.TryParse(Console.ReadLine(), out intMax))
                            logger.Error("Invalid int") ;
                        else
                            products = db.Products.Where(p => p.CategoryId >= intMin && p.CategoryId <= intMax).ToList();
                    }
                    break; 
                case "5": 
                    Console.Write("Enter text to search for: "); 
                    text = Console.ReadLine(); 
                    products = db.Products.Where(p => p.QuantityPerUnit.Contains(text)).ToList();
                    break; 
                case "6": 
                    decimal decimalMin; 
                    decimal decimalMax; 
                    Console.Write("Enter min (0 or above): "); 
                    if(!decimal.TryParse(Console.ReadLine(), out decimalMin))
                        logger.Error("Invalid Int"); 
                    else{
                        Console.Write("Enter max (0 or above): ");
                        if(!decimal.TryParse(Console.ReadLine(), out decimalMax))
                            logger.Error("Invalid int") ;
                        else
                            products = db.Products.Where(p => p.UnitPrice >= decimalMin && p.UnitPrice <= decimalMax).ToList();
                    }
                    break; 
                case "7": 
                    Console.Write("Enter min (0 or above): "); 
                    if(!short.TryParse(Console.ReadLine(), out shortMin))
                        logger.Error("Invalid Int"); 
                    else{
                        Console.Write("Enter max (0 or above): ");
                        if(!short.TryParse(Console.ReadLine(), out shortMax))
                            logger.Error("Invalid int") ;
                        else
                            products = db.Products.Where(p => p.UnitsInStock >= shortMin && p.UnitsInStock <= shortMax).ToList();
                    }
                    break; 
                case "8": 
                    Console.Write("Enter min (0 or above): "); 
                    if(!short.TryParse(Console.ReadLine(), out shortMin))
                        logger.Error("Invalid Int"); 
                    else{
                        Console.Write("Enter max (0 or above): ");
                        if(!short.TryParse(Console.ReadLine(), out shortMax))
                            logger.Error("Invalid int") ;
                        else
                            products = db.Products.Where(p => p.UnitsOnOrder >= shortMin && p.UnitsOnOrder <= shortMax).ToList();
                    }
                    break; 
                case "9": 
                    Console.Write("Enter min (0 or above): "); 
                    if(!short.TryParse(Console.ReadLine(), out shortMin))
                        logger.Error("Invalid Int"); 
                    else{
                        Console.Write("Enter max (0 or above): ");
                        if(!short.TryParse(Console.ReadLine(), out shortMax))
                            logger.Error("Invalid int") ;
                        else
                            products = db.Products.Where(p => p.ReorderLevel >= shortMin && p.ReorderLevel <= shortMax).ToList();
                    }
                    break; 
                case "10": 
                    Console.Write("Enter discontinued (true/false): "); 
                    bool discont;
                    if(!bool.TryParse(Console.ReadLine(), out discont))
                        logger.Error("Invaid boolean"); 
                    else 
                        products = db.Products.Where(p => p.Discontinued == discont).ToList(); 
                    break;
            }

            return products; 
        }
        private static bool isValidProduct(Products product){
            var db = new NWConsole_96_EXGContext(); 
            ValidationContext context = new ValidationContext(product, null, null); 
            List<ValidationResult> results = new List<ValidationResult>(); 

            var isValid = Validator.TryValidateObject(product, context, results);
            
            if(isValid){
                logger.Info("Validation Passed"); 
                //ensure unqie name
                if(db.Products.Any(p => p.ProductName.Equals(product.ProductName))){
                    isValid = false;
                    Console.WriteLine("Uh oh"); 
                    results.Add(new ValidationResult("Product name exists", new string[] {"ProductName"})); 
                }
                else
                    logger.Info("Validation Passed"); 
            }
            if(!isValid){
                //print/log error message
                foreach(var result in results){
                    logger.Error($"{result.MemberNames.FirstOrDefault()} : {result.ErrorMessage}");
                }
            }
            return isValid; 
        }

        private static bool isValidCategory(Categories category){
            var db = new NWConsole_96_EXGContext();
            ValidationContext context = new ValidationContext(category, null, null);
            List<ValidationResult> results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(category, context, results, true);
            if (isValid)
            {
                logger.Info("Validation passed");
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
                    
                }
            }
            if (!isValid)
            {
                foreach (var result in results)
                {
                    logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                }
            }
            return isValid;
        }

    }
}
