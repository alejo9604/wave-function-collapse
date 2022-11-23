namespace AllieJoe.SudokuSolver
{
    public class BoardStateData
    {
        public BoardCellData[] Cells;

        public BoardStateData(int cellAmount)
        {
            Cells = new BoardCellData[cellAmount];
        }
    }

    public class BoardCellData
    {
        public int X;
        public int Y;
        public int[] Domain;
        public int Value;
        public bool Collapsed;

        public BoardCellData(BoardCell cell)
        {
            X = cell.X;
            Y = cell.Y;
            Domain = cell.Domain.ToArray();
            Value = cell.Value;
            Collapsed = cell.Collapsed;
        }
    }

    public class BoardCellCollapseData
    {
        public int X;
        public int Y;
        public int ValueCollapsed;
        public int[] Domain;

        public BoardCellCollapseData(BoardCell cell)
        {
            X = cell.X;
            Y = cell.Y;
            Domain = cell.Domain.ToArray();
        }
    }
    
}