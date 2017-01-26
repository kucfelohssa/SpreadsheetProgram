﻿// Skeleton written by Joe Zachary for CS 3500, January 2017
// Appended by Ellen Brigance, January 25, 2017

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Formulas
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  Provides a means to evaluate Formulas.  Formulas can be composed of
    /// non-negative floating-point numbers, variables, left and right parentheses, and
    /// the four binary operator symbols +, -, *, and /.  (The unary operators + and -
    /// are not allowed.)
    /// </summary>
    public class Formula
    {
        /// <summary>
        /// Creates a Formula from a string that consists of a standard infix expression composed
        /// from non-negative floating-point numbers (using C#-like syntax for double/int literals), 
        /// variable symbols (a letter followed by zero or more letters and/or digits), left and right
        /// parentheses, and the four binary operator symbols +, -, *, and /.  White space is
        /// permitted between tokens, but is not required.
        /// 
        /// Examples of a valid parameter to this constructor are:
        ///     "2.5e9 + x5 / 17"
        ///     "(5 * 2) + 8"
        ///     "x*y-2+35/9"
        ///     
        /// Examples of invalid parameters are:
        ///     "_"
        ///     "-5.3"
        ///     "2 5 + 3"
        /// 
        /// If the formula is syntacticaly invalid, throws a FormulaFormatException with an 
        /// explanatory Message.
        /// </summary>
        public Formula(String formula)
        {
            //Check for tokens.
            //No tokens, throw FormulaFormatException.
            if (formula == null || formula == "")
            {
                throw new FormulaFormatException("No tokens given, formula format is invalid.");
            }

            //Deal with the formula, using evaluation of stacks.
            IEnumerable<string> s;
            Stack<string> formStack = new Stack<string>();
            for (int i = 0; i < formula.Length; i++)
            {
                s = GetTokens(formula);//Convert string to tokens, one at a time. Deal with them on the stacks accordingly.

                string token = s.ToString();
                //string topStack = "";
                //if(formStack.Count > 0)//Hold the value at the top of the stack.
                //{
                //   topStack = formStack.Peek();
                //}

                double output;
                if (Double.TryParse((token), out output) && output < 0)//Check for negative numbers
                {
                    throw new FormulaFormatException("Invalid token found, negative numbers not allowed.");
                }
                
                //CHECKS FOR INVALID CHARACTERS (not an operator, a variable, or an open/closed paren)
                if(!isOperator(token) && !(output > 0) && !Char.IsLetter(token[0]) && token != "(" && token != ")")
                {
                    throw new FormulaFormatException("Invalid token. Symbol not allowed.");
                }
                //IF WE HAVE A DOUBLE.
                if (formStack.Count == 0 && output >= 0)
                {
                    formStack.Push(token);
                    continue;
                }
                if (formStack.Count > 0 && output >= 0 && (formStack.Peek() == "/" || formStack.Peek() == "*"
                    || formStack.Peek() == "+" || formStack.Peek() == "-"))
                {
                    formStack.Pop();//Pop the operator and operand on the stack.
                    formStack.Pop();
                    continue;
                }
                if (formStack.Count > 0 && output >= 0
                    && (formStack.Peek() != "(") || !isOperator(formStack.Peek()))//No paren/operand on stack.
                {
                    throw new FormulaFormatException("Formula format invalid. Two operands in sequence.");
                }
                //IF WE HAVE AN OPEN PARENTESES.
                if (formStack.Count == 0 && token == "(")//Stack empty.
                {
                    formStack.Push(token);
                    continue;
                }
                if (formStack.Count > 0 && token == "(" && (formStack.Peek() == "("
                    || isOperator(formStack.Peek())))//Peek is a paren or operand.
                {
                    formStack.Push(token);
                    continue;
                }
                if (formStack.Count > 0 && token == "(" && (!isOperator(formStack.Peek())
                    || formStack.Peek() != "("))
                {
                    throw new FormulaFormatException("Formula invalid. Open parentheses adjacent to operand.");
                }
                //IF WE HAVE A CLOSED PARENTHESES.
                if (formStack.Count == 0 && token == ")")
                {
                    throw new FormulaFormatException("Formula invalid. Too many closing parenteses.");
                }
                if (formStack.Count > 0 && token == ")") //If the stack is full, pop until we see a "("
                {
                    while (formStack.Count > 0)
                    {
                        if (formStack.Peek() != "(")
                        {
                            formStack.Pop();
                        }
                    }
                    if (formStack.Count == 0)//We never saw a "("
                    {
                        throw new FormulaFormatException("Formula invalid. Too many closing parenteses.");
                    }
                    if (formStack.Peek() == "(")//We found a "("
                    {
                        formStack.Pop();
                        continue;
                    }
                }
                //IF WE HAVE AN OPERATOR.
                if (isOperator(token) && formStack.Count == 0)
                {
                    throw new FormulaFormatException("Formula invalid. Operator without operands.");
                }
                if (isOperator(token) && formStack.Count > 0 && isOperator(formStack.Peek()))//Stack top is also operand.
                {
                    throw new FormulaFormatException("Formula invalid. Two adjacent operands.");
                }
                if (isOperator(token) && formStack.Count > 0 && formStack.Peek() == "(")
                {
                    throw new FormulaFormatException("Formula invalid. Operator without operand.");
                }
                if (isOperator(token) && formStack.Count > 0)//At this point, we assume we have a valid operand.
                {
                    formStack.Push(token);
                    continue;
                }
                //IF WE HAVE A VARIABLE.
                if(formStack.Count == 0 && Char.IsLetter(token[0]))
                {
                    formStack.Push(token);
                    continue;
                }
                if(Char.IsLetter(token[0]) && formStack.Count > 0 && (isOperator(formStack.Peek()) || formStack.Peek() == "("))//Have an operator or an open paren.
                {
                    formStack.Push(token);
                    continue;
                }
                if(Char.IsLetter(token[0]) && formStack.Count > 0 && (!isOperator(formStack.Peek()) || formStack.Peek() != "("))//Have no operator or open paren.
                {
                    throw new FormulaFormatException("Formula invalid. Two operands adjacent.");
                }
            }
        }

        /// <summary>
        /// Returns true if we are looking at a valid operand. False, otherwise.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private bool isOperator(string s)
        {
            if (s == "/" || s == "*" || s == "+" || s == "-")
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Evaluates this Formula, using the Lookup delegate to determine the values of variables.  (The
        /// delegate takes a variable name as a parameter and returns its value (if it has one) or throws
        /// an UndefinedVariableException (otherwise).  Uses the standard precedence rules when doing the evaluation.
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, its value is returned.  Otherwise, throws a FormulaEvaluationException  
        /// with an explanatory Message.
        /// </summary>
        public double Evaluate(Lookup lookup)
        {
            return 0;
        }


        /// <summary>
        /// Given a formula, enumerates the tokens that compose it.  Tokens are left paren,
        /// right paren, one of the four operator symbols, a string consisting of a letter followed by
        /// zero or more digits and/or letters, a double literal, and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z][0-9a-zA-Z]*";
            // PLEASE NOTE:  I have added white space to this regex to make it more readable.
            // When the regex is used, it is necessary to include a parameter that says
            // embedded white space should be ignored.  See below for an example of this.
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: e[\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern.  It contains embedded white space that must be ignored when
            // it is used.  See below for an example of this.
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            // PLEASE NOTE:  Notice the second parameter to Split, which says to ignore embedded white space
            /// in the pattern.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }
        }
    }

    /// <summary>
    /// A Lookup method is one that maps some strings to double values.  Given a string,
    /// such a function can either return a double (meaning that the string maps to the
    /// double) or throw an UndefinedVariableException (meaning that the string is unmapped 
    /// to a value. Exactly how a Lookup method decides which strings map to doubles and which
    /// don't is up to the implementation of the method.
    /// </summary>
    public delegate double Lookup(string var);

    /// <summary>
    /// Used to report that a Lookup delegate is unable to determine the value
    /// of a variable.
    /// </summary>
    [Serializable]
    public class UndefinedVariableException : Exception
    {
        /// <summary>
        /// Constructs an UndefinedVariableException containing whose message is the
        /// undefined variable.
        /// </summary>
        /// <param name="variable"></param>
        public UndefinedVariableException(String variable)
            : base(variable)
        {
        }
    }

    /// <summary>
    /// Used to report syntactic errors in the parameter to the Formula constructor.
    /// </summary>
    [Serializable]
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message) : base(message)
        {

        }
    }

    /// <summary>
    /// Used to report errors that occur when evaluating a Formula.
    /// </summary>
    [Serializable]
    public class FormulaEvaluationException : Exception
    {
        /// <summary>
        /// Constructs a FormulaEvaluationException containing the explanatory message.
        /// </summary>
        public FormulaEvaluationException(String message) : base(message)
        {
        }
    }
}