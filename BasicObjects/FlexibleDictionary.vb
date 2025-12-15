Imports System.Collections.Generic
Imports System.Runtime.Serialization

Public Class FlexibleDictionary(Of TKey,TValue)
    Inherits Dictionary(Of TKey, TValue)
    Implements IFlexibleDictionary(Of TKey, TValue)

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

    Public Sub AddOrUpdate(key As TKey, value As TValue) Implements IFlexibleDictionary(Of TKey, TValue).AddOrUpdate
        If Me.ContainsKey(key) Then
            Me(key) = value
        Else
            Me.Add(key, value)
        End If
    End Sub

    Public Function SubstituteIfFound(key As TKey) As TValue Implements IFlexibleDictionary(Of TKey, TValue).SubstituteIfFound
        If Me.ContainsKey(key) Then
            return Me(key)
        Else
            return CType(Convert.ChangeType(key, GetType(TValue)), TValue)
        End If
    End Function

    Public Function ContainsKeyEndingWith(suffix As String) As Boolean Implements IFlexibleDictionary(Of TKey, TValue).ContainsKeyEndingWith
        For Each key As TKey In Me.Keys
            If Convert.ChangeType(key, GetType(String)).EndsWith(suffix) Then
                Return True
            End If
        Next
        Return False
    End Function

    Public Function GetValueByKeyEndingWith(suffix As String) As TValue Implements IFlexibleDictionary(Of TKey, TValue).GetValueByKeyEndingWith
        For Each key As TKey In Me.Keys
            If Convert.ChangeType(key, GetType(String)).EndsWith(suffix) Then
                Return Me(key)
            End If
        Next
        Return Nothing
    End Function
    
    Public Function GetKeyEndingWith(suffix As String) As TKey Implements IFlexibleDictionary(Of TKey, TValue).GetKeyEndingWith
        For Each key As TKey In Me.Keys
            If Convert.ChangeType(key, GetType(String)).EndsWith(suffix) Then
                Return key
            End If
        Next
        Return Nothing
    End Function
End Class
