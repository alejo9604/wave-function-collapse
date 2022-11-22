namespace AllieJoe.SudokuSolver
{
    public class BoardStateData
    {
        public BoardCellStateData[] Cells;
        public BoardCellToCollapse CellCollapsed;

        public BoardStateData(int cellAmount)
        {
            Cells = new BoardCellStateData[cellAmount];
        }

        public void SetCellCollapsed(BoardCell cell)
        {
            CellCollapsed = new BoardCellToCollapse(cell.X, cell.Y, cell.Value);
        }
        
    }

    public class BoardCellStateData
    {
        public int X;
        public int Y;
        public int[] Domain;
        public int Value;
        public bool Collapsed;

        public BoardCellStateData(BoardCell cell)
        {
            X = cell.X;
            Y = cell.Y;
            Domain = cell.Domain.ToArray();
            Value = cell.Value;
            Collapsed = cell.Collapsed;
        }
    }

    public class BoardCellToCollapse
    {
        public int X;
        public int Y;
        public int ValueCollapsed;

        public BoardCellToCollapse(int x, int y, int value)
        {
            X = x;
            Y = y;
            ValueCollapsed = value;
        }
    }
    
}