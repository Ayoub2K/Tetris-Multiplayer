using System;

namespace TetrisClient
{
    public class Tetromino
    {
        public TetrominoEnum Shape { get; set; }
        public Matrix MatrixInt { get; set; }
        public int OffsetY { get; set; }
        public int OffsetX { get; set; }
        
        /// <summary>
        ///     Tetrimino constructor
        /// </summary>
        public Tetromino(int offsetX = 5, int offsetY = 0)
        {
            Shape = RandomTetromino();
            MatrixInt = BuildTetromino(Shape);
            OffsetX = offsetX;
            OffsetY = offsetY;
        }

        /// <summary>
        ///     makes copy of given Tetrimono, for the NextTetrimonoPiece and the Ghostpiece
        /// </summary>
        public Tetromino(int offsetX, int offsetY, TetrominoEnum shape, Matrix matrixInt)
        {
            OffsetX = offsetX;
            OffsetY = offsetY;
            Shape = shape;
            MatrixInt = matrixInt;
        }

        /// <summary>
        ///     Picks random Tetrimno
        /// </summary>
        private static TetrominoEnum RandomTetromino()
        {
            Random random = new();
            switch (random.Next(0, 7))
            {
                case 0:
                    return (TetrominoEnum)Enum.GetValues(typeof(TetrominoEnum)).GetValue(0)!;
                case 1:
                    return (TetrominoEnum)Enum.GetValues(typeof(TetrominoEnum)).GetValue(1)!;
                case 2:
                    return (TetrominoEnum)Enum.GetValues(typeof(TetrominoEnum)).GetValue(2)!;
                case 3:
                    return (TetrominoEnum)Enum.GetValues(typeof(TetrominoEnum)).GetValue(3)!;
                case 4:
                    return (TetrominoEnum)Enum.GetValues(typeof(TetrominoEnum)).GetValue(4)!;
                case 5:
                    return (TetrominoEnum)Enum.GetValues(typeof(TetrominoEnum)).GetValue(5)!;
                case 6:
                    return (TetrominoEnum)Enum.GetValues(typeof(TetrominoEnum)).GetValue(6)!;
                default:
                    throw new Exception("Block generation went wrong!");
            }
        }

        /// <summary>
        ///     Gives tetrimono new Matrix values based on the shape given in RandomTetrimino()
        /// </summary>
        private static Matrix BuildTetromino(TetrominoEnum shape)
        {
            return shape switch
            {
                TetrominoEnum.Lines => new Matrix(new[,]
                {
                    { 0, 1, 0, 0 },
                    { 0, 1, 0, 0 },
                    { 0, 1, 0, 0 },
                    { 0, 1, 0, 0 }
                }),
                TetrominoEnum.Square => new Matrix(new[,]
                {
                    { 1, 1 },
                    { 1, 1 }
                }),
                TetrominoEnum.LThunder => new Matrix(new[,]
                {
                    { 1, 0, 0 },
                    { 1, 1, 0 },
                    { 0, 1, 0 }
                }),
                TetrominoEnum.RThunder => new Matrix(new[,]
                {
                    { 0, 1, 0 },
                    { 1, 1, 0 },
                    { 1, 0, 0 }
                }),
                TetrominoEnum.LeftT => new Matrix(new[,]
                {
                    { 1, 1, 0 },
                    { 0, 1, 0 },
                    { 0, 1, 0 }
                }),
                TetrominoEnum.RightT => new Matrix(new[,]
                {
                    { 0, 1, 1 },
                    { 0, 1, 0 },
                    { 0, 1, 0 }
                }),
                TetrominoEnum.Triangle => new Matrix(new[,]
                {
                    { 0, 1, 0 },
                    { 1, 1, 1 },
                    { 0, 0, 0 }
                }),
                _ => throw new ArgumentOutOfRangeException(nameof(shape), shape, null)
            };
        }
    }
}