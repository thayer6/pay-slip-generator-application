using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;


namespace PayrollSoftware
{
    class Program
    {
        static void Main(string[] args)
        {
            // create a new list of staff objects called myStaff
            List<Staff> myStaff = new List<Staff>();
            // create a new FileWriter object called fw
            FileWriter fw = new FileWriter();
            // create a new FileReader object called fr
            FileReader fr = new FileReader();
            // set month and year equal to 0
            int month = 0;
            int year = 0;

            // Write out the title of the program
            Console.Write("Welcome to the Pay Slip Generator!!");

            // while loop to get the year from the user
            while (year == 0)
            {
                // prompt the user to submit the year
                Console.Write("\nPlease enter the year: ");

                // convert the user input to an integer and store the year value
                try
                {
                    string userInput = Console.ReadLine();
                    year = Convert.ToInt32(userInput);
                }

                // if the format is not valid, print an error message
                catch (FormatException)
                {
                    Console.WriteLine("Please enter a valid year.");
                }
            }

            // while loop to get the month from the user
            while (month == 0)
            {
                // prompt the user to submit the month
                Console.Write("\nPlease enter the month: ");

                // convert the user input to an integer and store the month value
                try
                {
                    string userInput = Console.ReadLine();
                    month = Convert.ToInt32(userInput);

                    // if the month is less than 1 or greater than 12, print error message and reset month to 0
                    if (month < 1 || month > 12)
                    {
                        Console.WriteLine("Input is invalid. Please enter a valid month.");
                        month = 0;
                    }
                }

                // if the format is not valid, print error message
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + "Please try again.");
                }

            }


            // call the WriteFile method
            fw.WriteFile();

            // set the myStaff Staff object equal to the return value of the ReadFile method
            myStaff = fr.ReadFile();

            // loop through each staff member and prompt the user to submit the number of hours worked
            for (int i = 0; i < myStaff.Count; i++)
            {
                try
                {
                    // prompt the user to enter the number of hours worked for the staff member
                    Console.WriteLine("Enter hours worked for {0}", myStaff[i].NameOfStaff);
                    // read in the number of hours and convert to an integer
                    string userInput = Console.ReadLine();
                    myStaff[i].HoursWorked = Convert.ToInt32(userInput);
                    // calculate the pay for the staff member and print out the detailed results
                    myStaff[i].CalculatePay();
                    Console.WriteLine(myStaff[i].ToString());
                }

                // if there is an invalid input, print an error message, reset i to start the loop over
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    i--;
                }
            }

            // create a new PaySlip object and pass in the month and year values from the user
            PaySlip ps = new PaySlip(month, year);
            // generate the payslip and summary files
            ps.GeneratePaySlip(myStaff);
            ps.GenerateSummary(myStaff);
            // keep results on the output of the terminal
            Console.Read();
        }
    }

    class Staff
    {
        // declare hourlyRate and hWorked fields
        private float hourlyRate;
        private int hWorked;

        // three public auto-implemented properties
        // TotalPay is a float property with a protected setter
        public float TotalPay
        {
            get; protected set;
        }

        // BasicPay is a float property with a private setter
        public float BasicPay
        {
            get; private set;
        }

        // NameOfStaff is a string property with a private setter
        public string NameOfStaff
        {
            get; private set;
        }


        // HoursWorked is a public property with a backing field
        public int HoursWorked
        {
            // hWorked is a private int and is the backing field of the property
            get
            {
                return hWorked;
            }

            set
            {
                // if the number of hours worked is less than zero set the value to 0
                if (value > 0)
                    hWorked = value;
                else
                    hWorked = 0;
            }

        }
        // constructor Staff that accepts the name of the staff member and their hourly rate
        public Staff(string name, float rate)
        {
            NameOfStaff = name;
            hourlyRate = rate;
        }

        // define method for calculating pay
        public virtual void CalculatePay()
        {
            Console.WriteLine("Calculating Pay...");
            BasicPay = hWorked * hourlyRate;
            TotalPay = BasicPay;
        }

        // method to display the values of the fields and properties of the class
        public override string ToString()
        {
            return "\nHourly Rate: " + hourlyRate + "\nHours Worked: " +
                "\nTotal Pay: " + TotalPay + "\nBasic Pay: " + BasicPay +
                "\nHours Worked: " + HoursWorked;
        }
    }

    // Manager class is a child class of the Staff class
    class Manager : Staff
    {
        // define constant hourly rate for the managers as 50
        private const float managerHourlyRate = 50;

        // auto-implemented property with a private setter
        public int Allowance
        {
            get; private set;
        }

        // constructor Manager accepts a staff member name and calls the base constructor
        public Manager(string name) : base(name, managerHourlyRate)
        {
            
        }

        // method to calculate pay by adding allowance if worked overtime
        public override void CalculatePay()
        {
            // call the base method CalculatePay
            base.CalculatePay();
            Allowance = 1000;

            // if more than 160 hours are worked add allowance amount to total pay
            if (HoursWorked > 160)
                TotalPay = BasicPay + Allowance;
        }

        // ToString method that calls the base method
        public override string ToString()
        {
            return base.ToString();
        }

    }

    // Admin class is a child class of the Staff class
    class Admin : Staff
    {
        // define constant overtime rate and admin hourly rate
        private const float overtimeRate = 15.5f;
        private const float adminHourlyRate = 30;

        //  auto implemented property with a private setter
        public float Overtime
        {
            get; private set;
        }

        // Admin constructor that takes the name of the staff member and calls the base constructor
        public Admin(string name) : base(name, adminHourlyRate)
        {

        }

        // method to calculate the pay taking into account overtime
        public override void CalculatePay()
        {
            // call the base CalculatePay method
            base.CalculatePay();

            // if the staff member works over 160 hours, calculate overtime and add to total pay
            if (HoursWorked > 160)
            {
                Overtime = overtimeRate * (HoursWorked - 160);
                TotalPay = BasicPay + Overtime;
            }
        }

        // ToString method that calls the base method
            public override string ToString()
        {
            return base.ToString();
        }

    }

    // class to read in a file
    class FileReader
    {
        // method to read a file and return a list of Staff objects
        public List<Staff> ReadFile()
        {
            // make a new list of staff objects called myStaff
            List<Staff> myStaff = new List<Staff>();
            // define the result variable as an array of 3 strings
            string[] result = new string[2];
            // delcare the path to the text file
            string path = "staff.txt";
            // separate the strings in the array with commas
            string[] separator = { ", " };

            // check if the file exists, if not write that the file doesnt exist
            if (File.Exists(path))
            {
                // read file in line by line
                using (StreamReader sr = new StreamReader(path))
                {
                    while (!sr.EndOfStream)
                    {
                        // store each line as an element in the result array
                        result = sr.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        // if the second part of the result is a manager add the manager object to the myStaff list
                        if (result[1] == "Manager")
                            myStaff.Add(new Manager(result[0]));
                        // if the second part of the result is an admin, add the staff object to the myStaff list
                        else if (result[1] == "Admin")
                            myStaff.Add(new Admin(result[0]));
                    }
                    sr.Close();
                }
            }

            // if the file does exist print out an error message
            else
            {
                Console.WriteLine("Error: File does not exist");
            }

            return myStaff;

        }
    }

    class PaySlip
    {
        // declare month and year variables
        private int month;
        private int year;

        // define an enum for months of the year
        enum MonthsOfYear
        {
            JAN = 1, FEB = 2, MAR = 3, APR = 4, MAY = 5, JUN = 6,
            JUL = 7, AUG = 8, SEP = 9, OCT = 10, NOV = 11, DEC = 12
        }

        // PaySlip constructor that accepts pay month and pay year ints
        public PaySlip(int payMonth, int payYear)
        {
            month = payMonth;
            year = payYear;
        }

        // method to print out pay slips to individual files for each staff member
        public void GeneratePaySlip(List<Staff> myStaff)
        {
            // declare path of the file
            string path;

            // loop through all staff objects in the myStaff list
            foreach (Staff f in myStaff)
            {
                // create a new file name for each staff members text file
                path = f.NameOfStaff + ".txt";

                // write new file
                using (StreamWriter sw = new StreamWriter(path, true))
                {
                    // print payslip title with the month and the year
                    sw.WriteLine("PAYSLIP FOR {0} {1}", (MonthsOfYear)month, year);
                    sw.WriteLine("======================");
                    // print the name of the staff member, hours worked, and their basic pay
                    sw.WriteLine("Name of Staff: {0}", f.NameOfStaff);
                    sw.WriteLine("Hours Worked: {0}", f.HoursWorked);
                    sw.WriteLine("");
                    sw.WriteLine("Basic Pay: {0:C}", f.BasicPay);

                    // if the staff member is a manager, print the amount of allowance
                    if (f.GetType() == typeof(Manager))
                    {
                        sw.WriteLine("Allowance: {0:C}", ((Manager)f).Allowance);
                    }

                    // if the staff member is an admin, print the amount of overtime
                    if (f.GetType() == typeof(Admin))
                    {
                        sw.WriteLine("Overtime: {0:C}", ((Admin)f).Overtime);
                    }

                    // if another value is listed, write an error message
                    else
                    {
                        sw.WriteLine("Error: Invalid");
                    }

                    // print the total associated with the staff member
                    sw.WriteLine("");
                    sw.WriteLine("======================");
                    sw.WriteLine("Total Pay: {0}", f.TotalPay);
                    sw.WriteLine("======================");

                    // close out the file
                    sw.Close();
                }




            }

        }

        // define method to generate the summary file
        public void GenerateSummary(List<Staff> myStaff)
        {
            // use linq to select the staff members who worked less than 10 hours
            var result =
                from f in myStaff
                where (f.HoursWorked < 10)
                orderby (f.NameOfStaff)
                select f;

            // define the path for the summary text file
            string path = "summary.txt";

            // print the staff members names with less than 10 working ours
            using (StreamWriter sw = new StreamWriter(path, true))
            {
                sw.WriteLine("Staff with less than 10 working hours:");
                sw.WriteLine("");

                // loop through every staff object from the filtered list of staff members and print their name and hours worked
                foreach (Staff f in result)
                {
                    sw.WriteLine("Name of Staff: {0}, Hours Worked: {1}", f.NameOfStaff, f.HoursWorked);

                }

                // close out the file
                sw.Close();

            }
        }

        // define the ToString method that calls the base method
        public override string ToString()
        {
            return base.ToString();
        }

    }

   
    class FileWriter
    {
        // write the file that contains the staff members names and positions
        public void WriteFile()
        {
            // define the path of the staff file
            string staffFile = "staff.txt";

            // if the file already exists, overwrite it
            if (File.Exists(staffFile))
            {
                    File.Delete(staffFile);
            }
                // write the staff members and positions into a text file
                using (StreamWriter sw = new StreamWriter(staffFile, true))
                {
                    sw.WriteLine("Name, Position");
                    sw.WriteLine("Yvonne, Manager");
                    sw.WriteLine("Peter, Manager");
                    sw.WriteLine("John, Admin");
                    sw.WriteLine("Carol, Admin");

                    sw.Close();
                }
            

        }

    }


}
