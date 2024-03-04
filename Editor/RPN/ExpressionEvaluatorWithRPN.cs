using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace GMToolKit
{
    public class ExpressionEvaluatorWithRPN
    {
        private enum Associativity { Left, Right }
        private struct Operator
        {
            public char character;
            public int priority;
            public int inputs;
            public Associativity associativity;

            public Operator(char character, int priority, int inputs, Associativity associativity)
            {
                this.character = character;
                this.priority = priority;
                this.inputs = inputs;
                this.associativity = associativity;
            }
        }


        private static Dictionary<char, Operator> validOperator = new Dictionary<char, Operator>()
    {
        {'+', new Operator('+',1,2,Associativity.Left)},
        {'-', new Operator('-',1,2,Associativity.Left)},
        {'*', new Operator('*',2,2,Associativity.Left)},
        {'/', new Operator('/',2,2,Associativity.Left)},
        {'%', new Operator('%',2,2,Associativity.Left)},
        {'^', new Operator('^',3,2,Associativity.Right)},
        {'u', new Operator('u',4,1,Associativity.Left)},
    };

        private static HashSet<char> numericCharacter = new HashSet<char>()
    {
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.'
    };

        public static bool EvaluateInteger(string expression, out BigInteger result)
        {
            expression = TrimExpression(expression);

            var tokens = Tokenize(expression);

            FixMinusOperators(tokens);



            var rpnTokens = ConvertToRPN(tokens);

            return EvaluateBigInteger(rpnTokens, out result);

        }

        private static bool EvaluateBigInteger(string[] tokens, out BigInteger value)
        {
            Stack<string> stack = new Stack<string>();

            foreach (string token in tokens)
            {
                if (IsOperator(token))
                {
                    Operator oper = validOperator[token[0]];
                    List<BigInteger> values = new List<BigInteger>();
                    bool parsed = true;

                    while (stack.Count > 0 && !IsCommand(stack.Peek()) && values.Count < oper.inputs)
                    {
                        BigInteger newValue;
                        parsed &= TryParseBigInteger(stack.Pop(), out newValue);
                        values.Add(newValue);
                    }
                    values.Reverse();

                    if (parsed && values.Count == oper.inputs)
                        stack.Push(EvaluateBigInteger(values.ToArray(), token[0]).ToString());
                    else // Can't parse values or too few values for the operator -> exit
                    {
                        value = default;
                        return false;
                    }
                }
                else
                {
                    stack.Push(token);
                }
            }

            if (stack.Count == 1)
            {
                if (TryParseBigInteger(stack.Pop(), out value))
                    return true;
            }

            value = default;
            return false;
        }

        private static bool TryParseBigInteger(string expression, out BigInteger result)
        {
            // expression = expression.Replace(',', '.');
            // expression = expression.TrimEnd('f');

            bool success = false;
            result = default;
            BigInteger temp = default;
            success = BigInteger.TryParse(expression, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out temp);
            result = temp;
            return success;
        }

        private static BigInteger EvaluateBigInteger(BigInteger[] values, char oper)
        {
            if (values.Length == 1)
            {
                switch (oper)
                {
                    case 'u':
                        return -values[0];
                }
            }
            else if (values.Length == 2)
            {
                switch (oper)
                {
                    case '+':
                        return values[0] + values[1];
                    case '-':
                        return values[0] - values[1];
                    case '*':
                        return values[0] * values[1];
                    case '/':
                        return values[0] / values[1];
                    case '%':
                        return values[0] % values[1];
                    case '^':
                        return values[0] ^ values[1];
                }
            }
            return default;
        }

        private static string[] ConvertToRPN(List<string> tokens)
        {
            Stack<char> operatorStack = new Stack<char>();
            Queue<string> outputQueue = new Queue<string>();

            foreach (string token in tokens)
            {
                if (IsCommand(token))
                {
                    char command = token[0];
                    if (command == '(') // Bracket open
                    {
                        operatorStack.Push(command);
                    }
                    else if (command == ')') // Bracket close
                    {
                        while (operatorStack.Count > 0 && operatorStack.Peek() != '(')
                            outputQueue.Enqueue(operatorStack.Pop().ToString());

                        if (operatorStack.Count > 0)
                            operatorStack.Pop();
                    }
                    else // All the other operators
                    {
                        Operator o = validOperator[command];

                        while (NeedToPop(o))
                            outputQueue.Enqueue(operatorStack.Pop().ToString());

                        operatorStack.Push(command);
                    }
                }
                else // Not a command, just a regular number
                {
                    outputQueue.Enqueue(token);
                }
            }
            while (operatorStack.Count > 0)
                outputQueue.Enqueue(operatorStack.Pop().ToString());

            return outputQueue.ToArray();

            bool NeedToPop(Operator newOperator)
            {
                if (operatorStack.Count > 0)
                {
                    char topOfStack = operatorStack.Peek();

                    if (IsOperator(topOfStack))
                    {
                        var topOfOperator = validOperator[topOfStack];
                        if (newOperator.associativity == Associativity.Left && newOperator.priority <= topOfOperator.priority ||
                            newOperator.associativity == Associativity.Right && newOperator.priority < topOfOperator.priority)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        private static void FixMinusOperators(List<string> tokens)
        {
            if (tokens.Count == 0)
                return;

            if (tokens[0] == "-")
                tokens[0] = "u";

            for (int i = 1; i < tokens.Count - 1; i++)
            {
                string token = tokens[i];
                string previousToken = tokens[i - 1];
                string nextToken = tokens[i - 1];

                if (token == "-" && (IsCommand(previousToken) || nextToken == "(" || nextToken == ")"))
                    tokens[i] = "u";
            }
        }

        private static List<string> Tokenize(string expression)
        {
            List<string> result = new List<string>();
            StringBuilder tempNumber = new StringBuilder();
            foreach (var c in expression)
            {
                if (IsCommand(c))
                {
                    if (tempNumber.Length != 0)
                    {
                        result.Add(tempNumber.ToString());
                        tempNumber.Clear();
                    }

                    result.Add(c.ToString());
                }
                else
                {
                    tempNumber.Append(c);
                }
            }
            if (tempNumber.Length != 0)
            {
                result.Add(tempNumber.ToString());
                tempNumber.Clear();
            }
            return result;
        }

        private static string TrimExpression(string expression)
        {
            var result = expression.Trim();
            if (result.Length == 0) return result;

            var lastChar = result[result.Length - 1];
            while (IsOperator(lastChar))
            {
                result = result.Substring(0, result.Length - 1);
                if (result.Length == 0) return result;

                lastChar = result[result.Length - 1];
            }

            return result;
        }

        private static bool IsCommand(string s)
        {
            if (s.Length != 1) return false;
            return IsCommand(s[0]);
        }

        private static bool IsCommand(char c)
        {
            if (c == '(' || c == ')') return true;
            return IsOperator(c);
        }

        private static bool IsOperator(string s)
        {
            if (s.Length != 1) return false;
            return IsOperator(s[0]);
        }

        private static bool IsOperator(char c)
        {
            return validOperator.ContainsKey(c);
        }

        // public static bool EvaluateDouble(string expression, out double result)
        // {

        // }
    }
}