Imports System.Collections.Generic
Imports System.Runtime.Serialization

Public Class FlexibleDictionary(Of TKey,TValue)
    Inherits Dictionary(Of TKey, TValue)

    Public Sub New()
         MyBase.New()
    End Sub 

    Public Sub New(capacity As Integer)
         MyBase.New(capacity)
    End Sub

    Public Sub New(comparer As IEqualityComparer(Of TKey))
         MyBase.New(comparer)
    End Sub

    Public Sub New(dictionary As IDictionary(Of TKey, TValue))
         MyBase.New(dictionary)
    End Sub

    Public Sub New(capacity As Integer, comparer As IEqualityComparer(Of TKey))
         MyBase.New(capacity, comparer)
    End Sub

    Public Sub New(dictionary As IDictionary(Of TKey, TValue), comparer As IEqualityComparer(Of TKey))
         MyBase.New(dictionary, comparer)
    End Sub

    Public Sub New(info As SerializationInfo, context As StreamingContext)
         MyBase.New(info, context)
    End Sub

    Public Sub AddOrUpdate(key As TKey, value As TValue)
        If Me.ContainsKey(key) Then
            Me(key) = value
        Else
            Me.Add(key, value)
        End If
    End Sub

    Public Function SubstituteIfFound(key As TKey) As TValue
        If Me.ContainsKey(key) Then
            return Me(key)
        Else
            return CType(Convert.ChangeType(key, GetType(TValue)), TValue)
        End If
    End Function

End Class
