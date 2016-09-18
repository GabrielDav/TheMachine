
namespace GameLibrary.GridBuilder
{
    public class MatrixBuilder
    {
        private readonly TileType? _tileType;
        private readonly ITileBuilder _tileBuilder;
        private readonly int _startX;
        private readonly int _strartY;

        private int _currentPage;
        private int _horizontalCount;
        private int _verticalCount;
        private int _tileWidth;
        private int _tileHeight;
        private int _horizontalSpacing;
        private int _verticalSpacing;
        private int _totalCount;

        public MatrixBuilder(ITileBuilder tileBuilder, int startX, int startY, TileType? tileType)
        {
            _tileBuilder = tileBuilder;
            _startX = startX;
            _strartY = startY;
            _tileType = tileType;
        }

        public void BuildGrid(
            int horizontalCount,
            int verticalCount,
            int tileWidth,
            int tileHeight,
            int verticalSpacing,
            int horizontalSpacing,
            int totalCount,
            int distancePerPage)
        {            
            _tileBuilder.Init();

            _currentPage = 0;
            _horizontalCount = horizontalCount;
            _verticalCount = verticalCount;
            _tileWidth = tileWidth;
            _tileHeight = tileHeight;
            _horizontalSpacing = horizontalSpacing;
            _verticalSpacing = verticalSpacing;
            _totalCount = totalCount;


            var countPerPage = _horizontalCount * _verticalCount;
            var pageCount = _totalCount/countPerPage;

            var elementInCurrentPage = _totalCount - _currentPage * countPerPage;
            if (_totalCount < elementInCurrentPage)
            {
                elementInCurrentPage = _totalCount;
            }

            int nr = 0;
            int startX = _startX;
            int startY = _strartY;
            for (int page = 0; page < pageCount; page++)
            {
                nr = CreatePage(startX, startY, nr, page, elementInCurrentPage);
                startX = _startX + distancePerPage;
            }
        }

        private int CreatePage(int startX, int startY, int nr, int page, int elementInCurrentPage)
        {
            int currentNr = 0;
            for (var i = 0; i < _verticalCount; i++)
            {
                for (var j = 0; j < _horizontalCount; j++)
                {
                    var x = startX + j * _tileWidth + j * _horizontalSpacing;
                    var y = startY + i * _tileHeight + i * _verticalSpacing;
                    _tileBuilder.BuildTile(nr, x, y, _tileWidth, _tileHeight, _tileType);
                    nr++;
                    currentNr++;

                    if (elementInCurrentPage <= currentNr)
                    {
                        return nr;
                    }
                }
            }

            return nr;
        }

    }
}
