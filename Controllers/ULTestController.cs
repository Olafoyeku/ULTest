using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ULTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ULTestController : ControllerBase
    {
       
        private List<string> matchlist;
        private string[] bodMasOrdering = new[] { "/", "*", "+", "-" };

        [HttpGet("/expression")]
        public string Get(string expression)
        {
            //Check if an empty string or null value is inputed
            if (string.IsNullOrEmpty(expression))
            {
                return "Expression cannot be null or empty";
            } 

            //Remove/Replace Whitespace
            string refine = expression.Replace(" ", "");

            //Perform REgex to check Expression 
            Regex regexcheck = new Regex(@"^(\d+[-+\/*])+\d+$");

            if (!regexcheck.IsMatch(expression))
            {
                return "Expression not valid";
            }
            

            //Get individual Components of the expression
            Regex regexindividualcomp = new Regex(@"(\d+|[-+\/*]){1}");

            //Check if expression matches the regex expression and returns a list of each components
            matchlist = regexindividualcomp.Matches(expression).Select(x => x.Value).ToList();


            string returnvalue = Evaluate(matchlist);

            return returnvalue;



        }



        private decimal Compute(string firstnumeral, string arithmetricoperation, string secondnumeral)
        {

            //convert string parameter to decimal
            decimal firstNumber = decimal.Parse(firstnumeral); 
            decimal secondNumber = decimal.Parse(secondnumeral);

            // switch statement based on the arithmetric operation
            switch (arithmetricoperation)
            {
                case "+":
                    return checked(firstNumber + secondNumber);
                case "-":
                    return checked(firstNumber - secondNumber);
                case "*":
                    return checked(firstNumber * secondNumber);
                case "/":
                    return checked(firstNumber / secondNumber);
                default:
                    throw new InvalidOperationException($"Operation {arithmetricoperation} is not supported");
            }
        }

        private void RearrangeList(ref List<string> matchlist, int index, decimal newNumber)
        {
            matchlist.RemoveAt(index);
            matchlist.Insert(index, newNumber.ToString());
            matchlist.RemoveAt(index + 1);
            matchlist.RemoveAt(index - 1);
        }



        private string Evaluate(List<string> individualcomponents)
        {
            // While there is an individual component
            while (individualcomponents.Count > 1) 
            {

                foreach (string arithmetricOperator in bodMasOrdering)
                {
                    int operatorIndex = individualcomponents.ToList().IndexOf(arithmetricOperator);
                    if (operatorIndex > 0)
                    {
                        decimal smallResult = Compute(individualcomponents[operatorIndex - 1], individualcomponents[operatorIndex], individualcomponents[operatorIndex + 1]);
                        RearrangeList(ref individualcomponents, operatorIndex, smallResult);
                        break;
                    }
                }
            }
            return individualcomponents[0];
        }
    }
}
