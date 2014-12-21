VERSION 5.00
Begin VB.Form Form1 
   Caption         =   "Form1"
   ClientHeight    =   3195
   ClientLeft      =   60
   ClientTop       =   345
   ClientWidth     =   4680
   LinkTopic       =   "Form1"
   ScaleHeight     =   3195
   ScaleWidth      =   4680
   StartUpPosition =   3  'Windows Default
   Begin VB.CommandButton cmdCallDotNetType 
      Caption         =   "Call Dot Net type"
      Height          =   735
      Left            =   960
      TabIndex        =   0
      Top             =   840
      Width           =   1815
   End
End
Attribute VB_Name = "Form1"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Option Explicit

Private Sub cmdCallDotNetType_Click()
    Dim dotNetClass As SpikeDotNetComClasses.DotNetComType
    Set dotNetClass = New SpikeDotNetComClasses.DotNetComType
    dotNetClass.SomeMethod "Whatever"
    
    'notice that we have to declare this type as variant as the class interface for LateBoundDotNetComType
    'has not been exposed in the type library
    Dim lateBoundDotNetType As Variant
    Dim returnValue As String
    Set lateBoundDotNetType = New SpikeDotNetComClasses.LateBoundDotNetComType
    returnValue = lateBoundDotNetType.SomeStringFunction
End Sub
