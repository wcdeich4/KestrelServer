'based on https://www.techcoil.com/blog/implementing-the-singleton-pattern-in-visual-basic-dot-net/

Imports System.Threading
Public Class SingletonDictionary
    Inherits FlexibleDictionary(Of String, Object)
    Implements IDictionary(Of String, Object), IFlexibleDictionary(Of String, Object)

    ' For SyncLock to mark a critical section
    Private Shared classLocker As New Object()
 
    ' Allocate memory space to hold the 
    ' single object instance
    Private Shared objSingletonDictionary As SingletonDictionary
 
    ' Make the only constructor private 
    ' to prevent initialization outside of 
    ' the class.
    Private Sub New()
        MyBase.New()
    End Sub
 
    ' Expose GetInstance() for the retrieval 
    ' of the single object instance.
    Public Shared Function GetInstance() As SingletonDictionary
 
        ' Initialize SingletonDictionary through lazy 
        ' initialization to prevent unused 
        ' SingletonDictionary from taking up program 
        ' memory
        If (objSingletonDictionary Is Nothing) Then
            ' Mark a critical section where 
            ' threads take turns to execute
            SyncLock (classLocker)
                If (objSingletonDictionary Is Nothing) Then
                    objSingletonDictionary = New SingletonDictionary()
                End If
            End SyncLock
        End If
        Return objSingletonDictionary
 
    End Function
 
End Class
