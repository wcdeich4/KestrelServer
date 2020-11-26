Feature: Test Singleton Dictionary
	The Singleton Dictionary 
    in Visual Basic should be usable in
    other projects

Scenario Outline: Demonstrating use of the Singleton Dictionary
	Given We got the single instance of the Singleton Dictionary
	When we AddOrUpdate key value pair <Key1> <Value1>
	And We AddOrUpdate key value pair <Key2> <Value2>
	And We AddOrUpdate key value pair <Key2> <Value3>
	Then The value for <Key1> will be <Value1>
	And The value for <Key2> will be <Value3>
	And The SubstituteIfFound value for <KeyThatDoesNotExist> is <KeyThatDoesNotExist>

Examples:
| Key1 | Value1 | Key2 | Value2 | Value3 | KeyThatDoesNotExist |
| 1    | 1      | 2    | 2      | 3      | 0                   |