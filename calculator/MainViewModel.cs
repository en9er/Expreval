using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace calculator
{
    class MainViewModel:INotifyPropertyChanged
    {
        public ICommand IButtonClicked { get; }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private expreval.Expreval calc = new expreval.Expreval();
        private string _expression = "";
        private string _hotExpression = "";
        public ICommand SolveCommand { get; }
        public bool Error { 
            get
            {
                return calc.err;
            }
            set
            {
                if(value != calc.err)
                    calc.err = value;
            }
        
        }
        public string Expression
        {
            get
            {
                return _expression;
            }
            set
            {
                if (value == _expression)
                    return;
                _expression = value;
                OnPropertyChanged(nameof(Expression));
            }
        }

        public string HotExpression
        {
            get
            {
                return _hotExpression;
            }
            set
            {
                if (value != _hotExpression)
                    _hotExpression = value;
                OnPropertyChanged(nameof(HotExpression));
            }
        }

        public void Solve(string exp)
        {
            exp = calc.DeleteWhiteSpaces(exp);
            exp = exp.Replace('×', '*');
            exp = exp.Replace('÷', '/');
            exp = calc.EvaluateExpression(exp);

            if(Error)
            {
                Expression = "Error";
            }
            else
            {
                Expression = exp;
            }
        }

        public void HotSolve(string exp)
        {
            exp = calc.DeleteWhiteSpaces(exp);
            exp = exp.Replace('×', '*');
            exp = exp.Replace('÷', '/');
            exp = calc.EvaluateExpression(exp);

            if (Error)
            {
                HotExpression = "Error";
            }
            else
            {
                HotExpression = exp;
            }
        }

        public MainViewModel()
        {
            IButtonClicked = new RelayCommand<string>(x =>
            {
                if (Error)
                {
                    if(x == "⌫")
                    {
                        if (Expression.Length != 0)
                            Expression = Expression.Substring(0, Expression.Length - 1);
                        Error = false;
                    }
                }
                if (x == "=")
                {
                    Solve(Expression);
                    HotExpression = "";
                }
                else if (x == "C")
                {
                    Expression = "";
                    Error = false;
                }
                else if (x == "⌫")
                {
                    if (Expression.Length != 0)
                        Expression = Expression.Substring(0, Expression.Length - 1);
                }
                else
                {
                    if (Expression == "0" && x == "0")
                    {
                        
                    }
                    else if(Expression.Contains(',') && x == ",")
                    {
                        string str = Expression;
                        int index = str.Length - 1;
                        while(char.IsDigit(str[index]))
                        {
                            index--;
                        }
                        if(str[index] != ',')
                            Expression += x;
                    }
                    else if((x == "+" || x == "-" || x == "×" || x == "÷") && Expression.Length != 0)
                    {
                        char c = Expression[Expression.Length - 1];
                        if(c == x[0])
                        {

                        }
                        else if(c == '+' || c == '-' || c == '×' || c == '÷')
                        {
                            Expression = Expression.Substring(0, Expression.Length - 1) + x;
                        }
                        else if(c == '(')
                        {

                        }
                        else
                        {
                            Expression += x;
                        }
                        
                    }
                    else
                    {
                        Expression += x;
                    }


                }
                //Hot Solve Logic
                if (x != "+" && x != "-" && x != "÷" && x != "×" && x != "=")
                {
                    HotSolve(Expression);
                }
            }, x => string.IsNullOrWhiteSpace(x) == false);
        }

    }
}