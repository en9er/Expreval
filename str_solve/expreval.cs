using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using FluentAssertions;

namespace expreval
{
    public class Expreval
    {
        public bool err = false;
        public bool lackBrace = false;
        public string ReplaceSeparators(string expr)
        {
            int i = 0;
            string newStr = "";
            while (i < expr.Length)
            {
                if (expr[i] == ',')
                {
                    newStr += '.';
                }
                else
                {
                    newStr += expr[i];
                }
                i++;
            }
            return newStr;
        }
        public string ReverseString(string str)
        {
            char[] arr = str.ToCharArray();
            Array.Reverse(arr);
            return new String(arr);
        }


        public int IndexOfCorrespondingRightBrace(int leftBraceIndex, string str)
        {
            if (str[leftBraceIndex] != '(')
            {
                Console.WriteLine("The symbol with received index is not brace");
                return -1;
            }

            int innerBraces = 0;
            for (int i = leftBraceIndex + 1; i < str.Length; i++)
            {
                if (str[i] == '(')
                {
                    innerBraces++;
                }
                else if (str[i] == ')')
                {
                    innerBraces--;
                }
                if (innerBraces < 0)
                    return i;
            }
            return -1;


        }


        public double ParseArgument(string arg, bool right = false)
        {
            if (!right)
                arg = ReverseString(arg);
            double result;

            //current culture
            if (!double.TryParse(arg, System.Globalization.NumberStyles.Any, CultureInfo.CurrentCulture, out result) &&
                //US english
                !double.TryParse(arg, System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out result) &&
                //neutral language
                !double.TryParse(arg, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                return 0;
            }

            return result;
        }

        public double EvaluateExpressionWithNoBraces(string exp)
        {
            Stack<char> operators = new Stack<char>(new char[] { '-', '+', '/', '*' });
            while (operators.Count > 0)
            {
                if ((exp.IndexOf('*') == -1) && (exp.IndexOf('/') == -1) && (exp.IndexOf('+') == -1) && exp.StartsWith("-"))
                {
                    int minusCount = 0;
                    foreach (char c in exp)
                    {
                        if (c == '-')
                        {
                            minusCount++;
                        }
                    }

                    if (minusCount < 2)
                    {
                        break;
                    }
                }
                string right = "";
                string left = "";
                int index;
                if (exp[0] == '-' && operators.Peek() == '-')
                {
                    index = exp.Substring(1).IndexOf(operators.Peek());
                    index++;
                }
                else
                {
                    index = exp.IndexOf(operators.Peek());
                }
                if (index != -1)
                {
                    for (int j = index + 1; j < exp.Length; j++)
                    {
                        if (right.Length == 0 && exp[j] == '-')
                            right += exp[j];
                        else if (('0' <= exp[j] && exp[j] <= '9'))
                        {
                            right += exp[j];
                        }
                        else if (exp[j] == ',' || exp[j] == '.')
                        {
                            right += exp[j];
                        }
                        else
                            break;
                    }

                    for (int j = index - 1; j >= 0; j--)
                    {
                        if (('0' <= exp[j] && exp[j] <= '9') || (exp[j] == '.' || exp[j] == ',') || (exp[j] == '-'))
                        {
                            left += exp[j];
                            if (exp[j] == '-')
                                break;
                        }
                        else
                        {
                            break;
                        }
                    }

                    double dleft = ParseArgument(left);
                    double dright = ParseArgument(right, true);


                    double res = 0;
                    switch (operators.Peek())
                    {
                        case '*':
                            {
                                res = dleft * dright;
                                break;
                            }
                        case '/':
                            {
                                try
                                {
                                    res = dleft / dright;
                                }
                                catch (DivideByZeroException)
                                {
                                    throw new DivideByZeroException("Division by zero!");
                                }
                                break;
                            }
                        case '+':
                            {
                                res = dleft + dright;
                                break;
                            }
                        case '-':
                            {
                                res = dleft - dright;
                                break;
                            }
                    }
                    string s = exp.Substring(index - left.Length, left.Length + 1 + right.Length);
                    if (s[0] == '-')
                        exp = exp.Replace(exp.Substring(index - left.Length, left.Length + 1 + right.Length), ("+" + res.ToString()));
                    else
                        exp = exp.Replace(exp.Substring(index - left.Length, left.Length + 1 + right.Length), res.ToString());
                }
                else
                {
                    operators.Pop();
                }
            }
            double r;
            if (double.TryParse(exp, out r))
            {
                return r;
            }
            {
                err = true;
                return 0;
            }

        }



        public string EvaluateExpression(string exp)
        {
            if (exp.Contains("+") || exp.Contains("-") || exp.Contains("*") || exp.Contains("/") || exp.Contains("(") || exp.Contains(")"))
            {
                if (exp.Contains("("))
                {
                    if (exp.Contains(")"))
                    {
                        int ind = IndexOfCorrespondingRightBrace(exp.IndexOf('('), exp);
                        if (ind == -1)
                        {
                            err = true;
                            return "Error";
                        }
                        string subExpr = exp.Substring(exp.IndexOf('('), ind - exp.IndexOf('(') + 1);
                        string newExpr = EvaluateExpression(subExpr.Substring(1, subExpr.Length - 2));
                        int i = exp.IndexOf(subExpr);
                        if (i != 0)
                        {
                            if (char.IsDigit(exp[i - 1]))
                            {
                                newExpr = "*" + newExpr;
                            }
                        }
                        exp = exp.Replace(subExpr, newExpr);
                        return EvaluateExpression(exp);
                    }
                    else
                    {
                        lackBrace = true;
                        return "close )";
                        //throw new SystemException(") expected");
                    }
                }
                else if (exp.Contains(")"))
                {
                    lackBrace = true;
                    return "open (";
                    //throw new SystemException("Error ( expected");
                }
                else
                {
                    return EvaluateExpressionWithNoBraces(exp).ToString();
                }
            }
            else
            {
                return exp;
            }
        }

        public string DeleteWhiteSpaces(string expression)
        {
            string res = "";
            foreach (char sym in expression)
            {
                if (sym != ' ')
                    res += sym;
            }
            return res;
        }
    }


    [TestFixture]
    class ExprevalTests
    {
        Expreval _expreval;

        [SetUp]
        public void Setup()
        {
            _expreval = new Expreval();
        }

        [Test]
        public void ShouldReturnCorrespondingBraceIndex()
        {
            string expression = "(2+(3+5)+4)";
            Expreval tmp = new Expreval();
            expression = tmp.DeleteWhiteSpaces(expression);
            var res = tmp.IndexOfCorrespondingRightBrace(0, expression);

            res.Should().Be(10);
        }

        [TestCase("2 + 4 + ( 4 - 1 ) ", "2+4+(4-1)")]
        public void ShouldDeleteWhiteSpaces(string expression, string result)
        {
            expression = _expreval.DeleteWhiteSpaces(expression);
            expression.Should().Be(result);
        }



        [TestCase("-2+4", 2)]
        [TestCase("8.2 + 4 - 3", 9.2)]
        [TestCase("12 + 6", 18)]
        [TestCase("3", 3)]
        [TestCase("3 + 4", 7)]
        [TestCase("3 + 4 * 3.1", 15.4)]
        [TestCase("3 / 4 * 1", 0.75)]
        [TestCase("0 * 3 - 1", -1)]
        [TestCase("666 * 666", 443556)]
        [TestCase("-1", -1)]
        [TestCase("+1", 1)]
        [TestCase("5", 5)]
        [TestCase("1-3--5", 3)]
        [TestCase("1000.5-7-7-7-7-7-7-7-7-7-7-7-7-7-7-7-7-7-7-7-7", 860.5)]
        [TestCase("1000--7-7-7-7-7-7-7--7--7-7-7----------------------7-7--7---7--7-7-7-7-7---7", 937)]
        public void ShouldSolveExpressionWithNoBraces(string testString, double expected)
        {
            Expreval tmp = new Expreval();
            testString = tmp.DeleteWhiteSpaces(testString);
            var res = tmp.EvaluateExpressionWithNoBraces(testString);
            res.Should().Be(expected);
        }


        [TestCase("1 + 123 - (4 * 5)", "104")]
        [TestCase("5 + (2 - 4)", "3")]
        [TestCase("5 + 9", "14")]
        [TestCase("(3 + 4)", "7")]
        [TestCase("13 + 4 - 4 / (3 + 1)", "16")]
        [TestCase("1 + ((1 + 1) + 1) + (1 + 1) + 1", "7")]
        [TestCase("1000 - 7 * 3 - 1 - 1 - 8 - 343 - (2 - 1 * (5 * 4))", "644")]
        [TestCase("1000 - 7 - 7 - 7- 7- 7- 7- 7- 7- 7- 7- 7- 7- 7- 7- 7- 7- 7- 7- 7- 7- 7- 7- 7 -7- 7- 7- 7- 7- 7 -7", "790")]
        [TestCase("((2+3) * 4)", "20")]
        [TestCase("50-25+5000000000000000000000", "5,5E+22")]
        //[TestCase("55/555555555555555", "9,9×E−14")]
        [TestCase("(((10 / 5)+3) * 4)", "20")]
        [TestCase("", "")]
        public void ShouldReturnSolvedExpression(string testString, string expected)
        {
            Expreval tmp = new Expreval();
            testString = tmp.DeleteWhiteSpaces(testString);
            string res = tmp.EvaluateExpression(testString);
            res.Should().Be(expected);
        }


        [Test]
        public void ShouldReturnStrWithCorrectSeparators()
        {
            Expreval tmp = new Expreval();
            string exp = "2,3 + 4,2";
            string res = "2.3+4.2";
            exp = tmp.DeleteWhiteSpaces(exp);
            exp = tmp.ReplaceSeparators(exp);
            res.Should().Be(exp);
        }
    }

}
