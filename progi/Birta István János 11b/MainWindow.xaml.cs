

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Calculator
{ 
    public partial class MainWindow : Window
    {
        const int maxMemoryLabelLength = 6;
        const int defaultFontSize = 48;
        bool operationCheck;
        bool functionCheck;
        bool clearNext;
        bool isResult;
        bool isOldText;
        string previousText;
        enum trigModes
        {
            STANDARD,
            HYPERBOLIC,
            ARC
        }
        trigModes currentTrigMode;
        Dictionary<trigModes, string> trigModeSymbols = new Dictionary<trigModes, string>()
        {
            { trigModes.STANDARD, "STD" },
            { trigModes.ARC, "ARC" },
            { trigModes.HYPERBOLIC, "HYP" }
        };
        Angles.units angleUnit;
        Dictionary<Angles.units, string> angleUnitSymbols = new Dictionary<Angles.units, string>()
            {
                { Angles.units.RADIANS, "RAD" },
                { Angles.units.DEGREES, "DEG" },
                { Angles.units.GRADIANS, "GRAD" }
            };
        static string OVERFLOW = "Overflow";
        static string INVALID_INPUT = "Invalid input";
        static string NOT_A_NUMBER = "NaN";
        string[] errors = { OVERFLOW, INVALID_INPUT, NOT_A_NUMBER };
        operations currentOperation = operations.NULL;
        enum operations
        {
            ADDITION,
            SUBTRACTION,
            DIVISION,
            MULTIPLICATION,
            POWER,
            NULL 
        }
        private void showText(string text, bool clear=true)
        {
            try
            {
                if (double.Parse(text) == 0)
                    text = "0";
            }
            catch (Exception)
            {
                showError(INVALID_INPUT);
                return;
            }

            if (text.Length > 25)
                return;
            if (text.Length > 10)
                resultBox.FontSize = 25;
            if (text.Length > 24)
                resultBox.FontSize = 20;
            resultBox.Text = text;
        }
        private void showError(string text)
        {
            resultBox.Text = text;
            previousText = null;
            operationCheck = false;
            clearNext = true;
            updateEquationBox("");
            currentOperation = operations.NULL;
            resetFontSize();
        }
        private void updateEquationBox(string equation, bool append=false)
        {
            equation = Regex.Replace(equation, @"(\d+)\.\s", "$1 ");
            
            if (equation.Length > 10)
                equationBox.FontSize = 18;

            if (!append)
                equationBox.Text = equation;
            else
                equationBox.Text += equation;
        }
        private double getNumber()
        {
            double number = double.Parse(resultBox.Text);
            return number;
        }
        private void resetFontSize()
        {
            resultBox.FontSize = defaultFontSize;
        }
        private void calculateResult()
        {
            if (currentOperation == operations.NULL)
                return;

            double a = double.Parse(previousText);  
            double b = double.Parse(resultBox.Text); 
            double result;

            switch(currentOperation)
            {
                case operations.DIVISION:
                    result = a / b;
                    break;
                case operations.MULTIPLICATION:
                    result = a * b;
                    break;
                case operations.ADDITION:
                    result = a + b;
                    break;
                case operations.SUBTRACTION:
                    result = a - b;
                    break;
                case operations.POWER:
                    result = Math.Pow(a, b);
                    break;
                default:
                    return;
            }

            if (errors.Contains(resultBox.Text))
                return;

            operationCheck = false;
            previousText = null;
            string equation;
            if (!functionCheck)
                equation = equationBox.Text + b.ToString();
            else
            {
                equation = equationBox.Text;
                functionCheck = false;
            }
            updateEquationBox(equation);
            showText(result.ToString());
            currentOperation = operations.NULL;
            isResult = true;
        }
        private void numberClick(object sender, RoutedEventArgs e)
        {
            isResult = false;
            Button button = (Button)sender;

            if (resultBox.Text == "0" || errors.Contains(resultBox.Text))
                resultBox.Clear();

            string text;

            if (clearNext)
            {
                resetFontSize();
                text = button.Content.ToString();
                isOldText = false;
            }
            else
                text = resultBox.Text + button.Content.ToString();

            if (!operationCheck && equationBox.Text != "")
                updateEquationBox("");
            showText(text, false);
        }
        private void function(object sender, RoutedEventArgs e)
        {
            if (errors.Contains(resultBox.Text))
                return;
            Button button = (Button)sender;
            string buttonText = button.Content.ToString();
            double number = getNumber();
            string equation = "";
            string result = "";

            switch (buttonText)
            {
                case "!":
                    if (number < 0 || number.ToString().Contains("."))
                    {
                        showError(INVALID_INPUT);
                        return;
                    }
                    if (number > 3248) 
                    {
                        showError(OVERFLOW);
                        return;
                    }
                    double res = 1;
                    if (number == 1 || number == 0)
                        result = res.ToString();
                    else
                    {
                        for (int i = 2; i <= number; i++)
                        {
                            res *= i;
                        }
                    }
                    equation = "fact(" + number.ToString() + ")";
                    result = res.ToString();
                    break;
            }

            if (operationCheck)
            {
                equation = equationBox.Text + equation;
                functionCheck = true;
            }
            updateEquationBox(equation);
            showText(result);
        }
        private void trigFunction(object sender, RoutedEventArgs e)
        {
            if (errors.Contains(resultBox.Text))
                return;
            Button button = (Button)sender;
            string buttonText = button.Content.ToString();
            string equation = "";
            string result = "";
            double number = getNumber();
            if (operationCheck)
            {
                equation = equationBox.Text + equation;
                functionCheck = true;
            }
            updateEquationBox(equation);
            showText(result);
        }
        private void doubleOperandFunction(object sender, RoutedEventArgs e)
        {
            if (errors.Contains(resultBox.Text))
                return;
            Button button = (Button)sender;
            operationCheck = true;
            previousText = resultBox.Text;
            string buttonText = button.Content.ToString();
            string equation = previousText + " " + buttonText + " ";
            switch(buttonText)
            {
                case "/":
                    currentOperation = operations.DIVISION;
                    break;
                case "x":
                    currentOperation = operations.MULTIPLICATION;
                    break;
                case "-":
                    currentOperation = operations.SUBTRACTION;
                    break;
                case "+":
                    currentOperation = operations.ADDITION;
                    break;
            }
            updateEquationBox(equation);
            resetFontSize();
            showText(resultBox.Text);
            isOldText = true;
        }
        private void decimal_button_Click(object sender, RoutedEventArgs e)
        {
            if (!resultBox.Text.Contains("."))
            {
                string text = resultBox.Text += ".";
                showText(text, false);
            }
        }
        private void Equals_button_Click(object sender, RoutedEventArgs e)
        {
            calculateResult();
        }
        private void about_button_Click(object sender, RoutedEventArgs e)
        {
            AboutBox aboutForm = new AboutBox();
            aboutForm.ShowDialog();
        }
        private void equationBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            calculateResult();
        }
    }
}
