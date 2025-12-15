Public Interface IFlexibleDictionary(Of TKey, TValue)
  Inherits IDictionary(Of TKey, TValue)
    Sub AddOrUpdate(key As TKey, value As TValue)
    Function SubstituteIfFound(key As TKey) As TValue
    Function ContainsKeyEndingWith(suffix As String) As Boolean
    Function GetValueByKeyEndingWith(suffix As String) As TValue
    Function GetKeyEndingWith(suffix As String) As TKey
End Interface