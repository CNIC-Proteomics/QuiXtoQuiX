using System;
using System.Media;

namespace Mathlet
{
    class Cramer
    {
        private double[] resultsVector;
        private double[,] coefficientMatrix;

        private double[] solutions
        {
            get
            {
                int totSolutions = resultsVector.Length;
                double[] solutionVector = new double[totSolutions];

                Matrix denominatorMatrix = new Matrix();

                denominatorMatrix.elements = coefficientMatrix;
                double denominator = denominatorMatrix.determinant;

                for (int coef = 0; coef < totSolutions; coef++)
                {
                    Matrix numeratorMatrix = new Matrix();
                    numeratorMatrix.elements = new double[totSolutions, totSolutions];

                    double[,] newMatrix = new double[totSolutions, totSolutions];
                    for (int i = 0; i < totSolutions; i++)
                        for (int j = 0; j < totSolutions; j++)
                        {
                            if (j == coef)
                                numeratorMatrix.elements[i, j] = resultsVector[i];
                            else
                                numeratorMatrix.elements[i, j] = coefficientMatrix[i, j];
                        }

                    double numerator = numeratorMatrix.determinant;

                    solutionVector[coef] = numerator / denominator;
                }

                return solutionVector;
            }
        }

        public double[] cramerSolve(double[] _results, double[,] _coeffs)
        {
            resultsVector = _results;
            coefficientMatrix = _coeffs;

            return solutions;
        }
    }

    class Smoother
    {
        public double[,] original;

        public double[,] displaced
        {
            get
            {
                return displaceFunction(original, -initialValue);
            }
        }

        private double[,] displaceFunction(double[,] function, double addition)
        {
            int size = function.GetUpperBound(1) + 1;
            double[,] results = new double[2, size];

            for (int i = 0; i < size; i++)
            {
                results[0, i] = function[0, i] + addition;
                results[1, i] = function[1, i];
            }

            return results;
        }

        public double initialValue
        {
            get
            {
                return original[0, 0];
            }
        }

        public double[,] quadratic
        {
            get
            {
                double[] coefficients = quadraticCoefficientsABC;
                int size = displaced.GetUpperBound(1) + 1;
                double[,] results = new double[2, size];

                for (int i = 0; i < size; i++)
                {
                    double rt = displaced[0, i];
                    results[0, i] = rt;
                    results[1, i] = coefficients[0] + coefficients[1] * rt + coefficients[2] * Math.Pow(rt, 2);
                }

                return displaceFunction(results, initialValue);
            }
        }

        public double[] quadraticCoefficientsABC
        {
            get
            {
                return getQuadraticSmoothCoefficients(displaced);
            }
        }

        private double[] getQuadraticSmoothCoefficients(double[,] _originalGraph)
        {
            double sumY = 0;
            double sumX = 0;
            double sumX2 = 0;
            double sumX3 = 0;
            double sumX4 = 0;
            double sumXY = 0;
            double sumX2Y = 0;

            int dimension = _originalGraph.GetUpperBound(1) + 1;

            // X = mz --> 0
            // Y = intensity --> 1
            for (int i = 0; i < dimension; i++)
            {
                double X = _originalGraph[0, i];
                double X2 = Math.Pow(X, 2);
                double X3 = Math.Pow(X, 3);
                double X4 = Math.Pow(X, 4);

                double Y = _originalGraph[1, i];
                sumY += Y;
                sumX += X;
                sumX2 += X2;
                sumX3 += X3;
                sumX4 += X4;
                sumXY += X * Y;
                sumX2Y += X2 * Y;
            }

            double[] vector = {sumY, sumXY, sumX2Y};
            double[,] matrix = {
                                   {dimension,  sumX,       sumX2},
                                   {sumX,       sumX2,      sumX3},
                                   {sumX2,      sumX3,      sumX4}
                               };

            Cramer cramer = new Cramer();

            return cramer.cramerSolve(vector, matrix);
        }
    }

    class Matrix
    {
        #region Matrices to test:
        //double[,] m5x5 = { { 1, 1, 2, 3, 8 }, { 1, 3, 4, 4, 0 }, { 1, 9, 3, 4, 3 }, { 0, 3, 2, 2, 5 }, { 1, 2, 3, 4, 5 } };
        //double[,] m4x4 = { { 1, 1, 2, 3 }, { 1, 3, 4, 4 }, { 1, 9, 3, 4 }, { 0, 3, 2, 2 } };
        //double[,] m3x3 = { { 1, 1, 2 }, { 1, 3, 4 }, { 1, 9, 3 } };
        //double[,] m2x2 = { { 1, 1 }, { 2, 4 } };
        // det(m5x5) = 79
        // det(m4x4) = -13
        // det(m3x3) = -14
        // det(m2x2) = 2
        #endregion

        public double[,] elements;

        public double determinant
        {
            get
            {
                return getDeterminant(elements);
            }
        }

        private double getDeterminant(double[,] matrix)
        {
            if (matrix == null)
                return double.NaN;

            if (countColumns(matrix) != countColumns(matrix))
                return double.NaN;

            if (countRows(matrix) == 1)
                return matrix[0, 0];

            if (countRows(matrix) > 1)
            {
                double totalSum = 0;
                for (int i = 0; i < countRows(matrix); i++)
                {
                    int sign = (int)Math.Pow(-1, i);
                    totalSum += sign * matrix[0, i] * getDeterminant(getSubmatrix(matrix, 0, i));
                }
                return totalSum;
            }

            return double.NaN;
        }

        private double[,] getSubmatrix(double[,] matrix, int row, int column)
        {
            int dimension = countRows(matrix);
            int newDimension = dimension - 1;
            if (dimension <= 1)
                return null;

            double[,] subMatrix = new double[newDimension, newDimension];

            int nextRow = 0;
            for (int i = 0; i < dimension; i++)
            {
                if (i != row)
                {
                    int nextColumn = 0;
                    for (int j = 0; j < dimension; j++)
                    {
                        if (j != column)
                        {
                            subMatrix[nextRow, nextColumn] = matrix[i, j];
                            nextColumn++;
                        }
                    }
                    nextRow++;
                }
            }

            return subMatrix;
        }

        private int countRows(double[,] matrix)
        {
            return matrix.GetUpperBound(0) + 1;
        }

        private int countColumns(double[,] matrix)
        {
            return matrix.GetUpperBound(1) + 1;
        }
    }
}