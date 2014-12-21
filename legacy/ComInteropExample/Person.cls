VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
  Persistable = 0  'NotPersistable
  DataBindingBehavior = 0  'vbNone
  DataSourceBehavior  = 0  'vbNone
  MTSTransactionMode  = 0  'NotAnMTSObject
END
Attribute VB_Name = "Person"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = True
Option Explicit

Private mFirstName As String
Private mLastName As String

Property Get Firstname() As String
    Firstname = mFirstName
End Property
Property Let Firstname(ByVal Value As String)
    mFirstName = Value
End Property
Property Get Lastname() As String
    Lastname = mLastName
End Property
Property Let Lastname(Value As String)
    mLastName = Value
End Property
