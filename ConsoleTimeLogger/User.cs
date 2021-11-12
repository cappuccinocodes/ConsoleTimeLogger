﻿using System;
using System.Globalization;

namespace ConsoleTimeLogger
{
    class User
    {
        DatabaseManager DB;
        public User(string dbFile)
        {
            this.DB = new DatabaseManager(dbFile);
        }
        public void InputLoop()
        {
            //Maybe this loop should be in the Main program.cs instead of here?
            bool userWantsExit = false;
            while (!userWantsExit)
            {
                this.DB.View();
                Console.WriteLine("-------------------------------------");
                Console.WriteLine("Input your command");
                Console.WriteLine("\n");
                Console.WriteLine("0 to exit");
                Console.WriteLine("U to update hours");
                Console.WriteLine("D to delete a date");
                Console.WriteLine("I to insert a date");
                Console.WriteLine("V to view data");
                Console.WriteLine("-------------------------------------");
                string userInputCommand = Console.ReadLine().ToUpper();
                Console.Clear();
                switch (userInputCommand)
                {
                    case "0":
                        userWantsExit = true;
                        break;
                    case "U":
                        long userDateInput = GetUserDate();
                        if (userDateInput != -1)
                        {
                            long userHourInput = GetUserHours();
                            if (userHourInput != -1)
                            {
                                this.DB.Update(userHourInput, userDateInput);
                            }
                        }
                        break;
                    case "D":
                        this.DB.View("all");
                        Console.WriteLine("\n");
                        userDateInput = GetUserDate();
                        this.DB.Delete(userDateInput);
                        break;
                    case "I":
                        userDateInput = GetUserDate();
                        if (userDateInput != -1)
                        {
                            long userHourInput = GetUserHours();
                            if (userHourInput != -1)
                            {
                                this.DB.InsertRow(userDateInput, userHourInput);
                            }
                        }
                        break;
                    case "V":
                        //this.DB.View("all");
                        ViewSelect();
                        Console.WriteLine("\n Press any key to go back to main menu...");
                        Console.ReadLine();
                        Console.Clear();
                        break;
                    default:
                        Console.WriteLine("Incorrect input, try again.");
                        break;
                }
                Console.WriteLine("-------------------------------------");
                
            }
        }
        private void ViewSelect()
        {
            Console.Clear();
            Console.WriteLine("Which view would you like?");
            Console.WriteLine("A to see all");
            Console.WriteLine("T to see just today");
            Console.WriteLine("X to see a specific amount of the most recent rows");
            string UserChoice = Console.ReadLine().ToUpper();
            switch (UserChoice)
            {
                case "A":
                    this.DB.View("all");
                    break;
                case "T":
                    this.DB.View();
                    break;
                case "X":
                    //get a user int input
                    bool result = getUserInt(out int userInput);
                    if (result)
                    {
                        this.DB.View("limit", userInput);
                    }

                    break;
                default:
                    ViewSelect();
                    break;
            }

        }
        private static bool getUserInt(out int result)
        {
            Console.Clear();
            bool properInput = false;
            while (!properInput)
            {
                Console.WriteLine("Input a number");
                string userInput = Console.ReadLine();
                properInput = int.TryParse(userInput, out int parseResult);
                if (properInput == true)
                {
                    result = parseResult;
                    return true;
                }
            }
            result = -1;
            return properInput;
        }
        private static long GetUserHours()
            //want to make sure the user inputs a number
            //and give them more than 1 attempt to do it
            //but also limit the attempts so it does not continue forever
        {
            bool isTime = false;
            int attemptCounter = 0;
            while (!isTime)
            {
                Console.WriteLine("Input time (format: h:mm)");
                string hourInput = Console.ReadLine();
                Console.Clear();
                isTime = TimeSpan.TryParseExact(hourInput, "h\\:mm", CultureInfo.InvariantCulture, out TimeSpan result);
                TimeSpan minimumTime = new TimeSpan(0, 0, 0);
                int inputCheck = result.CompareTo(minimumTime);
                if (isTime && inputCheck >= 0 )
                {
                    long date = result.Ticks;
                    return date;
                }
                else if (attemptCounter > 5)
                {
                    Console.WriteLine("Too many incorrect inputs.");
                    break;
                }
                else
                {
                    attemptCounter++;
                    Console.WriteLine("Make sure to input a positive time in format h:mm");
                    Console.WriteLine("Try Again");
                }
            }
            return -1;
        }

        private static long GetUserDate()
        {
            Console.WriteLine("T to choose todays date");
            Console.WriteLine("C to choose a different date");
            Console.WriteLine("0 to go back to the main menu");
            string UserChoice = Console.ReadLine().ToUpper();
            Console.Clear();
            switch (UserChoice)
            {
                case "T":
                    return GetTodayDate();
                case "C":
                    return DateInput();
                case "0":
                    return -1;
                default:
                   return GetUserDate();
            }
        }
        private static long DateInput()
        {
            Console.WriteLine("Input a date (format: MM-dd-yyyy) ");
            bool finished = false;
            int attempts = 0;
            while (!finished && attempts < 5 )
            {
                string userIn = Console.ReadLine();
                Console.Clear();
                var culture = CultureInfo.CreateSpecificCulture("en-US");
                var styles = DateTimeStyles.None;
                finished = DateTime.TryParse(userIn, culture, styles, out DateTime result);
                if (finished)
                {
                    return result.Ticks;
                }
                attempts++;
                if (attempts == 5)
                {
                    Console.WriteLine("Too many incorrect attempts");
                }
                else
                {
                    Console.WriteLine("Incorrect input, try again (format: MM-dd-yyyy)");
                }
            }
            return -1;
        }

        private static long GetTodayDate()
        {
            return DateTime.Today.Ticks;
        }

    }
}