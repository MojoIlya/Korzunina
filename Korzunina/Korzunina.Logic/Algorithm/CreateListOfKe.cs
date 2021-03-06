﻿using System.Collections.Generic;

namespace Korzunina.Logic
{
    public class CreateListOfKe
    {
        #region переменные 
        private const double myu = 0.3;
        private double v;
        private int numbMatrK = 0;
        private int countBlock;
        private int[,] smezhMatrix;
        //private double[,] nodeMatrix;
        private List<Point> listCoords;
        private List<double[,]> listOfMatrixKe;
        #endregion

        public CreateListOfKe(Sheet sheet)
        {
            smezhMatrix = sheet.AdjacencyMatrix;
            listCoords = sheet.Coordinates;

            v = sheet.Volume;
            countBlock = sheet.BlocksCount;
            listOfMatrixKe = new List<double[,]>();
            listOfMatrixKe.Clear();
            listOfMatrixKe = CreateListOfKE();
        }

        public List<double[,]> ListOfMatrixKe
        {
            get { return listOfMatrixKe; }
        }

        #region методы перемножения матриц 
        private double[,] MultiplyMatrix(ref double[,] X, ref double[,] Y, int countRow, int countCol, int countInner)
        {
            double[,] BufMatrix = new double[countRow, countCol];
            for (int row = 0; row < countRow; row++)
            {
                for (int col = 0; col < countCol; col++)
                {
                    for (int inner = 0; inner < countInner; inner++)
                    {
                        BufMatrix[row, col] += X[row, inner] * Y[inner, col];
                    }
                }
            }
            return BufMatrix;
        }
        private double[,] Multiply3Matrix(ref double[,] Bt, ref double[,] D, ref double[,] B)
        {

            double[,] BtOnD = new double[12, 6];
            double[,] Ke = new double[12, 12];
            BtOnD = MultiplyMatrix(ref Bt, ref D, 12, 6, 6);
            Ke = MultiplyMatrix(ref BtOnD, ref B, 12, 12, 6);
            return Ke;
        }
        #endregion
        #region работа с матрицей B
        private void InitB(ref double[,] B)
        {
            for (int i = 0; i < 6; i++)
                for (int j = 0; j < 12; j++)
                    B[i, j] = 0;
        }
        private void quartetB(ref double[,] B, int numbQuartet)
        {
            int startStolbec = 0;
            switch (numbQuartet)
            {
                case 0:
                    startStolbec = 0;
                    break;
                case 1:
                    startStolbec = 3;
                    break;
                case 2:
                    startStolbec = 6;
                    break;
                case 3:
                    startStolbec = 9;
                    break;
            }
            B[0, startStolbec] = Det(1, numbQuartet);
            B[1, startStolbec + 1] = Det(2, numbQuartet);
            B[2, startStolbec + 2] = Det(3, numbQuartet);
            B[3, startStolbec] = Det(2, numbQuartet);
            B[3, startStolbec + 1] = Det(1, numbQuartet);
            B[4, startStolbec + 1] = Det(3, numbQuartet);
            B[4, startStolbec + 2] = Det(2, numbQuartet);
            B[5, startStolbec] = Det(3, numbQuartet);
            B[5, startStolbec + 2] = Det(1, numbQuartet);
        }
        private void multBOnKoef(ref double[,] B)
        {
            double koef = 1 / (6 * v);
            for (int i = 0; i < 6; i++)
                for (int j = 0; j < 12; j++)
                    B[i, j] *= koef;
        }
        private double[,] BTransp(ref double[,] B)
        {
            double[,] Bt = new double[12, 6];
            for (int i = 0; i < 6; i++)
                for (int j = 0; j < 12; j++)
                {
                    Bt[j, i] = B[i, j];
                }
            return Bt;
        }
        #endregion
        #region работа с матрицей D
        private void InitD(ref double[,] D)
        {
            for (int i = 0; i < 6; i++)
                for (int j = 0; j < 6; j++)
                    D[i, j] = 0;
            for (int i = 0; i < 6; i++)
                for (int j = 0; j < 6; j++)
                {
                    if (i == j && i > 2)
                        D[i, j] = (1 - 2 * myu) / (2 * (1 - myu));
                    if (i <= 2 && j <= 2)
                        if (i == j)
                            D[i, j] = 1;
                        else
                            D[i, j] = myu / (1 - myu);
                }
        }
        #endregion
        #region работа с определителем
        private double Det(int numbLetter, int numbKoef)
        {
            double res = 0.0;
            int KoefMult = 0;
            int Letter1 = 0, Letter2 = 0;
            int KoefLeftTop = 0, KoefLeftBottom = 0, KoefRight = 0;

            //Определяем какие столбцы нам нужны в матрице с узлами
            switch (numbLetter)
            {
                case 1:
                    Letter1 = 1;
                    Letter2 = 2;
                    if (numbKoef == 0 || numbKoef == 2) KoefMult = -1;
                    else KoefMult = 1;
                    break;
                case 2:
                    Letter1 = 0;
                    Letter2 = 2;
                    if (numbKoef == 0 || numbKoef == 2) KoefMult = 1;
                    else KoefMult = -1;
                    break;
                case 3:
                    Letter1 = 0;
                    Letter2 = 1;
                    if (numbKoef == 0 || numbKoef == 2) KoefMult = -1;
                    else KoefMult = 1;
                    break;
            }
            //Определяем какие столбцы нам нужны в матрице смежности
            switch (numbKoef)
            {
                case 0:
                    KoefLeftTop = 2;
                    KoefLeftBottom = 3;
                    KoefRight = 1;
                    break;
                case 1:
                    KoefLeftTop = 2;
                    KoefLeftBottom = 3;
                    KoefRight = 0;
                    break;
                case 2:
                    KoefLeftTop = 1;
                    KoefLeftBottom = 3;
                    KoefRight = 0;
                    break;
                case 3:
                    KoefLeftTop = 1;
                    KoefLeftBottom = 2;
                    KoefRight = 0;
                    break;
            }

            res = SubKoef(Letter1, smezhMatrix[numbMatrK, KoefLeftTop], smezhMatrix[numbMatrK, KoefRight]) * SubKoef(Letter2, smezhMatrix[numbMatrK, KoefLeftBottom], smezhMatrix[numbMatrK, KoefRight]) -
                SubKoef(Letter1, smezhMatrix[numbMatrK, KoefLeftBottom], smezhMatrix[numbMatrK, KoefRight]) * SubKoef(Letter2, smezhMatrix[numbMatrK, KoefLeftTop], smezhMatrix[numbMatrK, KoefRight]);

            return res * KoefMult;
        }
        private double SubKoef(int LetNumb, int Node1, int Node2)
        {
            double res = 0.0;
            switch (LetNumb)
            {
                case 0:
                    res = listCoords[Node1].X - listCoords[Node2].X;
                    break;
                case 1:
                    res = listCoords[Node1].Y - listCoords[Node2].Y;
                    break;
                case 2:
                    res = listCoords[Node1].Z - listCoords[Node2].Z;
                    break;
            }
            return res;
        }
        #endregion
        //ОСНОВНОЙ МЕТОД, ВОЗВРАЩАЮЩИЙ ЛИСТ ИЗ МАТРИЦ k^e
        private List<double[,]> CreateListOfKE()
        {
            List<double[,]> listke = new List<double[,]>();
            listke.Clear();
            double[,] D = new double[6, 6];
            InitD(ref D);
            double[,] B = new double[6, 12];
            double[,] Bt = new double[12, 6];
            double[,] Ke = new double[12, 12];
            for (int i = 0; i < countBlock * 6; i++)
            {
                InitB(ref B);
                quartetB(ref B, 0);
                quartetB(ref B, 1);
                quartetB(ref B, 2);
                quartetB(ref B, 3);
                multBOnKoef(ref B);
                Bt = BTransp(ref B);
                Ke = Multiply3Matrix(ref Bt, ref D, ref B);
                listke.Add(Ke);
                numbMatrK++;
            }
            return listke;
        }
    }
}
