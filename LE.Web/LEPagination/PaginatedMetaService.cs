using System.Collections.Generic;

namespace LE.Web.LEPagination
{
    public interface PaginatedMetaService
    {
        PaginatedMetaModel GetMetaData(int collectionSize, int selectedPageNumber, int itemsPerPage);
    }

    public class PaginatedMetaServiceImpl : PaginatedMetaService
    {

        private const int NumberOfNodesInPaginatedList = 5;

        private List<Page> _pages;

        private readonly PreviousPageService _previousPageService;
        private readonly NextPageService _nextPageService;
        private readonly NodeService _nodeService;
        private readonly LastPageInCollectionService _pageInCollectionService;

        public PaginatedMetaServiceImpl()
        {
            _previousPageService = new PreviousPageServiceImpl();
            _nextPageService = new NextPageServiceImpl();
            _nodeService = new NodeServiceImpl();
            _pageInCollectionService = new LastPageInCollectionServiceImpl();
        }


        public PaginatedMetaModel GetMetaData(int collectionSize, int selectedPageNumber, int itemsPerPage)
        {
            var lastPage = _pageInCollectionService.GetLastPageInCollection(collectionSize, itemsPerPage);

            // Cover > out of range exceptions
            if (lastPage < selectedPageNumber)
            {
                selectedPageNumber = lastPage;
            }

            // Cover < out of range exceptions
            if (selectedPageNumber < 1)
            {
                selectedPageNumber = 1;
            }

            if (collectionSize == 0)
            {
                return GetCollectionSizeZeroModel();
            }

            _pages = _nodeService.BuildPageNodes(collectionSize, selectedPageNumber, itemsPerPage, NumberOfNodesInPaginatedList);
            return new PaginatedMetaModel
            {
                PreviousPage = _previousPageService.BuildPreviousPage(_pages, collectionSize, selectedPageNumber, itemsPerPage),
                Pages = _pages,
                NextPage = _nextPageService.BuildNextPage(
                    _pages,
                    collectionSize,
                    selectedPageNumber,
                    itemsPerPage,
                    NumberOfNodesInPaginatedList
                )
            };
        }

        private static PaginatedMetaModel GetCollectionSizeZeroModel()
        {
            return new PaginatedMetaModel
            {
                PreviousPage = new PreviousPage
                {
                    Display = false
                },
                Pages = new List<Page>(),
                NextPage = new NextPage
                {
                    Display = false
                }
            };
        }
    }
}