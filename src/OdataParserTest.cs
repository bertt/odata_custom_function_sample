using Microsoft.AspNet.OData.Builder;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace odata_custom_function
{
    public class OdataParserTest
    {
        [Test]
        public void FirstTest()
        {
            var intparam = EdmCoreModel.Instance.GetInt32(false);
            var daysFromNowSignature = new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDate(false), intparam );
            CustomUriFunctions.AddCustomUriFunction("daysFromNow", daysFromNowSignature);

            var filterPath = $"persons?$filter = Birthday gt daysFromNow(-3)";

            var uri = new Uri(filterPath, UriKind.Relative);
            var model = CreateModel("persons");
            var parser = new ODataUriParser(model, new Uri("http://www.odata.com/OData"), uri);
            var filterClause = parser.ParseFilter();
            var parsedUri = parser.ParseUri();
            var expression = filterClause.Expression;

            var person1 = new Person { Id = 0, Name = "Hallo", Birthday = DateTime.Now.AddDays(-5) };
            var person2 = new Person { Id = 1, Name = "Hallo2", Birthday = DateTime.Now };
            var persons = new List<Person> { person1, person2 };

            var p = persons.Where(i => Evaluate(expression, i)).ToList();

            Assert.IsTrue(p.Count == 1);
            Assert.IsTrue(p.FirstOrDefault().Name == "Hallo2");
        }

        static bool Evaluate(SingleValueNode node, Person person)
        {
            var convertNode = ((ConvertNode)((BinaryOperatorNode)node).Right);
            var source = convertNode.Source;
            var singleValueFunctionCallNode = (SingleValueFunctionCallNode)source;
            if(singleValueFunctionCallNode.Name == "daysFromNow")
            {
                var queryNode = (ConstantNode)singleValueFunctionCallNode.Parameters.First();
                var days = (int)queryNode.Value;

                var isSelected = (person.Birthday > DateTime.Now.AddDays(days));
                return isSelected;
            }

            return false;
        }

        private static IEdmModel CreateModel(string nameOfTheEntity)
        {
            var builder = new ODataConventionModelBuilder();
            builder.EntitySet<Person>(nameOfTheEntity);
            var model = builder.GetEdmModel();
            return model;
        }
    }
}
