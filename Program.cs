using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

//Student ID: 98718584
//Name: Joseph Collins
//Module: AM6004
//Assignment: 3

namespace Assignment_3_Submission
{

    //we will use delegates to implement the functions
    //func1 will handle the 1 dimension functions
    public delegate double func1(double val);

    //we will use delegates to implement the functions
    //funcN will handle the multi-dimensional functions
    public delegate double funcN(double[] val);
    class Program
    {
        static void Main(string[] args)
        {
            //we will use this temporary array at the end when we want to look at the error of our estimate versus the exact solution for System 1
            double[,] temp = null;

            //Here we allow the user to specify which one of the 3 systems they would like to work with
            string str = "";
            int choice1 = 0;
            int choice2 = 0;
            int choice4 = 0;
            double choice3 = 0;
            double a = 0, b = 0, e = 0, c = 0;
            Console.Write("3 systems.\nEnter an integer between 1 and 3 to choose the system:");
            str = Console.ReadLine();
            bool b1 = int.TryParse(str, out choice1);
            while (!b1 || ((choice1 < 1) || (choice1 > 3)))
            {
                Console.Write("\nInvalid Input. Please enter an integer between 1 and 3: ");
                str = Console.ReadLine();
                b1 = int.TryParse(str, out choice1);
            }

            FunctionMatrix F = new FunctionMatrix(2, 1);

            FunctionMatrix F1A = new FunctionMatrix(1, 1);
            F1A[0, 0] = x => 1 + x[1] / x[0] + (x[1] / x[0]) * (x[1] / x[0]);

            //variable which we will use to help define the startingvalues later on
            int dim = 0;

            //user chooses which system they want to work with
            switch (choice1)
            {
                case 1:
                    Console.Write("\nWe will work with\n\ny'[x] = 1 + y/x + (y/x)^2\ny[1]=0 and 1<=x<=4.8\n");
                    F = F1A;
                    dim = 2;
                    break;
                case 2:
                    Console.Write("\nWe will work with\n\ny''[x] - e y'[t] (1- (y'[t])^2) + y[t] =0\ny[0]=a and y'[0]=b\nWe will assume 0<=t<=20\n");
                    Console.Write("\nEnter an integer between 1 and 5 to choose a, b and e:");
                    str = Console.ReadLine();
                    bool b2 = int.TryParse(str, out choice2);
                    switch (choice2)
                    {
                        case 1:
                            a = 0;
                            b = -1;
                            e = 0.1;
                            break;
                        case 2:
                            a = 1;
                            b = 2;
                            e = 0.1;
                            break;
                        case 3:
                            a = 1;
                            b = 2;
                            e = 0.5;
                            break;
                        case 4:
                            a = 1;
                            b = 2;
                            e = 1;
                            break;
                        case 5:
                            a = 1;
                            b = 2;
                            e = 5;
                            break;
                        default:
                            a = 0;
                            b = -1;
                            e = 0.1;
                            break;
                    }
                    Console.Write("\nValues for (a,b,e) = ({0},{1},{2})\n", a, b, e);
                    FunctionMatrix F2A = new FunctionMatrix(1, 2);
                    F2A[0, 0] = x => x[2];
                    F2A[0, 1] = x => e * x[2] * (1 - x[2] * x[2]) - x[1];
                    F = F2A;
                    dim = 3;
                    break;
                case 3:
                    Console.Write("\nWe will work with\n\nx'[t] = -y[t] - z[t]\ny'[t] = x[t] + 0.1 y[t]\nz'[t] = 0.1 + (x[t] - c) z[t]\nx[0] = y[0] = z[0] = 2 \nWe will assume 0<=t<=200\n");
                    Console.Write("\n\nEnter a value for c (4,6,8.5,8.7,9,12,12.8,13,18):");
                    str = Console.ReadLine();
                    bool b3 = double.TryParse(str, out choice3);
                    while (!b3)
                    {
                        Console.Write("\nInvalid Input. Please enter a double for c: ");
                        str = Console.ReadLine();
                        b3 = double.TryParse(str, out choice3);
                    }
                    if (b3)
                        c = choice3;
                    else
                        c = 4;
                    FunctionMatrix F3A = new FunctionMatrix(1, 3);
                    F3A[0, 0] = x => -x[2] - x[3];
                    F3A[0, 1] = x => x[1] + 0.1 * x[2];
                    F3A[0, 2] = x => 0.1 + (x[1] - c) * x[3];
                    F = F3A;
                    dim = 4;
                    break;
                default:
                    break;
            }

            Console.Write("\n4 Methods: \n\n(1) Runga Kutta\n(2) 4 Step Adam-Bashforth predictor and 3 step Adams Moutlon corrector\n(3) Runga Kutta Fehlberg with Adaptive Step Size Control\n(4) ABM Predictor Correct with Adaptive Step Size Control\n\nEnter an integer between 1 and 4:");
            str = Console.ReadLine();
            bool b4 = int.TryParse(str, out choice4);
            while (!b4 || ((choice4 < 1) || (choice4 > 4)))
            {
                Console.Write("\nInvalid Input. Please enter an integer between 1 and 4: ");
                str = Console.ReadLine();
                b4 = int.TryParse(str, out choice4);
            }

            switch (choice4)
            {
                case 1:
                    Console.Write("\nYou chose Runga Kutta.\n");
                    break;
                case 2:
                    Console.Write("\nYou chose 4 Step Adam-Bashforth predictor and 3 step Adams Moutlon corrector.\n");
                    break;
                case 3:
                    Console.Write("\nYou chose Runga Kutta Fehlberg with Adaptive Step Size Control.\n");
                    break;
                case 4:
                    Console.Write("\nYou chose ABM Predictor Corrector with Adaptive Step Size Control.\n");
                    break;
                default:
                    break;
            }

            DR p = new DR();

            //if the user chooses either Method 1 or 2, they also need to specify the step size
            double stepsize = 0;
            bool b5 = false;
            if ((choice4 == 1) || (choice4 == 2))
            {
                Console.Write("\nNow please enter the step size, h:");
                str = Console.ReadLine();
                b5 = double.TryParse(str, out stepsize);
                while (!b5)
                {
                    Console.Write("\nInvalid Input. Please enter a double: ");
                    str = Console.ReadLine();
                    b5 = double.TryParse(str, out stepsize);
                }
            }

            bool b6 = false;
            bool b7 = false;
            bool b8 = false;
            double stepsizelower = 0;
            double tolerance = 0;
            string strstepsize = null;
            string strstepsizelower = null;
            string strtolerance = null;

            //if the user chooses 3 or 4 they have enter the step size, the lower step size and the tolerance
            if ((choice4 == 3) || (choice4 == 4))
            {
                Console.Write("\nNow please enter the step size:");
                strstepsize = Console.ReadLine();
                b6 = double.TryParse(strstepsize, out stepsize);
                while (!b6)
                {
                    Console.Write("\nInvalid Input. Please re-enter a doubles: ");
                    strstepsize = Console.ReadLine();
                    b6 = double.TryParse(strstepsize, out stepsize);

                }
                Console.Write("\nNow please enter the step size lower:");
                strstepsizelower = Console.ReadLine();
                b7 = double.TryParse(strstepsizelower, out stepsizelower);
                while (!b7)
                {
                    Console.Write("\nInvalid Input. Please re-enter a doubles: ");
                    strstepsizelower = Console.ReadLine();
                    b7 = double.TryParse(strstepsizelower, out stepsizelower);
                }
                Console.Write("\nNow please enter the tolerance:");
                strtolerance = Console.ReadLine();
                b8 = double.TryParse(strtolerance, out tolerance);
                while (!b8)
                {
                    Console.Write("\nInvalid Input. Please re-enter a doubles: ");
                    strtolerance = Console.ReadLine();
                    b8 = double.TryParse(strtolerance, out tolerance);
                }
            }

            //now we have all the parameters we need so we can call the appropriate method
            double[] startingvalues = new double[dim];
            double upper = 0;
            double lower = 0;
            if (dim == 2)
            {
                startingvalues[0] = 1;
                startingvalues[1] = 0;
                upper = 4.8;
                lower = 1;
            }
            if (dim == 3)
            {
                startingvalues[0] = 0;
                startingvalues[1] = a;
                startingvalues[2] = b;
                upper = 20;
                lower = 0;
            }
            if (dim == 4)
            {
                startingvalues[0] = 0;
                startingvalues[1] = 2;
                startingvalues[2] = 2;
                startingvalues[3] = 2;
                upper = 200;
                lower = 0;
            }


            //depending on which model the user chose, we will run a particular method
            //and print the results to screen
            switch (choice4)
            {
                case 1:
                    double[,] output1 = p.RungeKutta(F, startingvalues, stepsize, lower, upper);
                    if (dim == 2)
                        Console.Write("\nThe output (x,y[x]) is:\n\n");
                    else if (dim == 3)
                        Console.Write("\nThe output (t,y[t],y'[t]) is:\n\n");
                    else
                        Console.Write("\nThe output (t,x[t],y[t],z[t]) is:\n\n");
                    Thread.Sleep(4000);
                    for (int i = 0; i < output1.GetLength(0); i++)
                    {
                        if (dim == 2)
                            Console.Write("({0,3:F5},{1,3:F5}) ", output1[i, 0], output1[i, 1]);
                        else if (dim == 3)
                            Console.Write("({0,3:F5},{1,3:F5},{2,3:F5}) ", output1[i, 0], output1[i, 1], output1[i, 2]);
                        else
                            Console.Write("({0,3:F5},{1,3:F5},{2,3:F5},{3,3:F5}) ", output1[i, 0], output1[i, 1], output1[i, 2], output1[i, 3]);
                    }
                    int nrows = output1.GetLength(0);
                    int ncols = output1.GetLength(1);
                    temp = new double[nrows, ncols];
                    for (int i = 0; i < nrows; i++)
                        for (int j = 0; j < ncols; j++)
                            temp[i, j] = output1[i, j];
                    break;
                case 2:
                    double[,] output2 = p.ABMSolve(F, startingvalues, stepsize, lower, upper);
                    if (dim == 2)
                        Console.Write("\nThe output (x,y[x]) is:\n\n");
                    else if (dim == 3)
                        Console.Write("\nThe output (t,y[t],y'[t]) is:\n\n");
                    else
                        Console.Write("\nThe output (t,x[t],y[t],z[t]) is:\n\n");
                    Thread.Sleep(4000);
                    for (int i = 0; i < output2.GetLength(0); i++)
                    {
                        if (dim == 2)
                            Console.Write("({0,3:F5},{1,3:F5}) ", output2[i, 0], output2[i, 1]);
                        else if (dim == 3)
                            Console.Write("({0,3:F5},{1,3:F5},{2,3:F5}) ", output2[i, 0], output2[i, 1], output2[i, 2]);
                        else
                            Console.Write("({0,3:F5},{1,3:F5},{2,3:F5},{3,3:F5}) ", output2[i, 0], output2[i, 1], output2[i, 2], output2[i, 3]);
                    }
                    nrows = output2.GetLength(0);
                    ncols = output2.GetLength(1);
                    temp = new double[nrows, ncols];
                    for (int i = 0; i < nrows; i++)
                        for (int j = 0; j < ncols; j++)
                            temp[i, j] = output2[i, j];
                    break;
                case 3:
                    double[,] output3 = p.RKFSolve(F, startingvalues, stepsizelower, stepsize, lower, upper, tolerance);
                    if (dim == 2)
                        Console.Write("\nThe output (x,y[x]) is:\n\n");
                    else if (dim == 3)
                        Console.Write("\nThe output (t,y[t],y'[t]) is:\n\n");
                    else
                        Console.Write("\nThe output (t,x[t],y[t],z[t]) is:\n\n");
                    Thread.Sleep(4000);
                    for (int i = 0; i < output3.GetLength(0); i++)
                    {
                        if (dim == 2)
                            Console.Write("({0,3:F5},{1,3:F5}) ", output3[i, 0], output3[i, 1]);
                        else if (dim == 3)
                            Console.Write("({0,3:F5},{1,3:F5},{2,3:F5}) ", output3[i, 0], output3[i, 1], output3[i, 2]);
                        else
                            Console.Write("({0,3:F5},{1,3:F5},{2,3:F5},{3,3:F5}) ", output3[i, 0], output3[i, 1], output3[i, 2], output3[i, 3]);
                    }
                    nrows = output3.GetLength(0);
                    ncols = output3.GetLength(1);
                    temp = new double[nrows, ncols];
                    for (int i = 0; i < nrows; i++)
                        for (int j = 0; j < ncols; j++)
                            temp[i, j] = output3[i, j];
                    break;
                case 4:
                    double[,] output4 = p.ABMStepSizeSolve(F, startingvalues, stepsizelower, stepsize, lower, upper, tolerance);
                    if (dim == 2)
                        Console.Write("\nThe output (x,y[x]) is:\n\n");
                    else if (dim == 3)
                        Console.Write("\nThe output (t,y[t],y'[t]) is:\n\n");
                    else
                        Console.Write("\nThe output (t,x[t],y[t],z[t]) is:\n\n");
                    Thread.Sleep(4000);
                    for (int i = 0; i < output4.GetLength(0); i++)
                    {
                        if (dim == 2)
                            Console.Write("({0,3:F5},{1,3:F5}) ", output4[i, 0], output4[i, 1]);
                        else if (dim == 3)
                            Console.Write("({0,3:F5},{1,3:F5},{2,3:F5}) ", output4[i, 0], output4[i, 1], output4[i, 2]);
                        else
                            Console.Write("({0,3:F5},{1,3:F5},{2,3:F5},{3,3:F5}) ", output4[i, 0], output4[i, 1], output4[i, 2], output4[i, 3]);
                    }
                    nrows = output4.GetLength(0);
                    ncols = output4.GetLength(1);
                    temp = new double[nrows, ncols];
                    for (int i = 0; i < nrows; i++)
                        for (int j = 0; j < ncols; j++)
                            temp[i, j] = output4[i, j];
                    break;
                default:
                    break;
            }

            //if we want to see how the estimated solution compares to the exact solution for system 1, the following is executed
            if (choice1 == 1)
            {
                Console.Write("\n\nFor System 1, how good is our estimate?\nTo see this will will present (x, yexact - yest):\n\n");
                Thread.Sleep(4000);
                int r = temp.GetLength(0);
                for (int i = 0; i < r; i++)
                {
                    double x = temp[i, 0];
                    double yexact = x * Math.Tan(Math.Log(x));
                    double yest = temp[i, 1];
                    Console.Write("({0,3:F5},{1,3:F5}) ", x, yexact - yest);
                }
            }

            Console.ReadLine();
            //end of main

        }
    }

    //Differentiation and RootFinding class
    class DR
    {
        //we will use this instance to help calculate the inverse of matrices, solve a system of simulataneous equations etc
        system_solver s = new system_solver();

        //Runge Kutta step
        public double[] RK4Step(FunctionMatrix f, double[] vals, double h)
        {
            int neqns = f.Cols;
            int m = vals.GetLength(0);
            //array to contain the output of the update
            double[] update = new double[m];

            double[] F1 = new double[neqns];
            double[] F2 = new double[neqns];
            double[] F3 = new double[neqns];
            double[] F4 = new double[neqns];

            //Calculating F1i
            for (int i = 0; i < neqns; i++)
            {
                F1[i] = h * f[0, i](vals);
            }

            //next we need to get the updated vector in order to calculate F2i
            double[] f2vals = new double[m];
            f2vals[0] = vals[0] + h / 2;
            for (int i = 1; i < m; i++)
            {
                f2vals[i] = vals[i] + F1[i - 1] / 2;
            }
            //Calculating F2i
            for (int i = 0; i < neqns; i++)
            {
                F2[i] = h * f[0, i](f2vals);
            }

            //next we need to get the updated vector in order to calculate F3i
            double[] f3vals = new double[m];
            f3vals[0] = vals[0] + h / 2;
            for (int i = 1; i < m; i++)
            {
                f3vals[i] = vals[i] + F2[i - 1] / 2;
            }
            //Calculating F3i
            for (int i = 0; i < neqns; i++)
            {
                F3[i] = h * f[0, i](f3vals);
            }

            //next we need to get the updated vector in order to calculate F4i
            double[] f4vals = new double[m];
            f4vals[0] = vals[0] + h;
            for (int i = 1; i < m; i++)
            {
                f4vals[i] = vals[i] + F3[i - 1];
            }
            //Calculating F4i
            for (int i = 0; i < neqns; i++)
            {
                F4[i] = h * f[0, i](f4vals);
            }

            //calculating the update
            update[0] = vals[0] + h;
            for (int i = 1; i < m; i++)
            {
                //update[i]=vals[i]+1/6*(F1[i-1]+2*F2[i-1]+2*F3[i-1]+F4[i-1]);
                update[i] = vals[i] + (1.0 / 6.0) * (F1[i - 1] + 2 * F2[i - 1] + 2 * F3[i - 1] + F4[i - 1]);

            }

            return update;
        }

        public double[,] RungeKutta(FunctionMatrix f, double[] vals, double h, double lower, double upper)
        {
            int steps = (int)((upper - lower) / h);
            int m = vals.GetLength(0);
            double[,] ans = new double[steps, m];
            double[] valsold = new double[m];
            for (int i = 0; i < m; i++)
            {
                valsold[i] = vals[i];
            }
            for (int i = 0; i < steps; i++)
            {
                //add the old vals to the "ans" matrix
                for (int j = 0; j < m; j++)
                {
                    ans[i, j] = valsold[j];
                }

                double[] valsnew = RK4Step(f, valsold, h);

                //now reassign the values
                for (int k = 0; k < m; k++)
                {
                    valsold[k] = valsnew[k];
                }
            }

            //return the answer
            return ans;

            //end of method
        }

        public double[] AB3Step(FunctionMatrix f, double[] vals1, double[] vals2, double[] vals3, double h)
        {
            int neqns = f.Cols;
            int m = vals1.GetLength(0);
            //array to contain the output of the update
            double[] update = new double[m];

            double[] F1 = new double[neqns];
            double[] F2 = new double[neqns];
            double[] F3 = new double[neqns];
            double[] F4 = new double[neqns];

            //Calculating F1i
            for (int i = 0; i < neqns; i++)
            {
                F1[i] = 23.0 * f[0, i](vals1);
            }

            //Calculating F2i
            for (int i = 0; i < neqns; i++)
            {
                F2[i] = 16.0 * f[0, i](vals2);
            }

            //Calculating F3i
            for (int i = 0; i < neqns; i++)
            {
                F3[i] = 5.0 * f[0, i](vals3);
            }

            //calculating the update
            update[0] = vals1[0] + h;
            for (int i = 1; i < m; i++)
            {
                update[i] = vals1[i] + (h / 12.0) * (F1[i - 1] - F2[i - 1] + F3[i - 1]);
            }

            return update;
            //end of method
        }

        public double[,] AB3Solve(FunctionMatrix f, double[] vals, double h, double lower, double upper)
        {
            int steps = (int)((upper - lower) / h);
            int m = vals.GetLength(0);
            double[,] ans = new double[steps, m];

            //Using Runge Kutta to get initial starting points
            double[] v3 = new double[m];
            for (int i = 0; i < m; i++)
            {
                v3[i] = vals[i];
            }
            double[] v2 = RK4Step(f, v3, h);
            double[] v1 = RK4Step(f, v2, h);
            //now add these points to the ans
            for (int i = 0; i < m; i++)
            {
                ans[0, i] = v3[i];
                ans[1, i] = v2[i];
                ans[2, i] = v1[i];
            }

            //now we can start applying the AB3Step
            for (int i = 3; i < steps; i++)
            {
                double[] vupdate = AB3Step(f, v1, v2, v3, h);
                //now add vupdate to the matrix
                for (int j = 0; j < m; j++)
                    ans[i, j] = vupdate[j];
                //now we need to reassign the values
                for (int k = 0; k < m; k++)
                {
                    v3[k] = v2[k];
                    v2[k] = v1[k];
                    v1[k] = vupdate[k];
                }
            }

            return ans;
        }

        public double[] AB4Step(FunctionMatrix f, double[] vals1, double[] vals2, double[] vals3, double[] vals4, double h)
        {
            int neqns = f.Cols;
            int m = vals1.GetLength(0);
            //array to contain the output of the update
            double[] update = new double[m];

            double[] F1 = new double[neqns];
            double[] F2 = new double[neqns];
            double[] F3 = new double[neqns];
            double[] F4 = new double[neqns];

            //Calculating F1i
            for (int i = 0; i < neqns; i++)
            {
                F1[i] = 55.0 * f[0, i](vals1);
            }

            //Calculating F2i
            for (int i = 0; i < neqns; i++)
            {
                F2[i] = -59.0 * f[0, i](vals2);
            }

            //Calculating F3i
            for (int i = 0; i < neqns; i++)
            {
                F3[i] = 37.0 * f[0, i](vals3);
            }

            //Calculating F4i
            for (int i = 0; i < neqns; i++)
            {
                F4[i] = -9.0 * f[0, i](vals4);
            }

            //calculating the update
            update[0] = vals1[0] + h;
            for (int i = 1; i < m; i++)
            {
                update[i] = vals1[i] + (h / 24.0) * (F1[i - 1] + F2[i - 1] + F3[i - 1] + F4[i - 1]);
            }

            return update;
            //end of method
        }

        public double[,] AB4Solve(FunctionMatrix f, double[] vals, double h, double lower, double upper)
        {
            int steps = (int)((upper - lower) / h);
            int m = vals.GetLength(0);
            double[,] ans = new double[steps, m];

            //Using Runge Kutta to get initial starting points
            double[] v4 = new double[m];
            for (int i = 0; i < m; i++)
            {
                v4[i] = vals[i];
            }
            double[] v3 = RK4Step(f, v4, h);
            double[] v2 = RK4Step(f, v3, h);
            double[] v1 = RK4Step(f, v2, h);
            //now add these points to the ans
            for (int i = 0; i < m; i++)
            {
                ans[0, i] = v4[i];
                ans[1, i] = v3[i];
                ans[2, i] = v2[i];
                ans[3, i] = v1[i];
            }

            //now we can start applying the AB4Step
            for (int i = 4; i < steps; i++)
            {
                double[] vupdate = AB4Step(f, v1, v2, v3, v4, h);
                //now add vupdate to the matrix
                for (int j = 0; j < m; j++)
                    ans[i, j] = vupdate[j];
                //now we need to reassign the values
                for (int k = 0; k < m; k++)
                {
                    v4[k] = v3[k];
                    v3[k] = v2[k];
                    v2[k] = v1[k];
                    v1[k] = vupdate[k];
                }
            }

            return ans;
        }

        public double[] AM3Step(FunctionMatrix f, double[] vals1, double[] vals2, double[] vals3, double[] vals4, double h)
        {
            int neqns = f.Cols;
            int m = vals1.GetLength(0);
            //array to contain the output of the update
            double[] update = new double[m];

            double[] F1 = new double[neqns];
            double[] F2 = new double[neqns];
            double[] F3 = new double[neqns];
            double[] F4 = new double[neqns];

            //Calculating F1i
            for (int i = 0; i < neqns; i++)
            {
                F1[i] = 9.0 * f[0, i](vals1);
            }

            //Calculating F2i
            for (int i = 0; i < neqns; i++)
            {
                F2[i] = 19.0 * f[0, i](vals2);
            }

            //Calculating F3i
            for (int i = 0; i < neqns; i++)
            {
                F3[i] = -5.0 * f[0, i](vals3);
            }

            //Calculating F4i
            for (int i = 0; i < neqns; i++)
            {
                F4[i] = f[0, i](vals4);
            }

            //calculating the update
            update[0] = vals2[0] + h;
            for (int i = 1; i < m; i++)
            {
                update[i] = vals2[i] + (h / 24.0) * (F1[i - 1] + F2[i - 1] + F3[i - 1] + F4[i - 1]);
            }

            return update;
            //end of method
        }

        public double[,] ABMSolve(FunctionMatrix f, double[] vals, double h, double lower, double upper)
        {
            int steps = (int)((upper - lower) / h);
            int m = vals.GetLength(0);
            double[,] ans = new double[steps, m];

            //Using Runge Kutta to get initial starting points
            double[] v4 = new double[m];
            for (int i = 0; i < m; i++)
            {
                v4[i] = vals[i];
            }
            double[] v3 = RK4Step(f, v4, h);
            double[] v2 = RK4Step(f, v3, h);
            double[] v1 = RK4Step(f, v2, h);
            //now add these points to the ans
            for (int i = 0; i < m; i++)
            {
                ans[0, i] = v4[i];
                ans[1, i] = v3[i];
                ans[2, i] = v2[i];
                ans[3, i] = v1[i];
            }

            //now we can start applying the algorithm
            for (int i = 4; i < steps; i++)
            {
                double[] vupdate = AB4Step(f, v1, v2, v3, v4, h);
                //now add vupdate to the matrix
                for (int j = 0; j < m; j++)
                    ans[i, j] = vupdate[j];
                double[] vupdatenew = AM3Step(f, vupdate, v1, v2, v3, h);
                //now add vupdatenew to the matrix
                for (int j = 0; j < m; j++)
                    ans[i, j] = vupdatenew[j];


                //now we need to reassign the values
                for (int k = 0; k < m; k++)
                {
                    v4[k] = v3[k];
                    v3[k] = v2[k];
                    v2[k] = v1[k];
                    v1[k] = vupdatenew[k];
                }
            }

            return ans;

        }

        public double[,] RKFStep(FunctionMatrix f, double[] vals, double h)
        {
            int neqns = f.Cols;
            int m = vals.GetLength(0);
            //arrays to contain the output of the update
            double[] update = new double[m];
            double[] updatenew = new double[m];
            double[] R = new double[m];
            double[,] output = new double[3, m];

            double[] F1 = new double[neqns];
            double[] F2 = new double[neqns];
            double[] F3 = new double[neqns];
            double[] F4 = new double[neqns];
            double[] F5 = new double[neqns];
            double[] F6 = new double[neqns];

            //Calculating F1i
            for (int i = 0; i < neqns; i++)
            {
                F1[i] = h * f[0, i](vals);
            }

            //next we need to get the updated vector in order to calculate F2i
            double[] f2vals = new double[m];
            f2vals[0] = vals[0] + h / 4.0;
            for (int i = 1; i < m; i++)
            {
                f2vals[i] = vals[i] + F1[i - 1] / 4.0;
            }
            //Calculating F2i
            for (int i = 0; i < neqns; i++)
            {
                F2[i] = h * f[0, i](f2vals);
            }

            //next we need to get the updated vector in order to calculate F3i
            double[] f3vals = new double[m];
            f3vals[0] = vals[0] + 3.0 * h / 8.0;
            for (int i = 1; i < m; i++)
            {
                f3vals[i] = vals[i] + 3 * F1[i - 1] / 32.0 + 9 * F2[i - 1] / 32.0;
            }
            //Calculating F3i
            for (int i = 0; i < neqns; i++)
            {
                F3[i] = h * f[0, i](f3vals);
            }

            //next we need to get the updated vector in order to calculate F4i
            double[] f4vals = new double[m];
            f4vals[0] = vals[0] + 12.0 * h / 13.0;
            for (int i = 1; i < m; i++)
            {
                f4vals[i] = vals[i] + 1932.0 * F1[i - 1] / 2197.0 - 7200.0 * F2[i - 1] / 2197.0 + 7296.0 * F3[i - 1] / 2197.0;
            }
            //Calculating F4i
            for (int i = 0; i < neqns; i++)
            {
                F4[i] = h * f[0, i](f4vals);
            }

            //next we need to get the updated vector in order to calculate F5i
            double[] f5vals = new double[m];
            f5vals[0] = vals[0] + h;
            for (int i = 1; i < m; i++)
            {
                f5vals[i] = vals[i] + 439.0 * F1[i - 1] / 216.0 - 8.0 * F2[i - 1] + 3680.0 * F3[i - 1] / 513.0 - 845.0 * F4[i - 1] / 4104.0;
            }
            //Calculating F5i
            for (int i = 0; i < neqns; i++)
            {
                F5[i] = h * f[0, i](f5vals);
            }

            //next we need to get the updated vector in order to calculate F6i
            double[] f6vals = new double[m];
            f6vals[0] = vals[0] + h / 2.0;
            for (int i = 1; i < m; i++)
            {
                f6vals[i] = vals[i] - 8.0 * F1[i - 1] / 27.0 + 2.0 * F2[i - 1] - 3544.0 * F3[i - 1] / 2565.0 + 1859.0 * F4[i - 1] / 4104.0 - 11.0 * F5[i - 1] / 40.0;
            }
            //Calculating F6i
            for (int i = 0; i < neqns; i++)
            {
                F6[i] = h * f[0, i](f6vals);
            }

            //calculating the update
            update[0] = vals[0] + h;
            for (int i = 1; i < m; i++)
            {
                update[i] = vals[i] + 25.0 * F1[i - 1] / 216.0 + 1408.0 * F3[i - 1] / 2565.0 + 2197.0 * F4[i - 1] / 4104.0 - F5[i - 1] / 5.0;
            }

            //calculating updatenew
            updatenew[0] = vals[0] + h;
            for (int i = 1; i < m; i++)
            {
                updatenew[i] = vals[i] + 16.0 * F1[i - 1] / 135.0 + 6656.0 * F3[i - 1] / 12825.0 + 28561.0 * F4[i - 1] / 56430.0 - 9 * F5[i - 1] / 50.0 + 2 * F6[i - 1] / 55.0;
            }

            //calculating updatenew
            R[0] = 0;
            for (int i = 1; i < m; i++)
            {
                R[i] = 1 / h * (Math.Abs(F1[i - 1] / 360.0 - 128.0 * F3[i - 1] / 4275.0 - 2197.0 * F4[i - 1] / 75240.0 + F5[i - 1] / 50.0 + 2 * F6[i - 1] / 55.0));
            }

            //updating the output array
            for (int i = 0; i < m; i++)
            {
                output[0, i] = update[i];
                output[1, i] = updatenew[i];
                output[2, i] = R[i];
            }

            return output;
        }

        public double[,] RKFSolve(FunctionMatrix f, double[] vals, double hmin, double hmax, double lower, double upper, double toler)
        {
            double dx = hmax;
            int counter = 0;
            int iter = 1;
            int niter = 0;
            int m = vals.GetLength(0);
            //might need to change the size of the array later on
            double[,] ans = new double[30000, m];

            //add the initial step to the ans array
            for (int i = 0; i < m; i++)
                ans[0, i] = vals[i];

            //we will use this array in the while loop
            double[] valsnew = new double[m];
            for (int i = 0; i < m; i++)
                valsnew[i] = vals[i];


            while ((iter == 1) && (niter < 20000))
            {
                double[] valsold = new double[m];
                for (int i = 0; i < m; i++)
                    valsold[i] = valsnew[i];

                double[,] rklist = RKFStep(f, valsold, dx);
                double maxnorm = 0;
                for (int i = 0; i < m; i++)
                {
                    if (rklist[2, i] > maxnorm)
                        maxnorm = rklist[2, i];
                }
                //error per step is acceptable
                if (maxnorm < toler)
                {
                    counter = counter + 1;
                    for (int i = 0; i < m; i++)
                        valsnew[i] = rklist[0, i];
                    for (int i = 0; i < m; i++)
                        ans[counter, i] = valsnew[i];
                }
                //compute step size conversion factor for next step
                double delta = 0.84 * Math.Pow((toler / maxnorm), 0.25);
                //
                if (delta < 0.1)
                    dx = 0.1 * dx;
                else
                    if (delta > 4)
                        dx = 4.0 * dx;
                    else
                        dx = delta * dx;
                //
                if (dx < hmin)
                    dx = hmin;
                if (dx > hmax)
                    dx = hmax;

                if (ans[counter, 0] > upper)
                    iter = 0;
                if (ans[counter, 0] + dx > upper)
                    iter = 0;

                niter = niter + 1;
            }

            int row = 0;

            //now there might be alot of zeros in the matrix, can we determine where these are
            for (int i = 0; i < ans.GetLength(0); i++)
            {
                double sum = 0;
                for (int j = 0; j < ans.GetLength(1); j++)
                {
                    sum = sum + ans[i, j];
                }
                if (sum == 0)
                {
                    row = i;
                    break;
                }
            }
            //now return only those values which we are interested in
            double[,] output = new double[row, m];
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < ans.GetLength(1); j++)
                {
                    output[i, j] = ans[i, j];
                }
            }

            return output;
        }

        public double[,] ABMStepSizeSolve(FunctionMatrix f, double[] vals, double hmin, double hmax, double lower, double upper, double toler)
        {
            double dx = hmax;
            int counter = 3;
            int iter = 1;
            int niter = 0;
            int m = vals.GetLength(0);
            //might need to change the size of the array later on
            double[,] ans = new double[30000, m];

            //Using Runge Kutta to get initial starting points
            double[] v4 = new double[m];
            for (int i = 0; i < m; i++)
            {
                v4[i] = vals[i];
            }
            double[] v3 = RK4Step(f, v4, dx);
            double[] v2 = RK4Step(f, v3, dx);
            double[] v1 = RK4Step(f, v2, dx);
            //now add these points to the ans
            for (int i = 0; i < m; i++)
            {
                ans[0, i] = v4[i];
                ans[1, i] = v3[i];
                ans[2, i] = v2[i];
                ans[3, i] = v1[i];
            }

            while ((iter == 1) && (niter < 20000))
            {
                double[] valsold = AB4Step(f, v1, v2, v3, v4, dx);
                double[] valsnew = AM3Step(f, valsold, v1, v2, v3, dx);
                double[] rklist = new double[m];
                for (int i = 0; i < m; i++)
                {
                    rklist[i] = Math.Abs(valsnew[i] - valsold[i]);
                }
                double maxnorm = 0;
                for (int i = 0; i < m; i++)
                {
                    if (rklist[i] > maxnorm)
                        maxnorm = rklist[i];
                }
                //error per step is acceptable
                if (maxnorm < toler)
                {
                    counter = counter + 1;
                    for (int i = 0; i < m; i++)
                        ans[counter, i] = valsnew[i];
                    //now we need to reassign the values
                    for (int k = 0; k < m; k++)
                    {
                        v4[k] = v3[k];
                        v3[k] = v2[k];
                        v2[k] = v1[k];
                        v1[k] = valsnew[k];
                    }

                }
                //compute step size conversion factor for next step
                double delta = 1.5 * Math.Pow((toler / maxnorm), 0.25);
                //
                if (delta < 0.1)
                    dx = 0.1 * dx;
                else
                    if (delta > 4)
                        dx = 4.0 * dx;
                    else
                        dx = delta * dx;
                //
                if (dx < hmin)
                    dx = hmin;
                if (dx > hmax)
                    dx = hmax;

                if (ans[counter, 0] > upper)
                    iter = 0;
                if (ans[counter, 0] + dx > upper)
                    iter = 0;

                niter = niter + 1;
            }

            int row = 0;

            //now there might be alot of zeros in the matrix, can we determine where these are
            for (int i = 0; i < ans.GetLength(0); i++)
            {
                double sum = 0;
                for (int j = 0; j < ans.GetLength(1); j++)
                {
                    sum = sum + ans[i, j];
                }
                if (sum == 0)
                {
                    row = i;
                    break;
                }
            }
            //now return only those values which we are interested in
            double[,] output = new double[row, m];
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < ans.GetLength(1); j++)
                {
                    output[i, j] = ans[i, j];
                }
            }

            return output;
        }


        //method to implement richardardson extrapolation in 1 dimension
        public double derivative(func1 f, double x, double dx)
        {

            if (dx == 0)
            {
                Console.WriteLine("1 dimension Richardson Extrapolation: dx = 0 hence no output produced.");
                return 0;
            }
            else
            {
                int ntab = 10;
                double con = 1.4;
                double con2 = Math.Sqrt(con);
                double big = 1000000;
                double safe = 2.0;
                int i = 0, j = 0;
                double err = 0, errt = 0, fac = 0, hh = 0, ans = 0;
                double[,] a = new double[ntab, ntab];

                hh = dx;

                // first approximation to f'(x)
                a[0, 0] = (f(x + hh) - f(x - hh)) / (2.0 * hh);

                err = big;

                for (i = 1; i < ntab; i++)
                {
                    hh = hh / con;
                    // approximation to f'(x) with smaller step size
                    a[0, i] = (f(x + hh) - f(x - hh)) / (2.0 * hh);

                    fac = con2;

                    // extrapolate the derivative to higher orders without extra function evaluations
                    for (j = 1; j < ntab; j++)
                    {
                        a[j, i] = (a[j - 1, i] * fac - a[j - 1, i - 1]) / (fac - 1.0);

                        fac = con2 * fac;

                        errt = Math.Max(Math.Abs(a[j, i] - a[j - 1, i]), Math.Abs(a[j, i] - a[j - 1, i - 1]));

                        // compute the new error with the error from the previous step
                        if (errt <= err)
                        {
                            err = errt;
                            // update the derivative estimate
                            ans = a[j, i];
                        }

                        //end of j for loop which resides in i loop which resides in else section
                    }

                    // if error has increased significantly stop
                    if (Math.Abs(a[i, i] - a[i - 1, i - 1]) >= safe * err)
                    {
                        break;
                    }
                    //end of i for loop which resides in the else section
                }

                //Console.WriteLine("The value of the derivative is {0}", ans);

                return ans;
            }

            //end of method
        }

        //method to implement richardson extrapolation in n dimension
        public double partial_derivative(funcN f, double[] x, double dx, int n, int dirn)
        {

            if (dx == 0)
            {
                Console.WriteLine("n dimension Richardson Extrapolation: dx = 0 hence no output produced.");
                return 0;
            }
            else
            {
                int ntab = 20;
                double con = 1.4;
                double con2 = Math.Sqrt(con);
                double big = 1000000;
                double safe = 2.0;
                int i = 0, j = 0, k = 0;
                double err = 0, errt = 0, fac = 0, hh = 0, ans = 0;
                double[,] a = new double[ntab, ntab];
                double[] xphh = new double[n + 1];
                double[] xmhh = new double[n + 1];

                hh = dx;

                /*************changed the indexing here************************/
                for (k = 0; k < n; k++)
                {
                    xphh[k] = x[k];
                    xmhh[k] = x[k];
                }
                xphh[dirn] = xphh[dirn] + hh;
                xmhh[dirn] = xmhh[dirn] - hh;

                // first approximation to f'(x)
                a[0, 0] = (f(xphh) - f(xmhh)) / (2.0 * hh);

                err = big;

                for (i = 1; i < ntab; i++)
                {
                    hh = hh / con;

                    /*************changed the indexing here************************/
                    for (k = 0; k < n; k++)
                    {
                        xphh[k] = x[k];
                        xmhh[k] = x[k];
                    }
                    xphh[dirn] = xphh[dirn] + hh;
                    xmhh[dirn] = xmhh[dirn] - hh;

                    // approximation to f'(x) with smaller step size
                    a[0, i] = (f(xphh) - f(xmhh)) / (2.0 * hh);

                    fac = con2;

                    // extrapolate the derivative to higher orders without extra function evaluations
                    for (j = 1; j <= i; j++)
                    {
                        a[j, i] = (a[j - 1, i] * fac - a[j - 1, i - 1]) / (fac - 1.0);
                        fac = con2 * fac;
                        errt = Math.Max(Math.Abs(a[j, i] - a[j - 1, i]), Math.Abs(a[j, i] - a[j - 1, i - 1]));

                        // compute the new error with the error from the previous step
                        if (errt <= err)
                        {
                            err = errt;
                            // update the derivative estimate
                            ans = a[j, i];
                        }

                        //end of j loop which resides in i, which in turn resides in else
                    }

                    // if error has increased significantly stop
                    if (Math.Abs(a[i, i] - a[i - 1, i - 1]) >= safe * err)
                    {
                        break;
                    }

                    //end of i loop which resides in else
                }

                //Console.WriteLine("The value of the derivative is {0}", ans);

                return ans;
                //end of else section
            }

            //end of method
        }

        //method to calculate the Jacobi at for a system of functions at a particular vector
        public double[,] jacobi(FunctionMatrix fmat, double[] x, double dx)
        {
            int rows = fmat.Rows;
            int cols = fmat.Cols;
            int vars = x.GetLength(0);

            //we will assume that FunctionMatrix is always of the form n*1
            double[,] jac = new double[rows, vars];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < vars; j++)
                {
                    jac[i, j] = partial_derivative(fmat[i, 0], x, dx, vars, j);
                }

            }
            return jac;
            //end of method
        }

        //method to calculate the root for a system of functions given an initial guess/vector
        public double[] newtonraphson(FunctionMatrix f, double[] x, double toler)
        {
            int maxiter = 1000;
            double err = 10000000;
            int counter = 0;
            int dim = x.GetLength(0);
            int r = f.Rows;
            double[] xn = new double[dim];
            //populating vector xn
            for (int i = 0; i < dim; i++)
                xn[i] = x[i];
            double[] xnp1 = new double[dim];

            while ((counter < maxiter) && (err > toler))
            {
                Vector Fn = f.EvaluateVector(xn);
                //converting to Vector Objects
                Vector Vxn = new Vector(xn);

                //Need to calculate the Jacobi and convert it to a matrix
                double[,] j = jacobi(f, xn, 0.001);
                double[,] jinv = s.find_inverse(j, j.GetLength(0));
                Matrix Jinv = new Matrix(jinv);

                //this is just F(xn)/F'(xn)
                Vector tmp = Jinv * Fn;

                //xnp1 = xn - F(xn)/F'(xn)
                Vector Vxnp1 = Vxn - tmp;

                //what is the size of the err i.e. xnp1-xn
                Vector Diff = Vxnp1 - Vxn;
                err = Diff.maxNorm();
                double[] t = new double[dim];

                xnp1 = (double[])Vxnp1;
                xn = xnp1;
                counter++;
            }

            return xn;
            //end of method
        }

        //method to calculate the inverse of a matrix using the Sherman Morrison formula
        //this method will be utilised in the Broyden method
        public double[,] BroydenAinv(double[,] Ainvold, double[] deltax, double[] deltaF)
        {
            Matrix MAinvold = new Matrix(Ainvold);
            Vector Vdeltax = new Vector(deltax);
            Vector VdeltaF = new Vector(deltaF);
            //placeholder vector, won't be used for anything apart from calling the outer product and dot product Vector methods
            Vector interim = new Vector(1);
            Vector temp = new Vector(interim.dot(MAinvold, VdeltaF));
            Vector u = new Vector(Vdeltax - temp);
            Vector v = new Vector(interim.dot(Vdeltax, MAinvold));
            Matrix numer = new Matrix(interim.outer(u, v));
            double denom = interim.dot(Vdeltax, temp);
            Matrix update = new Matrix(1 / denom * numer);
            Matrix MAinvnew = new Matrix(MAinvold + update);
            double[,] Ainvnew = (double[,])MAinvnew;
            return Ainvnew;
        }

        //method to calculate the root for a system of functions given an initial guess/vector
        public double[] BroydenShermanMorrison(FunctionMatrix f, double[] x, double toler)
        {
            double err = 10000000;
            int counter = 0;
            int dim = x.GetLength(0);
            int r = f.Rows;
            double[] xn = new double[dim];
            //populating vector xn
            for (int i = 0; i < dim; i++)
                xn[i] = x[i];
            double[] xnp1 = new double[dim];

            //first iteration i.e. use Newton Raphson for one iteration, the Inverse of the Jacobi will be used as the Ainv for Broyden
            Vector Fn = f.EvaluateVector(xn);
            Vector Vxn = new Vector(xn);
            //Need to calculate the Jacobi and convert it to a matrix
            double[,] j = jacobi(f, xn, 0.001);
            double[,] jinv = s.find_inverse(j, j.GetLength(0));
            Matrix Jinv = new Matrix(jinv);
            //this is just F(xn)/F'(xn)
            Vector tmp = Jinv * Fn;
            //xnp1 = xn - F(xn)/F'(xn)
            Vector Vxnp1 = Vxn - tmp;
            //what is the size of the err i.e. xnp1-xn
            Vector Vdeltax = Vxnp1 - Vxn;
            err = Vdeltax.maxNorm();
            xnp1 = (double[])Vxnp1;

            Vector placeholder = new Vector(1);

            //we'll use these later                    
            Vector VFnold = f.EvaluateVector(xn);
            Vector VFnew = f.EvaluateVector(xnp1);
            Vector VdeltaF = VFnew - VFnold;
            double[,] Aold = new double[dim, dim];
            double[,] Anew = new double[dim, dim];


            //now we'll start on the Broyden iteration element
            if (err < toler)
                return xn;
            else
            {
                while ((err > toler) && (counter < 1000))
                {
                    if (counter == 0)
                    {
                        Aold = jinv;
                        Anew = BroydenAinv(Aold, (double[])Vdeltax, (double[])VdeltaF);
                    }
                    else
                    {
                        Anew = BroydenAinv(Aold, (double[])Vdeltax, (double[])VdeltaF);
                    }
                    //now update all the variables
                    Vxnp1 = Vxn - placeholder.dot(new Matrix(Anew), VFnew);
                    Vdeltax = Vxnp1 - Vxn;
                    err = Vdeltax.maxNorm();
                    Aold = Anew;
                    VFnold = f.EvaluateVector((double[])Vxn);
                    VFnew = f.EvaluateVector((double[])Vxnp1);
                    VdeltaF = VFnew - VFnold;
                    Vxn = Vxnp1;
                    xn = (double[])Vxn;
                    counter++;
                }
                return xn;
            }
            //end of method
        }

        //method to calculate the root for a system of functions given an initial guess/vector
        //Gradient Descent with Fixed Alpha
        public double[] GradientAlpha(FunctionMatrix f, double[] x, double toler)
        {
            int counter = 0;
            int dim = x.GetLength(0);
            int r = f.Rows;

            double[,] Jmat = new double[r, dim];
            Matrix MJmat = new Matrix(Jmat);
            Vector VFg1 = new Vector(r);
            Vector Vz = new Vector(dim);
            Vector VFg3 = new Vector(r);
            Vector VFg2 = new Vector(r);
            Vector VFgc = new Vector(r);
            Vector VFgnew = new Vector(r);
            Vector Vxnew = new Vector(dim);


            double alpha2 = 0;
            double alpha3 = 0;
            double[] xold = x;
            double[] xnew = new double[dim];
            double g1 = 0;
            double g2 = 0;
            double g3 = 0;
            double gc = 0;
            double gnew = 0;
            double normz = 0;
            double h1 = 0;
            double h2 = 0;
            double h3 = 0;
            double ac = 0;

            do
            {
                VFg1 = f.EvaluateVector(xold);
                g1 = VFg1.Magnitude();

                //calculate the Jacobian
                Jmat = jacobi(f, xold, 0.001);
                //now update the Matrix object which represents the Jacobian
                for (int i = 0; i < r; i++)
                    for (int j = 0; j < dim; j++)
                        MJmat[i, j] = Jmat[i, j];


                Vz = 2 * (MJmat * VFg1);

                normz = Math.Sqrt(Vz.Magnitude());

                if (normz > toler)
                {
                    Vz = (1 / normz) * Vz;
                    alpha3 = 1;

                    VFg3 = f.EvaluateVector((double[])((Vector)xold - (alpha3 * Vz)));
                    g3 = VFg3.Magnitude();

                    while (Math.Abs(g3 - g1) > toler)
                    {
                        alpha3 = 0.5 * alpha3;

                        VFg3 = f.EvaluateVector((double[])((Vector)xold - (alpha3 * Vz)));
                        g3 = VFg3.Magnitude();
                        if (Math.Abs(alpha3) < 0.5 * toler)
                            break;
                    }

                    alpha2 = 0.5 * alpha3;
                    VFg2 = f.EvaluateVector((double[])((Vector)xold - (alpha2 * Vz)));
                    g2 = VFg2.Magnitude();

                    //perform interpolation
                    h1 = (g2 - g1) / alpha2;
                    h2 = (g3 - g2) / (alpha3 - alpha2);
                    h3 = (h2 - h1) / alpha3;
                    ac = 0.5 * (alpha2 - (h1 / h3));

                    VFgc = f.EvaluateVector((double[])((Vector)xold - (ac * Vz)));
                    gc = VFgc.Magnitude();

                    if (gc < g3)
                    {
                        Vxnew = ((Vector)xold) - (ac * Vz);
                        VFgnew = f.EvaluateVector((double[])Vxnew);
                        gnew = VFgnew.Magnitude();
                    }
                    else
                    {
                        Vxnew = ((Vector)xold) - (alpha3 * Vz);
                        VFgnew = f.EvaluateVector((double[])Vxnew);
                        gnew = VFgnew.Magnitude();
                    }

                    //test for convergence
                    if (Math.Abs(gnew - g1) < toler)
                        break;
                    else
                    {
                        xold = (double[])Vxnew;
                        counter++;
                    }

                    //end if
                }



            } while (counter < 100);

            return (double[])Vxnew;
            //end of method
        }

        //end of class
    }

    //FunctionMatrix class
    class FunctionMatrix
    {
        private int rows = 0;
        public int Rows
        {
            get { return rows; }

        }
        private int cols = 0;
        public int Cols
        {
            get { return cols; }

        }

        private funcN[,] f;
        public FunctionMatrix(int rows, int cols)
        {
            if (rows >= 1 && cols >= 1)
            {
                this.rows = rows;
                this.cols = cols;
                f = new funcN[rows, cols];
            }
        }
        public funcN this[int row, int col]
        {
            get
            {
                if (row >= 0 && row < rows && col >= 0 && col < cols)
                    return f[row, col];
                else
                    return null;
            }
            set
            {
                if (row >= 0 && row < rows && col >= 0 && col < cols)
                    f[row, col] = value;
            }
        }
        public Matrix EvaluateMatrix(double[] val)
        {
            Matrix tmp = new Matrix(rows, cols);
            int i, j;
            for (i = 0; i < rows; i++)
                for (j = 0; j < cols; j++)
                    tmp[i, j] = f[i, j](val);
            return tmp;
        }
        public Vector EvaluateVector(double[] val)
        {
            Vector tmp = new Vector(rows);
            int i;
            for (i = 0; i < rows; i++)
                tmp[i] = f[i, 0](val);
            return tmp;
        }


        //end of class
    }

    //Matrix Class
    class Matrix
    {
        private int rows;

        public int Rows
        {
            get { return rows; }
        }
        private int cols;

        public int Cols
        {
            get { return cols; }
        }
        private double[,] data;

        public Matrix(int rows, int cols)
        {
            if (rows > 0)
                this.rows = rows;
            else
                this.rows = 1;
            if (cols > 0)
                this.cols = cols;
            else
                this.cols = 1;
            data = new double[rows, cols];

        }
        public Matrix(double[,] x)
        {
            if (x.GetLength(0) > 0)
                this.rows = x.GetLength(0);
            else
                this.rows = 1;
            if (x.GetLength(1) > 0)
                this.cols = x.GetLength(1);
            else
                this.cols = 1;
            data = new double[this.rows, this.cols];
            for (int i = 0; i < this.rows; i++)
                for (int j = 0; j < this.cols; j++)
                    data[i, j] = x[i, j];
        }

        public Matrix(int rows, int cols, double dval)
        {
            if (rows > 0)
                this.rows = rows;
            else
                this.rows = 1;
            if (cols > 0)
                this.cols = cols;
            else
                this.cols = 1;
            data = new double[rows, cols];
            int i, j;
            for (i = 0; i < rows; i++)
                for (j = 0; j < cols; j++)
                    if (i == j)
                        data[i, j] = dval;
        }
        public Matrix(Matrix val)
        {
            rows = val.rows;
            cols = val.cols;
            data = new double[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    data[i, j] = val.data[i, j];
        }
        public Matrix getLDiagonal()
        {//the equivalent lower diagonal matrix
            int i, j;
            Matrix tmp = new Matrix(rows, cols);
            for (i = 0; i < rows; i++)
                for (j = 0; j < cols; j++)
                    if (i > j)
                        tmp[i, j] = this[i, j];
            return tmp;
        }
        public Matrix getUDiagonal()
        {//the equivalent lower diagonal matrix
            int i, j;
            Matrix tmp = new Matrix(rows, cols);
            for (i = 0; i < rows; i++)
                for (j = 0; j < cols; j++)
                    if (j > i)
                        tmp[i, j] = this[i, j];
            return tmp;
        }
        public Matrix getDiagonal()
        {//the equivalent lower diagonal matrix
            int i, j;
            Matrix tmp = new Matrix(rows, cols);
            for (i = 0; i < rows; i++)
                for (j = 0; j < cols; j++)
                    if (i == j)
                        tmp[i, j] = this[i, j];
            return tmp;
        }
        public Matrix getInverseDiagonal()
        {//the equivalent lower diagonal matrix
            int i, j;
            Matrix tmp = new Matrix(rows, cols);
            for (i = 0; i < rows; i++)
                for (j = 0; j < cols; j++)
                    if (i == j && this[i, j] != 0)
                        tmp[i, j] = 1 / this[i, j];
            return tmp;
        }
        public static Matrix operator +(Matrix a, Matrix b)
        {
            if (a.rows != b.rows || a.cols != b.cols)
                return null;
            Matrix tmp = new Matrix(a.rows, a.cols);
            for (int i = 0; i < a.rows; i++)
                for (int j = 0; j < a.cols; j++)
                    tmp.data[i, j] = a.data[i, j] + b.data[i, j];
            return tmp;
        }
        public static Matrix operator -(Matrix a, Matrix b)
        {
            if (a.rows != b.rows || a.cols != b.cols)
                return null;
            Matrix tmp = new Matrix(a.rows, a.cols);
            for (int i = 0; i < a.rows; i++)
                for (int j = 0; j < a.cols; j++)
                    tmp.data[i, j] = a.data[i, j] - b.data[i, j];
            return tmp;
        }
        public static Matrix operator +(Matrix a)
        {
            Matrix tmp = new Matrix(a);
            return tmp;
        }
        public static Matrix operator -(Matrix a)
        {
            Matrix tmp = new Matrix(a.rows, a.cols);
            for (int i = 0; i < a.rows; i++)
                for (int j = 0; j < a.cols; j++)
                    tmp.data[i, j] = -a.data[i, j];
            return tmp;
        }
        public static Matrix operator *(Matrix a, Matrix b)
        {
            Matrix tmp = new Matrix(a.rows, b.cols);
            double sum = 0;
            for (int i = 0; i < a.rows; i++)
                for (int j = 0; j < b.cols; j++)
                {
                    sum = 0;
                    for (int k = 0; k < b.rows; k++)
                        sum += a[i, k] * b[k, j];
                    tmp.data[i, j] = sum;
                }
            return tmp;
        }
        public static Matrix operator *(double a, Matrix b)
        {
            Matrix tmp = new Matrix(b.rows, b.cols);
            for (int i = 0; i < b.rows; i++)
                for (int j = 0; j < b.cols; j++)
                {
                    tmp.data[i, j] = a * b[i, j];
                }
            return tmp;
        }
        public void Output()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                    Console.Write("{0,4:F2}\t", data[i, j]);
                Console.WriteLine();
            }
            Console.WriteLine("\n");
        }
        public void setValue(int row, int col, double val)
        {
            if (!(row >= 0 && row < rows && col >= 0 && col < cols))
                return;
            data[row, col] = val;
        }
        public static explicit operator Matrix(double d)
        {
            Matrix tmp = new Matrix(3, 3);
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    tmp.data[i, j] = d;
            return tmp;
        }
        //matrix to double array
        public static explicit operator double[,](Matrix m)
        {
            double[,] tmp = new double[m.rows, m.cols];

            for (int i = 0; i < m.Rows; i++)
                for (int j = 0; j < m.Cols; j++)
                    tmp[i, j] = m.data[i, j];
            return tmp;
        }
        //indexer function
        public double this[int rowindex, int colindex]
        {
            get
            {
                if (rowindex < rows && rowindex >= 0 && colindex < cols && colindex >= 0)
                    return data[rowindex, colindex];
                else
                    return 0;
            }
            set
            {
                if (rowindex < rows && rowindex >= 0 && colindex < cols && colindex >= 0)
                    data[rowindex, colindex] = value;
                //else do nothing
            }
        }

        //end of class
    }

    //vector class
    class Vector
    {
        private int rows;

        public int Rows
        {
            get { return rows; }
        }

        private double[] data;
        public Vector(int rows)
        {
            if (rows > 0)
                this.rows = rows;
            else
                this.rows = 1;
            data = new double[rows];
        }
        public Vector(Vector val)
        {
            rows = val.rows;
            data = new double[rows];
            for (int i = 0; i < rows; i++)
                data[i] = val.data[i];
        }
        public Vector(double[] x)
        {
            rows = x.GetLength(0);
            data = new double[rows];
            for (int i = 0; i < rows; i++)
                data[i] = x[i];
        }
        public static Vector operator +(Vector a, Vector b)
        {
            if (a.rows != b.rows)
                return null;
            Vector tmp = new Vector(a.rows);
            for (int i = 0; i < a.rows; i++)
                tmp.data[i] = a.data[i] + b.data[i];
            return tmp;
        }
        public static Vector operator -(Vector a, Vector b)
        {
            if (a.rows != b.rows)
                return null;
            Vector tmp = new Vector(a.rows);
            for (int i = 0; i < a.rows; i++)
                tmp.data[i] = a.data[i] - b.data[i];
            return tmp;
        }
        public static Vector operator +(Vector a)
        {
            Vector tmp = new Vector(a);
            return tmp;
        }
        public static Vector operator -(Vector a)
        {
            Vector tmp = new Vector(a.rows);
            for (int i = 0; i < a.rows; i++)
                tmp.data[i] = -a.data[i];
            return tmp;
        }
        //scalar product
        public static double operator *(Vector a, Vector b)
        {
            if (a.rows != b.rows)
                return 0;
            Vector tmp = new Vector(a.rows);
            double sum = 0;
            for (int i = 0; i < a.rows; i++)
                sum += a[i] * b[i];
            return sum;
        }
        //scalar product
        public static Vector operator *(double a, Vector b)
        {
            Vector tmp = new Vector(b.rows);
            for (int i = 0; i < b.rows; i++)
                tmp[i] = a * b[i];
            return tmp;
        }

        //Product of Matrix and vector
        public static Vector operator *(Matrix a, Vector b)
        {
            if (a.Cols != b.rows)
                return null;
            int i, j;
            double sum = 0;
            Vector tmp = new Vector(a.Rows);
            for (i = 0; i < a.Rows; i++)
            {
                sum = 0;
                for (j = 0; j < a.Cols; j++)
                    sum += a[i, j] * b[j];
                tmp[i] = sum;
            }
            return tmp;
        }
        public void Output()
        {
            for (int i = 0; i < rows; i++)
            {
                Console.Write("{0,4:F5}\t", data[i]);
            }
            Console.WriteLine();
        }
        public void setValue(int row, double val)
        {
            if (!(row >= 0 && row < rows))
                return;
            data[row] = val;
        }
        public static explicit operator Vector(double d)
        {
            Vector tmp = new Vector(3);
            for (int i = 0; i < 3; i++)
                tmp.data[i] = d;
            return tmp;
        }
        //double array to Vector
        public static explicit operator Vector(double[] val)
        {
            Vector tmp = new Vector(val.Length);
            for (int i = 0; i < val.Length; i++)
                tmp.data[i] = val[i];
            return tmp;
        }
        //vector to a double array
        public static explicit operator double[](Vector val)
        {
            double[] tmp = new double[val.Rows];
            for (int i = 0; i < val.Rows; i++)
                tmp[i] = val[i];
            return tmp;
        }
        //indexer function
        public double this[int rowindex]
        {
            get
            {
                if (rowindex < rows && rowindex >= 0)
                    return data[rowindex];
                else
                    return 0;
            }
            set
            {
                if (rowindex < rows && rowindex >= 0)
                    data[rowindex] = value;
                //else do nothing
            }
        }
        public double maxNorm()
        {
            double max = 0, val;
            for (int i = 0; i < rows; i++)
                if ((val = Math.Abs(data[i])) > max)
                    max = val;
            return max;
        }

        //dot product of two vectors
        public double dot(Vector v1, Vector v2)
        {
            int r1 = v1.Rows;
            int r2 = v2.Rows;
            if (r1 == r2)
            {
                double sum = 0;
                for (int i = 0; i < r1; i++)
                {
                    sum = sum + v1.data[i] * v2.data[i];
                }
                return sum;
            }
            else return 0;
        }
        //dot product of a matrix and a vector
        public Vector dot(Matrix m, Vector v)
        {
            int mrows = m.Rows;
            int mcols = m.Cols;
            int vrows = v.Rows;
            if (mcols == vrows)
            {
                double[] temp = new double[mrows];
                for (int i = 0; i < mrows; i++)
                {
                    double sum = 0;
                    for (int j = 0; j < vrows; j++)
                    {
                        double c = m[i, j] * v[j];
                        sum = sum + m[i, j] * v[j];
                    }
                    temp[i] = sum;
                }
                Vector ans = new Vector(temp);
                return ans;
            }
            else
                return v;
        }
        //dot product of a vector and a Matrix
        public Vector dot(Vector v, Matrix m)
        {
            int mrows = m.Rows;
            int mcols = m.Cols;
            int vrows = v.Rows;
            if (mcols == vrows)
            {
                double[] temp = new double[mrows];
                for (int i = 0; i < mrows; i++)
                {
                    double sum = 0;
                    for (int j = 0; j < vrows; j++)
                    {
                        double c = m[i, j] * v[j];
                        sum = sum + m[i, j] * v[j];
                    }
                    temp[i] = sum;
                }
                Vector ans = new Vector(temp);
                return ans;
            }
            else
                return v;
        }


        //outer productover of two vectors
        //dot product of two vectors
        public Matrix outer(Vector v1, Vector v2)
        {
            int r1 = v1.Rows;
            int r2 = v2.Rows;
            double[,] temp = new double[v1.rows, v2.rows];
            for (int i = 0; i < r1; i++)
            {
                for (int j = 0; j < r2; j++)
                {
                    temp[i, j] = v1.data[i] * v2.data[j];
                }
            }
            Matrix m = new Matrix(temp);
            return m;
        }

        //returns the magnitude of the 
        public double Magnitude()
        {
            double sum = 0;
            for (int i = 0; i < this.Rows; i++)
            {
                sum = sum + data[i] * data[i];
            }
            return sum;
        }

        //end of class
    }

    //system solver class, i.e. matrix inverse, solving simulataneous equations etc
    class system_solver
    {

        // compute the LU decomposition of a matrix a using Crout's algorithm
        // a is written over by this function
        // upon output a contains L and U
        public void ludcmp(double[,] a, int[] indx, int size, ref double d)
        {
            /********************************NOT SURE HOW d is meant to be used******/
            int i = 0, imax = 0, j = 0, k = 0;
            double big = 0, dum = 0, sum = 0, temp = 0;
            double TINY = (1.0e-15);
            // create a vector for storing values for scaling
            double[] vv = new double[size + 1];

            /*******NOT SURE IF THIS IS WHAT THE C++ code is trying to do***********/
            // This keeps track any row interchanges that are made
            d = 1.0;

            // store scaling values in vv
            /*INDEXING CHANGE HERE*/
            for (i = 0; i < size; i++)
            {
                big = 0.0;
                /*INDEXING CHANGE HERE*/
                for (j = 0; j < size; j++)
                {
                    if ((temp = Math.Abs(a[i, j])) > big)
                    {
                        big = temp;
                    }
                }
                if (big == 0.0)
                {
                    Console.WriteLine("Singular matrix in routine LUDCMP");
                }

                vv[i] = 1.0 / big;
            }

            /*INDEXING CHANGE HERE*/
            for (j = 0; j < size; j++)
            {
                // Start applying Crout's algorithm for computing the LU Decomposition
                /*INDEXING CHANGE HERE*/
                for (i = 0; i < j; i++)
                {
                    sum = a[i, j];
                    for (k = 0; k < i; k++)
                    {
                        sum -= a[i, k] * a[k, j];
                    }
                    a[i, j] = sum;
                }

                // Find the pivot element
                big = 0.0;
                /*INDEXING CHANGE HERE*/
                for (i = j; i < size; i++)
                {
                    sum = a[i, j];
                    /*INDEXING CHANGE HERE*/
                    for (k = 0; k < j; k++)
                    {
                        sum -= a[i, k] * a[k, j];
                    }
                    a[i, j] = sum;
                    if ((dum = vv[i] * Math.Abs(sum)) >= big)
                    {
                        big = dum;
                        imax = i;
                    }
                }

                // Simulate a row swap if required
                if (j != imax)
                {
                    /*INDEXING CHANGE HERE*/
                    for (k = 0; k < size; k++)
                    {
                        dum = a[imax, k];
                        a[imax, k] = a[j, k];
                        a[j, k] = dum;
                    }
                    d = -(d);
                    vv[imax] = vv[j];
                }


                indx[j] = imax;

                if (a[j, j] == 0.0)
                {
                    a[j, j] = TINY;
                }

                // Scale a row by the pivot element
                if (j != size)
                {
                    dum = 1.0 / (a[j, j]);
                    /*INDEXING CHANGE HERE*/
                    for (i = j + 1; i < size; i++)
                    {
                        a[i, j] *= dum;
                    }
                }

                //end of Crout algorithm element
            }

            //end of method
        }

        //method to determine the inverse
        public double[,] find_inverse(double[,] a, int size)
        {
            // Perform the steps necessary to compute the inverse of the matrix A
            // A copy of a is made
            // The lu decomposition of a is stored in this array
            // the inverse of a is returned by this function

            // This variable is used to keep track of row interchanges inside the lu decomposition function
            double dtmp = 0.0;
            double d = 0;
            d = dtmp;

            // Create the necessary arrays
            int[] indx = new int[size + 1];
            double[] col = new double[size + 1];

            // Declare matrices to hold the data
            double[,] lua = new double[size + 1, size + 1];
            double[,] ainv = new double[size + 1, size + 1];

            // copy a into lua, then compute its lu decomposition
            for (int i = 0; i < size; i++)
            {
                indx[i] = 0;
                for (int j = 0; j < size; j++)
                {
                    lua[i, j] = a[i, j];
                }
            }

            // compute the lu decomposition of a, this is stored in lua
            ludcmp(lua, indx, size, ref d);

            luinv(lua, ainv, col, indx, size);

            //Joe Collins inserted this additional code here
            double[,] temp = new double[size, size];
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    temp[i, j] = ainv[i, j];

            return temp;
        }

        public void luinv(double[,] a, double[,] y, double[] col, int[] indx, int size)
        {
            // find the inverse of a matrix from its LU Decomposition
            // inverting by solving a.x=col, where col is a unit vector
            int i, j;
            /*INDEXING CHANGE HERE*/
            for (j = 0; j < size; j++)
            {
                /*INDEXING CHANGE HERE*/
                for (i = 0; i < size; i++)
                {
                    col[i] = 0.0;
                }

                col[j] = 1.0;
                lubksb(a, indx, col, size);
                /*INDEXING CHANGE HERE*/
                for (i = 0; i < size; i++)
                {
                    y[i, j] = col[i];
                }
            }

            //end of method
        }

        public void lubksb(double[,] a, int[] indx, double[] b, int size)
        {
            // solve the system of linear equations a.x = b by LU Decomposition
            // a is assumed to contain its LU decomposition
            // indx contains any row swap information
            // b contains the solution of the system of equations upon output

            int i = 0, ii = 0, ip = 0, j = 0;
            double sum = 0;

            // Forward substitution to solve L.y = b
            /*INDEX CHANGE HERE*/
            for (i = 0; i < size; i++)
            {

                ip = indx[i];
                sum = b[ip];
                b[ip] = b[i];
                /*LOGICAL CHANGE HERE, ORIGINALLY IT WAS if(ii)*/
                if (ii >= 0)
                {
                    for (j = ii; j <= i - 1; j++)
                    {
                        sum -= a[i, j] * b[j];
                    }
                }
                /*LOGICAL CHANGE HERE, originally it was if(sum)*/
                else if (sum != 0)
                {
                    ii = i;
                }
                b[i] = sum;
            }

            // Back substitution to solve U.x = y
            /*INDEX CHANGE HERE*/
            for (i = size - 1; i >= 0; i--)
            {
                /*INDEX CHANGE HERE*/
                sum = b[i];
                for (j = i + 1; j <= size; j++)
                {
                    sum -= a[i, j] * b[j];
                }
                // store the solution in b
                b[i] = sum / (a[i, i]);
            }

            //END OF METHOD
        }

        public double[] solve_system(double[,] a, double[] b, int size)
        {
            // Sovle the linear system A.x = b using LU Decomposition

            // This variable is used to keep track of row interchanges inside the lu decomposition function
            //double dtmp = 0.0;
            double d = 0;

            // Create the necessary arrays
            int[] indx = new int[size + 1];
            double[] sol = new double[size + 1];

            // Declare matrices to hold the data
            double[,] lua = new double[size + 1, size + 1];

            // copy a into lua, then compute its lu decomposition
            for (int i = 0; i < size; i++)
            {
                indx[i] = 0;
                sol[i] = b[i];
                for (int j = 0; j < size; j++)
                {
                    lua[i, j] = a[i, j];
                }
            }

            // compute the lu decomposition of a, this is stored in lua
            ludcmp(lua, indx, size, ref d);

            // solve the system using the backsubstitution
            lubksb(lua, indx, sol, size);

            return sol;
            //end of method
        }

        public void ludet(double[,] a, ref double d, int size)
        {
            // compute the determinant of a matrix from its LU decomposition
            int j = 0;
            for (j = 0; j < size; j++)
            {
                d *= a[j, j];
            }
        }

        //end of class        
    }


    //end of namespace
}
