# odata_custom_function_sample

Sample code for OData custom function

Sample code for using custom function in Microsoft.OData.UriParser.ODataUriParser.

## Example

In the following OData filter there is a custom function used: 'daysFromNow' 

```
persons?$filter = Birthday gt daysFromNow(-3)
```

Interface daysFromNow:

- Input days (integer)

- Output: DateTime.Now.AddDays(days)

This filter can be used to select items that are created in the last N days.

## Code

See OdataParserTest.cs form sample code.

First the custom function is defined:

```
var intparam = EdmCoreModel.Instance.GetInt32(false);
var daysFromNowSignature = new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDate(false), intparam );
CustomUriFunctions.AddCustomUriFunction("daysFromNow", daysFromNowSignature);
```

In the Evaluate method the custom function is handled:

```
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
```
