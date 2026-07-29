Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports SimpleDyno

<TestClass>
Public Class CurveFunctionsTests

    <TestMethod>
    Public Sub F_EvaluatesConstant()
        Dim coeffs As New List(Of Double) From {5.0}
        Assert.AreEqual(5.0, CurveFunctions.F(coeffs, 0.0), 0.0001)
        Assert.AreEqual(5.0, CurveFunctions.F(coeffs, 100.0), 0.0001)
    End Sub

    <TestMethod>
    Public Sub F_EvaluatesLinearPolynomial()
        'y = 3 + 2x
        Dim coeffs As New List(Of Double) From {3.0, 2.0}
        Assert.AreEqual(3.0, CurveFunctions.F(coeffs, 0.0), 0.0001)
        Assert.AreEqual(13.0, CurveFunctions.F(coeffs, 5.0), 0.0001)
        Assert.AreEqual(-7.0, CurveFunctions.F(coeffs, -5.0), 0.0001)
    End Sub

    <TestMethod>
    Public Sub F_EvaluatesQuadraticPolynomial()
        'y = 1 + 2x + 3x^2
        Dim coeffs As New List(Of Double) From {1.0, 2.0, 3.0}
        Assert.AreEqual(6.0, CurveFunctions.F(coeffs, 1.0), 0.0001)
        Assert.AreEqual(1.0 + 2.0 * 2.0 + 3.0 * 4.0, CurveFunctions.F(coeffs, 2.0), 0.0001)
    End Sub

    <TestMethod>
    Public Sub FindPolynomialLeastSquaresFit_NEW_RecoversExactLinearFit()
        'Points lie exactly on y = 3 + 2x, so the least-squares fit should recover that line exactly
        'and SentFY should reproduce SentY (zero residual).
        Dim x() As Double = {0.0, 1.0, 2.0, 3.0, 4.0}
        Dim y() As Double = {3.0, 5.0, 7.0, 9.0, 11.0}
        Dim fy(x.Length - 1) As Double

        Dim result As Boolean = CurveFunctions.FindPolynomialLeastSquaresFit_NEW(x, y, fy, 1)

        Assert.IsTrue(result)
        For i As Integer = 0 To x.Length - 1
            Assert.AreEqual(y(i), fy(i), 0.001, $"Point {i} should lie exactly on the recovered line")
        Next
    End Sub

    <TestMethod>
    Public Sub FindPolynomialLeastSquaresFit_NEW_RecoversExactQuadraticFit()
        'Points lie exactly on y = 1 + 0x + 2x^2
        Dim x() As Double = {-2.0, -1.0, 0.0, 1.0, 2.0}
        Dim y(x.Length - 1) As Double
        For i As Integer = 0 To x.Length - 1
            y(i) = 1.0 + 2.0 * x(i) * x(i)
        Next
        Dim fy(x.Length - 1) As Double

        Dim result As Boolean = CurveFunctions.FindPolynomialLeastSquaresFit_NEW(x, y, fy, 2)

        Assert.IsTrue(result)
        For i As Integer = 0 To x.Length - 1
            Assert.AreEqual(y(i), fy(i), 0.001, $"Point {i} should lie exactly on the recovered parabola")
        Next
    End Sub

End Class
